/*----------------------------------------------------------------------
 * Purpose:
 *		Test ExecutionAction class.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <testctlp.h>

#pragma warning( push )
#pragma warning( disable: 6255; disable: 6011 )
#include <atlbase.h>
#pragma warning( pop )

#define EVENT_SINK_FAIL_PROCESS_SINK	1

#pragma warning( disable: 4100 ) // Unreferenced argument.

class EventSink : 
	public ICfixTest�temEventSink,
	public ICfixTest�temContainerEventSink,
	public ICfixProcessEventSink,
	public ICfixEventSink
{
private: 
	ULONG Flags;

public:
	ULONG RefCount;
	ULONG CallbackCount;
	ULONG CallbackMask;

	EventSink( ULONG Flags ) 
		: RefCount( 0 )
		, CallbackCount( 0 )
		, CallbackMask( 0 )
		, Flags( Flags )
	{
	}

	STDMETHOD_( ULONG, AddRef )()
	{
		return ++this->RefCount;
	}

	STDMETHOD_( ULONG, Release )()
	{
		return --this->RefCount;
	}

	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr )
	{
		HRESULT Hr;

		if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
			 InlineIsEqualGUID( Iid, IID_ICfixTest�temEventSink ) )
		{
			*Ptr = static_cast< ICfixTest�temEventSink* >( this );
			Hr = S_OK;

			this->AddRef();
		}
		else if ( InlineIsEqualGUID( Iid, IID_ICfixTest�temContainerEventSink ) )
		{
			*Ptr = static_cast< ICfixTest�temContainerEventSink* >( this );
			Hr = S_OK;

			this->AddRef();
		}
		else if ( InlineIsEqualGUID( Iid, IID_ICfixProcessEventSink ) )
		{
			*Ptr = static_cast< ICfixProcessEventSink* >( this );
			Hr = S_OK;

			this->AddRef();
		}
		else if ( InlineIsEqualGUID( Iid, IID_ICfixEventSink ) )
		{
			*Ptr = static_cast< ICfixEventSink* >( this );
			Hr = S_OK;

			this->AddRef();
		}
		else
		{
			*Ptr = NULL;
			Hr = E_NOINTERFACE;
		}

		return Hr;
	}

	/*------------------------------------------------------------------
	 * ICfixReportEventSink.
	 */
	
	STDMETHOD( UnhandledException )(
		__in ULONG ExceptionCode,
		__in ULONG Reserved,
		__in ICfixStackTrace *StackTrace,
		__out CFIXCTL_REPORT_DISPOSITION *Disposition
		)
	{
		this->CallbackCount++;
		this->CallbackMask |= 1;

		*Disposition = CfixctlDispositionContinue;
		return S_OK;
	}

	STDMETHOD( FailedAssertion )(
		__in const BSTR Expression,
		__in const BSTR Routine,
		__in const BSTR File,
		__in const BSTR Message,
		__in ULONG Line,
		__in ULONG LastError,
		__in ULONG Flags,
		__in ULONG Reserved,
		__in ICfixStackTrace *StackTrace,
		__out CFIXCTL_REPORT_DISPOSITION *Disposition
		)
	{
		this->CallbackCount++;
		this->CallbackMask |= 2;
		return S_OK;
	}

	STDMETHOD( FailedRelateAssertion )(
		__in CFIXCTL_RELATE_OPERATOR Operator,
		__in const VARIANT ExpectedValue,
		__in const VARIANT ActualValue,
		__in const BSTR Routine,
		__in const BSTR File,
		__in const BSTR Message,
		__in ULONG Line,
		__in ULONG LastError,
		__in ULONG Flags,
		__in ULONG Reserved,
		__in ICfixStackTrace *StackTrace,
		__out CFIXCTL_REPORT_DISPOSITION *Disposition
		)
	{
		this->CallbackCount++;
		this->CallbackMask |= 2;
		return S_OK;
	}

	STDMETHOD( Inconclusive )(
		__in const BSTR Message,
		__in ULONG Reserved,
		__in ICfixStackTrace *StackTrace
		)
	{
		this->CallbackCount++;
		this->CallbackMask |= 4;
		return S_OK;
	}

	STDMETHOD( Log )(
		__in const BSTR Message,
		__in ULONG Reserved,
		__in ICfixStackTrace *StackTrace
		)
	{
		this->CallbackCount++;
		this->CallbackMask |= 8;
		return S_OK;
	}

	STDMETHOD( QueryDefaultFailedAssertionDisposition )(
		__out CFIXCTL_REPORT_DISPOSITION *Disposition
		)
	{
		this->CallbackCount++;
		this->CallbackMask |= 16;

		*Disposition = CfixctlDispositionContinue;
		return S_OK;
	}

	STDMETHOD( QueryDefaultUnhandledExceptionDisposition )(
		__out CFIXCTL_REPORT_DISPOSITION *Disposition
		)
	{
		this->CallbackCount++;
		this->CallbackMask |= 32;
		
		*Disposition = CfixctlDispositionContinue;
		return S_OK;
	}

	STDMETHOD( BeforeChildThreadStart )(
		__in ULONG ThreadId
		)
	{
		this->CallbackCount++;
		this->CallbackMask |= 64;
		return S_OK;
	}

	STDMETHOD( AfterChildThreadFinish )(
		__in ULONG ThreadId
		)
	{
		this->CallbackCount++;
		this->CallbackMask |= 128;
		return S_OK;
	}

	/*------------------------------------------------------------------
	 * ICfixTest�temEventSink.
	 */

	STDMETHOD( BeforeTestCaseStart )()
	{
		this->CallbackCount++;
		this->CallbackMask |= 256;
		return S_OK;
	}

	STDMETHOD( AfterTestCaseFinish )(
		__in BOOL RanToCompletion
		)
	{
		CFIX_ASSERT( RanToCompletion );
		this->CallbackCount++;
		this->CallbackMask |= 512;
		return S_OK;
	}

	/*------------------------------------------------------------------
	 * ICfixTest�temContainerEventSink.
	 */

	STDMETHOD( BeforeFixtureStart )()
	{
		this->CallbackCount++;
		this->CallbackMask |= 1024;
		return S_OK;
	}

	STDMETHOD( AfterFixtureFinish )(
		__in BOOL RanToCompletion
		)
	{
		CFIX_ASSERT( RanToCompletion );
		this->CallbackCount++;
		this->CallbackMask |= 2048;
		return S_OK;
	}

	STDMETHOD( GetTestItemEventSink )(
		__in ULONG TestCaseOrdinal,
		__in ULONG ThreadId,
		__out ICfixTest�temEventSink **Sink
		)
	{
		this->AddRef();
		*Sink = this;
		return S_OK;
	}

	/*------------------------------------------------------------------
	 * ICfixProcessEventSink.
	 */

	STDMETHOD( Notification )(
		__in HRESULT Hr
		)
	{
		this->CallbackCount++;
		this->CallbackMask |= 2048;
		return S_OK;
	}

	STDMETHOD( BeforeRunStart )()
	{
		this->CallbackCount++;
		this->CallbackMask |= 4096;
		return S_OK;
	}

	STDMETHOD( AfterRunFinish )()
	{
		this->CallbackCount++;
		this->CallbackMask |= 8192;
		return S_OK;
	}

	STDMETHOD( GetTestItemContainerEventSink )(
		__in ICfixTestModule *Module,
		__in ULONG FixtureOrdinal,
		__out ICfixTest�temContainerEventSink **Sink
		)
	{
		CFIX_ASSERT( Module );
		this->AddRef();
		*Sink = this;
		return S_OK;
	}

	/*------------------------------------------------------------------
	 * ICfixProcessEventSink.
	 */
	STDMETHOD( GetProcessEventSink )(
		__in ULONG ProcessId,
		__out ICfixProcessEventSink **Sink
		)
	{
		CFIX_ASSERT_EQUALS_ULONG( GetCurrentProcessId(), ProcessId );

		if ( this->Flags & EVENT_SINK_FAIL_PROCESS_SINK )
		{
			return E_FAIL;
		}
		else
		{
			this->AddRef();
			*Sink = this;
			return S_OK;
		}
	}
};

