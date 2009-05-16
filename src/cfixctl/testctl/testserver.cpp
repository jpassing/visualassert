/*----------------------------------------------------------------------
 * Purpose:
 *		Test COM server.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
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
		CFIX_ASSERT_OK( Exports.RegisterServer() );
		CFIX_ASSERT_OK( Exports.UnregisterServer() );
	}
};

COM_EXPORTS TestServer::Exports;

CFIXCC_BEGIN_CLASS( TestServer )
	CFIXCC_METHOD( Test )
	//CFIXCC_METHOD( TestSelfReg )
CFIXCC_END_CLASS()