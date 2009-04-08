/*----------------------------------------------------------------------
 * Purpose:
 *		Test Host class.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
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
			0,
			0,
			NULL,
			NULL,
			&Host ) );

		CfixTestModuleArch Arch;
		CFIXCC_ASSERT_OK( Host->GetArchitecture( &Arch ) );
		CFIXCC_ASSERT_EQUALS( TESTCTLP_OWN_ARCHITECTURE, Arch );

		ULONG Pid;
		CFIXCC_ASSERT_OK( Host->GetHostProcessId( &Pid ) );
		CFIXCC_ASSERT_EQUALS( GetCurrentProcessId(), Pid );

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
			CFIXCTL_E_TESTMODULE_NOT_FOUND,
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