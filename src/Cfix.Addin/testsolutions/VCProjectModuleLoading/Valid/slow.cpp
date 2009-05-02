#include <cfix.h>

static void __stdcall Slow()                 
{
	for ( int i = 0; i < 40; i++ )
	{
		Sleep( 10 );
		CFIX_ASSERT_MESSAGE( !"Fail", "sample %x", 0xF00 );
		CFIX_LOG( "Lap %d", i );
	}
}

static void __stdcall Slow2()
{
	for ( int i = 0; i < 10; i++ )
	{
		Sleep( 500 );

		//MessageBox( NULL, "test", "test", 0 );

		CFIX_LOG( "Lap %d", i );
	}
}

CFIX_BEGIN_FIXTURE( Slow )
	CFIX_FIXTURE_ENTRY( Slow )
	CFIX_FIXTURE_ENTRY( Slow2 )
CFIX_END_FIXTURE()

