/*----------------------------------------------------------------------
 * Purpose:
 *		Entry Point.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <windows.h>
#include <ole2.h>
#include <cfixctl.h>
#include <cfixctlsvr.h>
#include <crtdbg.h>

static HANDLE CfixhostsShutdownEvent = NULL;

/*----------------------------------------------------------------------
 *
 * Heap compatibility routines.
 *
 */

#ifndef _WIN64

#ifndef HeapEnableTerminationOnCorruption
#   define HeapEnableTerminationOnCorruption ( HEAP_INFORMATION_CLASS ) 1
#endif

typedef BOOL ( * HEAPSETINFORMATION_ROUTINE )(
	__in HANDLE HeapHandle,
	__in HEAP_INFORMATION_CLASS HeapInformationClass,
	__in PVOID HeapInformation,
	__in SIZE_T HeapInformationLength
	);

static BOOL CfixctlsHeapSetInformation(
	__in HANDLE HeapHandle,
	__in HEAP_INFORMATION_CLASS HeapInformationClass,
	__in PVOID HeapInformation,
	__in SIZE_T HeapInformationLength
	)
{
	HMODULE Kernel32Module = GetModuleHandle( L"kernel32" );
	_ASSERTE( Kernel32Module != NULL );

	HEAPSETINFORMATION_ROUTINE Routine = ( HEAPSETINFORMATION_ROUTINE ) 
		GetProcAddress( 
			Kernel32Module,
			"HeapSetInformation" );

	if ( Routine != NULL )
	{
		//
		// Windows XP or above.
		//
		return ( Routine )( 
			HeapHandle, 
			HeapInformationClass,
			HeapInformation,
			HeapInformationLength );
	}
	else
	{
		//
		// Windows 2000 or below.
		//
		return TRUE;
	}
}

#endif

/*----------------------------------------------------------------------
 *
 * Initialization.
 *
 */

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

#ifndef _WIN64
	//
	// Fail early on heap corruptions.
	//
	CfixctlsHeapSetInformation(
		NULL, 
		HeapEnableTerminationOnCorruption, 
		NULL, 
		0 );
#endif
	
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