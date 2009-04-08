#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Auxiliary header file. 
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#ifdef _WIN64
#define CFIXCTLCALLTYPE
#else
#define CFIXCTLCALLTYPE __stdcall
#endif

#if !defined(CFIXCTLAPI)
#define CFIXCTLAPI __declspec(dllimport)
#endif


#if _M_AMD64
	#define CFIXCTL_OWN_ARCHITECTURE CfixTestModuleArchAmd64
#elif _M_IX86 
	#define CFIXCTL_OWN_ARCHITECTURE CfixTestModuleArchI386
#else
	#error Unsupported architecture
#endif

typedef VOID ( CFIXCTLCALLTYPE * CFIXCRL_SERVER_UNLOCK_PROC )();

/*++
	Routine Description:
		Register a callback that will be invoked whenever the server 
		lock count drops to 0.
--*/
EXTERN_C CFIXCTLAPI HRESULT CFIXCTLCALLTYPE CfixctlRegisterServerUnlockCallback(
	__in CFIXCRL_SERVER_UNLOCK_PROC Callback
	);

/*++
	Routine Description:
		Obtain class factory. Analog to DllGetClassObject.
--*/
EXTERN_C CFIXCTLAPI HRESULT CFIXCTLCALLTYPE CfixctlGetClassObject(
	__in REFCLSID Clsid,
	__in REFIID Iid,
	__out PVOID *ClassObject 
	);
