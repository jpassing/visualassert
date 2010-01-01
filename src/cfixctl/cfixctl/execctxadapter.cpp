/*----------------------------------------------------------------------
 * Purpose:
 *		Fixture execution context.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <shlwapi.h>
#include "cfixctlp.h"

C_ASSERT( CfixctlDispositionContinue	== CfixContinue );
C_ASSERT( CfixctlDispositionBreak		== CfixBreak );
C_ASSERT( CfixctlDispositionBreakAlways == CfixBreakAlways );
C_ASSERT( CfixctlDispositionAbort		== CfixAbort );

//
// Note on locking:
//   This adapter assumes that there may be multiple concurrent
//   threads issueing assertions etc, but fixture and test case 
//   begin/end events are serialized.
//
//   That is, there is only one 'main thread', all other threads are
//   child threads created cvia CfixCreateThread.
//
typedef struct _CFIXCTLP_EXEC_CONTEXT
{
	CFIX_EXECUTION_CONTEXT Base;

	volatile LONG ReferenceCount;

	ICfixTestModuleInternal *Module;

	ICfixProcessEventSink *ProcessSink;
	ICfixTestÌtemContainerEventSink *FixtureSink;		// May be NULL.
	ICfixTestÌtemEventSink *TestCaseSink;				// May be NULL.

	volatile BOOL IssueAbort;

	BOOL AutoAdjustCurrentDirectory;
} CFIXCTLP_EXEC_CONTEXT, *PCFIXCTLP_EXEC_CONTEXT;

/*----------------------------------------------------------------------
 *
 * Helpers.
 *
 */

static ULONG CfixctlsGetFixtureOrdinal(
	__in PCFIX_FIXTURE Fixture
	)
{
	PCFIX_TEST_MODULE Module = Fixture->Module;
	
	for ( ULONG Index = 0; Index < Module->FixtureCount; Index++ )
	{
		if ( Module->Fixtures[ Index ] == Fixture )
		{
			return Index;
		}
	}

	ASSERT( !"Fixture not found" );
	return 0;
}

static ULONG CfixctlsGetTestCaseOrdinal(
	__in PCFIX_TEST_CASE TestCase
	)
{
	PCFIX_FIXTURE Fixture = TestCase->Fixture;
	return ( ULONG ) ( TestCase - Fixture->TestCases );
}

static ICfixReportEventSink* CfixctlsGetEffectiveReportSink(
	__in PCFIXCTLP_EXEC_CONTEXT Context
	)
{
	if ( Context->TestCaseSink != NULL )
	{
		return Context->TestCaseSink;
	}
	else if ( Context->FixtureSink != NULL )
	{
		return Context->FixtureSink;
	}
	else
	{
		ASSERT( !"No Sink" );
		return NULL;
	}
}

static HRESULT CfixctlsGetModuleDirectory(
	__in ICfixTestModule *Module,
	__in SIZE_T BufferCount,
	__out_ecount( BufferCount ) PWSTR Buffer 
	)
{
	ASSERT( Module );
	ASSERT( BufferCount );
	ASSERT( Buffer );

	if ( BufferCount > MAX_PATH )
	{
		return E_INVALIDARG;	// We are using shlwapi.
	}

	//
	// Obtain path of module.
	//

	BSTR BstrPath;
	HRESULT Hr = Module->GetPath( &BstrPath );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = StringCchCopy( Buffer, BufferCount, BstrPath );
	SysFreeString( BstrPath );

	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	//
	// Strip the file name.
	//
	PathRemoveFileSpec( Buffer );
	PathAddBackslash( Buffer );
	return S_OK;
}

/*----------------------------------------------------------------------
 *
 * Methods.
 *
 */

static VOID CfixctlsExecCtxReference(
	__in PCFIX_EXECUTION_CONTEXT This
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;
	InterlockedIncrement( &Context->ReferenceCount );
}

