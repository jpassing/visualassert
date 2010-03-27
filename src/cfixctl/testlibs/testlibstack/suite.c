/*----------------------------------------------------------------------
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfix.h>
#include <stdio.h>

static void Test4MB()
{
	UCHAR ThreeMeg[ 3 * 1024 * 1024 ];
	SecureZeroMemory( ThreeMeg, sizeof( ThreeMeg ) );
}

static void Test16MB()
{
	UCHAR FifteenMeg[ 15 * 1024 * 1024 ];
	SecureZeroMemory( FifteenMeg, sizeof( FifteenMeg ) );
}

CFIX_BEGIN_FIXTURE( LargeStack )
	CFIX_FIXTURE_ENTRY( Test4MB )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( HugeStack )
	CFIX_FIXTURE_ENTRY( Test16MB )
CFIX_END_FIXTURE()
