#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Internal declarations.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
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

#if DBG
	#define CFIXCTLP_TRACE( Args ) CfixctlpDbgPrint##Args
#else
	#define CFIXCTLP_TRACE( Args ) 
#endif

__inline VOID CfixctlpDbgPrint(
	__in PCWSTR Format,
	...
	)
{
	HRESULT hr;
	WCHAR Buffer[ 512 ];
	va_list lst;
	va_start( lst, Format );
	hr = StringCchVPrintf(
		Buffer, 
		_countof( Buffer ),
		Format,
		lst );
	va_end( lst );
	
	if ( SUCCEEDED( hr ) )
	{
		OutputDebugString( Buffer );
	}
}

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
IClassFactory& CfixctlpGetStackTraceFactory();
IClassFactory& CfixctlpGetStackTraceFrameFactory();

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
		Create an execution context proxy that performs a thread switch
		for each call. The thread servicing the calls must call 
		CfixctlpServiceThreadSwitchProxy.
--*/
HRESULT CfixctlpCreateThreadSwitchProxy(
	__in PCFIX_EXECUTION_CONTEXT Context,
	__out PCFIX_EXECUTION_CONTEXT *Proxy
	);

/*++
	Routine Description:
		Service all proxied calls..
--*/
HRESULT CfixctlpServiceThreadSwitchProxy(
	__in PCFIX_EXECUTION_CONTEXT Context,
	__in HANDLE WorkerThread
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

/*++
	Routine Description:
		Calculates the effective age of the installation, based on:
			* ExternalDate - date the client must pass.
			* Creation date of the folder the DLL has been loaded from.
			* Date of first use stored in registry

			The oldest date is taken as the basis for calulating the 
			effective age. The redudany is merely driven by the intent
			to confuse people trying to circumvent licensing.

		Parameters:
			ExternalDate - date of first use, in days since 2007-1-1.
--*/
HRESULT CfixctlpGetTrialInstallationAge(
	__in ULONG ExternalDate,
	__out ULONG *DaysInstalled
	);

/*++
	Routine Description:
		Check whether the trial period or the hard expiry date is over.
--*/
HRESULT CfixctlpIsTrialPeriodActive(
	__in ULONG DaysInstalled,
	__out BOOL *Active,
	__out ULONG *DaysLeft
	);

/*++
	Routine Description:
		Query license information, see CfixctlQueryLicenseInfo.
--*/
HRESULT CfixctlpQueryLicenseInfo(
	__in BOOL MachineWide,
	__in ULONG ExternalDateOfInstallation,
	__out PCFIXCTL_LICENSE_INFO Info
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

	STDMETHOD( CreateStackTrace )(
		__in PCFIX_STACKTRACE RawTrace,
		__out ICfixStackTrace **Trace
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

DEFINE_GUID( IID_ICfixStackTraceInternal, 
	0x6dca5f7c, 0xab9a, 0x4262, 0x8f, 0x11, 0xa4, 0xde, 0x27, 0x1e, 0xc1, 0x52);

interface
DECLSPEC_UUID( "6DCA5F7C-AB9A-4262-8F11-A4DE271EC152" ) 
ICfixStackTraceInternal : public ICfixStackTrace
{
	STDMETHOD( Initialize )(
		__in PCFIX_STACKTRACE StackTrace,
		__in CFIX_GET_INFORMATION_STACKFRAME_ROUTINE GetInfFrameRoutine
		) PURE;
};

DEFINE_GUID( IID_ICfixStackTraceFrameInternal, 
	0x6157cffd, 0x3680, 0x49ce, 0x8b, 0xbc, 0x2c, 0x82, 0xf3, 0xce, 0x3f, 0xb0);

interface
DECLSPEC_UUID( "6157CFFD-3680-49ce-8BBC-2C82F3CE3FB0" ) 
ICfixStackTraceFrameInternal : public ICfixStackTraceFrame
{
	STDMETHOD( Initialize )(
		__in ULONGLONG FramePc,
		__in CFIX_GET_INFORMATION_STACKFRAME_ROUTINE GetInfFrameRoutine
		) PURE;
};