static VOID CfixctlsExecCtxDereference(
	__in PCFIX_EXECUTION_CONTEXT This
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;
	LONG Refs = InterlockedDecrement( &Context->ReferenceCount );
	COM_TRACE( ( L"Dereference ExecCtxAdapter: %d\n", Refs ) );
	if ( 0 == Refs )
	{
		Context->Module->Release();
		Context->ProcessSink->Release();

		ASSERT( Context->TestCaseSink == NULL );
		ASSERT( Context->FixtureSink == NULL );

		free( Context );
	}
}

static CFIX_REPORT_DISPOSITION CfixctlsExecCtxQueryDefaultDisposition(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in CFIX_EVENT_TYPE EventType
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;
	
	ICfixReportEventSink *Sink = CfixctlsGetEffectiveReportSink( Context );
	ASSERT( Sink != NULL );
	if ( Sink == NULL )
	{
		return CfixBreak;
	}

	CFIXCTL_REPORT_DISPOSITION Disp;
	switch ( EventType )
	{
	case CfixEventFailedAssertion:
		if ( SUCCEEDED( Sink->QueryDefaultFailedAssertionDisposition( &Disp ) ) )
		{
			return ( CFIX_REPORT_DISPOSITION ) Disp;
		}
		else
		{
			 ( VOID ) Context->ProcessSink->Notification(
				CFIXCTL_E_QUERY_DEF_DISP_FAILED );
			return CfixBreak;
		}

	case CfixEventUncaughtException:
		if ( SUCCEEDED( Sink->QueryDefaultUnhandledExceptionDisposition( &Disp ) ) )
		{
			return ( CFIX_REPORT_DISPOSITION )Disp;
		}
		else
		{
			( VOID ) Context->ProcessSink->Notification(
				CFIXCTL_E_QUERY_DEF_DISP_FAILED );
			return CfixContinue;
		}

	case CfixEventInconclusiveness:	
		//
		// Ignored anyway.
		//
		return CfixContinue;

	case CfixEventLog:
		//
		// Ignored anyway.
		//
		return CfixContinue;

	default:
		ASSERT( !"Unknown event type!" );
		return CfixContinue;
	}
}

static CFIX_REPORT_DISPOSITION CfixctlsExecCtxReportEvent(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in PCFIX_THREAD_ID ThreadId,
	__in PCFIX_TESTCASE_EXECUTION_EVENT Event
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;

	UNREFERENCED_PARAMETER( ThreadId );
	
	if ( Context->IssueAbort )
	{
		return CfixAbort;
	}
	
	ICfixReportEventSink *Sink = CfixctlsGetEffectiveReportSink( Context );
	ASSERT( Sink != NULL );
	if ( Sink == NULL )
	{
		return CfixBreak;
	}

	CFIXCTL_REPORT_DISPOSITION Disp;
	HRESULT Hr;

	ICfixStackTrace *StackTrace;
	Hr = Context->Module->CreateStackTrace(
		&Event->StackTrace,				
		&StackTrace );
	if ( FAILED( Hr ) )
	{
		//
		// Do not use a stack trace - they are optional.
		//
		StackTrace = NULL;

		CFIXCTLP_TRACE( ( L"Stack trace creation failed: %x\n", Hr ) );
	}

	switch ( Event->Type )
	{
	case CfixEventFailedAssertion:
		{
			BSTR Routine	= SysAllocString( Event->Info.FailedAssertion.Routine );
			BSTR Expression = SysAllocString( Event->Info.FailedAssertion.Expression );
			BSTR File		= SysAllocString( Event->Info.FailedAssertion.File );

			Hr = Sink->FailedAssertion(
				Expression,
				Routine,
				File,
				NULL,
				Event->Info.FailedAssertion.Line,
				Event->Info.FailedAssertion.LastError,
				0,
				0,
				StackTrace,
				&Disp );

			SysFreeString( Routine );
			SysFreeString( Expression );
			SysFreeString( File );
			
			break;
		}

	case CfixEventUncaughtException:
		{
			Hr = Sink->UnhandledException(
				Event->Info.UncaughtException.ExceptionRecord.ExceptionCode,
				0,
				StackTrace,
				&Disp );
			break;
		}

	case CfixEventInconclusiveness:	
		{
			BSTR Message = SysAllocString( Event->Info.Inconclusiveness.Message );
			Hr = Sink->Inconclusive(
				Message,
				0,
				StackTrace );
			SysFreeString( Message );
			
			Disp = CfixctlDispositionContinue;
			break;
		}

	case CfixEventLog:
		{
			BSTR Message = SysAllocString( Event->Info.Inconclusiveness.Message );
			if ( Message == NULL )
			{
				//
				// Ignore this call.
				//
				Hr = S_OK;
			}
			else
			{
				Hr = Sink->Log(
					Message,
					0,
					StackTrace );
				SysFreeString( Message );
			}

			Disp = CfixctlDispositionContinue;
			break;
		}

	default:
		ASSERT( !"Unrecognized type" );
		Hr = E_FAIL;
		Disp = CfixctlDispositionBreak;
	}

	if ( StackTrace )
	{
		LONG Refs = StackTrace->Release();

		CFIXCTLP_TRACE( ( L"Stack trace released. Refs: %d\n", Refs ) );
		UNREFERENCED_PARAMETER( Refs );
	}

	CFIXCTLP_TRACE( ( L"Disposition is: %d\n", Disp) );

	if ( SUCCEEDED( Hr ) )
	{
		return ( CFIX_REPORT_DISPOSITION ) Disp;
	}
	else
	{
		//
		// Try passing a notification. If it fails, ignore.
		//
		( VOID ) Context->ProcessSink->Notification(
			CFIXCTL_E_REPORT_EVENT_FAILED );
		return CfixBreak;
	}
}

