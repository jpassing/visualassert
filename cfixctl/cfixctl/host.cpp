/*----------------------------------------------------------------------
 * Purpose:
 *		Host.
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
#include "comutil.h"

#if _M_AMD64
	#define CFIXCTLS_ARCH CfixTestModuleArchAmd64
#elif _M_IX86 
	#define CFIXCTLS_ARCH CfixTestModuleArchI386
#else
	#error Unsupported architecture
#endif

/*------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 */

class Host : 
	public ICfixHost
{
	DECLARE_NOT_COPYABLE( Host );

private:
	
protected:
	Host();

public:
	virtual ~Host();

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
		__in BSTR const Path,
		__out ICfixTestModule **Module
		);

	STDMETHOD( GetArchitecture )(
		__out CfixTestModuleArch *Arch
		);
};

/*------------------------------------------------------------------
 * 
 * Factory.
 *
 */
IClassFactory& CfixctlpGetHostFactory()
{
	static ComClassFactory< ComMtaObject< Host > > Factory;
	return Factory;
}

/*------------------------------------------------------------------
 * 
 * Host Class Implementation.
 *
 */
Host::Host()
{
}

Host::~Host()
{
}

/*------------------------------------------------------------------
 * IUnknown methods.
 */

STDMETHODIMP Host::QueryInterface( 
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

STDMETHODIMP Host::LoadModule(
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
		CFIXCTLS_ARCH,
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

STDMETHODIMP Host::GetArchitecture(
	__out CfixTestModuleArch *Arch
	)
{
	if ( ! Arch )
	{
		return E_POINTER;
	}
	else
	{
		*Arch = CFIXCTLS_ARCH;
		return S_OK;
	}
}