//
// N.B. This test reenters the test case execution logic. This is 
// not supported for cfixcc tests, therefore athe classic API is used.
//

static COM_EXPORTS Exports;
static ICfixHost *Host = NULL;
		
static void SetUp()
{
	CoInitialize( NULL );
	GetComExports( L"cfixctl.dll", &Exports );
}

static void TearDown()
{
	CoUninitialize();
}

void Before()
{
	IClassFactory *AgentFactory;

	CFIX_ASSERT_OK( Exports.GetClassObject( 
		CLSID_LocalAgent, IID_IClassFactory, ( PVOID* ) &AgentFactory ) );
	CFIX_ASSUME( AgentFactory );

	ICfixAgent *Agent;
	CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
		NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
	CFIX_ASSUME( Agent );

	CFIX_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

	CFIX_ASSERT_OK( Agent->CreateHost( 
		TESTCTLP_OWN_ARCHITECTURE,
		CLSCTX_INPROC_SERVER,
		0,
		0,
		NULL,
		NULL,
		NULL,
		&Host ) );

	Agent->Release();
	AgentFactory->Release();
}

void After()
{
	if ( Host )
	{
		Host->Release();
	}
}

void CreateAndReleaseActions()
{
	WCHAR OwnPath[ MAX_PATH ];
	CFIXCC_ASSERT( GetModuleFileName(
		GetModuleHandle( L"testctl" ),
		OwnPath,
		_countof( OwnPath ) ) );

	ICfixTestModule *Module;

	BSTR BstrOwnPath = SysAllocString( OwnPath );
	CFIX_ASSERT_OK( 
		Host->LoadModule(
			BstrOwnPath,
			&Module ) );
	SysFreeString( BstrOwnPath );

	//
	// Action for entire module.
	//
	ICfixAction *ModuleAction;
	CFIX_ASSERT_OK( Module->CreateExecutionAction(
		0, 0, &ModuleAction ) );
	ModuleAction->Release();

	//
	// Action for specific fixture.
	//
	ICfixTestContainer *Container;
	CFIX_ASSERT_OK( Module->QueryInterface( 
		IID_ICfixTestContainer, ( PVOID* ) &Container ) );

	ICfixTestItem *Fixture;
	CFIX_ASSERT_OK( Container->GetItem( 0, &Fixture ) );
	Container->Release();

	ICfixAction *FixtureAction;
	CFIX_ASSERT_OK( Fixture->CreateExecutionAction(
		0, 0, &FixtureAction ) );
	Fixture->Release();
	FixtureAction->Release();

	Module->Release();
}