static HRESULT CfixctlsExecCtxBeforeFixtureStart(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in PCFIX_THREAD_ID ThreadId,
	__in PCFIX_FIXTURE Fixture
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;
	
	ASSERT( Context->TestCaseSink == NULL );
	ASSERT( Context->FixtureSink == NULL );

	UNREFERENCED_PARAMETER( ThreadId );

	if ( Context->IssueAbort )
	{
		return CFIXCTL_E_USER_ABORT;
	}

	if ( Context->AutoAdjustCurrentDirectory )
	{
		WCHAR DirectoryPath[ MAX_PATH ];
		HRESULT Hr = CfixctlsGetModuleDirectory( 
			Context->Module,
			_countof( DirectoryPath ),
			DirectoryPath );

		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		CFIXCTLP_TRACE( 
			( L"ExecCtxAdapter: SetCurrentDirectory( %s )\n", DirectoryPath ) );

		if ( ! SetCurrentDirectory( DirectoryPath ) )
		{
			return HRESULT_FROM_WIN32( GetLastError() );
		}
	}

	//
	// Obtain new fixture sink.
	//
	if ( SUCCEEDED( Context->ProcessSink->GetTestItemContainerEventSink(
		Context->Module,
		CfixctlsGetFixtureOrdinal( Fixture ),
		&Context->FixtureSink ) ) )
	{
		HRESULT Hr = Context->FixtureSink->BeforeFixtureStart();
		if ( FAILED( Hr ) )
		{
			Context->FixtureSink->Release();
			Context->FixtureSink = NULL;
		}

		return Hr;
	}
	else
	{
		return CFIXCTL_E_NO_FIXTURE_SINK;
	}
}

static HRESULT CfixctlsExecCtxBeforeTestCaseStart(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in PCFIX_THREAD_ID ThreadId,
	__in PCFIX_TEST_CASE TestCase
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;

	ASSERT( Context->TestCaseSink == NULL );
	ASSERT( Context->FixtureSink != NULL );

	if ( Context->IssueAbort )
	{
		return CFIXCTL_E_USER_ABORT;
	}

	if ( ! Context->FixtureSink )
	{
		return E_UNEXPECTED;
	}

	//
	// Obtain new testcase sink.
	//
	if ( SUCCEEDED( Context->FixtureSink->GetTestItemEventSink(
		CfixctlsGetTestCaseOrdinal( TestCase ),
		ThreadId->MainThreadId,
		&Context->TestCaseSink ) ) )
	{
		HRESULT Hr = Context->TestCaseSink->BeforeTestCaseStart();
		if ( FAILED( Hr ) )
		{
			Context->TestCaseSink->Release();
			Context->TestCaseSink = NULL;
		}

		return Hr;
	}
	else
	{
		return CFIXCTL_E_NO_TESTCASE_SINK;
	}
}

