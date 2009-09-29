/*----------------------------------------------------------------------
 * Purpose:
 *		TestFixture class.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include "cfixctlp.h"

const GUID IID_ICfixTestFixtureInternal = 
	{ 0xfdac652, 0x27d6, 0x4283, { 0x8b, 0x53, 0x97, 0x5a, 0x99, 0x28, 0xb, 0x97 } };

C_ASSERT( CfixTestApiTypeBase == CfixApiTypeBase );
C_ASSERT( CfixTestApiTypeCc == CfixApiTypeCc );
C_ASSERT( CfixTestApiTypeWinUnitFixture == CfixApiTypeWinUnitFixture );
C_ASSERT( CfixTestApiTypeWinUnitStandalone == CfixApiTypeWinUnitStandalone );

/*------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 * N.B. This class marshals by value.
 *
 */

class TestFixture : 
	public ComObjectBase, 
	public ICfixTestFixtureInternal,
	public ICfixTestFixture,
	public IMarshal
{
	DECLARE_NOT_COPYABLE( TestFixture );

private:
	//
	// N.B. Members are effectively immutable.
	//
	// Children can be expected to usually be small, we thus use
	// sequential search rather than a map.
	//
	BSTR Name;
	ComArray< ICfixTestCaseInternal > Children;
	ULONG Ordinal;
	CfixTestApiType ApiType;
	ICfixActionFactory *ActionFactory;

protected:
	TestFixture();

public:
	virtual ~TestFixture();

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
	 * ICfixTestContainer methods.
	 */
	
	STDMETHOD( GetItemCount )(
		__out ULONG *Count
		);

	STDMETHOD( GetItem )(
		__in ULONG Ordinal,
		__out ICfixTestItem **Item
		);

	STDMETHOD( EnumItems )( 
		__in DWORD Flags,
		__out IEnumUnknown **Enum
		);

	/*------------------------------------------------------------------
	 * ICfixTestItem methods.
	 */

	STDMETHOD( GetName )(
        __out BSTR *Name
		);

	STDMETHOD( CreateExecutionAction )(
		__in ULONG SchedulingFlags,
		__in ULONG Reserved,
		__out ICfixAction **Action 
		);

	/*------------------------------------------------------------------
	 * ICfixTestFixture methods.
	 */

	STDMETHOD( GetApiType )(
        __out CfixTestApiType *Type
		);

	/*------------------------------------------------------------------
	 * ICfixTestFixtureInternal methods.
	 */

	STDMETHOD( Initialize )(
		__in PCFIX_FIXTURE Fixture,
		__in ULONG FixtureOrdinal,
		__in ICfixActionFactory *ActionFactory
		);

	STDMETHOD_( BSTR, GetNameInternal )();
};

/*------------------------------------------------------------------
 * 
 * Factory.
 *
 */
IClassFactory& CfixctlpGetTestFixtureFactory()
{
	static ComClassFactory< ComMtaObject< TestFixture >, CfixctlServerLock > Factory;
	return Factory;
}

/*------------------------------------------------------------------
 * 
 * Class Implementation.
 *
 */
TestFixture::TestFixture() 
	: Name( NULL )
	, Ordinal( 0 )
	, ApiType( CfixTestApiUnknown )
	, ActionFactory( NULL )
{
}

TestFixture::~TestFixture()
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

