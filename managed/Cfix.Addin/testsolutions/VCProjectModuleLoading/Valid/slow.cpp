#include <cfix.h>

static void __stdcall Slow()
{
	for ( int i = 0; i < 400; i++ )
	{
		Sleep( 10 );
		CFIX_ASSERT( !"Fail" );
		CFIX_LOG( "Lap %d", i );
	}
}

static void __stdcall Slow2()
{
	for ( int i = 0; i < 400; i++ )
	{
		Sleep( 5000 );

		MessageBox( NULL, "test", "test", 0 );

		CFIX_LOG( "Lap %d", i );
	}
}

CFIX_BEGIN_FIXTURE( Slow )
	CFIX_FIXTURE_ENTRY( Slow )
	CFIX_FIXTURE_ENTRY( Slow2 )
CFIX_END_FIXTURE()