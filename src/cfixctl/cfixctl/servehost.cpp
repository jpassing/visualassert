/*----------------------------------------------------------------------
 * Purpose:
 *		Entry Point.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#define CFIXCTLAPI

#include <windows.h>
#include <ole2.h>
#include "cfixctlp.h"

static HANDLE CfixctlsShutdownEvent = NULL;

/*----------------------------------------------------------------------
 *
 * Initialization.
 *
 */

static VOID CfixctlsServerUnlocked()
{
	SetEvent( CfixctlsShutdownEvent );
}

static HRESULT CfixctlsGetAgent( 
	__in PCWSTR ObjRefMoniker,
	__in IID Iid,
	__out PVOID *Agent
	)
{
	return CoGetObject(
		ObjRefMoniker,
		NULL,
		Iid,
		( PVOID* ) Agent );
}

static HRESULT CfixctlsCreateLocalHost( 
	__out ICfixHost **Host
	)
{
	IClassFactory *AgentFactory;
	HRESULT Hr = CfixctlGetClassObject( 
		CLSID_LocalAgent, IID_IClassFactory, ( PVOID* ) &AgentFactory );
	if ( SUCCEEDED( Hr ) )
	{
		ICfixAgent *LocalAgent;
		Hr = AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &LocalAgent );
		if ( SUCCEEDED( Hr ) )
		{
			Hr = LocalAgent->CreateHost(
				CFIXCTL_OWN_ARCHITECTURE,
				CLSCTX_INPROC_SERVER,
				0,
				0,
				NULL,	// HostImage.
				NULL,	// This process is already using the specified env.
				NULL,	// Parent has set the current directory for us.
				Host );

			LocalAgent->Release();
		}

		AgentFactory->Release();
	}

	return Hr;
}

static HRESULT CfixctlsCreateAndRegisterHost(
	__in PCWSTR ObjRefMoniker
	)
{
	//
	// Obtain agent.
	//
	ICfixAgent *Agent;
	HRESULT Hr = CfixctlsGetAgent( 
		ObjRefMoniker, IID_ICfixAgent, ( PVOID* ) &Agent );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	//
	// Create host.
	//
	ICfixHost *Host;
	Hr = CfixctlsCreateLocalHost( &Host );
	if ( SUCCEEDED( Hr ) )
	{
		//
		// Register with agent.
		//
		Hr = Agent->RegisterHost(
			GetCurrentProcessId(),
			Host );

		Host->Release();
	}
	
	Agent->Release();

	return Hr;
}

EXTERN_C HRESULT CfixctlServeHost()
{
	PWSTR ObjRefMoniker = NULL;

	//
	// Initialize.
	//
	CfixctlsShutdownEvent = CreateEvent( NULL, FALSE, FALSE, NULL );
	if ( ! CfixctlsShutdownEvent )
	{
		return HRESULT_FROM_WIN32( GetLastError() );
	}

	HRESULT Hr = CoInitializeEx( NULL, COINIT_MULTITHREADED );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = CfixctlpRegisterServerUnlockCallback( CfixctlsServerUnlocked );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Obtain moniker string from environment...
	//
	ULONG ObjRefMonikerLength = GetEnvironmentVariable(
		CFIXCTLP_MONIKER_ENVVAR_NAME,
		NULL,
		0 );
	if ( ObjRefMonikerLength == 0 )
	{
		Hr = CFIXCTL_E_MISSING_AGENT_MK;
		goto Cleanup;
	}

	ObjRefMoniker = new WCHAR[ ObjRefMonikerLength ];
	if ( ObjRefMoniker == NULL )
	{
		Hr = E_OUTOFMEMORY;
		goto Cleanup;
	}

	if ( 0 == GetEnvironmentVariable(
		CFIXCTLP_MONIKER_ENVVAR_NAME,
		ObjRefMoniker,
		ObjRefMonikerLength ) )
	{
		Hr = CFIXCTL_E_MISSING_AGENT_MK;
		goto Cleanup;
	}

	//
	// ...and connext to agent.
	//

	Hr = CfixctlsCreateAndRegisterHost( ObjRefMoniker );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Wait for all COM objects to be released.
	//
	( VOID ) WaitForSingleObject( CfixctlsShutdownEvent, INFINITE );

	//
	// All is well, but in case this is a custom host, do not run 
	// main.
	//
	Hr = CFIX_S_EXIT_PROCESS;

Cleanup:
	ASSERT( SUCCEEDED( Hr ) );

	if ( ObjRefMoniker != NULL )
	{
		delete [] ObjRefMoniker;
	}

	//
	// Shutdown.
	//
	CloseHandle( CfixctlsShutdownEvent );
	CoUninitialize();
	
	return Hr;
}