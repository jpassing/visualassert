/*----------------------------------------------------------------------
 * Purpose:
 *		COM Self Registration.
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