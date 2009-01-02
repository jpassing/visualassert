/*----------------------------------------------------------------------
 * Purpose:
 *		Agent.
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

class LocalAgent : public ICfixAgent
{
	DECLARE_NOT_COPYABLE( LocalAgent );

private:
	
protected:
	LocalAgent();

public:
	virtual ~LocalAgent();

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
	
	STDMETHOD( CreateHost )( 
		__in CfixTestModuleArch Arch,
		__in DWORD Clsctx,
		__out ICfixHost** Host
		);
};

/*------------------------------------------------------------------
 * 
 * Factory.
 *
 */
IClassFactory& CfixctlpGetLocalAgentFactory()
{
	static ComClassFactory< ComMtaObject< LocalAgent >, CfixctlServerLock > Factory;
	return Factory;
}

/*------------------------------------------------------------------
 * 
 * Class Implementation.
 *
 */
LocalAgent::LocalAgent()
{
}

LocalAgent::~LocalAgent()
{
}

/*------------------------------------------------------------------
 * IUnknown methods.
 */

STDMETHODIMP LocalAgent::QueryInterface( 
	__in REFIID Iid, 
	__out PVOID* Ptr )
{
	HRESULT Hr;

	if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
		 InlineIsEqualGUID( Iid, IID_ICfixAgent ) )
	{
		*Ptr = static_cast< ICfixAgent* >( this );
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

	
STDMETHODIMP LocalAgent::CreateHost( 
	__in CfixTestModuleArch Arch,
	__in DWORD Clsctx,
	__out ICfixHost** Host
	)
{
	if ( ! Host )
	{
		return E_POINTER;
	}
	else
	{
		*Host = NULL;
	}

	if ( Arch > CfixTestModuleArchMax ||
		( ( Clsctx & ( CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER ) ) != 0 ) )
	{
		return E_INVALIDARG;
	}

	if ( ( Clsctx & CLSCTX_INPROC_SERVER ) && Arch == CFIXCTLP_OWN_ARCHITECTURE )
	{
		return CfixctlpGetHostFactory().CreateInstance(
			NULL, IID_ICfixHost, ( PVOID* ) Host );
	}
	else
	{
		// TODO.
		return E_NOTIMPL;
	}
}