#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Internal header file.
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

#include <cfixcc.h>
#include <ole2.h>
#include "..\cfixctl\cfixctlp.h"

#include <ole2.h>
#include <shlwapi.h>

#define CFIXCC_ASSERT_OK( Expr ) CFIX_ASSERT_EQUALS_ULONG( S_OK, ( Expr ) )

#if _M_AMD64
	#define TESTCTLP_OWN_ARCHITECTURE CfixTestModuleArchAmd64
#elif _M_IX86 
	#define TESTCTLP_OWN_ARCHITECTURE CfixTestModuleArchI386
#else
	#error Unsupported architecture
#endif

/*----------------------------------------------------------------------
 *
 * Stub classes.
 *
 */
class StubActionFactory : 
	public ICfixActionFactory
{
public:
	STDMETHOD_( ULONG, AddRef )()
	{
		return 1;
	}

	STDMETHOD_( ULONG, Release )()
	{
		return 1;
	}

	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr )
	{
		HRESULT Hr;

		if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
			 InlineIsEqualGUID( Iid, IID_ICfixActionFactory ) )
		{
			*Ptr = static_cast< ICfixActionFactory* >( this );
			Hr = S_OK;

		}
		else
		{
			*Ptr = NULL;
			Hr = E_NOINTERFACE;
		}

		if ( SUCCEEDED( Hr ) )
		{
			this->AddRef();
		}

		return Hr;
	}

	STDMETHOD( CreateExecutionAction )( 
        __in ULONG FixtureOrdinal,
        __in ULONG TestCaseOrdinal,
        __in ULONG SchedulingFlags,
        __in ULONG Reserved,
        __out ICfixAction **Action
		)
	{
		UNREFERENCED_PARAMETER( FixtureOrdinal );
		UNREFERENCED_PARAMETER( TestCaseOrdinal );
		UNREFERENCED_PARAMETER( SchedulingFlags );
		UNREFERENCED_PARAMETER( Reserved );
		UNREFERENCED_PARAMETER( Action );
		return S_OK;
	}
};

struct TEST_CFIX_FIXTURE_WITH_ARRAY
{
	CFIX_FIXTURE Fixture;
	CFIX_TEST_CASE Cases[ 2 ];
};

/*----------------------------------------------------------------------
 *
 * COM Test Routines.
 *
 */

typedef HRESULT ( STDMETHODCALLTYPE * GETCLASSOBJ_PROC )(
	__in REFCLSID clsid,
	__in REFIID iid,
	__out PVOID *ClassObject 
	);

typedef HRESULT ( STDMETHODCALLTYPE * CANUNLOADNOW_PROC )();
typedef HRESULT ( STDMETHODCALLTYPE * SELFREGISTER_PROC )();

typedef struct _COM_EXPORTS
{
	GETCLASSOBJ_PROC GetClassObject;
	CANUNLOADNOW_PROC CanUnloadNow;
	SELFREGISTER_PROC RegisterServer;
	SELFREGISTER_PROC UnregisterServer;
} COM_EXPORTS, *PCOM_EXPORTS;

void GetComExports( 
	__in PCWSTR ModuleName,
	__out PCOM_EXPORTS Exports
	);

void TestComUnknown(
	__in IUnknown *Unk,
	__in REFIID Iid1,
	__in REFIID Iid2
	);

void TestComClassFactory(
	__in IClassFactory *Factory,
	__in REFIID Iid
	);

void TestComServer(
	__in PCOM_EXPORTS Exports,
	__in REFCLSID Clsid
	);