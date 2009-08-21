/*----------------------------------------------------------------------
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfix.h>
#include <crtdbg.h>

extern void ExportFromTestLib15();

static void CallImport()
{
	ExportFromTestLib15();
}

CFIX_BEGIN_FIXTURE( ImportTest )
	CFIX_FIXTURE_ENTRY( CallImport )
CFIX_END_FIXTURE()

int __cdecl wmain()
{
	_ASSERTE( !"Should never be called" );
	return 0xDEAD;
}