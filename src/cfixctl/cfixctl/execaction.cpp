/*----------------------------------------------------------------------
 * Purpose:
 *		Execution action. Used to execute one or more fixtures
 *		(depending on the underlying PCFIX_ACTION) of a single module.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include "cfixctlp.h"
#include <cfixevnt.h>
#include <cfixutil.h>

const GUID IID_ICfixExecutionActionInternal = 
	{ 0x5bcd9d4e, 0xa622, 0x4d60, { 0x86, 0xed, 0x97, 0xd, 0x11, 0xc1, 0x73, 0x8d } };

#define CFIXCTLP_LARGE_STACK_SIZE	(  4 * 1024 * 1024 )
#define CFIXCTLP_HUGE_STACK_SIZE	( 16 * 1024 * 1024 )

/*----------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 */

class ExecutionAction :
	public ComObjectBase, 
	public ICfixExecutionActionInternal
{
	DECLARE_NOT_COPYABLE( ExecutionAction );

private:
	ICfixTestModule *Module;
	PCFIX_ACTION Action;
	ULONG TestCaseCount;
	
	//
	// When a run is active, CurrentAdapter (guarded by
	// CurrentAdapterLock) holds a pointer to the Adapter object
	// which can be used to issue abortions.
	//

	PCFIX_EXECUTION_CONTEXT CurrentAdapter;
	CRITICAL_SECTION CurrentAdapterLock;

	//
	// Optional: Event Sink.
	//
	PCFIX_EVENT_SINK EventSink;

protected:
	ExecutionAction();

public:
	virtual ~ExecutionAction();

	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )() PURE;
	STDMETHOD_( ULONG, Release )() PURE;
	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr );

	/*------------------------------------------------------------------
	 * ICfixAction methods.
	 */

	STDMETHOD( Run )( 
		__in ICfixEventSink *Sink,
		__in ULONG Flags
		);

	STDMETHOD( Stop )();

	STDMETHOD( GetTestCaseCount )(
		__out ULONG* Count 
		);

	STDMETHOD( RegisterEventDll )(
		__in BSTR DllPath,
		__in BSTR Options,
		__reserved ULONG Reserved
		);

	/*------------------------------------------------------------------
	 * ICfixExecutionActionInternal methods.
	 */

	STDMETHOD( Initialize )(
		__in ICfixTestModule *Module,
		__in PCFIX_ACTION Action,
		__in ULONG TestCaseCount
		);
};

/*----------------------------------------------------------------------
 * 
 * Factory.
 *
 */

IClassFactory& CfixctlpGetExecutionActionFactory()
{
	static ComClassFactory< 
		ComMtaObject< ExecutionAction >, CfixctlServerLock > Factory;
	return Factory;
}

/*----------------------------------------------------------------------
 * 
 * Helpers.
 *
 */

typedef struct _CFIXCTLP_RUN_ARGS
{
	PCFIX_ACTION Action;
	PCFIX_EXECUTION_CONTEXT Context;
} CFIXCTLP_RUN_ARGS, *PCFIXCTLP_RUN_ARGS;

static HRESULT CfixctlsRunDirect(
	__in PCFIX_ACTION Action,
	__in PCFIX_EXECUTION_CONTEXT Context
	)
{
	ASSERT( Action );
	ASSERT( Context );

	return Action->Run( Action, Context );
}

static DWORD CfixctlsRunIndirectThreadProc( PVOID Args )
{
	PCFIXCTLP_RUN_ARGS ThreadArgs = ( PCFIXCTLP_RUN_ARGS ) Args;
	
	return ( DWORD ) CfixctlsRunDirect(
		ThreadArgs->Action,
		ThreadArgs->Context );
}

