/*----------------------------------------------------------------------
 * Purpose:
 *		COM Self Registration.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <windows.h>

typedef enum _CFIXCTLP_SERVER_TYPE
{
	CfixctlpServerTypeInproc,
	CfixctlpServerTypeLocal,
} CFIXCTLP_SERVER_TYPE;

typedef enum _CFIXCTLP_SERVER_REGSCOPE
{
	CfixctlpServerRegScopeGlobal,
	CfixctlpServerRegScopeUser,
} CFIXCTLP_SERVER_REGSCOPE;

/*++
	Routine Description:
		Self (un-) register COM server.
--*/
EXTERN_C HRESULT CfixctlpRegisterServer(
	__in HMODULE Module,
	__in CFIXCTLP_SERVER_TYPE ServerType,
	__in CFIXCTLP_SERVER_REGSCOPE RegScope,
	__in REFCLSID Clsid,
	__in PCWSTR FriendlyName,
	__in PCWSTR VersionIndependentProgId,
	__in PCWSTR ProgId,
	__in PCWSTR ThreadingModel,
	__in BOOL Register
	);