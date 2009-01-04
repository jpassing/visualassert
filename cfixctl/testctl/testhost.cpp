/*----------------------------------------------------------------------
 * Purpose:
 *		Test Host class.
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

class TestHost : public cfixcc::TestFixture
{
private:
	static COM_EXPORTS Exports;
	ICfixHost *Host;
	
public:
	TestHost() : Host( NULL )
	{
	}

	static void SetUp()
	{
		CoInitialize( NULL );
		GetComExports( L"cfixctl.dll", &Exports );
	}

	static void TearDown()
	{
		CoUninitialize();
	}

	virtual void Before()
	{
		IClassFactory *AgentFactory;

		CFIXCC_ASSERT_OK( Exports.GetClassObject( 
			CLSID_LocalAgent, IID_IClassFactory, ( PVOID* ) &AgentFactory ) );
		CFIXCC_ASSERT( AgentFactory );
		__assume( AgentFactory );

		ICfixAgent *Agent;
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		CFIXCC_ASSERT_OK( Agent->CreateHost( 
			TESTCTLP_OWN_ARCHITECTURE,
			CLSCTX_INPROC_SERVER,
			&Host ) );

		Agent->Release();
		AgentFactory->Release();
	}

	virtual void After()
	{
		if ( Host )
		{
			Host->Release();
		}
	}

	void TestUnknown()
	{
		TestComUnknown( this->Host, IID_ICfixHost, IID_IUnknown );
	}

	void LoadNonExisting()
	{
		BSTR Foo = SysAllocString( L"foo" );
		BSTR SomeDll = SysAllocString( L"idonotexist.dll" );
		BSTR SomeFile = SysAllocString( L"idonotexist.xxx" );

		ICfixTestModule *Module;
		CFIXCC_ASSERT_EQUALS( 
			E_INVALIDARG,
			Host->LoadModule(
				Foo,
				&Module ) );
		CFIXCC_ASSERT_EQUALS( 
			HRESULT_FROM_WIN32( ERROR_MOD_NOT_FOUND ),
			Host->LoadModule(
				SomeDll,
				&Module ) );
		CFIXCC_ASSERT_EQUALS( 
			CFIXCTL_E_UNRECOGNIZED_MODULE_TYPE,
			Host->LoadModule(
				SomeFile,
				&Module ) );

		SysFreeString( Foo );
		SysFreeString( SomeDll );
		SysFreeString( SomeFile );
	}

	void LoadUser()
	{
		WCHAR OwnPath[ MAX_PATH ];
		CFIXCC_ASSERT( GetModuleFileName(
			GetModuleHandle( L"testctl" ),
			OwnPath,
			_countof( OwnPath ) ) );

		ICfixTestModule *Module;
		CFIXCC_ASSERT_OK( 
			Host->LoadModule(
				OwnPath,
				&Module ) );

		CFIXCC_ASSERT_EQUALS( 0UL, Module->Release() );
	}
};

COM_EXPORTS TestHost::Exports;

CFIXCC_BEGIN_CLASS( TestHost )
	CFIXCC_METHOD( TestUnknown )
	CFIXCC_METHOD( LoadNonExisting )
	CFIXCC_METHOD( LoadUser )
CFIXCC_END_CLASS()