static HRESULT CfixctlsRunOnWorkerThread(
	__in PCFIX_ACTION Action,
	__in PCFIX_EXECUTION_CONTEXT Adapter,
	__in ULONG ThreadStackSize
	)
{
	ASSERT( Action );
	ASSERT( Adapter );

	//
	// N.B. The thread we are about to spawn will not be a COM thread - 
	// maybe some test will call CoInitializeEx, maybe not. Therefore,
	// we may not perform any COM activity on this thread, which in turn
	// means that we cannot pass the Adapter over to the thread as the
	// adapter would indeed perform (D)COM calls.
	//
	// Therefore, wrap the Adapter.
	//

	HANDLE Thread = NULL;
	PCFIX_EXECUTION_CONTEXT Proxy = NULL;

	HRESULT Hr = CfixctlpCreateThreadSwitchProxy( Adapter, &Proxy );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	CFIXCTLP_RUN_ARGS ThreadArgs;
	ThreadArgs.Action	= Action;
	ThreadArgs.Context	= Proxy;

	Thread = CreateThread(
		NULL,
		ThreadStackSize,
		CfixctlsRunIndirectThreadProc,
		&ThreadArgs,
		0,
		NULL );
	if ( Thread == NULL )
	{
		Hr = HRESULT_FROM_WIN32( GetLastError() );
		goto Cleanup;
	}

	//
	// Service execution context callbacks until the thread dies.
	//
	Hr = CfixctlpServiceThreadSwitchProxy( Proxy, Thread );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	DWORD ExitCode;
	if ( ! GetExitCodeThread( Thread, &ExitCode ) )
	{
		Hr =  HRESULT_FROM_WIN32( GetLastError() );
	}
	else
	{
		Hr = ( HRESULT ) ExitCode;
	}

Cleanup:

	if ( Thread != NULL )
	{
		VERIFY( CloseHandle( Thread ) );
	}

	if ( Proxy != NULL )
	{
		Proxy->Dereference( Proxy );
	}

	return Hr;
}

/*----------------------------------------------------------------------
 * 
 * Class Implementation.
 *
 */

ExecutionAction::ExecutionAction()
	: Module( NULL )
	, Action( NULL )
	, CurrentAdapter( NULL )
	, EventSink( NULL )
	, TestCaseCount( 0 )
{
	InitializeCriticalSection( &this->CurrentAdapterLock );
}

ExecutionAction::~ExecutionAction()
{
	ASSERT( ! this->CurrentAdapter );

	if ( this->Action )
	{
		Action->Dereference( this->Action );
	}

	if ( this->Module )
	{
		this->Module->Release();
	}

	if ( this->EventSink )
	{
		this->EventSink->Dereference( this->EventSink );
	}

	DeleteCriticalSection( &this->CurrentAdapterLock );
}

/*----------------------------------------------------------------------
 * IUnknown methods.
 */

STDMETHODIMP ExecutionAction::QueryInterface( 
	__in REFIID Iid, 
	__out PVOID* Ptr )
{
	HRESULT Hr;

	if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
		 InlineIsEqualGUID( Iid, IID_ICfixAction )||
		 InlineIsEqualGUID( Iid, IID_ICfixExecutionActionInternal ) )
	{
		*Ptr = static_cast< ICfixExecutionActionInternal* >( this );
		Hr = S_OK;

	}
	else
	{
		*Ptr = NULL;
		Hr = E_NOINTERFACE;
	}

	if ( SUCCEEDED( Hr ) )
	{
		this->AddRef();
	}

	return Hr;
}

/*------------------------------------------------------------------
 * ICfixAction methods.
 */

