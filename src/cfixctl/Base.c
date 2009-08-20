#include <cfix.h>
static void CFIXCALLTYPE Test()
{
	CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
}

CFIX_BEGIN_FIXTURE(Base)
	CFIX_FIXTURE_ENTRY(Test)
CFIX_END_FIXTURE()

