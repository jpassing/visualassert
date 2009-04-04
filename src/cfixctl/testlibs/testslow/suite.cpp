/*----------------------------------------------------------------------
 * Copyright:
 *		2008, Johannes Passing (passing at users.sourceforge.net)
 *
 * This file is part of cfix.
 *
 * cfix is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * cfix is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with cfix.  If not, see <http://www.gnu.org/licenses/>.
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