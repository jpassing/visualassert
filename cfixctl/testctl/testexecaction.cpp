/*----------------------------------------------------------------------
 * Purpose:
 *		Test ExecutionAction class.
 *
 * Copyright:
 *		2008, Johannes Passing (passing at users.sourceforge.net)
 *
 * This file is part of cfix.
 *
 * cfix is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * cfix is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with cfix.  If not, see <http://www.gnu.org/licenses/>.
 */

#include <testctlp.h>

#define EVENT_SINK_FAIL_PROCESS_SINK	1

#pragma warning( disable: 4100 ) // Unreferenced argument.

class EventSink : 
	public ICfixTestCaseEventSink,
	public ICfixFixtureEventSink,
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
			 InlineIsEqualGUID( Iid, IID_ICfixTestCaseEventSink ) )
		{
			*Ptr = static_cast< ICfixTestCaseEventSink* >( this );
			Hr = S_OK;

			this->AddRef();
		}
		else if ( InlineIsEqualGUID( Iid, IID_ICfixFixtureEventSink ) )
		{
			*Ptr = static_cast< ICfixFixtureEventSink* >( this );
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
	 * ICfixTestCaseEventSink.
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
	 * ICfixFixtureEventSink.
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

	STDMETHOD( GetTestCaseEventSink )(
		__in ULONG TestCaseOrdinal,
		__in ULONG ThreadId,
		__out ICfixTestCaseEventSink **Sink
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

	STDMETHOD( GetFixtureEventSink )(
		__in ICfixTestModule *Module,
		__in ULONG FixtureOrdinal,
		__out ICfixFixtureEventSink **Sink
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

	CFIXCC_ASSERT_OK( Exports.GetClassObject( 
		CLSID_LocalAgent, IID_IClassFactory, ( PVOID* ) &AgentFactory ) );
	CFIX_ASSUME( AgentFactory );

	ICfixAgent *Agent;
	CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
		NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
	CFIX_ASSUME( Agent );

	CFIXCC_ASSERT_OK( Agent->CreateHost( 
		TESTCTLP_OWN_ARCHITECTURE,
		CLSCTX_INPROC_SERVER,
		0,
		0,
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
	CFIXCC_ASSERT_OK( 
		Host->LoadModule(
			OwnPath,
			&Module ) );

	//
	// Action for entire module.
	//
	ICfixAction *ModuleAction;
	CFIXCC_ASSERT_OK( Module->CreateExecutionAction(
		0, 0, &ModuleAction ) );
	ModuleAction->Release();

	//
	// Action for specific fixture.
	//
	ICfixTestContainer *Container;
	CFIXCC_ASSERT_OK( Module->QueryInterface( 
		IID_ICfixTestContainer, ( PVOID* ) &Container ) );

	ICfixTestItem *Fixture;
	CFIXCC_ASSERT_OK( Container->GetItem( 0, &Fixture ) );
	Container->Release();

	ICfixAction *FixtureAction;
	CFIXCC_ASSERT_OK( Fixture->CreateExecutionAction(
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
	CFIXCC_ASSERT_OK( Host->LoadModule( Path, &Module ) );

	for ( ULONG Flags = 0; Flags <= EVENT_SINK_FAIL_PROCESS_SINK; Flags++ )
	{
		//
		// Action for entire module.
		//
		ICfixAction *ModuleAction;
		CFIXCC_ASSERT_OK( Module->CreateExecutionAction(
			0, 0, &ModuleAction ) );

		EventSink Sink( Flags );

		CFIXCC_ASSERT_EQUALS( S_FALSE, ModuleAction->Stop() );
		HRESULT Hr = ModuleAction->Run( &Sink );
		CFIXCC_ASSERT_EQUALS( S_FALSE, ModuleAction->Stop() );

		if ( Flags & EVENT_SINK_FAIL_PROCESS_SINK )
		{
			CFIXCC_ASSERT_EQUALS( E_FAIL, Hr );
		}
		else
		{
			CFIXCC_ASSERT_OK( Hr );
		}

		ModuleAction->Release();

		if ( Flags == 0 )
		{
			CFIXCC_ASSERT( Sink.CallbackMask & 8 ); // Log.
			CFIXCC_ASSERT_EQUALS( 
				8UL + 256UL + 512UL + 1024UL + 2048UL, 
				Sink.CallbackMask );
		}

		CFIXCC_ASSERT_EQUALS( 0UL, Sink.RefCount );
	}

	Module->Release();
}

CFIX_BEGIN_FIXTURE( TestExecAction )
	CFIX_FIXTURE_SETUP( SetUp )
	CFIX_FIXTURE_TEARDOWN( TearDown )
	CFIX_FIXTURE_BEFORE( Before )
	CFIX_FIXTURE_AFTER( After )

	CFIX_FIXTURE_ENTRY( CreateAndReleaseActions )
	CFIX_FIXTURE_ENTRY( RunFixturesFromTestlib10 )
CFIX_END_FIXTURE()
