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

typedef enum CFIXCTLP_MESSAGE_TYPE
{
	CfixctlpReportEvent,
	CfixctlpQueryDefaultDisposition,
	CfixctlpBeforeFixtureStart,
	CfixctlpAfterFixtureFinish,
	CfixctlpBeforeTestCaseStart,
	CfixctlpAfterTestCaseFinish,
	CfixctlpCreateChildThread,
	CfixctlpBeforeChildThreadStart,
	CfixctlpAfterChildThreadFinish,
	CfixctlpOnUnhandledException
} CFIXCTLP_MESSAGE_TYPE;

typedef struct _CFIXCTLP_MESSAGE
{
	CFIXCTLP_MESSAGE_TYPE Type;
	union
	{
		struct
		{
			ULONG MainThreadId;
			CFIX_EVENT_TYPE EventType;
		} QueryDefaultDispositionRequest;

		struct
		{
			CFIX_REPORT_DISPOSITION Disposition;
		} QueryDefaultDispositionResponse;

		struct
		{
			ULONG MainThreadId;
			PCFIX_TESTCASE_EXECUTION_EVENT Event;
		} ReportEventRequest;

		struct
		{
			CFIX_REPORT_DISPOSITION Disposition;
		} ReportEventResponse;

		struct
		{
			ULONG MainThreadId;
			PCFIX_FIXTURE Fixture;
		} BeforeFixtureStartRequest;
		
		struct
		{
			HRESULT Hresult;
		} BeforeFixtureStartResponse;

		struct
		{
			ULONG MainThreadId;
			PCFIX_TEST_CASE TestCase;
		} BeforeTestCaseStartRequest;
		
		struct
		{
			HRESULT Hresult;
		} BeforeTestCaseStartResponse;

		struct
		{
			ULONG MainThreadId;
			PCFIX_FIXTURE Fixture;
			BOOL RanToCompletion;			
		} AfterFixtureFinishRequest;

		struct
		{
			ULONG MainThreadId;
			PCFIX_TEST_CASE TestCase;
			BOOL RanToCompletion;			
		} AfterTestCaseFinishRequest;

		struct
		{
			ULONG MainThreadId;
			PVOID ParentContext;
		} BeforeChildThreadStartRequest;

		struct
		{
			ULONG MainThreadId;
			PVOID ParentContext;
		} AfterChildThreadFinishRequest;

		struct
		{
			ULONG MainThreadId;
		} CreateChildThreadRequest;

		struct
		{
			HRESULT Hresult;
			PVOID ContextForChild;
		} CreateChildThreadResponse;

		struct
		{
			ULONG MainThreadId;
			PEXCEPTION_POINTERS ExcpPointers;
		} OnUnhandledExceptionRequest;
	} Data;
} CFIXCTLP_MESSAGE, *PCFIXCTLP_MESSAGE;

typedef struct _CFIXCTLP_THREADSWITCH_PROXY
{
	CFIX_EXECUTION_CONTEXT Base;

	//
	// The wrapped adapter which we pass all events to.
	//
	PCFIX_EXECUTION_CONTEXT Wrapped;

	volatile LONG ReferenceCount;

	struct
	{
		CRITICAL_SECTION HandoffLock;
		HANDLE MessageAvailableEvent;
		HANDLE MessageDoneEvent;
		PCFIXCTLP_MESSAGE Message;
	} Gate;
} CFIXCTLP_THREADSWITCH_PROXY, *PCFIXCTLP_THREADSWITCH_PROXY;

/*----------------------------------------------------------------------
 *
 * Thread switching/Message passing.
 *
 */

