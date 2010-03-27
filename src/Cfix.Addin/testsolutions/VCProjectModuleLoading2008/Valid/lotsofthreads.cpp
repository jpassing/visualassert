#include <cfix.h>
#include <crtdbg.h>

static volatile BOOL Stop = FALSE;

static DWORD CALLBACK LogThreadProc( PVOID )
{
	while ( ! Stop )
	{
		CFIX_LOG( "Log on Thread %d", GetCurrentThreadId() );
		Sleep( 0 );
	}

	return 0;
}

static void __stdcall LogOnLotsOfThreads()
{
	HANDLE Threads[ 64 ];
	Stop = FALSE;
	for ( ULONG i = 0; i < _countof( Threads ); i++ )
	{
		Threads[ i ] = CfixCreateThread(
			NULL,
			0,
			LogThreadProc,
			NULL,
			0,
			NULL );
		CFIX_ASSERT( Threads[ i ] );
	}

	Sleep( 4000 );
	Stop = TRUE;
	WaitForMultipleObjects( _countof( Threads ), Threads, TRUE, INFINITE );

	for ( ULONG i = 0; i < _countof( Threads ); i++ )
	{
		CFIX_ASSERT( CloseHandle( Threads[ i ] ) );
	}
}


static DWORD CALLBACK AssertThreadProc( PVOID )
{
	CFIX_ASSERT( !"Fail" );

	_ASSERTE( !"Should not get here" );
	return 0;
}

static void __stdcall AssertOnLotsOfThreads()
{
	HANDLE Threads[ 64 ];
	for ( ULONG i = 0; i < _countof( Threads ); i++ )
	{
		Threads[ i ] = CfixCreateThread(
			NULL,
			0,
			AssertThreadProc,
			NULL,
			0,
			NULL );
		CFIX_ASSERT( Threads[ i ] );
	}

	WaitForMultipleObjects( _countof( Threads ), Threads, TRUE, INFINITE );
	
	for ( ULONG i = 0; i < _countof( Threads ); i++ )
	{
		CFIX_ASSERT( CloseHandle( Threads[ i ] ) );
	}
}

CFIX_BEGIN_FIXTURE( Threads )
	CFIX_FIXTURE_ENTRY( LogOnLotsOfThreads )
	CFIX_FIXTURE_ENTRY( AssertOnLotsOfThreads )
CFIX_END_FIXTURE()