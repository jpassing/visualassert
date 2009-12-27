/*----------------------------------------------------------------------
 * Purpose:
 *		Test TestFixture class.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <testctlp.h>

const GUID IID_ICfixTestFixtureInternal = 
	{ 0xfdac652, 0x27d6, 0x4283, { 0x8b, 0x53, 0x97, 0x5a, 0x99, 0x28, 0xb, 0x97 } };

class TestFixture : public cfixcc::TestFixture
{
private:
	static COM_EXPORTS Exports;
	IClassFactory *FixtureFactory;
	IClassFactory *CaseFactory;
	DWORD TestCaseRegistrationCookie;
	DWORD TestFixtureRegistrationCookie;

public:
	TestFixture() : FixtureFactory( NULL )
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
			CLSID_TestFixture, IID_IClassFactory, ( PVOID* ) &this->FixtureFactory ) );
		CFIXCC_ASSERT( this->FixtureFactory );

		CFIX_ASSERT_OK( CoRegisterClassObject(
			CLSID_TestFixture,
			this->FixtureFactory,
			CLSCTX_INPROC_SERVER,
			REGCLS_MULTIPLEUSE,
			&this->TestFixtureRegistrationCookie ) );


		CFIX_ASSERT_OK( Exports.GetClassObject( 
			CLSID_TestCase, IID_IClassFactory, ( PVOID* ) &this->CaseFactory ) );
		CFIXCC_ASSERT( this->FixtureFactory );
		
		CFIX_ASSERT_OK( CoRegisterClassObject(
			CLSID_TestCase,
			this->CaseFactory,
			CLSCTX_INPROC_SERVER,
			REGCLS_MULTIPLEUSE,
			&this->TestCaseRegistrationCookie ) );
	}

	virtual void After()
	{
		CFIX_ASSERT_OK( CoRevokeClassObject( this->TestCaseRegistrationCookie ) );
		CFIX_ASSERT_OK( CoRevokeClassObject( this->TestFixtureRegistrationCookie ) );

		if ( this->FixtureFactory )
		{
			this->FixtureFactory->Release();
		}

		if ( this->CaseFactory )
		{
			this->CaseFactory->Release();
		}
	}

	void TestClassFactory()
	{
		TestComClassFactory( this->FixtureFactory, IID_ICfixTestItem );
	}

	void TestUnknown()
	{
		IUnknown *Item;
		CFIX_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_IUnknown, ( PVOID* ) &Item ) );

		TestComUnknown( Item, IID_IUnknown, IID_ICfixTestItem );

		CFIXCC_ASSERT_EQUALS( 0UL, Item->Release() );
	}

	void TestMarshal( __in PCFIX_FIXTURE Fixture )
	{
		ICfixTestFixtureInternal *InitItem;
		CFIX_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_ICfixTestFixtureInternal, ( PVOID* ) &InitItem ) );

		//
		// Initialize.
		//
		StubActionFactory Factory;
		CFIX_ASSERT_OK( InitItem->Initialize( Fixture, 0, &Factory ) );
		CFIXCC_ASSERT_EQUALS( 
			E_UNEXPECTED, InitItem->Initialize( Fixture, 0, &Factory ) );

		//
		// Marshal.
		//
		IStream *Stm;
		CFIX_ASSERT_OK( CoMarshalInterThreadInterfaceInStream( 
			IID_ICfixTestFixtureInternal, 
			InitItem, &Stm ) );

		CFIXCC_ASSERT_EQUALS( 0UL, InitItem->Release() );

		//
		// Unmarshal.
		//
		ICfixTestFixture *Item2;
		CFIX_ASSERT_OK( CoGetInterfaceAndReleaseStream(
			Stm,
			IID_ICfixTestFixture,
			( PVOID* ) &Item2 ) );

		BSTR Name2;
		CFIX_ASSERT_OK( Item2->GetName( &Name2 ) );
		CFIXCC_ASSERT_EQUALS( Fixture->Name, Name2 );
		SysFreeString( Name2 );

		CfixTestApiType ApiType;
		CFIX_ASSERT_OK( Item2->GetApiType( &ApiType ) );
		CFIXCC_ASSERT_EQUALS( 
			( CfixTestApiType ) Fixture->ApiType, 
			ApiType );

		CFIXCC_ASSERT_EQUALS( 0UL, Item2->Release() );
	}

	void TestMarshalWithEmptyNameAndFixture()
	{
		CFIX_FIXTURE Fixture = {
			L"",
			NULL,
			NULL,
			NULL,
			NULL,
			NULL,
			CfixApiTypeCc,
			NULL,
			0 };

		TestMarshal( &Fixture );
	}

	void TestMarshalWithNameAndFixture()
	{
		CFIX_FIXTURE Fixture = {
			L"fix",
			NULL,
			NULL,
			NULL,
			NULL,
			NULL,
			CfixApiTypeWinUnitFixture,
			0,
			NULL,
			1,
			{ L"test", NULL, NULL } };
		TestMarshal( &Fixture );
	}

	void TestGetItem()
	{
		CFIX_FIXTURE FixtureDefinition = {
			L"fix",
			NULL,
			NULL,
			NULL,
			NULL,
			NULL,
			CfixApiTypeCc,
			0,
			NULL,
			1,
			{ L"test", NULL, NULL } };

		ICfixTestFixtureInternal *Fixture;
		CFIX_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_ICfixTestFixtureInternal, ( PVOID* ) &Fixture ) );

		StubActionFactory Factory;
		CFIX_ASSERT_OK( Fixture->Initialize( &FixtureDefinition, 0, &Factory ) );

		ICfixTestContainer *Container;
		CFIX_ASSERT_OK( Fixture->QueryInterface( 
			IID_ICfixTestContainer, ( PVOID* ) &Container ) );

		ULONG Count;
		CFIX_ASSERT_OK( Container->GetItemCount( &Count ) );
		CFIXCC_ASSERT_EQUALS( 1UL, Count );

		ICfixTestItem *Item;
		CFIX_ASSERT_OK( Container->GetItem( 0, &Item ) );
		Item->Release();

		CFIXCC_ASSERT_EQUALS( E_INVALIDARG, Container->GetItem( 1, &Item ) );
		
		Fixture->Release();
		Container->Release();
	}

	void TestGetObject()
	{
		CFIX_FIXTURE FixtureDefinition = {
			L"fix",
			NULL,
			NULL,
			NULL,
			NULL,
			NULL,
			CfixApiTypeBase,
			0,
			NULL,
			1,
			{ L"test", NULL, NULL } };

		ICfixTestFixtureInternal *Fixture;
		CFIX_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_ICfixTestFixtureInternal, ( PVOID* ) &Fixture ) );

		StubActionFactory Factory;
		CFIX_ASSERT_OK( Fixture->Initialize( &FixtureDefinition, 0, &Factory ) );

		Fixture->Release();
	}

	void TestEnumObjectsEmpty()
	{
		CFIX_FIXTURE FixtureDefinition = {
			L"",
			NULL,
			NULL,
			NULL,
			NULL,
			NULL,
			CfixApiTypeWinUnitFixture,
			NULL,
			0 };

		ICfixTestFixtureInternal *Fixture;
		CFIX_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_ICfixTestFixtureInternal, ( PVOID* ) &Fixture ) );

		StubActionFactory Factory;
		CFIX_ASSERT_OK( Fixture->Initialize( &FixtureDefinition, 0, &Factory ) );
		
		ICfixTestContainer *Container;
		CFIX_ASSERT_OK( Fixture->QueryInterface( 
			IID_ICfixTestContainer, ( PVOID* ) &Container ) );

		IEnumUnknown *Enum;
		CFIX_ASSERT_OK( Container->EnumItems( 0, &Enum ) );

		Fixture->Release();
		CFIXCC_ASSERT_EQUALS( 1UL, Container->Release() );	// N.B. Parent-link.

		CFIX_ASSERT_OK( Enum->Skip( 0 ) );
		CFIXCC_ASSERT_EQUALS( S_FALSE, Enum->Skip( 1 ) );

		IUnknown *Unk;
		ULONG Fetched;
		CFIXCC_ASSERT_EQUALS( S_FALSE, Enum->Next( 1, &Unk, &Fetched ) );
		CFIXCC_ASSERT_EQUALS( 0UL, Fetched );
		CFIX_ASSERT_OK( Enum->Reset() );

		IEnumUnknown *EnumClone;
		CFIX_ASSERT_OK( Enum->Clone( &EnumClone ) );
		Enum->Release();

		EnumClone->Release();
	}

	void TestEnumObjectsNonEmpty()
	{
		TEST_CFIX_FIXTURE_WITH_ARRAY FixtureDefinition = { 
			{
				L"",
				NULL,
				NULL,
				NULL,
				NULL,
				NULL,
				CfixApiTypeBase,
				0,
				NULL,
				3,
				{ L"test1", NULL, NULL } 
			},
			{ 
				{ L"test2", NULL, NULL },
				{ L"test3", NULL, NULL }
			} };

		ICfixTestFixtureInternal *Fixture;
		CFIX_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_ICfixTestFixtureInternal, ( PVOID* ) &Fixture ) );

		StubActionFactory Factory;
		CFIX_ASSERT_OK( Fixture->Initialize( &FixtureDefinition.Fixture, 0, &Factory ) );
		
		ICfixTestContainer *Container;
		CFIX_ASSERT_OK( Fixture->QueryInterface( 
			IID_ICfixTestContainer, ( PVOID* ) &Container ) );

		IEnumUnknown *Enum;
		CFIX_ASSERT_OK( Container->EnumItems( 0, &Enum ) );

		Fixture->Release();
		CFIXCC_ASSERT_EQUALS( 1UL, Container->Release() );	// N.B. Parent-link.

		//
		// Fetch first two.
		//
		IUnknown* Unks[ 4 ];
		ULONG Fetched;
		CFIX_ASSERT_OK( Enum->Next( 2, Unks, &Fetched ) );
		CFIXCC_ASSERT_EQUALS( 2UL, Fetched );

		Unks[ 0 ]->Release();
		Unks[ 1 ]->Release();

		//
		// Fetch remaining one.
		//
		CFIXCC_ASSERT_EQUALS( S_FALSE, Enum->Next( 2, Unks, &Fetched ) );
		CFIXCC_ASSERT_EQUALS( 1UL, Fetched );

		Unks[ 0 ]->Release();

		//
		// Fetch all.
		//
		CFIX_ASSERT_OK( Enum->Reset() );
		CFIX_ASSERT_OK( Enum->Next( 3, Unks, &Fetched ) );
		CFIXCC_ASSERT_EQUALS( 3UL, Fetched );

		Unks[ 0 ]->Release();
		Unks[ 1 ]->Release();
		Unks[ 2 ]->Release();

		CFIXCC_ASSERT_EQUALS( S_FALSE, Enum->Next( 1, Unks, &Fetched ) );
		CFIXCC_ASSERT_EQUALS( 0UL, Fetched );
		
		CFIX_ASSERT_OK( Enum->Next( 0, Unks, &Fetched ) );
		CFIXCC_ASSERT_EQUALS( 0UL, Fetched );
		
		Enum->Release();
	}
};

COM_EXPORTS TestFixture::Exports;

CFIXCC_BEGIN_CLASS( TestFixture )
	CFIXCC_METHOD( TestClassFactory )
	CFIXCC_METHOD( TestUnknown )
	CFIXCC_METHOD( TestMarshalWithEmptyNameAndFixture )
	CFIXCC_METHOD( TestMarshalWithNameAndFixture )	
	CFIXCC_METHOD( TestGetItem )	
	CFIXCC_METHOD( TestGetObject )	
	CFIXCC_METHOD( TestEnumObjectsEmpty )	
	CFIXCC_METHOD( TestEnumObjectsNonEmpty )	
CFIXCC_END_CLASS()