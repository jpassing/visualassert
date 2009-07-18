/*----------------------------------------------------------------------
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfix.h>
#include <crtdbg.h>

CFIX_BEGIN_FIXTURE( Empty )
CFIX_END_FIXTURE()

int __cdecl wmain()
{
	_ASSERTE( !"Should never be called" );
	return 0xDEAD;
}