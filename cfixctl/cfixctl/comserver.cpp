/*----------------------------------------------------------------------
 * Purpose:
 *		COM Server.
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

#include <olectl.h>
#include "cfixctlp.h"
#include "selfreg.h"

static volatile LONG CfixctlsServerLocks = 0;
static HMODULE CfixctlsModule = NULL;

typedef IClassFactory& ( * CFIXCTLS_GETCLASSFACTORY_PROC )();

#pragma warning( disable: 4510 )	// Reference member in POD struct.
#pragma warning( disable: 4512 )
#pragma warning( disable: 4610 )

struct CFIXCTLS_SERVER
{
	CFIXCTLS_GETCLASSFACTORY_PROC GetClassFactory;
	REFCLSID Clsid;
	PCWSTR FriendlyName;
	PCWSTR ProgId;
	PCWSTR VerIndependentProgId;
	PCWSTR ThreadingModel;
};

static CFIXCTLS_SERVER CfixctlsServers[] =
{
	{ 
		CfixctlpGetTestCaseFactory, 
		CLSID_TestCase, 
		L"Cfix TestCase",
		L"Cfix.Control.TestCase",
		L"Cfix.Control.TestCase.1",
		L"free"
	},
	{
		CfixctlpGetTestFixtureFactory,
		CLSID_TestFixture,
		L"Cfix TestCase",
		L"Cfix.Control.TestFixture",
		L"Cfix.Control.TestFixture.1",
		L"free"
	},
	{
		CfixctlpGetTestModuleFactory,
		CLSID_TestModule,
		L"Cfix TestModule",
		L"Cfix.Control.TestModule",
		L"Cfix.Control.TestModule.1",
		L"free"
	},
	{
		CfixctlpGetHostFactory,
		CLSID_Host,
		L"Cfix Host",
		L"Cfix.Control.Host",
		L"Cfix.Control.Host.1",
		L"free"
	}
};

static HRESULT CfixctlsRegisterServer( BOOL Register )
{
	HRESULT Hr;

	for ( ULONG Index = 0; Index < _countof( CfixctlsServers ); Index++ )
	{
		if ( FAILED( Hr = CfixctlpRegisterServer(
			CfixctlsModule,
			CfixctlpServerTypeInproc,
			CfixctlpServerRegScopeUser,
			CfixctlsServers[ Index ].Clsid,
			CfixctlsServers[ Index ].FriendlyName,
			CfixctlsServers[ Index ].ProgId,
			CfixctlsServers[ Index ].VerIndependentProgId,
			CfixctlsServers[ Index ].ThreadingModel,
			Register ) ) )
		{
			goto Cleanup;
		}
	}

	Hr = S_OK;
	
Cleanup:
	if ( FAILED( Hr ) )
	{
		if ( Register )
		{
			//
			// Unregister everything.
			//
			( VOID ) CfixctlsRegisterServer( FALSE );
		}
		else
		{
			//
			// Unregistration failed - there is little we can do.
			//
		}
	}

	return Hr;
}

static HRESULT CfixctlsRegisterTypeLib()
{
	ASSERT( CfixctlsModule );

	WCHAR ModulePath[ MAX_PATH ];
	if ( ! GetModuleFileName(
		CfixctlsModule,
		ModulePath,
		_countof( ModulePath ) ) )
	{
		return HRESULT_FROM_WIN32( GetLastError() );
	}

	//
	// Append resource identifier.
	//
	HRESULT Hr;
	if ( FAILED( Hr = StringCchCat(
		ModulePath,
		_countof( ModulePath ),
		L"\\1" ) ) )
	{
		return Hr;
	}
	
	ITypeLib *TypeLib;
	if ( FAILED( Hr = LoadTypeLibEx( ModulePath, REGKIND_REGISTER, &TypeLib ) ) )
	{
		return Hr;
	}

	return S_OK;
}

static HRESULT CfixctlsUnregisterTypeLib()
{
	return UnRegisterTypeLib(
		LIBID_Cfixctl,
		1,
		0,
		0,
#if _WIN64
		SYS_WIN64
#else
		SYS_WIN32
#endif
		);
}

/*----------------------------------------------------------------------
 *
 * Server lock maintenance.
 *
 */

void CfixctlServerLock::LockServer(
	__in BOOL Lock
	)
{
	if ( Lock )
	{
		InterlockedIncrement( &CfixctlsServerLocks );
	}
	else
	{
		ASSERT( CfixctlsServerLocks > 0 );
		InterlockedDecrement( &CfixctlsServerLocks );
	}
}

/*----------------------------------------------------------------------
 *
 * DllMain.
 *
 */
BOOL APIENTRY DllMain( 
	__in HMODULE Module,
	__in DWORD Reason,
	__in LPVOID Reserved
)
{
	UNREFERENCED_PARAMETER( Module );
	UNREFERENCED_PARAMETER( Reserved );

	if ( Reason ==  DLL_PROCESS_ATTACH )
	{
		CfixctlsModule = Module;
		return TRUE;
	}
	else if ( Reason ==  DLL_PROCESS_DETACH )
	{
#ifdef DBG	
		_CrtDumpMemoryLeaks();
#endif
		return TRUE;
	}
	else
	{
		return TRUE;
	}
}

/*----------------------------------------------------------------------
 *
 * COM Server Exports.
 *
 */

HRESULT STDMETHODCALLTYPE DllCanUnloadNow()
{
	return CfixctlsServerLocks == 0 ? S_OK : S_FALSE;
}

HRESULT STDMETHODCALLTYPE DllGetClassObject(
	__in REFCLSID Clsid,
	__in REFIID Iid,
	__out PVOID *ClassObject 
	)
{
	if ( ! ClassObject )
	{
		return E_INVALIDARG;
	}

	for ( ULONG Index = 0; Index < _countof( CfixctlsServers ); Index++ )
	{
		if ( InlineIsEqualGUID( Clsid, CfixctlsServers[ Index ].Clsid ) )
		{
			IClassFactory& factory = ( CfixctlsServers[ Index ].GetClassFactory )();
			return factory.QueryInterface( Iid, ClassObject );
		}
	}

	return CLASS_E_CLASSNOTAVAILABLE;
}

HRESULT STDMETHODCALLTYPE DllRegisterServer()
{
	if ( FAILED( CfixctlsRegisterTypeLib() ) )
	{
		return SELFREG_E_TYPELIB;
	}

	if ( FAILED( CfixctlsRegisterServer( TRUE ) ) )
	{
		return SELFREG_E_CLASS;
	}

	return S_OK;
}

HRESULT STDMETHODCALLTYPE DllUnregisterServer()
{
	if ( FAILED( CfixctlsUnregisterTypeLib() ) )
	{
		return SELFREG_E_TYPELIB;
	}

	if ( FAILED( CfixctlsRegisterServer( FALSE ) ) )
	{
		return SELFREG_E_CLASS;
	}

	return S_OK;
}