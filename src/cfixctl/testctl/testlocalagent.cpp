/*----------------------------------------------------------------------
 * Purpose:
 *		Test LocalAgent class.
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

static DWORD LaunchHostAndWait( 
	PCWSTR HostPath,
	PWSTR CommandLine )
{
	PROCESS_INFORMATION ProcessInfo;
	STARTUPINFO StartupInfo;
	ZeroMemory( &StartupInfo, sizeof( STARTUPINFO ) );
	StartupInfo.cb = sizeof( STARTUPINFO );

	CFIX_ASSERT( CreateProcess(
		HostPath,
		CommandLine,
		NULL,
		NULL,
		FALSE,
		0,
		NULL,
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

class TestLocalAgent : public cfixcc::TestFixture
{
private:
	static COM_EXPORTS Exports;
	IClassFactory *AgentFactory;;
	
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
		CFIXCC_ASSERT_OK( Exports.GetClassObject( 
			CLSID_LocalAgent, IID_IClassFactory, ( PVOID* ) &this->AgentFactory ) );
		CFIXCC_ASSERT( this->AgentFactory );
	}

	virtual void After()
	{
		if ( this->AgentFactory )
		{
			 this->AgentFactory->Release();
		}
	}


	void RegisterAndObtainWithoutWaiting()
	{
		ICfixAgent *Agent;
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		ICfixHost *LocalHost;
		CFIXCC_ASSERT_OK( Agent->CreateHost( 
			TESTCTLP_OWN_ARCHITECTURE,
			CLSCTX_INPROC_SERVER,
			0,
			0,
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
		CFIXCC_ASSERT_OK( Agent->RegisterHost( 0xB00, LocalHost ) );
		LocalHost->Release();

		CFIXCC_ASSERT_EQUALS( CFIXCTL_E_HOST_NOT_FOUND,
			Agent->WaitForHostConnection(
				0xF00, 0, &ObtainedHost ) );
		CFIXCC_ASSERT( ObtainedHost == NULL );

		CFIXCC_ASSERT_OK( 
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
		CFIXCC_ASSERT_OK( Agent->CreateHost( 
			TESTCTLP_OWN_ARCHITECTURE,
			CLSCTX_INPROC_SERVER,
			0,
			0,
			NULL,
			NULL,
			&LocalHost ) );

		CFIXCC_ASSERT_OK( Agent->RegisterHost( 0xB00, LocalHost ) );
		LocalHost->Release();

		return 0;
	}

	void RegisterAndObtainWithWaiting()
	{
		ICfixAgent *Agent;
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );


		HANDLE Thread = CfixCreateThread(
			NULL, 0, RegisterThreadProc, Agent, 0, NULL );
		CFIXCC_ASSERT( Thread != NULL );

		//
		// Query.
		//
		ICfixHost *ObtainedHost;
		CFIXCC_ASSERT_OK( 
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
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		BSTR HostPath = NULL;
		CFIXCC_ASSERT_OK( Agent->GetHostPath(
			CFIXCTL_OWN_ARCHITECTURE,
			&HostPath ) );
		CFIXCC_ASSERT( HostPath );

		CFIX_ASSERT( GetFileAttributes( HostPath ) != INVALID_FILE_ATTRIBUTES );

		Agent->Release();
	}

	void SpawnSameArch()
	{
		ICfixAgent *Agent;
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIX_ASSUME( Agent );

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
					WorkingDir,
					&Host ) );
			SysFreeString( WorkingDir );

			CFIXCC_ASSERT_OK( Agent->CreateHost(
				CFIXCTL_OWN_ARCHITECTURE,
				CLSCTX_LOCAL_SERVER,
				FlagSets[ Flags ],
				INFINITE,
				NULL,
				NULL,
				&Host ) );

			CfixTestModuleArch Arch;
			CFIXCC_ASSERT_OK( Host->GetArchitecture( &Arch ) );
			CFIXCC_ASSERT_EQUALS( TESTCTLP_OWN_ARCHITECTURE, Arch );

			ULONG Pid;
			CFIXCC_ASSERT_OK( Host->GetHostProcessId( &Pid ) );
			CFIXCC_ASSERT_NOT_EQUALS( GetCurrentProcessId(), Pid );

			CFIX_ASSUME( Host );
			Host->Release();
		}

		Agent->Release();
	}

	void SpawnAllArchs()
	{
		ICfixAgent *Agent;
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIX_ASSUME( Agent );

		ULONG FlagSets[] = { 0, CFIXCTL_AGENT_FLAG_USE_JOB };

		for ( ULONG Flags = 0; Flags < _countof( FlagSets ); Flags++ )
		{
			CfixTestModuleArch Archs[] = {
				CfixTestModuleArchI386,
				CfixTestModuleArchAmd64
			};

			for ( ULONG Arch = 0; Arch < _countof( Archs ); Arch++ )
			{
				ICfixHost *Host;
				CFIXCC_ASSERT_OK( Agent->CreateHost(
					Archs[ Arch ],
					CLSCTX_LOCAL_SERVER,
					FlagSets[ Flags ],
					INFINITE,
					NULL,
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
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIX_ASSUME( Agent );

		ICfixMessageResolver *Resolver;
		CFIXCC_ASSERT_OK( Agent->CreateMessageResolver( &Resolver ) );

		CFIXCC_ASSERT_EQUALS( E_INVALIDARG, Resolver->ResolveMessage( 
			( ULONG ) CFIXCTL_E_UNRECOGNIZED_MODULE_TYPE, 
			0, 
			NULL ) );
		
		BSTR Message;
		CFIXCC_ASSERT_EQUALS( E_INVALIDARG, Resolver->ResolveMessage( 
			( ULONG ) CFIXCTL_E_UNRECOGNIZED_MODULE_TYPE, 
			1, 
			&Message ) );
		
		CFIXCC_ASSERT_OK( Resolver->ResolveMessage( 
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
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		BSTR HostPath = NULL;
		CFIXCC_ASSERT_OK( Agent->GetHostPath(
			CFIXCTL_OWN_ARCHITECTURE,
			&HostPath ) );
		CFIXCC_ASSERT( HostPath );

		CFIX_ASSERT( GetFileAttributes( HostPath ) != INVALID_FILE_ATTRIBUTES );

		Agent->Release();

		//
		// Missing moniker.
		//
		CFIXCC_ASSERT_EQUALS( 
			E_INVALIDARG,
			( HRESULT ) LaunchHostAndWait(
				HostPath,
				L"" ) );

		SysFreeString( HostPath );
	}

	void SpawnWithWrongMoniker()
	{
		ICfixAgent *Agent;
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		BSTR HostPath = NULL;
		CFIXCC_ASSERT_OK( Agent->GetHostPath(
			CFIXCTL_OWN_ARCHITECTURE,
			&HostPath ) );
		CFIXCC_ASSERT( HostPath );

		CFIX_ASSERT( GetFileAttributes( HostPath ) != INVALID_FILE_ATTRIBUTES );

		Agent->Release();

		
		//
		// Create moniker to agentfactory rather than to agent.
		//

		IMoniker *AgentMk = NULL;
		CFIXCC_ASSERT_OK( CreateObjrefMoniker( AgentFactory, &AgentMk ) );
		
		IBindCtx *BindCtx = NULL;
		CFIXCC_ASSERT_OK( CreateBindCtx( 0, &BindCtx ) );
		
		LPOLESTR DisplayName = NULL;
		CFIXCC_ASSERT_OK( AgentMk->GetDisplayName( BindCtx, NULL, &DisplayName ) );

		SIZE_T CmdLineLen = wcslen( DisplayName ) * sizeof( WCHAR ) + 32;
		PWSTR CmdLine = new WCHAR[ CmdLineLen ];
		CFIXCC_ASSERT_OK( StringCchPrintf( 
			CmdLine,
			CmdLineLen,
			L"cfixhost %s",
			DisplayName ) );

		//
		// Missing moniker.
		//
		CFIXCC_ASSERT_EQUALS( 
			E_NOINTERFACE,
			( HRESULT ) LaunchHostAndWait(
				HostPath,
				CmdLine ) );

		delete [] CmdLine;
		SysFreeString( HostPath );
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
CFIXCC_END_CLASS()