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
#include <list.h>

/*------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 */
struct RegistrationEntry
{
	DWORD Cookie;
	ICfixHost *Host;

	RegistrationEntry( 
		__in DWORD Cookie,
		__in ICfixHost *Host
		) 
		: Cookie ( Cookie )
		, Host( Host )
	{
		this->Host->AddRef();
	}

	~RegistrationEntry()
	{
		this->Host->Release();
	}
};

class LocalAgent : public ICfixAgent
{
	DECLARE_NOT_COPYABLE( LocalAgent );

private:
	//
	// N.B. We currently allow one pending registration only.
	//
	CRITICAL_SECTION RegistrationLock;
	RegistrationEntry *Registration;
	HANDLE NewRegistrationEvent;

protected:
	LocalAgent();

public:
	virtual ~LocalAgent();

	void ClearRegistrations();

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

	STDMETHOD( RegisterHost )(
		__in DWORD Cookie,
		__in ICfixHost *Host 
		);

	STDMETHOD( WaitForHostConnection )(
		__in DWORD Cookie,
		__in ULONG Timeout,
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
LocalAgent::LocalAgent() : Registration( NULL ), NewRegistrationEvent( NULL )
{
	InitializeCriticalSection( &this->RegistrationLock );

	this->NewRegistrationEvent = CreateEvent( NULL, TRUE, FALSE, NULL );
	ASSERT( this->NewRegistrationEvent );
}

LocalAgent::~LocalAgent()
{
	ClearRegistrations();
	DeleteCriticalSection( &this->RegistrationLock );

	if ( this->NewRegistrationEvent != NULL )
	{
		VERIFY( CloseHandle( this->NewRegistrationEvent ) );
	}
}

void LocalAgent::ClearRegistrations()
{
	EnterCriticalSection( &this->RegistrationLock );

	if ( this->Registration != NULL )
	{
		delete this->Registration;
		this->Registration = NULL;
	}

	LeaveCriticalSection( &this->RegistrationLock );
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
		( ( Clsctx & ~( CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER ) ) != 0 ) )
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

STDMETHODIMP LocalAgent::RegisterHost(
	__in DWORD Cookie,
	__in ICfixHost *Host 
	)
{
	if ( Cookie == 0 || Host == NULL )
	{
		return E_INVALIDARG;
	}

	if ( this->NewRegistrationEvent == NULL )
	{
		return E_UNEXPECTED;
	}

	HRESULT Hr;
	EnterCriticalSection( &this->RegistrationLock );

	if ( this->Registration != NULL )
	{
		Hr = E_UNEXPECTED;
	}
	else
	{
		this->Registration = new RegistrationEntry( Cookie, Host );
		if ( this->Registration == NULL )
		{
			Hr = E_OUTOFMEMORY;
		}
		else
		{
			VERIFY( SetEvent( this->NewRegistrationEvent ) );
			Hr = S_OK;
		}
	}

	LeaveCriticalSection( &this->RegistrationLock );

	return Hr;
}

STDMETHODIMP LocalAgent::WaitForHostConnection(
	__in DWORD Cookie,
	__in ULONG Timeout,
	__out ICfixHost** Host
	)
{
	if ( Host == NULL )
	{
		return E_POINTER;
	}
	else
	{
		*Host = NULL;
	}

	if ( Cookie == 0  )
	{
		return E_INVALIDARG;
	}

	HRESULT Hr = E_FAIL;
	do
	{
		EnterCriticalSection( &this->RegistrationLock );
	
		if ( this->Registration != NULL )
		{
			if ( this->Registration->Cookie == Cookie )
			{
				*Host = Registration->Host;
				( *Host )->AddRef();

				delete this->Registration;
				this->Registration = NULL;
				Hr = S_OK;
			}
			else
			{
				Hr = CFIXCTL_E_HOST_NOT_FOUND;
			}

			break;
		}

		LeaveCriticalSection( &this->RegistrationLock );

		if ( *Host == NULL )
		{
			if ( Timeout == NULL )
			{
				Hr = CFIXCTL_E_HOST_NOT_FOUND;
				break;
			}
			else
			{
				//
				// Wait for a new registration.
				//
				DWORD WaitRes = WaitForSingleObject( 
					this->NewRegistrationEvent, Timeout );
				if ( WaitRes == WAIT_TIMEOUT )
				{
					return HRESULT_FROM_WIN32( ERROR_TIMEOUT );
				}
				else if ( WaitRes != WAIT_OBJECT_0 )
				{
					return HRESULT_FROM_WIN32( GetLastError() );
				}
				else
				{
					//
					// Search again.
					//
				}
			}
		}
	}
	while ( *Host == NULL );

	ASSERT( SUCCEEDED( Hr ) == ( *Host != NULL ) );
	return Hr;
}
