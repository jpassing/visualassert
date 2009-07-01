/*----------------------------------------------------------------------
 * Purpose:
 *		COM Server.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#define CFIXCTLAPI

#include <olectl.h>
#include "cfixctlp.h"
#include "selfreg.h"

static volatile LONG CfixctlsServerLocks = 0;
static HMODULE CfixctlsModule = NULL;

static CFIXCRL_SERVER_UNLOCK_PROC CfixctlsUnlockCallback = NULL;

typedef IClassFactory& ( * CFIXCTLS_GETCLASSFACTORY_PROC )();

struct CFIXCTLS_SERVER
{
	CFIXCTLS_GETCLASSFACTORY_PROC GetClassFactory;
	CLSID Clsid;
	PCWSTR FriendlyName;
	PCWSTR ProgId;
	PCWSTR VerIndependentProgId;
	PCWSTR ThreadingModel;
};

static CFIXCTLS_SERVER CfixctlsServers[] =
{
	{ 
		CfixctlpGetTestCaseFactory,			// Creatable due to MBV.
		CLSID_TestCase, 
		L"Cfix TestCase",
		L"Cfix.Control.TestCase",
		L"Cfix.Control.TestCase.1",
		L"both"
	},
	{
		CfixctlpGetTestFixtureFactory,		// Creatable due to MBV.
		CLSID_TestFixture,
		L"Cfix TestFixture",
		L"Cfix.Control.TestFixture",
		L"Cfix.Control.TestFixture.1",
		L"both"
	},
	{
		CfixctlpGetStackTraceFactory,		// Creatable due to MBV.
		CLSID_StackTrace,
		L"Cfix StackTrace",
		L"Cfix.Control.StackTrace",
		L"Cfix.Control.StackTrace.1",
		L"both"
	},
	{
		CfixctlpGetStackTraceFrameFactory,	// Creatable due to MBV.
		CLSID_StackTraceFrame,
		L"Cfix StackTraceFrame",
		L"Cfix.Control.StackTraceFrame",
		L"Cfix.Control.StackTraceFrame.1",
		L"both"
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
		NULL,
		CLSID_LocalHost,
		L"Cfix LocalHost",
		L"Cfix.Control.LocalHost",
		L"Cfix.Control.LocalHost.1",
		L"free"
	},
	{
		NULL,
		CLSID_ProcessHost,
		L"Cfix ProcessHost",
		L"Cfix.Control.ProcessHost",
		L"Cfix.Control.ProcessHost.1",
		L"free"
	},
	{
		NULL,
		CLSID_FixtureExecutionAction,
		L"Cfix ExecutionAction",
		L"Cfix.Control.ExecutionAction",
		L"Cfix.Control.ExecutionAction.1",
		L"free"
	},
	{
		CfixctlpGetLocalAgentFactory,
		CLSID_LocalAgent,
		L"Cfix LocalAgent",
		L"Cfix.Control.LocalAgent",
		L"Cfix.Control.LocalAgent.1",
		L"free"							// Mind the blocking operations.
	},
	{
		NULL,
		CLSID_MessageResolver,
		L"Cfix MessageResolver",
		L"Cfix.Control.MessageResolver",
		L"Cfix.Control.MessageResolver.1",
		L"both"
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

HMODULE CfixctlpGetModule()
{
	return CfixctlsModule;
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
		if ( 0 == InterlockedDecrement( &CfixctlsServerLocks ) )
		{
			CFIXCTLP_TRACE( ( L"Last server lock released\n" ) );

			if ( CfixctlsUnlockCallback )
			{
				( CfixctlsUnlockCallback )();
			}
		}
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

	if ( Reason == DLL_PROCESS_ATTACH )
	{
		CfixctlsModule = Module;
		return TRUE;
	}
	else if ( Reason == DLL_PROCESS_DETACH )
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

HRESULT CfixctlpRegisterServerUnlockCallback(
	__in CFIXCRL_SERVER_UNLOCK_PROC Callback
	)
{
	if ( ! Callback )
	{
		return E_INVALIDARG;
	}

	if ( CfixctlsUnlockCallback )
	{
		return E_UNEXPECTED;
	}

	CfixctlsUnlockCallback = Callback;

	return S_OK;
}

/*----------------------------------------------------------------------
 *
 * Exports.
 *
 */

EXTERN_C HRESULT CfixctlGetClassObject(
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
			if ( CfixctlsServers[ Index ].GetClassFactory == NULL )
			{
				//
				// Noncreatable.
				//
				return CLASS_E_CLASSNOTAVAILABLE;
			}
			else
			{
				IClassFactory& factory = ( CfixctlsServers[ Index ].GetClassFactory )();
				return factory.QueryInterface( Iid, ClassObject );
			}
		}
	}

	return CLASS_E_CLASSNOTAVAILABLE;
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
	return CfixctlGetClassObject( Clsid, Iid, ClassObject );
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