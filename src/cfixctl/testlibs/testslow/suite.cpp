/*----------------------------------------------------------------------
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfix.h>

static void SlowTest()
{
	Sleep( 1000 );
}

CFIX_BEGIN_FIXTURE( SlowFixture )
	CFIX_FIXTURE_ENTRY( SlowTest )
CFIX_END_FIXTURE()

EXTERN_C __declspec(dllexport)
PCFIX_TEST_PE_DEFINITION CFIXCALLTYPE __CfixFixturePeSlowExport()
{
	static CFIX_PE_DEFINITION_ENTRY Entries[] = {
	{ CfixEntryTypeEnd, NULL, NULL }			
	};			

	static CFIX_TEST_PE_DEFINITION Fixture = {
		CFIX_PE_API_VERSION,				
		Entries								
	};		

	Sleep( 1000 );

	return &Fixture;						
}