/*----------------------------------------------------------------------
 * Purpose:
 *		Fixture execution context.
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

#include "cfixctlp.h"

C_ASSERT( CfixctlDispositionContinue	== CfixContinue );
C_ASSERT( CfixctlDispositionBreak		== CfixBreak );
C_ASSERT( CfixctlDispositionBreakAlways == CfixBreakAlways );
C_ASSERT( CfixctlDispositionAbort		== CfixAbort );

typedef struct _CFIXCTLP_EXEC_CONTEXT
{
	CFIX_EXECUTION_CONTEXT Base;

	volatile LONG ReferenceCount;

	ICfixTestModule *Module;

	ICfixProcessEventSink *ProcessSink;
	ICfixFixtureEventSink *FixtureSink;		// May be NULL.
	ICfixTestCaseEventSink *TestCaseSink;	// May be NULL.
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
	if ( 0 == InterlockedDecrement( &Context->ReferenceCount ) )
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
	__in ULONG MainThreadId,
	__in CFIX_EVENT_TYPE EventType
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;

	
	UNREFERENCED_PARAMETER( MainThreadId );
	
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
			VERIFY( SUCCEEDED( Context->ProcessSink->Notification(
				CFIXCTL_E_QUERY_DEF_DISP_FAILED ) ) );
			return CfixBreak;
		}

	case CfixEventUncaughtException:
		if ( SUCCEEDED( Sink->QueryDefaultUnhandledExceptionDisposition( &Disp ) ) )
		{
			return ( CFIX_REPORT_DISPOSITION )Disp;
		}
		else
		{
			VERIFY( SUCCEEDED( Context->ProcessSink->Notification(
				CFIXCTL_E_QUERY_DEF_DISP_FAILED ) ) );
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
	__in ULONG MainThreadId,
	__in PCFIX_TESTCASE_EXECUTION_EVENT Event
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;

	
	UNREFERENCED_PARAMETER( MainThreadId );
	
	ICfixReportEventSink *Sink = CfixctlsGetEffectiveReportSink( Context );
	ASSERT( Sink != NULL );
	if ( Sink == NULL )
	{
		return CfixBreak;
	}

	CFIXCTL_REPORT_DISPOSITION Disp;
	HRESULT Hr;

	switch ( Event->Type )
	{
	case CfixEventFailedAssertion:
		{
			BSTR Routine	= SysAllocString( Event->Info.FailedAssertion.Routine );
			BSTR Expression = SysAllocString( Event->Info.FailedAssertion.Expression );
			BSTR File		= SysAllocString( Event->Info.FailedAssertion.File );

			Hr = Sink->FailedAssertion(
				NULL,
				Routine,
				File,
				Expression,
				Event->Info.FailedAssertion.Line,
				Event->Info.FailedAssertion.LastError,
				0,
				0,
				NULL,	// TODO.
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
				NULL,	// TODO.
				&Disp );
			break;
		}

	case CfixEventInconclusiveness:	
		{
			BSTR Message = SysAllocString( Event->Info.Inconclusiveness.Message );
			Hr = Sink->Inconclusive(
				Message,
				0,
				NULL );	// TODO.
			SysFreeString( Message );
			
			Disp = CfixctlDispositionContinue;
			break;
		}

	case CfixEventLog:
		{
			BSTR Message = SysAllocString( Event->Info.Inconclusiveness.Message );
			Hr = Sink->Log(
				Message,
				0,
				NULL );	// TODO.
			SysFreeString( Message );

			Disp = CfixctlDispositionContinue;
			break;
		}

	default:
		ASSERT( !"Unrecognized type" );
		Hr = E_FAIL;
		Disp = CfixctlDispositionBreak;
	}

	if ( SUCCEEDED( Hr ) )
	{
		return ( CFIX_REPORT_DISPOSITION ) Disp;
	}
	else
	{
		VERIFY( SUCCEEDED( Context->ProcessSink->Notification(
			CFIXCTL_E_REPORT_EVENT_FAILED ) ) );
		return CfixBreak;
	}
}

static VOID CfixctlsExecCtxBeforeFixtureStart(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in PCFIX_FIXTURE Fixture
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;

	
	ASSERT( Context->TestCaseSink == NULL );
	ASSERT( Context->FixtureSink == NULL );

	UNREFERENCED_PARAMETER( MainThreadId );
	
	//
	// Obtain new fixture sink.
	//
	if ( SUCCEEDED( Context->ProcessSink->GetFixtureEventSink(
		Context->Module,
		CfixctlsGetFixtureOrdinal( Fixture ),
		&Context->FixtureSink ) ) )
	{
		VERIFY( SUCCEEDED( Context->FixtureSink->BeforeFixtureStart() ) );
	}
	else
	{
		VERIFY( SUCCEEDED( Context->ProcessSink->Notification(
			CFIXCTL_E_NO_FIXTURE_SINK ) ) );
	}
}

static VOID CfixctlsExecCtxBeforeTestCaseStart(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in PCFIX_TEST_CASE TestCase
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;

	
	ASSERT( Context->TestCaseSink == NULL );
	ASSERT( Context->FixtureSink != NULL );

	UNREFERENCED_PARAMETER( MainThreadId );
	
	if ( ! Context->FixtureSink )
	{
		return;
	}

	//
	// Obtain new testcase sink.
	//
	if ( SUCCEEDED( Context->FixtureSink->GetTestCaseEventSink(
		CfixctlsGetTestCaseOrdinal( TestCase ),
		MainThreadId,
		&Context->TestCaseSink ) ) )
	{
		VERIFY( SUCCEEDED( Context->TestCaseSink->BeforeTestCaseStart() ) );
	}
	else
	{
		VERIFY( SUCCEEDED( Context->ProcessSink->Notification(
			CFIXCTL_E_NO_TESTCASE_SINK ) ) );
	}
}

static VOID CfixctlsExecCtxAfterFixtureFinish(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in PCFIX_FIXTURE Fixture,
	__in BOOL RanToCompletion
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;

	
	ASSERT( Context->TestCaseSink == NULL );
	ASSERT( Context->FixtureSink != NULL );

	UNREFERENCED_PARAMETER( MainThreadId );
	UNREFERENCED_PARAMETER( Fixture );

	if ( ! Context->FixtureSink )
	{
		return;
	}

	VERIFY( SUCCEEDED( Context->FixtureSink->AfterFixtureFinish(
		RanToCompletion ) ) );

	Context->FixtureSink->Release();
	Context->FixtureSink = NULL;
}


static VOID CfixctlsExecCtxAfterTestCaseFinish(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in PCFIX_TEST_CASE TestCase,
	__in BOOL RanToCompletion
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;

	
	ASSERT( Context->TestCaseSink != NULL );
	ASSERT( Context->FixtureSink != NULL );

	UNREFERENCED_PARAMETER( MainThreadId );
	UNREFERENCED_PARAMETER( TestCase );

	if ( ! Context->TestCaseSink )
	{
		return;
	}

	VERIFY( SUCCEEDED( Context->TestCaseSink->AfterTestCaseFinish(
		RanToCompletion ) ) );

	Context->TestCaseSink->Release();
	Context->TestCaseSink = NULL;
}

VOID CfixctlsExecCtxBeforeChildThreadStart(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in_opt PVOID ParentContext
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;

	
	ASSERT( Context->FixtureSink != NULL );

	UNREFERENCED_PARAMETER( MainThreadId );
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

	VERIFY( SUCCEEDED( Sink->BeforeChildThreadStart(
		GetCurrentThreadId() ) ) );
}

VOID CfixctlsExecCtxAfterChildThreadFinish(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in_opt PVOID ParentContext
	)
{
	PCFIXCTLP_EXEC_CONTEXT Context = ( PCFIXCTLP_EXEC_CONTEXT ) This;

	
	ASSERT( Context->FixtureSink != NULL );

	UNREFERENCED_PARAMETER( MainThreadId );
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

	VERIFY( SUCCEEDED( Sink->AfterChildThreadFinish(
		GetCurrentThreadId() ) ) );
}

static HRESULT CfixctlsExecCtxCreateChildThread(
	__in struct _CFIX_EXECUTION_CONTEXT *This,
	__in ULONG MainThreadId,
	__out PVOID *ContextForChild
	)
{
	UNREFERENCED_PARAMETER( This );
	UNREFERENCED_PARAMETER( MainThreadId );
	UNREFERENCED_PARAMETER( ContextForChild );

	return S_OK;
}

static VOID CfixctlsExecCtxOnUnhandledException(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in PEXCEPTION_POINTERS ExcpPointers
	)
{
	UNREFERENCED_PARAMETER( This );
	UNREFERENCED_PARAMETER( MainThreadId );
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
	__out PCFIX_EXECUTION_CONTEXT *Context
	)
{
	PCFIXCTLP_EXEC_CONTEXT NewContext;

	if ( ! Module || ! ProcessSink || ! Context )
	{
		return E_INVALIDARG;
	}

	NewContext = ( PCFIXCTLP_EXEC_CONTEXT ) 
		malloc( sizeof( CFIXCTLP_EXEC_CONTEXT ) );
	if ( ! NewContext )
	{
		return E_OUTOFMEMORY;
	}

	ZeroMemory( NewContext, sizeof( CFIXCTLP_EXEC_CONTEXT ) );

	Module->AddRef();
	ProcessSink->AddRef();

	NewContext->ReferenceCount				= 1;
	NewContext->Module						= Module;
	NewContext->ProcessSink					= ProcessSink;

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