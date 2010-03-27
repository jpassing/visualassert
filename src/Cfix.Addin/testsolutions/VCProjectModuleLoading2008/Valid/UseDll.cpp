#include <cfix.h>

EXTERN_C extern BOOL CallMe();

static void __stdcall CallDllFunc()
{
	CallMe();
}

CFIX_BEGIN_FIXTURE( UseDll )
	CFIX_FIXTURE_ENTRY( CallDllFunc )
CFIX_END_FIXTURE()