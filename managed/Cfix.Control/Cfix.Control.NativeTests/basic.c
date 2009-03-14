#include <cfix.h>
#include <crtdbg.h>

static DWORD CALLBACK ThreadProc( PVOID Args )
{
	UNREFERENCED_PARAMETER( Args );
	CFIX_LOG( "test" );
	return 0;
}

static void __stdcall Log()
{
	HANDLE Thr;
	
	CFIX_LOG( "test" );

	Thr = CfixCreateThread( NULL, 0, ThreadProc, NULL, 0, 0 );
	CFIX_ASSERT( Thr );
	WaitForSingleObject( Thr, INFINITE );
}

static void __stdcall Nop01()
{
}

static void __stdcall Nop02()
{
}

static void __stdcall Inconclusive()
{
	CFIX_INCONCLUSIVE( NULL );
}

static void __stdcall StopMe()
{
	CFIX_LOG( "Stop me now" );
	CFIX_ASSERT( !"Seize this assert to abort" );
	_ASSERTE( !"Should have been stopped" );
}

static void __stdcall FailingAssertion()
{
	SetLastError( 5 );
	CFIX_ASSERT( FALSE );
}

static void __stdcall SucceedingAssertionAndLog()
{
	CFIX_ASSERT( TRUE );
	CFIX_LOG( L"test" );
}

static void __stdcall Throw()
{
	RaiseException( 0xCAFEBABE, 0, 0, NULL );
}

//
// N.B. Alphabetic order, Fail must be 1st.
//

CFIX_BEGIN_FIXTURE( Fail )
	CFIX_FIXTURE_ENTRY( FailingAssertion )
	CFIX_FIXTURE_ENTRY( Throw )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( Inconclusive )
	CFIX_FIXTURE_ENTRY( Inconclusive )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( LogTwice )
	CFIX_FIXTURE_ENTRY( Log )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( StopInTest )
	CFIX_FIXTURE_ENTRY( StopMe )
	CFIX_FIXTURE_ENTRY( Nop01 )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( StopInAfter )
	CFIX_FIXTURE_AFTER( StopMe )
	CFIX_FIXTURE_ENTRY( Nop01 )
	CFIX_FIXTURE_ENTRY( Nop02 )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( StopInBefore )
	CFIX_FIXTURE_BEFORE( StopMe )
	CFIX_FIXTURE_ENTRY( Nop01 )
	CFIX_FIXTURE_ENTRY( Nop02 )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( StopInSetup )
	CFIX_FIXTURE_SETUP( StopMe )
	CFIX_FIXTURE_ENTRY( Nop01 )
	CFIX_FIXTURE_ENTRY( Nop02 )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( StopInTeardown )
	CFIX_FIXTURE_TEARDOWN( StopMe )
	CFIX_FIXTURE_ENTRY( Nop01 )
	CFIX_FIXTURE_ENTRY( Nop02 )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( SinkTest )
	CFIX_FIXTURE_SETUP( SucceedingAssertionAndLog )
	CFIX_FIXTURE_TEARDOWN( SucceedingAssertionAndLog )
	CFIX_FIXTURE_ENTRY( SucceedingAssertionAndLog )
CFIX_END_FIXTURE()