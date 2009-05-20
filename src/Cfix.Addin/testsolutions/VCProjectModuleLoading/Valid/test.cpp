#include <cfixcc.h>

static void __stdcall Dummy()
{
	//CFIX_ASSERT( !"fail" );
}





static void __stdcall Dummy2()
{
	CFIXCC_ASSERT_EQUALS( 1, 2, "hello" );
}

static void __stdcall LeakCS()
{
	CRITICAL_SECTION Cs;
	InitializeCriticalSection( &Cs );
	EnterCriticalSection( &Cs );
}

static void __stdcall Av()
{
	int* a = NULL;
	*a = 1;
}

CFIX_BEGIN_FIXTURE( Valid )
	CFIX_FIXTURE_SETUP( Dummy )
	CFIX_FIXTURE_ENTRY( Dummy2 )
	CFIX_FIXTURE_ENTRY( Av )
	//CFIX_FIXTURE_ENTRY( LeakCS )
CFIX_END_FIXTURE()