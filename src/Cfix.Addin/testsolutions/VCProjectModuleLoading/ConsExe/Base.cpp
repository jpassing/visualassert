#include "StdAfx.h"
#include <cfix.h>

EXTERN_C extern BOOL ValidExport();

static void CFIXCALLTYPE TestBase()
{
	ValidExport();
}

CFIX_BEGIN_FIXTURE(Base)
	CFIX_FIXTURE_ENTRY(TestBase)
CFIX_END_FIXTURE()

