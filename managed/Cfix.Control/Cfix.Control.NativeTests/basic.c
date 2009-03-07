#include <cfix.h>

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

static void __stdcall Inconclusive()
{
	CFIX_INCONCLUSIVE( NULL );
}

static void __stdcall FailingAssertion()
{
	SetLastError( 5 );
	CFIX_ASSERT( FALSE );
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