void RunFixturesFromTestlib10()
{
	WCHAR Path[ MAX_PATH ];
	CFIXCC_ASSERT( GetModuleFileName(
		GetModuleHandle( L"testctl" ),
		Path,
		_countof( Path ) ) );
	PathRemoveFileSpec( Path );
	PathAppend( Path,  L"testlib10.dll" );

	ICfixTestModule *Module;

	BSTR BstrPath = SysAllocString( Path );
	CFIX_ASSERT_OK( Host->LoadModule( BstrPath, &Module ) );
	SysFreeString( BstrPath );

	for ( ULONG Flags = 0; Flags <= EVENT_SINK_FAIL_PROCESS_SINK; Flags++ )
	{
		//
		// Action for entire module.
		//
		ICfixAction *ModuleAction;
		CFIX_ASSERT_OK( Module->CreateExecutionAction(
			0, 0, &ModuleAction ) );

		//
		// Reuse action.
		//
		for ( ULONG RunFlags = 0; 
			  RunFlags <= CFIXCTL_ACTION_COM_NEUTRAL; 
			  RunFlags++ )
		{
			EventSink Sink( Flags );

			CFIXCC_ASSERT_EQUALS( S_FALSE, ModuleAction->Stop() );
			HRESULT Hr = ModuleAction->Run( &Sink, RunFlags );
			CFIXCC_ASSERT_EQUALS( S_FALSE, ModuleAction->Stop() );

			if ( Flags & EVENT_SINK_FAIL_PROCESS_SINK )
			{
				CFIXCC_ASSERT_EQUALS( E_FAIL, Hr );
			}
			else
			{
				CFIX_ASSERT_OK( Hr );
			}

			ULONG TestCases;
			CFIXCC_ASSERT_EQUALS( E_POINTER, ModuleAction->GetTestCaseCount( NULL ) );
			CFIX_ASSERT_OK( ModuleAction->GetTestCaseCount( &TestCases ) );
			CFIXCC_ASSERT_EQUALS( 2UL, TestCases );

			if ( Flags == 0 )
			{
				CFIXCC_ASSERT( Sink.CallbackMask & 8 ); // Log.
				CFIXCC_ASSERT_EQUALS( 
					8UL + 256UL + 512UL + 1024UL + 2048UL + 4096UL + 8192UL, 
					Sink.CallbackMask );
			}

			CFIXCC_ASSERT_EQUALS( 0UL, Sink.RefCount );
		}

		ModuleAction->Release();
	}

	Module->Release();
}

