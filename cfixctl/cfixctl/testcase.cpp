/*----------------------------------------------------------------------
 * Purpose:
 *		TestCase class.
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

// {221F2D4B-0DF5-4755-9CF4-CD6B566B0874}
const GUID IID_ICfixTestCaseInternal = 
	{ 0x221f2d4b, 0xdf5, 0x4755, { 0x9c, 0xf4, 0xcd, 0x6b, 0x56, 0x6b, 0x8, 0x74 } };

/*------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 * N.B. This class marshals by value.
 *
 */

class TestCase : 
	public ICfixTestCaseInternal,
	public IMarshal
{
	DECLARE_NOT_COPYABLE( TestCase );

private:
	//
	// N.B. Members are effectively immutable.
	//
	BSTR Name;
	ULONG FixtureOrdinal;
	ULONG Ordinal;
	ICfixActionFactory *ActionFactory;

protected:
	TestCase();

public:
	virtual ~TestCase();

	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )() PURE;
	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr );

	/*------------------------------------------------------------------
	 * IMarshal methods.
	 */

	STDMETHOD( GetUnmarshalClass )( 
		__in REFIID Iid,
		__in PVOID Itf,
		__in DWORD DestContext,
		__in PVOID Reserved,
		__in DWORD Mshlflags,
		__out CLSID *Clsid
		);

	STDMETHOD( GetMarshalSizeMax )( 
		__in REFIID riid,
		__in void *pv,
		__in DWORD dwDestContext,
		__in void *pvDestContext,
		__in DWORD mshlflags,
		__out DWORD *pSize
		);

	STDMETHOD( MarshalInterface )( 
		__in IStream *pStm,
		__in REFIID riid,
		__in void *pv,
		__in DWORD dwDestContext,
		__in void *pvDestContext,
		__in DWORD mshlflags
		);

	STDMETHOD( UnmarshalInterface )( 
		__in IStream *pStm,
		__in REFIID riid,
		__out void **ppv
		);

	STDMETHOD( ReleaseMarshalData )( 
		__in IStream *Stm
		);

	STDMETHOD( DisconnectObject )( 
		__in DWORD Reserved
		);

	/*------------------------------------------------------------------
	 * ICfixTestItem methods.
	 */

	STDMETHOD( GetName )(
        __out BSTR *Name
		);

	STDMETHOD( CreateExecutionAction )(
		__in ULONG Flags,
		__in ULONG Reserved,
		__out ICfixAction **Action 
		);

	/*------------------------------------------------------------------
	 * ICfixTestCaseInternal methods.
	 */

	STDMETHOD( Initialize )(
		__in PCWSTR Name,
		__in ULONG FixtureOrdinal,
		__in ULONG TestCaseOrdinal,
		__in ICfixActionFactory *ActionFactory
		);

	STDMETHOD_( BSTR, GetNameInternal )();
};

/*------------------------------------------------------------------
 * 
 * Factory.
 *
 */
IClassFactory& CfixctlpGetTestCaseFactory()
{
	static ComClassFactory< ComMtaObject< TestCase >, CfixctlServerLock > Factory;
	return Factory;
}

/*------------------------------------------------------------------
 * 
 * Class Implementation.
 *
 */
TestCase::TestCase() 
	: Name( NULL )
	, Ordinal( 0 )
	, FixtureOrdinal( 0 )
	, ActionFactory( NULL )
{
}

TestCase::~TestCase()
{
	if ( this->Name != NULL )
	{
		SysFreeString( this->Name );
	}

	if ( this->ActionFactory != NULL )
	{
		ActionFactory->Release();
	}
}

/*------------------------------------------------------------------
 * IUnknown methods.
 */

STDMETHODIMP TestCase::QueryInterface( 
	__in REFIID Iid, 
	__out PVOID* Ptr )
{
	HRESULT Hr;

	if ( ! Ptr )
	{
		return E_POINTER;
	}

	if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
	     InlineIsEqualGUID( Iid, IID_ICfixTestItem ) )
	{
		*Ptr = static_cast< ICfixTestItem* >( this );
		Hr = S_OK;

	}
	else if ( InlineIsEqualGUID( Iid, IID_IMarshal ) )
	{
		*Ptr = static_cast< IMarshal* >( this );
		Hr = S_OK;
	}
	else if ( InlineIsEqualGUID( Iid, IID_ICfixTestCaseInternal ) )
	{
		*Ptr = static_cast< ICfixTestCaseInternal* >( this );
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
};

/*------------------------------------------------------------------
 * IMarshal methods.
 */

STDMETHODIMP TestCase::GetUnmarshalClass( 
	__in REFIID Iid,
	__in PVOID Itf,
	__in DWORD DestContext,
	__in PVOID Reserved,
	__in DWORD Mshlflags,
	__out CLSID *Clsid
	)
{
	UNREFERENCED_PARAMETER( Iid );
	UNREFERENCED_PARAMETER( Itf );
	UNREFERENCED_PARAMETER( DestContext );
	UNREFERENCED_PARAMETER( Reserved );
	UNREFERENCED_PARAMETER( Mshlflags );

	//
	// Marshal by value, i.e. use own CLSID.
	//
	*Clsid = CLSID_TestCase;
	return S_OK;
}

