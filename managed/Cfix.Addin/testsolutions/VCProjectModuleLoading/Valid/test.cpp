#include <cfix.h>

static void __stdcall Dummy()
{}

CFIX_BEGIN_FIXTURE( Valid )
	CFIX_FIXTURE_SETUP( Dummy )
	CFIX_FIXTURE_ENTRY( Dummy )
CFIX_END_FIXTURE()