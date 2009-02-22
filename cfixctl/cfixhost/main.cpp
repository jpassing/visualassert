/*----------------------------------------------------------------------
 * Purpose:
 *		Entry Point.
 *
 * Copyright:
 *		2009, Johannes Passing (passing at users.sourceforge.net)
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
#include <windows.h>
#include <ole2.h>
#include <cfixctl.h>
#include <cfixctlsvr.h>

static HANDLE CfixhostsShutdownEvent = NULL;

static VOID CfixhostsServerUnlocked()
{
	SetEvent( CfixhostsShutdownEvent );
}

static HRESULT CfixhostsGetAgent( 
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

static HRESULT CfixhostsCreateLocalHost( 
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
				NULL,	// Parent has set the current directory for us.
				Host );

			LocalAgent->Release();
		}

		AgentFactory->Release();
	}

	return Hr;
}

static HRESULT CfixhostsCreateAndRegisterHost(
	__in PCWSTR ObjRefMoniker
	)
{
	//
	// Obtain agent.
	//
	ICfixAgent *Agent;
	HRESULT Hr = CfixhostsGetAgent( 
		ObjRefMoniker, IID_ICfixAgent, ( PVOID* ) &Agent );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	//
	// Create host.
	//
	ICfixHost *Host;
	Hr = CfixhostsCreateLocalHost( &Host );
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


int wWinMain(
	__in HINSTANCE Instance,
    __in HINSTANCE PrevInstance,
    __in PWSTR CmdLine,
    __in int CmdShow
	)
{
	UNREFERENCED_PARAMETER( Instance );
	UNREFERENCED_PARAMETER( PrevInstance );
	UNREFERENCED_PARAMETER( CmdShow );

	if ( ! CmdLine || wcslen( CmdLine ) == 0 )
	{
		return E_INVALIDARG;
	}

	//
	// No 'drive not ready'-dialogs, please.
	//
	SetErrorMode( SetErrorMode( 0 ) | SEM_FAILCRITICALERRORS );

	//
	// Initialize.
	//
	CfixhostsShutdownEvent = CreateEvent( NULL, FALSE, FALSE, NULL );
	if ( ! CfixhostsShutdownEvent )
	{
		return HRESULT_FROM_WIN32( GetLastError() );
	}

	HRESULT Hr = CoInitializeEx( NULL, COINIT_MULTITHREADED );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = CfixctlRegisterServerUnlockCallback( CfixhostsServerUnlocked );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// The commandline is assumed to hold an OBJREF moniker.
	//
	Hr = CfixhostsCreateAndRegisterHost( CmdLine );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Wait for all COM objects to be released.
	//
	( VOID ) WaitForSingleObject( CfixhostsShutdownEvent, INFINITE );

	Hr = S_OK;

Cleanup:
	//
	// Shutdown.
	//
	CloseHandle( CfixhostsShutdownEvent );
	CoUninitialize();
	
	return Hr;
}