static VOID CfixctlsDispatchMessage(
	__in PCFIXCTLP_THREADSWITCH_PROXY Proxy,
	__inout PCFIXCTLP_MESSAGE Message 
	)
{
	ASSERT( Proxy );
	ASSERT( Message );

	switch ( Message->Type )
	{
	case CfixctlpReportEvent:
		Message->Data.ReportEventResponse.Disposition =
			Proxy->Wrapped->ReportEvent(
				Proxy->Wrapped,
				Message->Data.ReportEventRequest.MainThreadId,
				Message->Data.ReportEventRequest.Event );
		break;

	case CfixctlpQueryDefaultDisposition:
		Message->Data.QueryDefaultDispositionResponse.Disposition =
			Proxy->Wrapped->QueryDefaultDisposition(
				Proxy->Wrapped,
				Message->Data.QueryDefaultDispositionRequest.MainThreadId,
				Message->Data.QueryDefaultDispositionRequest.EventType );
		break;

	case CfixctlpBeforeFixtureStart:
		Message->Data.BeforeFixtureStartResponse.Hresult =
			Proxy->Wrapped->BeforeFixtureStart(
				Proxy->Wrapped,
				Message->Data.BeforeFixtureStartRequest.MainThreadId,
				Message->Data.BeforeFixtureStartRequest.Fixture );
		break;

	case CfixctlpAfterFixtureFinish:
		Proxy->Wrapped->AfterFixtureFinish(
			Proxy->Wrapped,
			Message->Data.AfterFixtureFinishRequest.MainThreadId,
			Message->Data.AfterFixtureFinishRequest.Fixture,
			Message->Data.AfterFixtureFinishRequest.RanToCompletion );
		break;

	case CfixctlpBeforeTestCaseStart:
		Message->Data.BeforeTestCaseStartResponse.Hresult =
			Proxy->Wrapped->BeforeTestCaseStart(
				Proxy->Wrapped,
				Message->Data.BeforeTestCaseStartRequest.MainThreadId,
				Message->Data.BeforeTestCaseStartRequest.TestCase );
		break;

	case CfixctlpAfterTestCaseFinish:
		Proxy->Wrapped->AfterTestCaseFinish(
			Proxy->Wrapped,
			Message->Data.AfterTestCaseFinishRequest.MainThreadId,
			Message->Data.AfterTestCaseFinishRequest.TestCase,
			Message->Data.AfterTestCaseFinishRequest.RanToCompletion );
		break;

	case CfixctlpCreateChildThread:
		Message->Data.CreateChildThreadResponse.Hresult =
			Proxy->Wrapped->CreateChildThread(
				Proxy->Wrapped,
				Message->Data.CreateChildThreadRequest.MainThreadId,
				&Message->Data.CreateChildThreadResponse.ContextForChild );
		break;

	case CfixctlpBeforeChildThreadStart:
		Proxy->Wrapped->BeforeChildThreadStart(
			Proxy->Wrapped,
			Message->Data.BeforeChildThreadStartRequest.MainThreadId,
			Message->Data.BeforeChildThreadStartRequest.ParentContext );
		break;

	case CfixctlpAfterChildThreadFinish:
		Proxy->Wrapped->AfterChildThreadFinish(
			Proxy->Wrapped,
			Message->Data.AfterChildThreadFinishRequest.MainThreadId,
			Message->Data.AfterChildThreadFinishRequest.ParentContext );
		break;

	case CfixctlpOnUnhandledException:
		Proxy->Wrapped->OnUnhandledException(
			Proxy->Wrapped,
			Message->Data.OnUnhandledExceptionRequest.MainThreadId,
			Message->Data.OnUnhandledExceptionRequest.ExcpPointers );
		break;

	default:
		ASSERT( !"Invalid message type" );
	}
}

static VOID CfixctlsTransact(
	__in PCFIXCTLP_THREADSWITCH_PROXY Proxy,
	__inout PCFIXCTLP_MESSAGE Message 
	)
{
	EnterCriticalSection( &Proxy->Gate.HandoffLock );

	//
	// It is our turn to hand off a message through the gate.
	//
	ASSERT( Proxy->Gate.Message == NULL );
	Proxy->Gate.Message = Message;

	( VOID ) SignalObjectAndWait( 
		Proxy->Gate.MessageAvailableEvent,
		Proxy->Gate.MessageDoneEvent,
		INFINITE,
		FALSE );

	//
	// Response is now available.
	//

	Proxy->Gate.Message = NULL;
	
	LeaveCriticalSection( &Proxy->Gate.HandoffLock );
}

