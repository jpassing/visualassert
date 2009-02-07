#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Internal declarations.
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

#include <cfix.h>
#include <cfixapi.h>
#include <ole2.h>
#include <cfixctl.h>
#include <cfixctlsvr.h>
#include <cfixctlmsg.h>
#include <crtdbg.h>
#include "comutil.h"

#pragma warning( push )
#pragma warning( disable: 6011; disable: 6387 )
#include <strsafe.h>
#pragma warning( pop )

#define ASSERT _ASSERTE

#ifndef VERIFY
	#if defined( DBG ) || defined( _DEBUG )
		#define VERIFY ASSERT
	#else
		#define VERIFY( x ) ( VOID ) ( x )
	#endif
#endif

/*----------------------------------------------------------------------
 *
 * Class factories.
 *
 */

#define CfixcrlpIsValidArch( Arch )										\
	( ( Arch ) >= CfixTestModuleArchI386 && ( Arch ) <= CfixTestModuleArchMax )

class CfixctlServerLock
{
public:
	void LockServer(
		__in BOOL Lock
		);
};

IClassFactory& CfixctlpGetTestCaseFactory();
IClassFactory& CfixctlpGetTestFixtureFactory();
IClassFactory& CfixctlpGetTestModuleFactory();
IClassFactory& CfixctlpGetLocalHostFactory();
IClassFactory& CfixctlpGetProcessHostFactory();
IClassFactory& CfixctlpGetExecutionActionFactory();
IClassFactory& CfixctlpGetLocalAgentFactory();
IClassFactory& CfixctlpGetMessageResolverFactory();

/*----------------------------------------------------------------------
 *
 * Internal Routines.
 *
 */

/*++
	Routine Description:
		Create a wrapper execution context that delegates events to
		a ICfixProcessEventSink.
--*/
HRESULT CfixctlpCreateExecutionContextAdapter(
	__in ICfixTestModule *Module,
	__in ICfixProcessEventSink *ProcessSink,
	__out PCFIX_EXECUTION_CONTEXT *Context
	);

/*++
	Routine Description:
		Issue an abort as soon as possible s.t. the execution is
		stopped orderly.
--*/
HRESULT CfixctlpAbortExecutionContextAdapter(
	__in PCFIX_EXECUTION_CONTEXT Context
	);


/*++
	Routine Description:
		Trace routine for debugging.
--*/
VOID CfixctlpDbgPrint(
	__in PSZ Format,
	...
	);

/*++
	Routine Description:
		Returns module handle of thie DLL.
--*/
HMODULE CfixctlpGetModule();

/*++
	Routine Description:
		Calls IsProcessInJob if available, else sets Result to FALSE.
--*/
BOOL CfixctlpIsProcessInJob(
	__in HANDLE ProcessHandle,
	__in HANDLE JobHandle,
	__out PBOOL Result
	);


/*----------------------------------------------------------------------
 *
 * Internal interfaces.
 *
 */

DEFINE_GUID( IID_ICfixTestCaseInternal, 
	0x221f2d4b, 0xdf5, 0x4755, 0x9c, 0xf4, 0xcd, 0x6b, 0x56, 0x6b, 0x8, 0x74 );

interface
DECLSPEC_UUID( "221F2D4B-0DF5-4755-9CF4-CD6B566B0874" ) 
ICfixTestCaseInternal : public ICfixTestItem
{
	STDMETHOD( Initialize )(
		__in PCWSTR Name,
		__in ULONG FixtureOrdinal,
		__in ULONG TestCaseOrdinal,
		__in ICfixActionFactory *ActionFactory
		) PURE;

	/*++
		Method Description:
			Return pointer to the name, valid only as long as the
			object itself remains valid.
	--*/
	STDMETHOD_( BSTR, GetNameInternal )() PURE;
};

DEFINE_GUID( IID_ICfixTestFixtureInternal, 
	0xfdac652, 0x27d6, 0x4283, 0x8b, 0x53, 0x97, 0x5a, 0x99, 0x28, 0xb, 0x97);

interface
DECLSPEC_UUID( "0FDAC652-27D6-4283-8B53-975A99280B97" ) 
ICfixTestFixtureInternal : public ICfixTestItem
{
	STDMETHOD( Initialize )(
		__in PCFIX_FIXTURE Fixture,
		__in ULONG FixtureOrdinal,
		__in ICfixActionFactory *ActionFactory
		) PURE;

	/*++
		Method Description:
			Return pointer to the name, valid only as long as the
			object itself remains valid.
	--*/
	STDMETHOD_( BSTR, GetNameInternal )() PURE;
};

DEFINE_GUID( IID_ICfixTestModuleInternal, 
	0x2e8f211, 0xe29b, 0x40e0, 0xb2, 0xba, 0x39, 0xba, 0x4c, 0x56, 0xb8, 0x39);

interface
DECLSPEC_UUID( "02E8F211-E29B-40e0-B2BA-39BA4C56B839" ) 
ICfixTestModuleInternal : public ICfixTestModule
{
	STDMETHOD( Initialize )(
		__in PCWSTR Path,
		__in CfixTestModuleType Type,
		__in CfixTestModuleArch Architecture,
		__in PCFIX_TEST_MODULE Module
		) PURE;
};

DEFINE_GUID( IID_ICfixExecutionActionInternal, 
	0x5bcd9d4e, 0xa622, 0x4d60, 0x86, 0xed, 0x97, 0xd, 0x11, 0xc1, 0x73, 0x8d);

interface
DECLSPEC_UUID( "5BCD9D4E-A622-4d60-86ED-970D11C1738D" ) 
ICfixExecutionActionInternal : public ICfixAction
{
	STDMETHOD( Initialize )(
		__in ICfixTestModule *Module,
		__in PCFIX_ACTION Action,
		__in ULONG TestCaseCount
		) PURE;
};

DEFINE_GUID( IID_ICfixProcessHostInternal, 
	0xeabee003, 0xa088, 0x4d2f, 0x82, 0x18, 0x17, 0x26, 0x1d, 0x39, 0xd6, 0x4c);

interface
DECLSPEC_UUID( "EABEE003-A088-4d2f-8218-17261D39D64C" ) 
ICfixProcessHostInternal : public ICfixHost
{
	STDMETHOD( Initialize )(
		__in ICfixHost *RemoteHost,
		__in HANDLE ProcessOrJob,
		__in BOOL UsesJob
		) PURE;
};