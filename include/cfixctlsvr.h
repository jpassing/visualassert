#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Auxiliary header file. 
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