void RunSingleTestCaseFromTestlib11()
{
	WCHAR Path[ MAX_PATH ];
	CFIXCC_ASSERT( GetModuleFileName(
		GetModuleHandle( L"testctl" ),
		Path,
		_countof( Path ) ) );
	PathRemoveFileSpec( Path );
	PathAppend( Path,  L"testlib11.dll" );

	ICfixTestModule *Module;

	BSTR BstrPath = SysAllocString( Path );
	CFIX_ASSERT_OK( Host->LoadModule( BstrPath, &Module ) );
	SysFreeString( BstrPath );

	ICfixTestItem *Fixture;
	CFIX_ASSERT_OK( Module->GetItem( 0, &Fixture ) );
	Module->Release();

	BSTR Name;
	CFIX_ASSERT_OK( Fixture->GetName( &Name ) );
	CFIXCC_ASSERT_EQUALS( L"TestExecActionDummy", ( PCWSTR ) Name );
	SysFreeString( Name );

	ICfixTestContainer *FixtureContainer;
	CFIX_ASSERT_OK( Fixture->QueryInterface(
		IID_ICfixTestContainer, ( PVOID* ) &FixtureContainer ) );
	Fixture->Release();

	ICfixTestItem *TestCase;
	CFIX_ASSERT_OK( FixtureContainer->GetItem( 1, &TestCase ) );
	FixtureContainer->Release();

	CFIX_ASSERT_OK( TestCase->GetName( &Name ) );
	CFIXCC_ASSERT_EQUALS( L"Two", ( PCWSTR ) Name );
	SysFreeString( Name );

	ICfixAction *Action;
	CFIX_ASSERT_OK( TestCase->CreateExecutionAction(
		0, 0, &Action ) );
	TestCase->Release();

	EventSink Sink( 0 );
	CFIX_ASSERT_OK( Action->Run( &Sink, 0 ) );

	CFIXCC_ASSERT_EQUALS( 
		8UL + 256UL + 512UL + 1024UL + 2048UL + 4096UL + 8192UL, 
		Sink.CallbackMask );

	CFIXCC_ASSERT_EQUALS( 
		7UL, 
		Sink.CallbackCount );
	Action->Release();
}

void RunTestWithEventDll()
{
	WCHAR Path[ MAX_PATH ];
	CFIXCC_ASSERT( GetModuleFileName(
		GetModuleHandle( L"testctl" ),
		Path,
		_countof( Path ) ) );
	PathRemoveFileSpec( Path );
	PathAppend( Path,  L"testlib10.dll" );

	ICfixTestModule *Module;

	BSTR BstrPath = SysAllocString( Path );
	CFIX_ASSERT_OK( Host->LoadModule( BstrPath, &Module ) );
	SysFreeString( BstrPath );

	ICfixAction *Action;
	CFIX_ASSERT_OK( Module->CreateExecutionAction( 0, 0, &Action ) );
	Module->Release();

	CFIX_ASSERT_HRESULT( 
		CFIX_E_LOADING_EVENTDLL_FAILED,
		Action->RegisterEventDll(
			CComBSTR( L"notexistant.dll" ),
			NULL,
			0 ) );

	CFIX_ASSERT_HRESULT( 
		CFIX_E_MISSING_EVENT_SINK_EXPORT,
		Action->RegisterEventDll(
			CComBSTR( L"cfix.dll" ),
			NULL,
			0 ) );

	CFIX_ASSERT_HRESULT( 
		E_INVALIDARG,
		Action->RegisterEventDll(
			CComBSTR( L"cfixcons.dll" ),
			CComBSTR( L"invalidparms" ),
			0 ) );

	CFIX_ASSERT_OK(
		Action->RegisterEventDll(
			CComBSTR( L"cfixcons.dll" ),
			CComBSTR( L"" ),
			0 ) );

	CFIX_ASSERT_HRESULT( 
		E_UNEXPECTED,
		Action->RegisterEventDll(
			CComBSTR( L"cfixcons.dll" ),
			NULL,
			0 ) );

	EventSink Sink( 0 );
	CFIX_ASSERT_OK( Action->Run( 
		&Sink, 
		CFIXCTL_ACTION_COM_NEUTRAL ) );
	Action->Release();
}

