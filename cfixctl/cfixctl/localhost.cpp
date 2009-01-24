/*----------------------------------------------------------------------
 * Purpose:
 *		LocalHost.
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

#include "cfixctlp.h"

/*------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 */

class LocalHost : 
	public ComObjectBase,
	public ICfixHost
{
	DECLARE_NOT_COPYABLE( LocalHost );

private:
	
protected:
	LocalHost();

public:
	virtual ~LocalHost();

	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )() PURE;
	STDMETHOD_( ULONG, Release )() PURE;
	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr );

	/*------------------------------------------------------------------
	 * ICfixHost methods.
	 */
	
	STDMETHOD( LoadModule )(
		__in const BSTR Path,
		__out ICfixTestModule **Module
		);

	STDMETHOD( GetArchitecture )(
		__out CfixTestModuleArch *Arch
		);

	STDMETHOD( Terminate )();

	STDMETHOD( SearchModules )(
		__in const BSTR PathFilter,
		__in ULONG Flags,
		__in ULONG Types,
		__in ULONG Architectures,
		__in ICfixSearchModulesCallback *Callback
		);
};

/*------------------------------------------------------------------
 * 
 * Factory.
 *
 */
IClassFactory& CfixctlpGetLocalHostFactory()
{
	static ComClassFactory< ComMtaObject< LocalHost >, CfixctlServerLock > Factory;
	return Factory;
}

/*------------------------------------------------------------------
 * 
 * LocalHost Class Implementation.
 *
 */
LocalHost::LocalHost()
{
}

LocalHost::~LocalHost()
{
}

/*------------------------------------------------------------------
 * IUnknown methods.
 */