STDMETHODIMP ExecutionAction::Run( 
	__in ICfixEventSink *Sink,
	__in ULONG Flags
	)
{
	if ( Sink == NULL )
	{
		return E_POINTER;
	}

	if ( Flags > 
		( CFIXCTL_ACTION_COM_NEUTRAL | 
		  CFIXCTL_ACTION_AUTO_ADJUST_CURRENT_DIRECTORY |
		  CFIXCTL_ACTION_LARGE_STACK |
		  CFIXCTL_ACTION_HUGE_STACK ) )
	{
		return E_INVALIDARG;
	}

	if ( this->Module == NULL || 
		 this->Action == NULL ||
		 this->CurrentAdapter != NULL )
	{
		return E_UNEXPECTED;
	}

	ICfixProcessEventSink *ProcessSink = NULL;
	PCFIX_EXECUTION_CONTEXT Adapter = NULL;

	//
	// Query process sink - we do not need a ICfixEventSink.
	//
	HRESULT Hr = Sink->GetProcessEventSink( 
		GetCurrentProcessId(),
		&ProcessSink );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Create adapter.
	//
	Hr = CfixctlpCreateExecutionContextAdapter(
		this->Module,
		ProcessSink,
		( Flags & CFIXCTL_ACTION_AUTO_ADJUST_CURRENT_DIRECTORY ),
		&Adapter );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	if ( this->EventSink != NULL )
	{
		PCFIX_EXECUTION_CONTEXT Proxy;
		Hr = CfixCreateEventEmittingExecutionContextProxy(
			Adapter,
			this->EventSink,
			&Proxy );
		if ( FAILED( Hr ) )
		{
			goto Cleanup;
		}

		Adapter->Dereference( Adapter );
		Adapter = Proxy;
	}

	Hr = ProcessSink->BeforeRunStart();
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Before starting the run, make the adapter object available
	// s.t. concurrent threads can issue abortions.
	//
	EnterCriticalSection( &this->CurrentAdapterLock );
	this->CurrentAdapter = Adapter;
	LeaveCriticalSection( &this->CurrentAdapterLock );

	//
	// Let it run and tunnel events through the adapter.
	//
	if ( CfixctlpFlagOn( Flags, CFIXCTL_ACTION_COM_NEUTRAL ) )
	{
		ULONG StackSize;

		if ( CfixctlpFlagOn( Flags, CFIXCTL_ACTION_HUGE_STACK ) )
		{
			StackSize = CFIXCTLP_HUGE_STACK_SIZE;
		}
		else if ( CfixctlpFlagOn( Flags, CFIXCTL_ACTION_LARGE_STACK ) )
		{
			StackSize = CFIXCTLP_LARGE_STACK_SIZE;
		}
		else
		{
			StackSize = 0; // System default.
		}

		Hr = CfixctlsRunOnWorkerThread( Action, Adapter, StackSize );
	}
	else
	{
		Hr = CfixctlsRunDirect( Action, Adapter );
	}

	EnterCriticalSection( &this->CurrentAdapterLock );
	this->CurrentAdapter = NULL;
	LeaveCriticalSection( &this->CurrentAdapterLock );

	( VOID ) ProcessSink->AfterRunFinish();

Cleanup:
	if ( ProcessSink )
	{
		ProcessSink->Release();
	}

	if ( Adapter )
	{
		Adapter->Dereference( Adapter );
	}

	return Hr;
}

STDMETHODIMP ExecutionAction::Stop()
{
	HRESULT Hr;
	
	EnterCriticalSection( &this->CurrentAdapterLock );
	
	if ( this->CurrentAdapter == NULL )
	{
		//
		// No run active.
		//
		Hr = S_FALSE;
	}
	else
	{
		Hr = CfixctlpAbortExecutionContextAdapter( this->CurrentAdapter );
	}

	LeaveCriticalSection( &this->CurrentAdapterLock );

	return Hr;
}

STDMETHODIMP ExecutionAction::GetTestCaseCount(
	__out ULONG* Count 
	)
{
	if ( ! Count )
	{
		return E_POINTER;
	}
	else
	{
		*Count = this->TestCaseCount;
		return S_OK;
	}
}

STDMETHODIMP ExecutionAction::RegisterEventDll(
	__in BSTR DllPath,
	__in BSTR Options,
	__reserved ULONG Reserved
	)
{
	if ( DllPath == NULL || Reserved != 0 )
	{
		return E_INVALIDARG;
	}

	if ( this->EventSink != NULL )
	{
		return E_UNEXPECTED;
	}

	return CfixutilLoadEventSinkFromDll(
		( PCWSTR ) DllPath,
		CFIX_EVENT_SINK_FLAG_SHOW_STACKTRACE_SOURCE_INFORMATION,
		SysStringByteLen( Options ) > 0
			? Options
			: NULL,
		&this->EventSink );
}


/*----------------------------------------------------------------------
 * ICfixExecutionActionInternal methods.
 */

STDMETHODIMP ExecutionAction::Initialize(
	__in ICfixTestModule *Module,
	__in PCFIX_ACTION Action,
	__in ULONG TestCaseCount
	)
{
	if ( ! Module || ! Action )
	{
		return E_POINTER;
	}

	if ( this->Module != NULL || this->Action != NULL )
	{
		return E_UNEXPECTED;
	}

	Module->AddRef();
	Action->Reference( Action );

	this->Module			= Module;
	this->Action			= Action;
	this->TestCaseCount		= TestCaseCount;

	return S_OK;
}