STDMETHODIMP TestFixture::QueryInterface( 
	__in REFIID Iid, 
	__out PVOID* Ptr )
{
	HRESULT Hr;

	if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
	     InlineIsEqualGUID( Iid, IID_ICfixTestItem ) ||
	     InlineIsEqualGUID( Iid, IID_ICfixTestContainer ) ||
		 InlineIsEqualGUID( Iid, IID_ICfixTestFixture ) )
	{
		*Ptr = static_cast< ICfixTestFixture* >( this );
		Hr = S_OK;

	}
	else if ( InlineIsEqualGUID( Iid, IID_IMarshal ) )
	{
		*Ptr = static_cast< IMarshal* >( this );
		Hr = S_OK;
	}
	else if ( InlineIsEqualGUID( Iid, IID_ICfixTestFixtureInternal ) )
	{
		*Ptr = static_cast< ICfixTestFixtureInternal* >( this );
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
 * IMarshal methods.
 */

STDMETHODIMP TestFixture::GetUnmarshalClass( 
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
	*Clsid = CLSID_TestFixture;
	return S_OK;
}

STDMETHODIMP TestFixture::GetMarshalSizeMax( 
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
	
	ULONG SizeNeeded;
	
	SizeNeeded = ComGetUnmarshalSizeMaxBSTR( this->Name );
	SizeNeeded += sizeof( ULONG );	// Count.
	SizeNeeded += sizeof( ULONG );	// Ordinal.
	SizeNeeded += sizeof( ULONG );	// ApiType.

	ULONG ItfSize;
	for ( ULONG Index = 0; Index < this->Children.GetCount(); Index++ )
	{
		HRESULT Hr = CoGetMarshalSizeMax(
			&ItfSize,
			IID_ICfixTestCaseInternal,
			this->Children.Get( Index ),
			DestContext,
			Reserved,
			Mshlflags );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		SizeNeeded += ItfSize;
	}

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
	SizeNeeded += ItfSize;

	*Size = SizeNeeded;
	return S_OK;
}

STDMETHODIMP TestFixture::MarshalInterface( 
	__in IStream *Stm,
	__in REFIID Iid,
	__in PVOID Itf,
	__in DWORD DestContext,
	__in PVOID Reserved,
	__in DWORD Mshlflags
	)
{
	UNREFERENCED_PARAMETER( Iid );
	UNREFERENCED_PARAMETER( Itf );
	
	ASSERT( this->Name != NULL );

	HRESULT Hr = ComMarshalBSTR( Stm, this->Name );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	ULONG Count = this->Children.GetCount();
	Hr = Stm->Write( &Count, sizeof( ULONG ), NULL );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = Stm->Write( &this->Ordinal, sizeof( ULONG ), NULL );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = Stm->Write( &this->ApiType, sizeof( ULONG ), NULL );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	for ( ULONG Index = 0; Index < this->Children.GetCount(); Index++ )
	{
		Hr = CoMarshalInterface(
			Stm,
			IID_ICfixTestCaseInternal,
			this->Children.Get( Index ),
			DestContext,
			Reserved,
			Mshlflags );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}
	}

	return CoMarshalInterface(
		Stm,
		IID_ICfixActionFactory,
		this->ActionFactory,
		DestContext,
		Reserved,
		Mshlflags );
}

