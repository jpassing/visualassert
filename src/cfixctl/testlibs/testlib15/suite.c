/*----------------------------------------------------------------------
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfix.h>
#include <stdio.h>


__declspec( dllexport )
void ExportFromTestLib15()
{
	printf( "test" );
}

static void Foo()
{
	CFIX_LOG( L"Test" );
}

CFIX_BEGIN_FIXTURE( Test )
	CFIX_FIXTURE_ENTRY( Foo )
CFIX_END_FIXTURE()