static HRESULT CfixctlsDispatch(
	__in PCFIXCTLP_THREADSWITCH_PROXY Proxy,
	__in HANDLE StopHandle
	)
{
	for ( ;; )
	{
		HANDLE Objects[] = { StopHandle, Proxy->Gate.MessageAvailableEvent };
		DWORD WaitRes = WaitForMultipleObjects(
			_countof( Objects ),
			Objects,
			FALSE,
			INFINITE );
		if ( WaitRes == WAIT_OBJECT_0 )
		{
			return S_OK;
		}
		else if ( WaitRes == WAIT_OBJECT_0 + 1 )
		{
			//
			// Message available.
			//
			ASSERT( Proxy->Gate.Message != NULL );

			CfixctlsDispatchMessage( 
				Proxy,
				Proxy->Gate.Message );

			if ( ! SetEvent( Proxy->Gate.MessageDoneEvent ) )
			{
				return HRESULT_FROM_WIN32( GetLastError() );
			}
		}
		else
		{
			return HRESULT_FROM_WIN32( GetLastError() );
		}
	}
}

/*----------------------------------------------------------------------
 *
 * Basic Methods.
 *
 */

static VOID CfixctlsThreadSwitchProxyReference(
	__in PCFIX_EXECUTION_CONTEXT This
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	InterlockedIncrement( &Proxy->ReferenceCount );
}

static VOID CfixctlsThreadSwitchProxyDereference(
	__in PCFIX_EXECUTION_CONTEXT This
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	LONG Refs = InterlockedDecrement( &Proxy->ReferenceCount );
	COM_TRACE( ( L"Dereference ThrSwitchAdapter: %d", Refs ) );
	if ( 0 == Refs )
	{
		Proxy->Wrapped->Dereference( Proxy->Wrapped );

		DeleteCriticalSection( &Proxy->Gate.HandoffLock );
		VERIFY( CloseHandle( Proxy->Gate.MessageAvailableEvent ) );
		VERIFY( CloseHandle( Proxy->Gate.MessageDoneEvent ) );

		ASSERT( Proxy->Gate.Message == NULL );

		free( Proxy );
	}
}

/*----------------------------------------------------------------------
 *
 * Delegating Methods.
 *
 */

static CFIX_REPORT_DISPOSITION CfixctlsThreadSwitchProxyQueryDefaultDisposition(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in CFIX_EVENT_TYPE EventType
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	CFIXCTLP_MESSAGE Message;
	
	Message.Type												= CfixctlpQueryDefaultDisposition;
	Message.Data.QueryDefaultDispositionRequest.MainThreadId	= MainThreadId;
	Message.Data.QueryDefaultDispositionRequest.EventType		= EventType;

	CfixctlsTransact( Proxy, &Message );

	return Message.Data.QueryDefaultDispositionResponse.Disposition;
}

static CFIX_REPORT_DISPOSITION CfixctlsThreadSwitchProxyReportEvent(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in PCFIX_TESTCASE_EXECUTION_EVENT Event
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	CFIXCTLP_MESSAGE Message;
	
	Message.Type												= CfixctlpReportEvent;
	Message.Data.ReportEventRequest.MainThreadId				= MainThreadId;
	Message.Data.ReportEventRequest.Event						= Event;

	CfixctlsTransact( Proxy, &Message );

	return Message.Data.ReportEventResponse.Disposition;
}

static HRESULT CfixctlsThreadSwitchProxyBeforeFixtureStart(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in PCFIX_FIXTURE Fixture
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	CFIXCTLP_MESSAGE Message;
	
	Message.Type												= CfixctlpBeforeFixtureStart;
	Message.Data.BeforeFixtureStartRequest.MainThreadId			= MainThreadId;
	Message.Data.BeforeFixtureStartRequest.Fixture				= Fixture;

	CfixctlsTransact( Proxy, &Message );

	return Message.Data.BeforeFixtureStartResponse.Hresult;
}