STDMETHODIMP LocalHost::QueryInterface( 
	__in REFIID Iid, 
	__out PVOID* Ptr )
{
	HRESULT Hr;

	if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
		 InlineIsEqualGUID( Iid, IID_ICfixHost ) )
	{
		*Ptr = static_cast< ICfixHost* >( this );
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

/*------------------------------------------------------------------
 * ICfixHost methods.
 */

STDMETHODIMP LocalHost::LoadModule(
	__in const BSTR Path,
	__out ICfixTestModule **Result
	)
{
	if ( ! Result )
	{
		return E_POINTER;
	}
	else
	{
		*Result = NULL;
	}

	SIZE_T PathLen;
	if ( ! Path || ( PathLen = wcslen( Path ) ) < 5 )
	{
		return E_INVALIDARG;
	}

	ICfixTestModuleInternal *ModuleObject = NULL;
	PCFIX_TEST_MODULE Module = NULL;
	HRESULT Hr;
	CfixTestModuleType ModuleType;

	PCWSTR Extension = Path + PathLen - 4;
	if ( 0 == _wcsicmp( L".sys", Extension ) )
	{
		//
		// It is a driver.
		//
		ModuleType = CfixTestModuleTypeKernel;
		Hr = CfixklCreateTestModuleFromDriver(
			Path,
			&Module,
			NULL,
			NULL );
	}
	else if ( 0 == _wcsicmp( L".dll", Extension ) )
	{
		//
		// Assume DLL (may have custom extension).
		//
		ModuleType = CfixTestModuleTypeUser;
		Hr = CfixCreateTestModuleFromPeImage(
			Path,
			&Module );
	}
	else
	{
		ModuleType = CfixTestModuleTypeUser;
		Hr = CFIXCTL_E_UNRECOGNIZED_MODULE_TYPE;
	}

	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Create TestModule object.
	//
	Hr = CfixctlpGetTestModuleFactory().CreateInstance(
		NULL,
		IID_ICfixTestModuleInternal,
		( PVOID* ) &ModuleObject );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	Hr = ModuleObject->Initialize(
		Path,
		ModuleType,
		CFIXCTL_OWN_ARCHITECTURE,
		Module );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	*Result = ModuleObject;
	
Cleanup:
	if ( Module )
	{
		Module->Routines.Dereference( Module );
	}

	if ( FAILED( Hr ) )
	{
		if ( ModuleObject )
		{
			ModuleObject->Release();
		}
	}

	return Hr;
}

STDMETHODIMP LocalHost::GetArchitecture(
	__out CfixTestModuleArch *Arch
	)
{
	if ( ! Arch )
	{
		return E_POINTER;
	}
	else
	{
		*Arch = CFIXCTL_OWN_ARCHITECTURE;
		return S_OK;
	}
}

STDMETHODIMP LocalHost::Terminate()
{
	//
	// Terminating a local host is pointless.
	//
	return E_NOTIMPL;
}

static BOOL CfixctlsIsDll(
	__in PCWSTR Path
	)
{
	size_t Len = wcslen( Path );
	return ( Len > 4 && 0 == _wcsicmp( Path + Len - 4, L".dll" ) );
}

static BOOL CfixctlsIsSys(
	__in PCWSTR Path
	)
{
	size_t Len = wcslen( Path );
	return ( Len > 4 && 0 == _wcsicmp( Path + Len - 4, L".sys" ) );
}

struct CFIXCTLP_SEARCH_CONTEXT
{
	ICfixSearchModulesCallback *Callback;
	ULONG Type;
	ULONG Architecture;
};

static HRESULT CfixctlsFileCallback(
	__in BSTR Path,
	__in CFIXCTLP_SEARCH_CONTEXT *Context
	)
{
	//
	// Filter file types.
	//
	CfixTestModuleType Type;
	if ( CfixctlsIsDll( Path ) )
	{
		Type = CfixTestModuleTypeUser;
	}
	else if ( CfixctlsIsSys( Path ) )
	{
		Type = CfixTestModuleTypeKernel;
	}
	else
	{
		//
		// Not of interest.
		//
		return S_OK;
	}

	if ( Context->Type != ( ULONG ) -1 && 
		 Context->Type != ( ULONG ) Type )
	{
		return S_OK;
	}
	
	//
	// Query details.
	//
	CfixTestModuleArch Arch;

	CFIX_MODULE_INFO Info;
	Info.SizeOfStruct = sizeof( CFIX_MODULE_INFO );
	HRESULT Hr = CfixQueryPeImage( Path, &Info );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	if ( ! Info.FixtureExportsPresent )
	{
		//
		// Not a cfix module -- we are not interested.
		//
		return S_OK;
	}

	switch ( Info.MachineType )
	{
	case IMAGE_FILE_MACHINE_I386:
		Arch = CfixTestModuleArchI386;
		break;

	case IMAGE_FILE_MACHINE_AMD64:
		Arch = CfixTestModuleArchAmd64;
		break;

	default:
		//
		// Unrecognized - skip.
		//
		return S_OK;
	}

	if ( Context->Architecture != ( ULONG ) -1 && 
		 Context->Architecture != ( ULONG ) Arch )
	{
		return S_OK;
	}

	return Context->Callback->FoundModule(
		Path,
		Type,
		Arch );
}

static HRESULT CfixctlsSearchCallback(
	__in PCWSTR Path,
	__in CFIXUTIL_VISIT_TYPE Type,
	__in_opt PVOID PvContext,
	__in BOOL SearchPerformed
	)
{
	UNREFERENCED_PARAMETER( SearchPerformed );

	CFIXCTLP_SEARCH_CONTEXT *Context = 
		static_cast< CFIXCTLP_SEARCH_CONTEXT* >( PvContext );

	BSTR PathBstr = SysAllocString( Path );
	if ( PathBstr == NULL )
	{
		return E_OUTOFMEMORY;
	}

	HRESULT Hr;
	switch ( Type )
	{
	case CfixutilEnterDirectory:
		Hr = Context->Callback->EnterDirectory( PathBstr );
		break;

	case CfixutilLeaveDirectory:
		Hr = Context->Callback->LeaveDirectory( PathBstr );
		break;

	case CfixutilFile:
		Hr = CfixctlsFileCallback( PathBstr, Context );
		break;

	default:
		Hr = E_UNEXPECTED;
	}

	SysFreeString( PathBstr );
	return Hr;
}

STDMETHODIMP LocalHost::SearchModules(
	__in const BSTR PathFilter,
	__in ULONG Flags,
	__in ULONG Type,
	__in ULONG Arch,
	__in ICfixSearchModulesCallback *Callback
	)
{
	if ( PathFilter == NULL ||
		 Type != ( ULONG ) -1 && Type > CfixTestModuleTypeMax ||
		 Arch != ( ULONG ) -1 && Arch > CfixTestModuleArchMax ||
		 Callback == NULL )
	{
		return E_INVALIDARG;
	}

	CFIXCTLP_SEARCH_CONTEXT Context;
	Context.Callback		= Callback;
	Context.Type			= Type;
	Context.Architecture	= Arch;

	return CfixutilSearch(
		PathFilter,
		Flags & CFIXCTL_SEARCH_FLAG_RECURSIVE,
		CfixctlsSearchCallback,
		&Context );
}