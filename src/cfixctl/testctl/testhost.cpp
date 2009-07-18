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
	ICfixAgent *Agent;
	
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

		CFIX_ASSERT_OK( Exports.GetClassObject( 
			CLSID_LocalAgent, IID_IClassFactory, ( PVOID* ) &AgentFactory ) );
		CFIXCC_ASSERT( AgentFactory );
		__assume( AgentFactory );

		CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		CFIX_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

		CFIX_ASSERT_OK( Agent->CreateHost( 
			TESTCTLP_OWN_ARCHITECTURE,
			CLSCTX_INPROC_SERVER,
			0,
			0,
			NULL,
			NULL,
			NULL,
			&Host ) );

		CfixTestModuleArch Arch;
		CFIX_ASSERT_OK( Host->GetArchitecture( &Arch ) );
		CFIXCC_ASSERT_EQUALS( TESTCTLP_OWN_ARCHITECTURE, Arch );

		ULONG Pid;
		CFIX_ASSERT_OK( Host->GetHostProcessId( &Pid ) );
		CFIXCC_ASSERT_EQUALS( GetCurrentProcessId(), Pid );

		AgentFactory->Release();
	}

	virtual void After()
	{
		if ( Agent )
		{
			Agent->Release();
		}

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
			CFIXCTL_E_TESTMODULE_NOT_FOUND,
			Host->LoadModule(
				SomeFile,
				&Module ) );

		SysFreeString( Foo );
		SysFreeString( SomeDll );
		SysFreeString( SomeFile );
	}

	void LoadUserDll()
	{
		WCHAR OwnPath[ MAX_PATH ];
		CFIXCC_ASSERT( GetModuleFileName(
			GetModuleHandle( L"testctl" ),
			OwnPath,
			_countof( OwnPath ) ) );

		BSTR BstrPath = SysAllocString( OwnPath );

		ICfixTestModule *Module;
		CFIX_ASSERT_OK( 
			Host->LoadModule(
				BstrPath,
				&Module ) );

		SysFreeString( BstrPath );

		CFIXCC_ASSERT_EQUALS( 0UL, Module->Release() );
	}

	void LoadInprocUserExe()
	{
		ICfixHost* CustomHost;
		CFIX_ASSERT_OK( Agent->CreateHost( 
			TESTCTLP_OWN_ARCHITECTURE,
			CLSCTX_LOCAL_SERVER,
			0,
			INFINITE,
			NULL,
			NULL,
			NULL,
			&CustomHost ) );

		ICfixTestModule *Module;
		CFIX_ASSERT_OK( 
			CustomHost->LoadModule(
				NULL,
				&Module ) );

		ICfixTestContainer *Container;
		CFIX_ASSERT_OK( Module->QueryInterface( 
			IID_ICfixTestContainer, ( PVOID* ) &Container ) );

		ULONG FixtureCount;
		CFIX_ASSERT_OK( Container->GetItemCount( &FixtureCount ) );
		CFIXCC_ASSERT_EQUALS( 0UL, FixtureCount );

		Container->Release();
		CFIXCC_ASSERT_EQUALS( 0UL, Module->Release() );

		CustomHost->Release();
	}

	void LoadUserExe()
	{
		WCHAR Path[ MAX_PATH ];
		CFIXCC_ASSERT( GetModuleFileName(
			GetModuleHandle( L"testctl" ),
			Path,
			_countof( Path ) ) );
		PathRemoveFileSpec( Path );
		PathAppend( Path,  L"testexe11.exe" );

		ICfixHost* CustomHost;
		CFIX_ASSERT_OK( Agent->CreateHost( 
			TESTCTLP_OWN_ARCHITECTURE,
			CLSCTX_LOCAL_SERVER,
			0,
			INFINITE,
			Path,
			NULL,
			NULL,
			&CustomHost ) );

		ICfixTestModule *Module;
		CFIX_ASSERT_OK( 
			CustomHost->LoadModule(
				NULL,
				&Module ) );

		ICfixTestContainer *Container;
		CFIX_ASSERT_OK( Module->QueryInterface( 
			IID_ICfixTestContainer, ( PVOID* ) &Container ) );

		ULONG FixtureCount;
		CFIX_ASSERT_OK( Container->GetItemCount( &FixtureCount ) );
		CFIXCC_ASSERT_EQUALS( 2UL, FixtureCount );

		Container->Release();
		CFIXCC_ASSERT_EQUALS( 0UL, Module->Release() );

		CustomHost->Release();
	}
};

COM_EXPORTS TestHost::Exports;

CFIXCC_BEGIN_CLASS( TestHost )
	CFIXCC_METHOD( TestUnknown )
	CFIXCC_METHOD( LoadNonExisting )
	CFIXCC_METHOD( LoadUserDll )
	CFIXCC_METHOD( LoadInprocUserExe )
	CFIXCC_METHOD( LoadUserExe )
CFIXCC_END_CLASS()