static VOID CfixctlsExecCtxAfterFixtureFinish(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in PCFIX_THREAD_ID ThreadId,
	__in PCFIX_FIXTURE Fixture,
	__in BOOL RanToCompletion
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;
	
	ASSERT( Context->TestCaseSink == NULL );
	ASSERT( Context->FixtureSink != NULL );

	UNREFERENCED_PARAMETER( ThreadId );
	UNREFERENCED_PARAMETER( Fixture );

	if ( ! Context->FixtureSink )
	{
		return;
	}

	HRESULT Hr = Context->FixtureSink->AfterFixtureFinish(
		RanToCompletion );
	if ( FAILED( Hr ) )
	{
		//
		// Does not really matter.
		//
		CFIXCTLP_TRACE( ( L"AfterFixtureFinish failed: 0x%08X\n", Hr ) );
	}

	Context->FixtureSink->Release();
	Context->FixtureSink = NULL;
}


static VOID CfixctlsExecCtxAfterTestCaseFinish(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in PCFIX_THREAD_ID ThreadId,
	__in PCFIX_TEST_CASE TestCase,
	__in BOOL RanToCompletion
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;
	
	ASSERT( Context->TestCaseSink != NULL );
	ASSERT( Context->FixtureSink != NULL );

	UNREFERENCED_PARAMETER( ThreadId );
	UNREFERENCED_PARAMETER( TestCase );

	if ( ! Context->TestCaseSink )
	{
		return;
	}

	HRESULT Hr = Context->TestCaseSink->AfterTestCaseFinish(
		RanToCompletion );
	if ( FAILED( Hr ) )
	{
		//
		// Does not really matter.
		//
		CFIXCTLP_TRACE( ( L"AfterTestCaseFinish failed: 0x%08X\n", Hr ) );
	}

	Context->TestCaseSink->Release();
	Context->TestCaseSink = NULL;
}

static VOID CfixctlsExecCtxBeforeChildThreadStart(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in PCFIX_THREAD_ID ThreadId,
	__in_opt PVOID ParentContext
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;
	
	ASSERT( Context->FixtureSink != NULL );

	UNREFERENCED_PARAMETER( ThreadId );
	UNREFERENCED_PARAMETER( ParentContext );

	ICfixReportEventSink *Sink = CfixctlsGetEffectiveReportSink( Context );
	ASSERT( Sink != NULL );
	if ( Sink == NULL )
	{
		return;
	}

	//
	// N.B. We are guaranteed to be called on the child thread.
	//

	( VOID ) Sink->BeforeChildThreadStart(
		GetCurrentThreadId() );
}

static VOID CfixctlsExecCtxAfterChildThreadFinish(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in PCFIX_THREAD_ID ThreadId,
	__in_opt PVOID ParentContext
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;
	
	ASSERT( Context->FixtureSink != NULL );

	UNREFERENCED_PARAMETER( ThreadId );
	UNREFERENCED_PARAMETER( ParentContext );

	ICfixReportEventSink *Sink = CfixctlsGetEffectiveReportSink( Context );
	ASSERT( Sink != NULL );
	if ( Sink == NULL )
	{
		return;
	}

	//
	// N.B. We are guaranteed to be called on the child thread.
	//

	( VOID ) Sink->AfterChildThreadFinish(
		GetCurrentThreadId() );
}

