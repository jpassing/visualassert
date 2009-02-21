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

CFIX_BEGIN_FIXTURE( LogTwice )
	CFIX_FIXTURE_ENTRY( Log )
CFIX_END_FIXTURE()