/*----------------------------------------------------------------------
 * Purpose:
 *		ProcessHost.
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

const GUID IID_ICfixProcessHostInternal = 
	{ 0xeabee003, 0xa088, 0x4d2f, { 0x82, 0x18, 0x17, 0x26, 0x1d, 0x39, 0xd6, 0x4c } };


/*------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 */

class ProcessHost : 
	public ICfixProcessHostInternal
{
	DECLARE_NOT_COPYABLE( ProcessHost );

private:
	ICfixHost *Host;
	HANDLE ProcessOrJob;
	BOOL UsesJob;

protected:
	ProcessHost();

public:
	virtual ~ProcessHost();

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

	/*------------------------------------------------------------------
	 * IID_ICfixProcessHostInternal methods.
	 */

	STDMETHOD( Initialize )(
		__in ICfixHost *RemoteHost,
		__in HANDLE ProcessOrJob,
		__in BOOL UsesJob
		);
};

/*------------------------------------------------------------------
 * 
 * Factory.
 *
 */

IClassFactory& CfixctlpGetProcessHostFactory()
{
	static ComClassFactory< ComMtaObject< ProcessHost >, CfixctlServerLock > Factory;
	return Factory;
}

/*------------------------------------------------------------------
 * 
 * ProcessHost Class Implementation.
 *
 */

ProcessHost::ProcessHost() 
	: Host( NULL )
	, ProcessOrJob( NULL )
	, UsesJob( FALSE )
{
}

ProcessHost::~ProcessHost()
{
	if ( this->Host )
	{
		this->Host->Release();
	}

	if ( this->ProcessOrJob )
	{
		VERIFY( CloseHandle( this->ProcessOrJob ) );
	}
}

/*------------------------------------------------------------------
 * IUnknown methods.
 */

STDMETHODIMP ProcessHost::QueryInterface( 
	__in REFIID Iid, 
	__out PVOID* Ptr )
{
	HRESULT Hr;

	if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
		 InlineIsEqualGUID( Iid, IID_ICfixHost ) ||
		 InlineIsEqualGUID( Iid, IID_ICfixProcessHostInternal ) )
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

STDMETHODIMP ProcessHost::LoadModule(
	__in const BSTR Path,
	__out ICfixTestModule **Result
	)
{
	if ( ! this->Host )
	{
		return E_UNEXPECTED;
	}
	else
	{
		return Host->LoadModule( Path, Result );
	}
}

STDMETHODIMP ProcessHost::GetArchitecture(
	__out CfixTestModuleArch *Arch
	)
{
	if ( ! this->Host )
	{
		return E_UNEXPECTED;
	}
	else
	{
		return Host->GetArchitecture( Arch );
	}
}

STDMETHODIMP ProcessHost::Terminate()
{
	if ( ! this->Host || ! this->ProcessOrJob )
	{
		return E_UNEXPECTED;
	}
	
	BOOL Success;
	if ( this->UsesJob )
	{
		Success = TerminateJobObject( 
			this->ProcessOrJob, ( UINT ) CFIXCTL_E_USER_ABORT );
	}
	else
	{
		Success = TerminateProcess( 
			this->ProcessOrJob, ( UINT ) CFIXCTL_E_USER_ABORT );
	}

	if ( Success )
	{
		return S_OK;
	}
	else
	{
		return HRESULT_FROM_WIN32( GetLastError() );
	}
}

STDMETHODIMP ProcessHost::SearchModules(
	__in const BSTR PathFilter,
	__in ULONG Flags,
	__in ULONG Type,
	__in ULONG Arch,
	__in ICfixSearchModulesCallback *Callback
	)
{
	if ( ! this->Host )
	{
		return E_UNEXPECTED;
	}
	else
	{
		return Host->SearchModules( 
			PathFilter, Flags, Type, Arch, Callback );
	}
}

/*------------------------------------------------------------------
 * IID_ICfixProcessHostInternal methods.
 */

STDMETHODIMP ProcessHost::Initialize(
	__in ICfixHost *RemoteHost,
	__in HANDLE ProcessOrJob,
	__in BOOL UsesJob
	)
{
	if ( this->Host != NULL || this->ProcessOrJob != NULL )
	{
		return E_UNEXPECTED;
	}

	if ( RemoteHost == NULL || ProcessOrJob == NULL )
	{
		return E_INVALIDARG;
	}

	RemoteHost->AddRef();
	this->Host			= RemoteHost;
	this->ProcessOrJob	= ProcessOrJob;
	this->UsesJob		= UsesJob;

	return S_OK;
}