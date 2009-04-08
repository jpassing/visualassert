/*----------------------------------------------------------------------
 * Purpose:
 *		MessageResolver.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
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
		L"cfixkl.dll",
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
	
	WCHAR Buffer[ 256 ] = { 0 };

	HRESULT Hr = this->Resolver->ResolveMessage(
		this->Resolver,
		MessageId,
		Reserved,
		NULL,
		_countof( Buffer ),
		Buffer );
	if ( SUCCEEDED( Hr ) )
	{
		*Message = SysAllocString( Buffer );
		if ( *Message == NULL )
		{
			Hr = E_OUTOFMEMORY;
		}
	}

	return Hr;
}