static HRESULT CfixctlsThreadSwitchProxyBeforeTestCaseStart(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in PCFIX_TEST_CASE TestCase
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	CFIXCTLP_MESSAGE Message;
	
	Message.Type												= CfixctlpBeforeTestCaseStart;
	Message.Data.BeforeTestCaseStartRequest.MainThreadId		= MainThreadId;
	Message.Data.BeforeTestCaseStartRequest.TestCase			= TestCase;

	CfixctlsTransact( Proxy, &Message );

	return Message.Data.BeforeTestCaseStartResponse.Hresult;
}

static VOID CfixctlsThreadSwitchProxyAfterFixtureFinish(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in PCFIX_FIXTURE Fixture,
	__in BOOL RanToCompletion
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	CFIXCTLP_MESSAGE Message;
	
	Message.Type												= CfixctlpAfterFixtureFinish;
	Message.Data.AfterFixtureFinishRequest.MainThreadId			= MainThreadId;
	Message.Data.AfterFixtureFinishRequest.Fixture				= Fixture;
	Message.Data.AfterFixtureFinishRequest.RanToCompletion		= RanToCompletion;

	CfixctlsTransact( Proxy, &Message );
}

static VOID CfixctlsThreadSwitchProxyAfterTestCaseFinish(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in PCFIX_TEST_CASE TestCase,
	__in BOOL RanToCompletion
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	CFIXCTLP_MESSAGE Message;
	
	Message.Type												= CfixctlpAfterTestCaseFinish;
	Message.Data.AfterTestCaseFinishRequest.MainThreadId		= MainThreadId;
	Message.Data.AfterTestCaseFinishRequest.TestCase			= TestCase;
	Message.Data.AfterTestCaseFinishRequest.RanToCompletion		= RanToCompletion;

	CfixctlsTransact( Proxy, &Message );
}

static VOID CfixctlsThreadSwitchProxyBeforeChildThreadStart(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in_opt PVOID ParentContext
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	CFIXCTLP_MESSAGE Message;
	
	Message.Type												= CfixctlpBeforeChildThreadStart;
	Message.Data.BeforeChildThreadStartRequest.MainThreadId		= MainThreadId;
	Message.Data.BeforeChildThreadStartRequest.ParentContext	= ParentContext;

	CfixctlsTransact( Proxy, &Message );
}

static VOID CfixctlsThreadSwitchProxyAfterChildThreadFinish(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in_opt PVOID ParentContext
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	CFIXCTLP_MESSAGE Message;
	
	Message.Type												= CfixctlpAfterChildThreadFinish;
	Message.Data.AfterChildThreadFinishRequest.MainThreadId		= MainThreadId;
	Message.Data.AfterChildThreadFinishRequest.ParentContext	= ParentContext;

	CfixctlsTransact( Proxy, &Message );
}

static HRESULT CfixctlsThreadSwitchProxyCreateChildThread(
	__in struct _CFIX_EXECUTION_CONTEXT *This,
	__in ULONG MainThreadId,
	__out PVOID *ContextForChild
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	CFIXCTLP_MESSAGE Message;
	
	Message.Type												= CfixctlpCreateChildThread;
	Message.Data.CreateChildThreadRequest.MainThreadId			= MainThreadId;

	CfixctlsTransact( Proxy, &Message );

	*ContextForChild = Message.Data.CreateChildThreadResponse.ContextForChild;
	return Message.Data.CreateChildThreadResponse.Hresult;
}

static VOID CfixctlsThreadSwitchProxyOnUnhandledException(
	__in PCFIX_EXECUTION_CONTEXT This,
	__in ULONG MainThreadId,
	__in PEXCEPTION_POINTERS ExcpPointers
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY Proxy = ( PCFIXCTLP_THREADSWITCH_PROXY ) This;
	CFIXCTLP_MESSAGE Message;
	
	Message.Type												= CfixctlpOnUnhandledException;
	Message.Data.OnUnhandledExceptionRequest.MainThreadId		= MainThreadId;
	Message.Data.OnUnhandledExceptionRequest.ExcpPointers		= ExcpPointers;

	CfixctlsTransact( Proxy, &Message );
}

