/*----------------------------------------------------------------------
 * Purpose:
 *		Test COM server.
 *
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

#include <testctlp.h>

class TestServer : public cfixcc::TestFixture
{
private:
	static COM_EXPORTS Exports;

public:
	static void SetUp()
	{
		CoInitialize( NULL );
		GetComExports( L"cfixctl.dll", &Exports );
	}

	static void TearDown()
	{
		CoUninitialize();
	}

	void Test()
	{
		TestComServer( &Exports, CLSID_TestCase );
	}

	void TestSelfReg()
	{
		//
		// N.B. Requires admin rights.
		//
		CFIXCC_ASSERT_OK( Exports.RegisterServer() );
		CFIXCC_ASSERT_OK( Exports.UnregisterServer() );
	}
};

COM_EXPORTS TestServer::Exports;

CFIXCC_BEGIN_CLASS( TestServer )
	CFIXCC_METHOD( Test )
	//CFIXCC_METHOD( TestSelfReg )
CFIXCC_END_CLASS()