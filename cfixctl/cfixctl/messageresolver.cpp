/*----------------------------------------------------------------------
 * Purpose:
 *		MessageResolver.
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
#include <cdiag.h>

/*------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 */

class MessageResolver : 
	public ComObjectBase,
	public ICfixMessageResolver
{
	DECLARE_NOT_COPYABLE( MessageResolver );

private:
	PCDIAG_MESSAGE_RESOLVER Resolver;

protected:
	MessageResolver();

public:
	virtual ~MessageResolver();

	STDMETHOD( FinalConstruct )();

	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )() PURE;
	STDMETHOD_( ULONG, Release )() PURE;
	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr );

	/*------------------------------------------------------------------
	 * ICfixMessageResolver methods.
	 */
	
	STDMETHOD( ResolveMessage )(
		__in ULONG MessageId,
		__in ULONG Reserved,
		__out BSTR* Message
		);
};

/*------------------------------------------------------------------
 * 
 * Factory.
 *
 */
IClassFactory& CfixctlpGetMessageResolverFactory()
{
	static ComClassFactory< ComMtaObject< MessageResolver >, CfixctlServerLock > Factory;
	return Factory;
}

/*------------------------------------------------------------------
 * 
 * MessageResolver Class Implementation.
 *
 */
MessageResolver::MessageResolver() : Resolver( NULL )
{
}

MessageResolver::~MessageResolver()
{
	if ( this->Resolver )
	{
		this->Resolver->Dereference( this->Resolver );
	}
}

STDMETHODIMP MessageResolver::FinalConstruct() 
{
	HRESULT Hr = CdiagCreateMessageResolver( &this->Resolver );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	//
	// Register cfix message DLLs.
	//
	PWSTR Dlls[] = { 
		L"cfixctl.dll",
		L"cfix.dll",
		L"cdiag.dll"
	};

	for ( ULONG Index = 0; Index < _countof( Dlls ); Index++ )
	{
		Hr = this->Resolver->RegisterMessageDll(
			this->Resolver,
			Dlls[ Index ],
			0,
			0 );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}
	}

	return Hr;
}

/*------------------------------------------------------------------
 * IUnknown methods.
 */

STDMETHODIMP MessageResolver::QueryInterface( 
	__in REFIID Iid, 
	__out PVOID* Ptr )
{
	HRESULT Hr;

	if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
		 InlineIsEqualGUID( Iid, IID_ICfixMessageResolver ) )
	{
		*Ptr = static_cast< ICfixMessageResolver* >( this );
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
 * ICfixMessageResolver methods.
 */

STDMETHODIMP MessageResolver::ResolveMessage(
	__in ULONG MessageId,
	__in ULONG Reserved,
	__out BSTR* Message
	)
{
	if ( ! Message )
	{
		return E_INVALIDARG;
	}
	else
	{
		*Message = NULL;
	}

	if ( Reserved != 0 )
	{
		return E_INVALIDARG;
	}

	ULONG MessageSize = 256;
	BSTR Buffer = SysAllocStringLen( NULL, MessageSize );
	if ( ! Buffer ) 
	{
		return E_OUTOFMEMORY;
	}

	HRESULT Hr = this->Resolver->ResolveMessage(
		this->Resolver,
		MessageId,
		Reserved,
		NULL,
		MessageSize,
		Buffer );
	if ( SUCCEEDED( Hr ) )
	{
		*Message = Buffer;
	}
	else
	{
		SysFreeString( Buffer );
	}

	return Hr;
}