void RunTestOnLargeStack()
{
	WCHAR Path[ MAX_PATH ];
	CFIXCC_ASSERT( GetModuleFileName(
		GetModuleHandle( L"testctl" ),
		Path,
		_countof( Path ) ) );
	PathRemoveFileSpec( Path );
	PathAppend( Path,  L"testlibstack.dll" );

	ICfixTestModule *Module;

	BSTR BstrPath = SysAllocString( Path );
	CFIX_ASSERT_OK( Host->LoadModule( BstrPath, &Module ) );
	SysFreeString( BstrPath );

	ICfixTestItem *Fixture;
	CFIX_ASSERT_OK( Module->GetItem( 1, &Fixture ) );
	Module->Release();

	BSTR Name;
	CFIX_ASSERT_OK( Fixture->GetName( &Name ) );
	CFIXCC_ASSERT_EQUALS( L"LargeStack", ( PCWSTR ) Name );
	SysFreeString( Name );

	ICfixAction *Action;
	CFIX_ASSERT_OK( Fixture->CreateExecutionAction( 0, 0, &Action ) );
	Fixture->Release();

	EventSink Sink( 0 );
	CFIX_ASSERT_OK( Action->Run( 
		&Sink, 
		CFIXCTL_ACTION_COM_NEUTRAL | CFIXCTL_ACTION_LARGE_STACK ) );
	Action->Release();
}

void RunTestOnHugeStack()
{
	WCHAR Path[ MAX_PATH ];
	CFIXCC_ASSERT( GetModuleFileName(
		GetModuleHandle( L"testctl" ),
		Path,
		_countof( Path ) ) );
	PathRemoveFileSpec( Path );
	PathAppend( Path,  L"testlibstack.dll" );

	ICfixTestModule *Module;

	BSTR BstrPath = SysAllocString( Path );
	CFIX_ASSERT_OK( Host->LoadModule( BstrPath, &Module ) );
	SysFreeString( BstrPath );

	ICfixTestItem *Fixture;
	CFIX_ASSERT_OK( Module->GetItem( 0, &Fixture ) );
	Module->Release();

	BSTR Name;
	CFIX_ASSERT_OK( Fixture->GetName( &Name ) );
	CFIXCC_ASSERT_EQUALS( L"HugeStack", ( PCWSTR ) Name );
	SysFreeString( Name );

	ICfixAction *Action;
	CFIX_ASSERT_OK( Fixture->CreateExecutionAction( 0, 0, &Action ) );
	Fixture->Release();

	EventSink Sink( 0 );
	CFIX_ASSERT_OK( Action->Run( 
		&Sink, 
		CFIXCTL_ACTION_COM_NEUTRAL | CFIXCTL_ACTION_HUGE_STACK ) );
	Action->Release();
}

CFIX_BEGIN_FIXTURE( TestExecAction )
	CFIX_FIXTURE_SETUP( SetUp )
	CFIX_FIXTURE_TEARDOWN( TearDown )
	CFIX_FIXTURE_BEFORE( Before )
	CFIX_FIXTURE_AFTER( After )

	CFIX_FIXTURE_ENTRY( CreateAndReleaseActions )
	CFIX_FIXTURE_ENTRY( RunFixturesFromTestlib10 )
	CFIX_FIXTURE_ENTRY( RunSingleTestCaseFromTestlib11 )

	CFIX_FIXTURE_ENTRY( RunTestOnLargeStack )
	CFIX_FIXTURE_ENTRY( RunTestOnHugeStack )

	CFIX_FIXTURE_ENTRY( RunTestWithEventDll )
CFIX_END_FIXTURE()
