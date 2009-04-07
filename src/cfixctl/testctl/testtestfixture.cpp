/*----------------------------------------------------------------------
 * Purpose:
 *		Test TestFixture class.
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
		CFIXCC_ASSERT_OK( Exports.GetClassObject( 
			CLSID_TestFixture, IID_IClassFactory, ( PVOID* ) &this->FixtureFactory ) );
		CFIXCC_ASSERT( this->FixtureFactory );

		CFIXCC_ASSERT_OK( CoRegisterClassObject(
			CLSID_TestFixture,
			this->FixtureFactory,
			CLSCTX_INPROC_SERVER,
			REGCLS_MULTIPLEUSE,
			&this->TestFixtureRegistrationCookie ) );


		CFIXCC_ASSERT_OK( Exports.GetClassObject( 
			CLSID_TestCase, IID_IClassFactory, ( PVOID* ) &this->CaseFactory ) );
		CFIXCC_ASSERT( this->FixtureFactory );
		
		CFIXCC_ASSERT_OK( CoRegisterClassObject(
			CLSID_TestCase,
			this->CaseFactory,
			CLSCTX_INPROC_SERVER,
			REGCLS_MULTIPLEUSE,
			&this->TestCaseRegistrationCookie ) );
	}

	virtual void After()
	{
		CFIXCC_ASSERT_OK( CoRevokeClassObject( this->TestCaseRegistrationCookie ) );
		CFIXCC_ASSERT_OK( CoRevokeClassObject( this->TestFixtureRegistrationCookie ) );

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
		CFIXCC_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_IUnknown, ( PVOID* ) &Item ) );

		TestComUnknown( Item, IID_IUnknown, IID_ICfixTestItem );

		CFIXCC_ASSERT_EQUALS( 0UL, Item->Release() );
	}

	void TestMarshal( __in PCFIX_FIXTURE Fixture )
	{
		ICfixTestFixtureInternal *InitItem;
		CFIXCC_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_ICfixTestFixtureInternal, ( PVOID* ) &InitItem ) );

		//
		// Initialize.
		//
		StubActionFactory Factory;
		CFIXCC_ASSERT_OK( InitItem->Initialize( Fixture, 0, &Factory ) );
		CFIXCC_ASSERT_EQUALS( 
			E_UNEXPECTED, InitItem->Initialize( Fixture, 0, &Factory ) );

		//
		// Marshal.
		//
		IStream *Stm;
		CFIXCC_ASSERT_OK( CoMarshalInterThreadInterfaceInStream( 
			IID_ICfixTestFixtureInternal, 
			InitItem, &Stm ) );

		CFIXCC_ASSERT_EQUALS( 0UL, InitItem->Release() );

		//
		// Unmarshal.
		//
		ICfixTestItem *Item2;
		CFIXCC_ASSERT_OK( CoGetInterfaceAndReleaseStream(
			Stm,
			IID_ICfixTestItem,
			( PVOID* ) &Item2 ) );

		BSTR Name2;
		CFIXCC_ASSERT_OK( Item2->GetName( &Name2 ) );
		CFIXCC_ASSERT_EQUALS( Fixture->Name, Name2 );
		SysFreeString( Name2 );

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
			NULL,
			1,
			{ L"test", NULL, NULL } };

		ICfixTestFixtureInternal *Fixture;
		CFIXCC_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_ICfixTestFixtureInternal, ( PVOID* ) &Fixture ) );

		StubActionFactory Factory;
		CFIXCC_ASSERT_OK( Fixture->Initialize( &FixtureDefinition, 0, &Factory ) );

		ICfixTestContainer *Container;
		CFIXCC_ASSERT_OK( Fixture->QueryInterface( 
			IID_ICfixTestContainer, ( PVOID* ) &Container ) );

		ULONG Count;
		CFIXCC_ASSERT_OK( Container->GetItemCount( &Count ) );
		CFIXCC_ASSERT_EQUALS( 1UL, Count );

		ICfixTestItem *Item;
		CFIXCC_ASSERT_OK( Container->GetItem( 0, &Item ) );
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
			NULL,
			1,
			{ L"test", NULL, NULL } };

		ICfixTestFixtureInternal *Fixture;
		CFIXCC_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_ICfixTestFixtureInternal, ( PVOID* ) &Fixture ) );

		StubActionFactory Factory;
		CFIXCC_ASSERT_OK( Fixture->Initialize( &FixtureDefinition, 0, &Factory ) );

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
			NULL,
			0 };

		ICfixTestFixtureInternal *Fixture;
		CFIXCC_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_ICfixTestFixtureInternal, ( PVOID* ) &Fixture ) );

		StubActionFactory Factory;
		CFIXCC_ASSERT_OK( Fixture->Initialize( &FixtureDefinition, 0, &Factory ) );
		
		ICfixTestContainer *Container;
		CFIXCC_ASSERT_OK( Fixture->QueryInterface( 
			IID_ICfixTestContainer, ( PVOID* ) &Container ) );

		IEnumUnknown *Enum;
		CFIXCC_ASSERT_OK( Container->EnumItems( 0, &Enum ) );

		Fixture->Release();
		CFIXCC_ASSERT_EQUALS( 1UL, Container->Release() );	// N.B. Parent-link.

		CFIXCC_ASSERT_OK( Enum->Skip( 0 ) );
		CFIXCC_ASSERT_EQUALS( S_FALSE, Enum->Skip( 1 ) );

		IUnknown *Unk;
		ULONG Fetched;
		CFIXCC_ASSERT_EQUALS( S_FALSE, Enum->Next( 1, &Unk, &Fetched ) );
		CFIXCC_ASSERT_EQUALS( 0UL, Fetched );
		CFIXCC_ASSERT_OK( Enum->Reset() );

		IEnumUnknown *EnumClone;
		CFIXCC_ASSERT_OK( Enum->Clone( &EnumClone ) );
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
				NULL,
				3,
				{ L"test1", NULL, NULL } 
			},
			{ 
				{ L"test2", NULL, NULL },
				{ L"test3", NULL, NULL }
			} };

		ICfixTestFixtureInternal *Fixture;
		CFIXCC_ASSERT_OK( this->FixtureFactory->CreateInstance( 
			NULL, IID_ICfixTestFixtureInternal, ( PVOID* ) &Fixture ) );

		StubActionFactory Factory;
		CFIXCC_ASSERT_OK( Fixture->Initialize( &FixtureDefinition.Fixture, 0, &Factory ) );
		
		ICfixTestContainer *Container;
		CFIXCC_ASSERT_OK( Fixture->QueryInterface( 
			IID_ICfixTestContainer, ( PVOID* ) &Container ) );

		IEnumUnknown *Enum;
		CFIXCC_ASSERT_OK( Container->EnumItems( 0, &Enum ) );

		Fixture->Release();
		CFIXCC_ASSERT_EQUALS( 1UL, Container->Release() );	// N.B. Parent-link.

		//
		// Fetch first two.
		//
		IUnknown* Unks[ 4 ];
		ULONG Fetched;
		CFIXCC_ASSERT_OK( Enum->Next( 2, Unks, &Fetched ) );
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
		CFIXCC_ASSERT_OK( Enum->Reset() );
		CFIXCC_ASSERT_OK( Enum->Next( 3, Unks, &Fetched ) );
		CFIXCC_ASSERT_EQUALS( 3UL, Fetched );

		Unks[ 0 ]->Release();
		Unks[ 1 ]->Release();
		Unks[ 2 ]->Release();

		CFIXCC_ASSERT_EQUALS( S_FALSE, Enum->Next( 1, Unks, &Fetched ) );
		CFIXCC_ASSERT_EQUALS( 0UL, Fetched );
		
		CFIXCC_ASSERT_OK( Enum->Next( 0, Unks, &Fetched ) );
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