STDMETHODIMP TestFixture::UnmarshalInterface( 
	__in IStream *Stm,
	__in REFIID Iid,
	__out PVOID *Object
	)
{
	ASSERT( this->Name == NULL );
	ASSERT( this->Children.GetCount() == 0 );
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

	ULONG Count;
	Hr = Stm->Read( &Count, sizeof( ULONG ), NULL );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = Stm->Read( &this->Ordinal, sizeof( ULONG ), NULL );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = Stm->Read( &this->ApiType, sizeof( ULONG ), NULL );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = this->Children.Initialize( Count );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	for ( ULONG Index = 0; Index < this->Children.GetCount(); Index++ )
	{
		ICfixTestCaseInternal *Item;
		Hr = CoUnmarshalInterface(
			Stm,
			IID_ICfixTestCaseInternal,
			( PVOID* ) &Item );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		this->Children.Set( Index, Item );
		Item->Release();
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

STDMETHODIMP TestFixture::ReleaseMarshalData( 
	__in IStream *Stm
	)
{
	UNREFERENCED_PARAMETER( Stm );
	return S_OK;
}

STDMETHODIMP TestFixture::DisconnectObject( 
	__in DWORD Reserved
	)
{
	UNREFERENCED_PARAMETER( Reserved );
	return S_OK;
}

/*------------------------------------------------------------------
 * ICfixTestContainer methods.
 */

STDMETHODIMP TestFixture::GetItemCount(
	__out ULONG *Count
	)
{
	if ( ! Count )
	{
		return E_POINTER;
	}
	else
	{
		*Count = this->Children.GetCount();
		return S_OK;
	}
}

STDMETHODIMP TestFixture::GetItem(
	__in ULONG Ordinal,
	__out ICfixTestItem **Item
	)
{
	if ( ! Item )
	{
		return E_POINTER;
	}
	else
	{
		*Item = NULL;
	}

	if ( Ordinal >= this->Children.GetCount() )
	{
		return E_INVALIDARG;
	}

	*Item = this->Children.Get( Ordinal );
	( *Item )->AddRef();
	return S_OK;
}

STDMETHODIMP TestFixture::EnumItems( 
	__in DWORD Flags,
	__out IEnumUnknown **Enum
	)
{
	UNREFERENCED_PARAMETER( Flags );

	*Enum = new ComArrayEnumerator< ICfixTestCaseInternal >( 
		this->Children, 
		static_cast< ICfixTestFixtureInternal* >( this ) );

	if ( *Enum == NULL )
	{
		return E_OUTOFMEMORY;
	}
	else
	{
		return S_OK;
	}
}

/*------------------------------------------------------------------
 * ICfixTestItem methods.
 */

STDMETHODIMP TestFixture::GetName(
    __out BSTR *Name
	)
{
	return ComGetProperty( this->Name, Name );
}

STDMETHODIMP TestFixture::CreateExecutionAction(
	__in ULONG SchedulingFlags,
	__in ULONG Reserved,
	__out ICfixAction **Action 
	)
{
	return this->ActionFactory->CreateExecutionAction(
		this->Ordinal,
		CFIXCTL_EXECUTE_ALL,
		SchedulingFlags,
		Reserved,
		Action );
}

/*------------------------------------------------------------------
 * ICfixTestItem methods.
 */

STDMETHODIMP TestFixture::GetApiType(
    __out CfixTestApiType *Type
	)
{
	*Type = this->ApiType;
	return S_OK;
}

/*------------------------------------------------------------------
 * ICfixTestFixtureInternal methods.
 */

STDMETHODIMP TestFixture::Initialize(
	__in PCFIX_FIXTURE Fixture,
	__in ULONG FixtureOrdinal,
	__in ICfixActionFactory *ActionFactory
	)
{
	if ( this->Name != NULL || 
		 this->Ordinal != 0 ||
		 this->Children.GetCount() != 0  || 
		 this->ActionFactory != NULL )
	{
		return E_UNEXPECTED;
	}

	HRESULT Hr = E_UNEXPECTED;

	this->Name = SysAllocString( Fixture->Name );
	if ( this->Name == NULL )
	{
		Hr = E_OUTOFMEMORY;
		goto Cleanup;
	}

	if ( Fixture == NULL || ActionFactory == NULL )
	{
		return E_POINTER;
	}

	this->Ordinal = FixtureOrdinal;
	this->ApiType = ( CfixTestApiType ) Fixture->ApiType;

	ActionFactory->AddRef();
	this->ActionFactory = ActionFactory;

	//
	// Create TestCase objects.
	//
	Hr = this->Children.Initialize( Fixture->TestCaseCount );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	IClassFactory& Factory = CfixctlpGetTestCaseFactory();
	for ( ULONG Index = 0; Index < Fixture->TestCaseCount; Index++ )
	{
		ICfixTestCaseInternal *Init;
		Hr = Factory.CreateInstance(
			NULL,
			IID_ICfixTestCaseInternal,
			( PVOID* ) &Init );
		if ( FAILED( Hr ) )
		{
			goto Cleanup;
		}

		Hr = Init->Initialize( 
			Fixture->TestCases[ Index ].Name, 
			FixtureOrdinal,
			Index,
			ActionFactory );
		if ( SUCCEEDED( Hr ) )
		{
			this->Children.Set( Index, Init );
		}

		Init->Release();

		if ( FAILED( Hr ) )
		{
			goto Cleanup;
		}
	}

	Hr = S_OK;
	
Cleanup:
	if ( FAILED( Hr ) )
	{
		if ( this->Name != NULL )
		{
			SysFreeString( this->Name );
		}

		this->Children.Clear();
	}

	return Hr;
}

BSTR TestFixture::GetNameInternal()
{
	return this->Name;
}