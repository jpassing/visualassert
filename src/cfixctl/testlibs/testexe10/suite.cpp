/*----------------------------------------------------------------------
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfixcc.h>
#include <crtdbg.h>

class Empty : public cfixcc::TestFixture
{
};

CFIXCC_BEGIN_CLASS( Empty )
CFIXCC_END_CLASS()

int __cdecl wmain()
{
	return 0xBABE;
}