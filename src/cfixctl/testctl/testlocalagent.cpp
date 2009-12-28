/*----------------------------------------------------------------------
 * Purpose:
 *		Test LocalAgent class.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <testctlp.h>

class TestLocalAgent : public cfixcc::TestFixture
{
private:
	static COM_EXPORTS Exports;
	IClassFactory *AgentFactory;
	WCHAR Environment[ 300 ];
	BSTR EnvironmentBstr;
	
public:
	TestLocalAgent() : AgentFactory( NULL )
	{
	}

	static void SetUp()
	{
		CoInitializeEx( NULL, COINIT_MULTITHREADED );
		GetComExports( L"cfixctl.dll", &Exports );
	}

	static void TearDown()
	{
		CoUninitialize();
	}

	virtual void Before()
	{
		CFIX_ASSERT_OK( Exports.GetClassObject( 
			CLSID_LocalAgent, IID_IClassFactory, ( PVOID* ) &this->AgentFactory ) );
		CFIXCC_ASSERT( this->AgentFactory );

		WCHAR SystemRoot[ 200 ];
		CFIX_ASSERT( GetEnvironmentVariable(
			L"SystemRoot",
			SystemRoot,
			_countof( SystemRoot ) ) );
		CFIX_ASSERT_OK( StringCchPrintf(
			Environment,
			_countof( Environment ),
			L"SystemRoot=%s\n",
			SystemRoot ) );

		EnvironmentBstr = SysAllocString( Environment );
	}

	virtual void After()
	{
		SysFreeString( EnvironmentBstr );

		if ( this->AgentFactory )
		{
			 this->AgentFactory->Release();
		}
	}

	DWORD LaunchHostAndWait( 
		PCWSTR HostPath,
		PWSTR CustomEnvironment 
		)
	{
		PROCESS_INFORMATION ProcessInfo;
		STARTUPINFO StartupInfo;
		ZeroMemory( &StartupInfo, sizeof( STARTUPINFO ) );
		StartupInfo.cb = sizeof( STARTUPINFO );

		WCHAR Environment[ 512 ];
		CFIX_ASSERT_OK( StringCchPrintf(
			Environment,
			_countof( Environment ),
			L"%s%s",
			this->Environment,
			CustomEnvironment != NULL ? CustomEnvironment : L"" ) );

		//
		// Replace all \n by \0 so that the buffer becomes a valid
		// environment string.
		//

		PWSTR Newline;
		while ( ( Newline = wcsrchr( Environment, L'\n' ) ) != NULL )
		{
			*Newline = UNICODE_NULL;
		}

		CFIX_ASSERT( CreateProcess(
			HostPath,
			NULL,
			NULL,
			NULL,
			FALSE,
			CREATE_UNICODE_ENVIRONMENT,
			Environment,
			NULL,
			&StartupInfo,
			&ProcessInfo ) );
		CFIXCC_ASSERT_EQUALS( WAIT_OBJECT_0, WaitForSingleObject(
			ProcessInfo.hProcess, INFINITE ) );

		DWORD ExitCode;
		CFIX_ASSERT( GetExitCodeProcess( ProcessInfo.hProcess, &ExitCode ) );

		CloseHandle( ProcessInfo.hThread );
		CloseHandle( ProcessInfo.hProcess );

		return ExitCode;
	}


	void RegisterAndObtainWithoutWaiting()
	{
		ICfixAgent *Agent;
		CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		CFIX_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

		ICfixHost *LocalHost;
		CFIX_ASSERT_OK( Agent->CreateHost( 
			TESTCTLP_OWN_ARCHITECTURE,
			CLSCTX_INPROC_SERVER,
			0,
			0,
			NULL,
			NULL,
			NULL,
			&LocalHost ) );

		//
		// Query invalid host.
		//
		ICfixHost *ObtainedHost;
		CFIXCC_ASSERT_EQUALS( CFIXCTL_E_HOST_NOT_FOUND,
			Agent->WaitForHostConnection(
				0xF00, 0, &ObtainedHost ) );
		CFIXCC_ASSERT( ObtainedHost == NULL );

		CFIXCC_ASSERT_EQUALS( HRESULT_FROM_WIN32( ERROR_TIMEOUT ),
			Agent->WaitForHostConnection(
				0xF00, 5, &ObtainedHost ) );
		CFIXCC_ASSERT( ObtainedHost == NULL );

		//
		// Register & Query.
		//
		CFIX_ASSERT_OK( Agent->RegisterHost( 0xB00, LocalHost ) );
		LocalHost->Release();

		CFIXCC_ASSERT_EQUALS( CFIXCTL_E_HOST_NOT_FOUND,
			Agent->WaitForHostConnection(
				0xF00, 0, &ObtainedHost ) );
		CFIXCC_ASSERT( ObtainedHost == NULL );

		CFIX_ASSERT_OK( 
			Agent->WaitForHostConnection(
				0xB00, 0, &ObtainedHost ) );
		CFIX_ASSUME( ObtainedHost );
		ObtainedHost->Release();

		Agent->Release();
	}

	static DWORD RegisterThreadProc( __in PVOID PvAgent )
	{
		ICfixAgent *Agent = static_cast< ICfixAgent* >( PvAgent );

		Sleep( 200 );

		ICfixHost *LocalHost;
		CFIX_ASSERT_OK( Agent->CreateHost( 
			TESTCTLP_OWN_ARCHITECTURE,
			CLSCTX_INPROC_SERVER,
			0,
			0,
			NULL,
			NULL,
			NULL,
			&LocalHost ) );

		CFIX_ASSERT_OK( Agent->RegisterHost( 0xB00, LocalHost ) );
		LocalHost->Release();

		return 0;
	}

	void RegisterAndObtainWithWaiting()
	{
		ICfixAgent *Agent;
		CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		CFIX_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

		HANDLE Thread = CfixCreateThread(
			NULL, 0, RegisterThreadProc, Agent, 0, NULL );
		CFIXCC_ASSERT( Thread != NULL );

		//
		// Query.
		//
		ICfixHost *ObtainedHost;
		CFIX_ASSERT_OK( 
			Agent->WaitForHostConnection(
				0xB00, 500, &ObtainedHost ) );
		CFIX_ASSUME( ObtainedHost );
		ObtainedHost->Release();
		
		//
		// Query again.
		//
		CFIXCC_ASSERT_EQUALS( CFIXCTL_E_HOST_NOT_FOUND,
			Agent->WaitForHostConnection(
				0xF00, 0, &ObtainedHost ) );
		CFIXCC_ASSERT( ObtainedHost == NULL );

		Agent->Release();

		WaitForSingleObject( Thread, INFINITE );
		CloseHandle( Thread );
	}

	void GetHostPath()
	{
		ICfixAgent *Agent;
		CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		BSTR HostPath = NULL;
		CFIX_ASSERT_OK( Agent->GetHostPath(
			CFIXCTL_OWN_ARCHITECTURE,
			&HostPath ) );
		CFIXCC_ASSERT( HostPath );

		CFIX_ASSERT( GetFileAttributes( HostPath ) != INVALID_FILE_ATTRIBUTES );

		Agent->Release();
	}

	void SpawnSameArch()
	{
		ICfixAgent *Agent;
		CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIX_ASSUME( Agent );

		CFIX_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

		ULONG FlagSets[] = { 0, CFIXCTL_AGENT_FLAG_USE_JOB };

		for ( ULONG Flags = 0; Flags < _countof( FlagSets ); Flags++ )
		{
			ICfixHost *Host;
			BSTR WorkingDir = SysAllocString( L"c:\\idonotexist" );
			CFIXCC_ASSERT_EQUALS( 
				HRESULT_FROM_WIN32( ERROR_DIRECTORY ), 
				Agent->CreateHost(
					CFIXCTL_OWN_ARCHITECTURE,
					CLSCTX_LOCAL_SERVER,
					FlagSets[ Flags ],
					INFINITE,
					NULL,
					this->EnvironmentBstr,
					WorkingDir,
					&Host ) );
			SysFreeString( WorkingDir );

			CFIX_ASSERT_OK( Agent->CreateHost(
				CFIXCTL_OWN_ARCHITECTURE,
				CLSCTX_LOCAL_SERVER,
				FlagSets[ Flags ],
				INFINITE,
				NULL,
				this->EnvironmentBstr,
				NULL,
				&Host ) );

			CfixTestModuleArch Arch;
			CFIX_ASSERT_OK( Host->GetArchitecture( &Arch ) );
			CFIXCC_ASSERT_EQUALS( TESTCTLP_OWN_ARCHITECTURE, Arch );

			ULONG Pid;
			CFIX_ASSERT_OK( Host->GetHostProcessId( &Pid ) );
			CFIXCC_ASSERT_NOT_EQUALS( GetCurrentProcessId(), Pid );

			CFIX_ASSUME( Host );
			Host->Release();
		}

		Agent->Release();
	}

	void SpawnAllArchs()
	{
		ICfixAgent *Agent;
		CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIX_ASSUME( Agent );

		CFIX_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

		ULONG FlagSets[] = { 0, CFIXCTL_AGENT_FLAG_USE_JOB };

		for ( ULONG Flags = 0; Flags < _countof( FlagSets ); Flags++ )
		{
			CfixTestModuleArch Archs[] = {
				CfixTestModuleArchI386,
				CfixTestModuleArchAmd64
			};

			for ( ULONG Arch = 0; Arch < _countof( Archs ); Arch++ )
			{
				if ( ! IsArchitectureSupportedOnThisMachine( Archs[ Arch ] ) )
				{
					continue;
				}

				ICfixHost *Host;
				CFIX_ASSERT_OK( Agent->CreateHost(
					Archs[ Arch ],
					CLSCTX_LOCAL_SERVER,
					FlagSets[ Flags ],
					INFINITE,
					NULL,
					this->EnvironmentBstr,
					NULL,
					&Host ) );

				CFIX_ASSUME( Host );
				Host->Release();
			}
		}

		Agent->Release();
	}

	void ResolveMessage()
	{
		ICfixAgent *Agent;
		CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIX_ASSUME( Agent );

		CFIX_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

		ICfixMessageResolver *Resolver;
		CFIX_ASSERT_OK( Agent->CreateMessageResolver( &Resolver ) );

		CFIXCC_ASSERT_EQUALS( E_INVALIDARG, Resolver->ResolveMessage( 
			( ULONG ) CFIXCTL_E_UNRECOGNIZED_MODULE_TYPE, 
			0, 
			NULL ) );
		
		BSTR Message;
		CFIXCC_ASSERT_EQUALS( E_INVALIDARG, Resolver->ResolveMessage( 
			( ULONG ) CFIXCTL_E_UNRECOGNIZED_MODULE_TYPE, 
			1, 
			&Message ) );
		
		CFIX_ASSERT_OK( Resolver->ResolveMessage( 
			( ULONG ) CFIXCTL_E_UNRECOGNIZED_MODULE_TYPE, 
			0, 
			&Message ) );
		CFIXCC_ASSERT( SysStringLen( Message ) > 0 );
		SysFreeString( Message );

		Resolver->Release();
		Agent->Release();
	}

	void SpawnWithoutMoniker()
	{
		ICfixAgent *Agent;
		CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		CFIX_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

		BSTR HostPath = NULL;
		CFIX_ASSERT_OK( Agent->GetHostPath(
			CFIXCTL_OWN_ARCHITECTURE,
			&HostPath ) );
		CFIXCC_ASSERT( HostPath );

		CFIX_ASSERT( GetFileAttributes( HostPath ) != INVALID_FILE_ATTRIBUTES );

		Agent->Release();

		//
		// Missing moniker.
		//
		CFIXCC_ASSERT_EQUALS( 
			CFIXCTL_E_MISSING_AGENT_MK,
			( HRESULT ) LaunchHostAndWait(
				HostPath,
				L"FOO=BAR\n" ) );

		SysFreeString( HostPath );
	}

	void SpawnWithWrongMoniker()
	{
		ICfixAgent *Agent;
		CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		CFIX_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

		BSTR HostPath = NULL;
		CFIX_ASSERT_OK( Agent->GetHostPath(
			CFIXCTL_OWN_ARCHITECTURE,
			&HostPath ) );
		CFIXCC_ASSERT( HostPath );

		CFIX_ASSERT( GetFileAttributes( HostPath ) != INVALID_FILE_ATTRIBUTES );

		Agent->Release();

		
		//
		// Create moniker to agentfactory rather than to agent.
		//

		IMoniker *AgentMk = NULL;
		CFIX_ASSERT_OK( CreateObjrefMoniker( AgentFactory, &AgentMk ) );
		
		IBindCtx *BindCtx = NULL;
		CFIX_ASSERT_OK( CreateBindCtx( 0, &BindCtx ) );
		
		LPOLESTR DisplayName = NULL;
		CFIX_ASSERT_OK( AgentMk->GetDisplayName( BindCtx, NULL, &DisplayName ) );

		SIZE_T EnvCch = wcslen( L"CFIX_AGENT_MK=" ) + 
			wcslen( DisplayName ) + 2;
		PWSTR Env = new WCHAR[ EnvCch ];
		ZeroMemory( Env, EnvCch * sizeof( WCHAR ) );
		CFIX_ASSERT_OK( StringCchPrintf( 
			Env,
			EnvCch,
			L"CFIX_AGENT_MK=%s\n",
			DisplayName ) );

		//
		// Wrong moniker.
		//
		CFIXCC_ASSERT_EQUALS( 
			E_NOINTERFACE,
			( HRESULT ) LaunchHostAndWait(
				HostPath,
				Env ) );

		delete [] Env;
		SysFreeString( HostPath );
		CoTaskMemFree( DisplayName );
		AgentMk->Release();
		BindCtx->Release();
	}

	void SpawnCustomHostWithoutEmbedding()
	{
		WCHAR Path[ MAX_PATH ];
		CFIXCC_ASSERT( GetModuleFileName(
			GetModuleHandle( L"testctl" ),
			Path,
			_countof( Path ) ) );
		PathRemoveFileSpec( Path );
		PathAppend( Path,  L"testexe10.exe" );

		//
		// The CRT initializer should not mess with the process.
		//
		CFIX_ASSERT_EQUALS_DWORD( 
			0xBABE,
			LaunchHostAndWait( Path, NULL ) );
	}

	void SpawnCustomHostWithNonExistingEmbeddingExport()
	{
		PWSTR ExportEnvVar[] = {
			CFIX_EMB_INIT_ENVVAR_NAME L"=cfixctl.dll!Idonotexist\n",
			CFIX_EMB_INIT_ENVVAR_NAME L"=Idonotexist.dll!Idonotexist\n"
		};

		WCHAR Path[ MAX_PATH ];
		CFIXCC_ASSERT( GetModuleFileName(
			GetModuleHandle( L"testctl" ),
			Path,
			_countof( Path ) ) );
		PathRemoveFileSpec( Path );
		PathAppend( Path,  L"testexe10.exe" );

		//
		// The CRT initializer should not mess with the process.
		//
		for ( ULONG Index = 0; Index < _countof( ExportEnvVar ); Index++ )
		{
			CFIX_ASSERT_EQUALS_DWORD( 
				0xBABE,
				LaunchHostAndWait( 
					Path, 
					ExportEnvVar[ Index ] ) );
		}
	}

	void SpawnCustomHost()
	{
		ICfixAgent *Agent;
		CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIX_ASSUME( Agent );

		CFIX_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

		//
		// C only, C++ only, C/C++ mixed.
		//
		PCWSTR HostImages[] = {
			L"testexe11.exe",
			L"testexe12.exe",
			L"testexe13.exe"
		};

		for ( ULONG ImageIndex = 0; ImageIndex < _countof( HostImages ); ImageIndex++ )
		{
			WCHAR Path[ MAX_PATH ];
			CFIXCC_ASSERT( GetModuleFileName(
				GetModuleHandle( L"testctl" ),
				Path,
				_countof( Path ) ) );
			PathRemoveFileSpec( Path );
			PathAppend( Path,  HostImages[ ImageIndex ] );

			BSTR HostImage = SysAllocString( Path );

			ULONG FlagSets[] = { 0, CFIXCTL_AGENT_FLAG_USE_JOB };

			for ( ULONG Flags = 0; Flags < _countof( FlagSets ); Flags++ )
			{
				ICfixHost *Host;
				CFIX_ASSERT_OK( Agent->CreateHost(
					CFIXCTL_OWN_ARCHITECTURE,
					CLSCTX_LOCAL_SERVER,
					FlagSets[ Flags ],
					INFINITE,
					HostImage,
					this->EnvironmentBstr,
					NULL,
					&Host ) );

				CFIX_ASSUME( Host );
				Host->Release();
			}

			SysFreeString( HostImage );
		}

		Agent->Release();
	}

	void SpawnCustomHostWithNoFixtures()
	{
		ICfixAgent *Agent;
		CFIX_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIX_ASSUME( Agent );

		CFIX_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

		WCHAR Path[ MAX_PATH ];
		CFIXCC_ASSERT( GetModuleFileName(
			GetModuleHandle( L"testctl" ),
			Path,
			_countof( Path ) ) );
		PathRemoveFileSpec( Path );
		PathAppend( Path, L"testexe14.exe" );

		BSTR HostImage = SysAllocString( Path );

		ULONG FlagSets[] = { 0, CFIXCTL_AGENT_FLAG_USE_JOB };

		for ( ULONG Flags = 0; Flags < _countof( FlagSets ); Flags++ )
		{
			ICfixHost *Host;
			CFIX_ASSERT_HRESULT( 
				CFIXCTL_E_HOST_IMAGE_HAS_NO_FIXTURES,
				Agent->CreateHost(
					CFIXCTL_OWN_ARCHITECTURE,
					CLSCTX_LOCAL_SERVER,
					FlagSets[ Flags ],
					INFINITE,
					HostImage,
					this->EnvironmentBstr,
					NULL,
					&Host ) );
		}

		SysFreeString( HostImage );

		Agent->Release();
	}
	
	void SpawnCustomHostWithoutMoniker()
	{
		WCHAR Path[ MAX_PATH ];
		CFIXCC_ASSERT( GetModuleFileName(
			GetModuleHandle( L"testctl" ),
			Path,
			_countof( Path ) ) );
		PathRemoveFileSpec( Path );
		PathAppend( Path,  L"testexe11.exe" );

		BSTR HostImage = SysAllocString( Path );

		//
		// Missing moniker.
		//
		CFIXCC_ASSERT_EQUALS( 
			CFIXCTL_E_MISSING_AGENT_MK,
			( HRESULT ) LaunchHostAndWait(
				HostImage,
				CFIX_EMB_INIT_ENVVAR_NAME L"=cfixctl.dll!CfixctlServeHost\n" ) );

		SysFreeString( HostImage );
	}

	void SpawnCustomHostWithWrongMoniker()
	{
		WCHAR Path[ MAX_PATH ];
		CFIXCC_ASSERT( GetModuleFileName(
			GetModuleHandle( L"testctl" ),
			Path,
			_countof( Path ) ) );
		PathRemoveFileSpec( Path );
		PathAppend( Path,  L"testexe11.exe" );

		BSTR HostImage = SysAllocString( Path );

		//
		// Create moniker to agentfactory rather than to agent.
		//

		IMoniker *AgentMk = NULL;
		CFIX_ASSERT_OK( CreateObjrefMoniker( AgentFactory, &AgentMk ) );
		
		IBindCtx *BindCtx = NULL;
		CFIX_ASSERT_OK( CreateBindCtx( 0, &BindCtx ) );
		
		LPOLESTR DisplayName = NULL;
		CFIX_ASSERT_OK( AgentMk->GetDisplayName( BindCtx, NULL, &DisplayName ) );

		PCWSTR FixedEnvPart = 
			CFIX_EMB_INIT_ENVVAR_NAME L"=cfixctl.dll!CfixctlServeHost\nCFIX_AGENT_MK=";

		SIZE_T EnvCch = wcslen( FixedEnvPart ) + wcslen( DisplayName ) + 2;
		PWSTR Env = new WCHAR[ EnvCch ];
		ZeroMemory( Env, EnvCch * sizeof( WCHAR ) );
		CFIX_ASSERT_OK( StringCchPrintf( 
			Env,
			EnvCch,
			L"%s%s\n",
			FixedEnvPart,
			DisplayName ) );

		//
		// Wrong moniker.
		//
		CFIXCC_ASSERT_EQUALS( 
			E_NOINTERFACE,
			( HRESULT ) LaunchHostAndWait(
				HostImage,
				Env ) );

		delete [] Env;
		SysFreeString( HostImage );
		CoTaskMemFree( DisplayName );
		AgentMk->Release();
		BindCtx->Release();
	}
};

COM_EXPORTS TestLocalAgent::Exports;

CFIXCC_BEGIN_CLASS( TestLocalAgent )
	CFIXCC_METHOD( SpawnSameArch )
	CFIXCC_METHOD( RegisterAndObtainWithoutWaiting )
	CFIXCC_METHOD( RegisterAndObtainWithWaiting )
	CFIXCC_METHOD( GetHostPath )
	CFIXCC_METHOD( SpawnAllArchs )
	CFIXCC_METHOD( ResolveMessage )
	CFIXCC_METHOD( SpawnWithoutMoniker )
	CFIXCC_METHOD( SpawnWithWrongMoniker )
	CFIXCC_METHOD( SpawnCustomHostWithoutEmbedding )
	CFIXCC_METHOD( SpawnCustomHostWithNonExistingEmbeddingExport )
	CFIXCC_METHOD( SpawnCustomHost )
	CFIXCC_METHOD( SpawnCustomHostWithNoFixtures )
	CFIXCC_METHOD( SpawnCustomHostWithoutMoniker )
	CFIXCC_METHOD( SpawnCustomHostWithWrongMoniker )
CFIXCC_END_CLASS()