static HRESULT CfixctlsExecCtxCreateChildThread(
	__in struct _CFIX_EXECUTION_CONTEXT *This,
	__in PCFIX_THREAD_ID ThreadId,
	__out PVOID *ContextForChild
	)
{
	UNREFERENCED_PARAMETER( This );
	UNREFERENCED_PARAMETER( ThreadId );
	UNREFERENCED_PARAMETER( ContextForChild );

	return S_OK;
}

static VOID CfixctlsExecCtxOnUnhandledException(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in PCFIX_THREAD_ID ThreadId,
	__in PEXCEPTION_POINTERS ExcpPointers
	)
{
	UNREFERENCED_PARAMETER( This );
	UNREFERENCED_PARAMETER( ThreadId );
	UNREFERENCED_PARAMETER( ExcpPointers );
}

/*----------------------------------------------------------------------
 *
 * Internals.
 *
 */

HRESULT CfixctlpCreateExecutionContextAdapter(
	__in ICfixTestModule *Module,
	__in ICfixProcessEventSink *ProcessSink,
	__in BOOL AutoAdjustCurrentDirectory,
	__out PCFIX_EXECUTION_CONTEXT *Context
	)
{
	PCFIXCTLP_EXEC_CONTEXT NewContext;
	ICfixTestModuleInternal *ModuleInternal;

	if ( ! Module || ! ProcessSink || ! Context )
	{
		return E_INVALIDARG;
	}

	HRESULT Hr = Module->QueryInterface( 
		IID_ICfixTestModuleInternal, 
		( PVOID* ) &ModuleInternal );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}
	
	NewContext = ( PCFIXCTLP_EXEC_CONTEXT ) 
		malloc( sizeof( CFIXCTLP_EXEC_CONTEXT ) );
	if ( ! NewContext )
	{
		return E_OUTOFMEMORY;
	}

	ZeroMemory( NewContext, sizeof( CFIXCTLP_EXEC_CONTEXT ) );

	ProcessSink->AddRef();

	NewContext->ReferenceCount				= 1;
	NewContext->Module						= ModuleInternal;
	NewContext->ProcessSink					= ProcessSink;
	NewContext->IssueAbort					= FALSE;
	NewContext->AutoAdjustCurrentDirectory	= AutoAdjustCurrentDirectory;

	NewContext->Base.Version				= CFIX_TEST_CONTEXT_VERSION;
	NewContext->Base.ReportEvent			= CfixctlsExecCtxReportEvent;
	NewContext->Base.QueryDefaultDisposition= CfixctlsExecCtxQueryDefaultDisposition;
	NewContext->Base.BeforeFixtureStart		= CfixctlsExecCtxBeforeFixtureStart;
	NewContext->Base.AfterFixtureFinish		= CfixctlsExecCtxAfterFixtureFinish;
	NewContext->Base.BeforeTestCaseStart	= CfixctlsExecCtxBeforeTestCaseStart;
	NewContext->Base.AfterTestCaseFinish	= CfixctlsExecCtxAfterTestCaseFinish;
	NewContext->Base.CreateChildThread		= CfixctlsExecCtxCreateChildThread;
	NewContext->Base.BeforeChildThreadStart	= CfixctlsExecCtxBeforeChildThreadStart;
	NewContext->Base.AfterChildThreadFinish	= CfixctlsExecCtxAfterChildThreadFinish;
	NewContext->Base.OnUnhandledException	= CfixctlsExecCtxOnUnhandledException;
	NewContext->Base.Reference				= CfixctlsExecCtxReference;
	NewContext->Base.Dereference			= CfixctlsExecCtxDereference;

	*Context = &NewContext->Base;

	return S_OK;
}

HRESULT CfixctlpAbortExecutionContextAdapter(
	__in PCFIX_EXECUTION_CONTEXT Context
	)
{
	if ( ! Context )
	{
		return E_INVALIDARG;
	}

	PCFIXCTLP_EXEC_CONTEXT Obj = ( PCFIXCTLP_EXEC_CONTEXT ) Context;
	Obj->IssueAbort = TRUE;

	return S_OK;
}