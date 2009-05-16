/*----------------------------------------------------------------------
 * Purpose:
 *		Test TestCase class.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <testctlp.h>

const GUID IID_ICfixTestCaseInternal = 
	{ 0x221f2d4b, 0xdf5, 0x4755, { 0x9c, 0xf4, 0xcd, 0x6b, 0x56, 0x6b, 0x8, 0x74 } };

class TestCase : public cfixcc::TestFixture
{
private:
	static COM_EXPORTS Exports;
	IClassFactory *Factory;

public:
	TestCase() : Factory( NULL )
	{
	}

	static void SetUp()
	{
		CoInitialize( NULL );
		GetComExports( L"cfixctl.dll", &Exports );
	}

	static void TearDown()
	{
		CoUninitialize();
	}

	virtual void Before()
	{
		CFIX_ASSERT_OK( Exports.GetClassObject( 
			CLSID_TestCase, IID_IClassFactory, ( PVOID* ) &this->Factory ) );
		CFIXCC_ASSERT( this->Factory );
	}

	virtual void After()
	{
		if ( this->Factory )
		{
			this->Factory->Release();
		}
	}

	void TestClassFactory()
	{
		TestComClassFactory( this->Factory, IID_ICfixTestItem );
	}

	void TestUnknown()
	{
		IUnknown *Item;
		CFIX_ASSERT_OK( this->Factory->CreateInstance( 
			NULL, IID_IUnknown, ( PVOID* ) &Item ) );

		TestComUnknown( Item, IID_IUnknown, IID_ICfixTestItem );

		CFIXCC_ASSERT_EQUALS( 0UL, Item->Release() );
	}

	void TestMarshal( __in PCWSTR Name )
	{
		ICfixTestCaseInternal *InitItem;
		CFIX_ASSERT_OK( this->Factory->CreateInstance( 
			NULL, IID_ICfixTestCaseInternal, ( PVOID* ) &InitItem ) );

		//
		// Initialize.
		//
		StubActionFactory MockFactory;
		CFIX_ASSERT_OK( InitItem->Initialize( Name, 0, 0, &MockFactory ) );
		CFIXCC_ASSERT_EQUALS( 
			E_UNEXPECTED, InitItem->Initialize( Name, 0, 0, &MockFactory ) );

		//
		// Marshal.
		//
		IMarshal *Mrsh;
		CFIX_ASSERT_OK( InitItem->QueryInterface( 
			IID_IMarshal, ( PVOID* ) &Mrsh ) );

		InitItem->Release();

		IStream *Stm;
		CFIX_ASSERT_OK( CreateStreamOnHGlobal( NULL, TRUE, &Stm ) );

		//
		// Note: Requires TLB to be registered.
		//
		CFIX_ASSERT_OK( Mrsh->MarshalInterface(
			Stm,
			IID_ICfixTestCaseInternal,
			InitItem,
			0,
			NULL,
			0 ) );

		CFIXCC_ASSERT_EQUALS( 0UL, Mrsh->Release() );

		//
		// Reposition stream.
		//
		LARGE_INTEGER Zero;
		Zero.QuadPart = 0;
		CFIX_ASSERT_OK( Stm->Seek( Zero, STREAM_SEEK_SET, NULL ) );

		//
		// Unmarshal.
		//
		IMarshal *ItemUnmrsh;
		CFIX_ASSERT_OK( this->Factory->CreateInstance( 
			NULL, IID_IMarshal, ( PVOID* ) &ItemUnmrsh ) );

		ICfixTestItem *Item2;
		CFIX_ASSERT_OK( ItemUnmrsh->UnmarshalInterface(
			Stm,
			IID_ICfixTestItem,
			( PVOID* ) &Item2 ) );

		ItemUnmrsh->Release();
		Stm->Release();
		
		BSTR Name2;
		CFIX_ASSERT_OK( Item2->GetName( &Name2 ) );
		CFIXCC_ASSERT_EQUALS( Name, Name2 );
		SysFreeString( Name2 );

		CFIXCC_ASSERT_EQUALS( 0UL, Item2->Release() );
	}

	void TestMarshalWithEmptyName()
	{
		TestMarshal( L"" );
	}

	void TestMarshalWithName()
	{
		TestMarshal( L"test" );
	}
};

COM_EXPORTS TestCase::Exports;

CFIXCC_BEGIN_CLASS( TestCase )
	CFIXCC_METHOD( TestClassFactory )
	CFIXCC_METHOD( TestUnknown )
	CFIXCC_METHOD( TestMarshalWithEmptyName )
	CFIXCC_METHOD( TestMarshalWithName )
CFIXCC_END_CLASS()