STDMETHODIMP TestCase::GetMarshalSizeMax( 
	__in REFIID Iid,
	__in PVOID Itf,
	__in DWORD DestContext,
	__in PVOID Reserved,
	__in DWORD Mshlflags,
	__out DWORD *Size
	)
{
	UNREFERENCED_PARAMETER( Iid );
	UNREFERENCED_PARAMETER( Itf );

	if ( ! Size )
	{
		return E_POINTER;
	}
	else
	{
		*Size = 0;
	}

	ULONG ItfSize;
	HRESULT Hr = CoGetMarshalSizeMax(
		&ItfSize,
		IID_ICfixActionFactory,
		this->ActionFactory,
		DestContext,
		Reserved,
		Mshlflags );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	*Size = 
		ComGetUnmarshalSizeMaxBSTR( this->Name ) +
		sizeof( ULONG ) +	// FixtureOrdinal.
		sizeof( ULONG ) +	// Ordinal.
		ItfSize;			// ActionFactory.
	return S_OK;
}

STDMETHODIMP TestCase::MarshalInterface( 
	__in IStream *Stm,
	__in REFIID Iid,
	__in PVOID Itf,
	__in DWORD DestContext,
	__in PVOID Reserved,
	__in DWORD Mshlflags
	)
{
	ASSERT( this->Name != NULL );

	UNREFERENCED_PARAMETER( Iid );
	UNREFERENCED_PARAMETER( Itf );
	UNREFERENCED_PARAMETER( DestContext );
	UNREFERENCED_PARAMETER( Reserved );
	UNREFERENCED_PARAMETER( Mshlflags );

	HRESULT Hr = ComMarshalBSTR( Stm, this->Name );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = Stm->Write( &this->FixtureOrdinal, sizeof( ULONG ), NULL );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = Stm->Write( &this->Ordinal, sizeof( ULONG ), NULL );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	return CoMarshalInterface( 
		Stm, 
		IID_ICfixActionFactory, 
		this->ActionFactory, 
		DestContext, 
		Reserved, 
		Mshlflags );
}

STDMETHODIMP TestCase::UnmarshalInterface( 
	__in IStream *Stm,
	__in REFIID Iid,
	__out PVOID *Object
	)
{
	ASSERT( this->Name == NULL );
	ASSERT( this->ActionFactory == NULL );

	if ( ! Object )
	{
		return E_POINTER;
	}
	else
	{
		*Object = NULL;
	}

	HRESULT Hr = ComUnmarshalBSTR( Stm, &this->Name );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}
	
	ASSERT( this->Name != NULL );

	Hr = Stm->Read( &this->FixtureOrdinal, sizeof( ULONG ), NULL );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = Stm->Read( &this->Ordinal, sizeof( ULONG ), NULL );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = CoUnmarshalInterface( 
		Stm, 
		IID_ICfixActionFactory, 
		( PVOID* ) &this->ActionFactory );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	return QueryInterface( Iid, Object );
}

STDMETHODIMP TestCase::ReleaseMarshalData( 
	__in IStream *Stm
	)
{
	UNREFERENCED_PARAMETER( Stm );
	return S_OK;
}

STDMETHODIMP TestCase::DisconnectObject( 
	__in DWORD Reserved
	)
{
	UNREFERENCED_PARAMETER( Reserved );
	return S_OK;
}

/*------------------------------------------------------------------
 * ICfixTestItem methods.
 */

STDMETHODIMP TestCase::GetName(
    __out BSTR *Name
	)
{
	return ComGetProperty( this->Name, Name );
}

STDMETHODIMP TestCase::CreateExecutionAction(
	__in ULONG Flags,
	__in ULONG Reserved,
	__out ICfixAction **Action 
	)
{
	return this->ActionFactory->CreateExecutionAction(
		this->FixtureOrdinal,
		this->Ordinal,
		Flags,
		Reserved,
		Action );
}

/*------------------------------------------------------------------
 * ICfixTestCaseInternal methods.
 */

STDMETHODIMP TestCase::Initialize(
	__in PCWSTR Name,
	__in ULONG FixtureOrdinal,
	__in ULONG TestCaseOrdinal,
	__in ICfixActionFactory *ActionFactory
	)
{
	if ( this->Name != NULL || 
		 this->FixtureOrdinal != 0 || 
		 this->Ordinal != 0 || 
		 this->ActionFactory != NULL )
	{
		return E_UNEXPECTED;
	}

	if ( Name == NULL || ActionFactory == NULL )
	{
		return E_POINTER;
	}

	this->Name = SysAllocString( Name );
	if ( this->Name == NULL )
	{
		return E_OUTOFMEMORY;
	}

	this->FixtureOrdinal = FixtureOrdinal;
	this->Ordinal = TestCaseOrdinal;

	ActionFactory->AddRef();
	this->ActionFactory = ActionFactory;

	return S_OK;
}

BSTR TestCase::GetNameInternal()
{
	return this->Name;
}