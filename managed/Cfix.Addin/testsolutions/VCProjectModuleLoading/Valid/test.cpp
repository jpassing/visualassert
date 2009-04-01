#include <cfix.h>

static void __stdcall Dummy()
{
	//CFIX_ASSERT( !"fail" );
}

static void __stdcall Dummy2()
{}

CFIX_BEGIN_FIXTURE( Valid )
	CFIX_FIXTURE_SETUP( Dummy )
	CFIX_FIXTURE_ENTRY( Dummy2 )
CFIX_END_FIXTURE()