/*----------------------------------------------------------------------
 *
 * Internals.
 *
 */

HRESULT CfixctlpCreateThreadSwitchProxy(
	__in PCFIX_EXECUTION_CONTEXT Context,
	__out PCFIX_EXECUTION_CONTEXT *Proxy
	)
{
	PCFIXCTLP_THREADSWITCH_PROXY NewAdapter;
	
	if ( ! Context || ! Proxy )
	{
		return E_INVALIDARG;
	}

	HANDLE MessageAvailableEvent = CreateEvent( NULL, FALSE, FALSE, NULL );
	if ( MessageAvailableEvent == NULL )
	{
		return HRESULT_FROM_WIN32( GetLastError() );
	}

	HANDLE MessageDoneEvent = CreateEvent( NULL, FALSE, FALSE, NULL );
	if ( MessageAvailableEvent == NULL )
	{
		VERIFY( CloseHandle( MessageAvailableEvent ) );
		return HRESULT_FROM_WIN32( GetLastError() );
	}
	
	NewAdapter = ( PCFIXCTLP_THREADSWITCH_PROXY ) 
		malloc( sizeof( CFIXCTLP_THREADSWITCH_PROXY ) );
	if ( ! NewAdapter )
	{
		return E_OUTOFMEMORY;
	}

	ZeroMemory( NewAdapter, sizeof( CFIXCTLP_THREADSWITCH_PROXY ) );

	Context->Reference( Context );

	NewAdapter->ReferenceCount				= 1;
	NewAdapter->Wrapped						= Context;

	NewAdapter->Gate.MessageAvailableEvent	= MessageAvailableEvent;
	NewAdapter->Gate.MessageDoneEvent		= MessageDoneEvent;
	InitializeCriticalSection( &NewAdapter->Gate.HandoffLock );
	
	NewAdapter->Base.Version				= CFIX_TEST_CONTEXT_VERSION;
	NewAdapter->Base.ReportEvent			= CfixctlsThreadSwitchProxyReportEvent;
	NewAdapter->Base.QueryDefaultDisposition= CfixctlsThreadSwitchProxyQueryDefaultDisposition;
	NewAdapter->Base.BeforeFixtureStart		= CfixctlsThreadSwitchProxyBeforeFixtureStart;
	NewAdapter->Base.AfterFixtureFinish		= CfixctlsThreadSwitchProxyAfterFixtureFinish;
	NewAdapter->Base.BeforeTestCaseStart	= CfixctlsThreadSwitchProxyBeforeTestCaseStart;
	NewAdapter->Base.AfterTestCaseFinish	= CfixctlsThreadSwitchProxyAfterTestCaseFinish;
	NewAdapter->Base.CreateChildThread		= CfixctlsThreadSwitchProxyCreateChildThread;
	NewAdapter->Base.BeforeChildThreadStart	= CfixctlsThreadSwitchProxyBeforeChildThreadStart;
	NewAdapter->Base.AfterChildThreadFinish	= CfixctlsThreadSwitchProxyAfterChildThreadFinish;
	NewAdapter->Base.OnUnhandledException	= CfixctlsThreadSwitchProxyOnUnhandledException;
	NewAdapter->Base.Reference				= CfixctlsThreadSwitchProxyReference;
	NewAdapter->Base.Dereference			= CfixctlsThreadSwitchProxyDereference;

	*Proxy = &NewAdapter->Base;

	return S_OK;
}

HRESULT CfixctlpServiceThreadSwitchProxy(
	__in PCFIX_EXECUTION_CONTEXT Context,
	__in HANDLE WorkerThread
	)
{
	if ( ! Context || ! WorkerThread )
	{
		return E_INVALIDARG;
	}

	return CfixctlsDispatch(
		( PCFIXCTLP_THREADSWITCH_PROXY ) Context,
		WorkerThread );
}
