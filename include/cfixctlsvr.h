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


/*------------------------------------------------------------------------------
 *
 * License.
 *
 */

typedef enum CFIXCTL_LICENSE_TYPE
{
	CfixctlLicensed,
	CfixctlTrial
} CFIXCTL_LICENSE_TYPE;

typedef struct _CFIXCTL_LICENSE_INFO
{
	ULONG SizeOfStruct;

	CFIXCTL_LICENSE_TYPE Type;

	//
	// License key; only valid if Type is CfixctlLicensed.
	//
	WCHAR Key[ 30 ];
	
	//
	// Licensed Procuct/SubProduct; only valid if Type is CfixctlLicensed.
	//
	// N.B. The client has to validate this information.
	//
	UCHAR Product;
	UCHAR SubProduct;

	//
	// Indicates whether license is valid/trial period still active.
	//
	BOOL Valid;

	//
	// Days elapsed since installation/first use.
	//
	ULONG DaysInstalled;

	//
	// Days left for trial; only valid if Type is CfixctlTrial.
	//
	ULONG DaysLeft;
} CFIXCTL_LICENSE_INFO, *PCFIXCTL_LICENSE_INFO;

/*++
	Routine Description:
		Validate key.
--*/
EXTERN_C CFIXCTLAPI HRESULT CFIXCTLCALLTYPE CfixctlValidateLicense(
	__in PCWSTR Key
	);

/*++
	Routine Description:
		Validate and install license key.

		Requires administrative privileges.

	Arguments:
		Key				- The key to install. If NULL, lincense is removed.
		MachineWide		- Must be TRUE.
--*/
EXTERN_C CFIXCTLAPI HRESULT CFIXCTLCALLTYPE CfixctlInstallLicense(
	__in BOOL MachineWide,
	__in PCWSTR Key
	);

/*++
	Routine Description:
		Query information about current licensing situation.

	Arguments:
		MachineWide		- Must be TRUE.
		ExternalDateOfI - date of first use, in days since 1601-1-1.
		Info			- Result.
--*/
EXTERN_C CFIXCTLAPI HRESULT CFIXCTLCALLTYPE CfixctlQueryLicenseInfo(
	__in BOOL MachineWide,
	__in ULONG ExternalDateOfInstallation,
	__out PCFIXCTL_LICENSE_INFO Info
	);

