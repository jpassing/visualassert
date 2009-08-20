#include "StdAfx.h"
#include <cfix.h>
static void CFIXCALLTYPE TestBase()
{
	CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
}

CFIX_BEGIN_FIXTURE(Base)
	CFIX_FIXTURE_ENTRY(TestBase)
CFIX_END_FIXTURE()

