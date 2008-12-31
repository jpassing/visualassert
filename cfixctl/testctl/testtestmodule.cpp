/*----------------------------------------------------------------------
 * Purpose:
 *		Test TestModule class.
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

const GUID IID_ICfixTestModuleInternal = 
	{ 0x2e8f211, 0xe29b, 0x40e0, { 0xb2, 0xba, 0x39, 0xba, 0x4c, 0x56, 0xb8, 0x39 } };

static TEST_CFIX_FIXTURE_WITH_ARRAY SampleFixtureDefinition = { 
{
	L"Sample",
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

static PCFIX_FIXTURE SampleModuleFixtures[] = 
{ 
	&SampleFixtureDefinition.Fixture, 
	&SampleFixtureDefinition.Fixture, 
	&SampleFixtureDefinition.Fixture 
};

static void SampleAdjustReferencesRoutine( __in PCFIX_TEST_MODULE TestModule )
{
	UNREFERENCED_PARAMETER( TestModule );
}

static CFIX_TEST_MODULE SampleModule = {
	CFIX_TEST_MODULE_VERSION,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	SampleAdjustReferencesRoutine,
	SampleAdjustReferencesRoutine,
	NULL,
	L"Module",
	3,
	SampleModuleFixtures
};

class TestModule : public cfixcc::TestFixture
{
private:
	static COM_EXPORTS Exports;
	IClassFactory *ModuleFactory;
	DWORD TestModuleRegistrationCookie;

public:
	TestModule() : ModuleFactory( NULL )
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
			CLSID_TestModule, IID_IClassFactory, ( PVOID* ) &this->ModuleFactory ) );
		CFIXCC_ASSERT( this->ModuleFactory );

		CFIXCC_ASSERT_OK( CoRegisterClassObject(
			CLSID_TestModule,
			this->ModuleFactory,
			CLSCTX_INPROC_SERVER,
			REGCLS_MULTIPLEUSE,
			&this->TestModuleRegistrationCookie ) );
	}

	virtual void After()
	{
		CFIXCC_ASSERT_OK( CoRevokeClassObject( this->TestModuleRegistrationCookie ) );

		if ( this->ModuleFactory )
		{
			this->ModuleFactory->Release();
		}
	}

	void TestClassFactory()
	{
		TestComClassFactory( this->ModuleFactory, IID_ICfixTestModule );
	}

	void TestUnknown()
	{
		IUnknown *Item;
		CFIXCC_ASSERT_OK( this->ModuleFactory->CreateInstance( 
			NULL, IID_IUnknown, ( PVOID* ) &Item ) );

		TestComUnknown( Item, IID_ICfixTestModule, IID_ICfixTestItem );

		CFIXCC_ASSERT_EQUALS( 0UL, Item->Release() );
	}

	void TestBasics()
	{
		ICfixTestModuleInternal *Module;
		CFIXCC_ASSERT_OK( this->ModuleFactory->CreateInstance( 
			NULL, IID_ICfixTestModuleInternal, ( PVOID* ) &Module ) );

		CFIXCC_ASSERT_OK( Module->Initialize( 
			L"path", 
			CfixTestModuleTypeUser, 
			CfixTestModuleArchAmd64,
			&SampleModule ) );

		BSTR Path;
		CFIXCC_ASSERT_OK( Module->GetPath( &Path ) );
		CFIXCC_ASSERT_EQUALS( L"path", ( PCWSTR ) Path );
		SysFreeString( Path );

		BSTR Name;
		CFIXCC_ASSERT_OK( Module->GetName( &Name ) );
		CFIXCC_ASSERT_EQUALS( SampleModule.Name, Name );
		SysFreeString( Name );

		CfixTestModuleType Type;
		CfixTestModuleArch Arch;
		
		CFIXCC_ASSERT_OK( Module->GetType( &Type, &Arch ) );
		CFIXCC_ASSERT_EQUALS( CfixTestModuleTypeUser, Type );
		CFIXCC_ASSERT_EQUALS( CfixTestModuleArchAmd64, Arch );

		CFIXCC_ASSERT_EQUALS( 0UL, Module->Release() );
	}

	void TestGetItem()
	{
		ICfixTestModuleInternal *Module;
		CFIXCC_ASSERT_OK( this->ModuleFactory->CreateInstance( 
			NULL, IID_ICfixTestModuleInternal, ( PVOID* ) &Module ) );

		CFIXCC_ASSERT_OK( Module->Initialize( 
			L"path", 
			CfixTestModuleTypeUser, 
			CfixTestModuleArchAmd64,
			&SampleModule ) );

		ICfixTestContainer *Container;
		CFIXCC_ASSERT_OK( Module->QueryInterface( 
			IID_ICfixTestContainer, ( PVOID* ) &Container ) );

		ULONG Count;
		CFIXCC_ASSERT_OK( Container->GetItemCount( &Count ) );
		CFIXCC_ASSERT_EQUALS( 3UL, Count );

		ICfixTestItem *Item;
		CFIXCC_ASSERT_OK( Container->GetItem( 0, &Item ) );
		Item->Release();

		CFIXCC_ASSERT_OK( Container->GetItem( 2, &Item ) );
		Item->Release();

		CFIXCC_ASSERT_EQUALS( E_INVALIDARG, Container->GetItem( 3, &Item ) );
		

		Module->Release();
		Container->Release();
	}

	void TestGetObject()
	{
		ICfixTestModuleInternal *Module;
		CFIXCC_ASSERT_OK( this->ModuleFactory->CreateInstance( 
			NULL, IID_ICfixTestModuleInternal, ( PVOID* ) &Module ) );

		CFIXCC_ASSERT_OK( Module->Initialize( 
			L"path", 
			CfixTestModuleTypeUser, 
			CfixTestModuleArchAmd64,
			&SampleModule ) );

		IOleItemContainer *Container;
		CFIXCC_ASSERT_OK( Module->QueryInterface( 
			IID_IOleItemContainer, ( PVOID* ) &Container ) );

		WCHAR Name[] = L"Sample";
		WCHAR WrongName[] = L"Xxx";
		WCHAR EmptyName[] = L"";
		
		CFIXCC_ASSERT_OK( Container->IsRunning( Name ) );
		CFIXCC_ASSERT_EQUALS( MK_E_NOOBJECT, Container->IsRunning( WrongName ) );
		CFIXCC_ASSERT_EQUALS( MK_E_NOOBJECT, Container->IsRunning( EmptyName ) );
		CFIXCC_ASSERT_EQUALS( MK_E_NOOBJECT, Container->IsRunning( NULL ) );

		ICfixTestItem *Item;
		CFIXCC_ASSERT_OK( Container->GetObject( 
			Name, 0, NULL, IID_ICfixTestItem, ( PVOID* ) &Item ) );
		BSTR FixtureName;
		CFIXCC_ASSERT_OK( Item->GetName( &FixtureName ) );
		CFIXCC_ASSERT_EQUALS( Name, FixtureName );
		SysFreeString( FixtureName );
		Item->Release();
		
		CFIXCC_ASSERT_EQUALS( MK_E_NOOBJECT, Container->GetObject( 
			WrongName, 0, NULL, IID_ICfixTestItem, ( PVOID* ) &Item ) );
		CFIXCC_ASSERT_EQUALS( MK_E_NOOBJECT, Container->GetObject( 
			EmptyName, 0, NULL, IID_ICfixTestItem, ( PVOID* ) &Item ) );

		Module->Release();
		Container->Release();
	}

	void TestEnumObjects()
	{
		ICfixTestModuleInternal *Module;
		CFIXCC_ASSERT_OK( this->ModuleFactory->CreateInstance( 
			NULL, IID_ICfixTestModuleInternal, ( PVOID* ) &Module ) );

		CFIXCC_ASSERT_OK( Module->Initialize( 
			L"path", 
			CfixTestModuleTypeUser, 
			CfixTestModuleArchAmd64,
			&SampleModule ) );

		IOleItemContainer *Container;
		CFIXCC_ASSERT_OK( Module->QueryInterface( 
			IID_IOleItemContainer, ( PVOID* ) &Container ) );

		IEnumUnknown *Enum;
		CFIXCC_ASSERT_OK( Container->EnumObjects( 0, &Enum ) );

		Module->Release();
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

COM_EXPORTS TestModule::Exports;

CFIXCC_BEGIN_CLASS( TestModule )
	CFIXCC_METHOD( TestClassFactory )
	CFIXCC_METHOD( TestUnknown )
	CFIXCC_METHOD( TestBasics )
	CFIXCC_METHOD( TestGetItem )
	CFIXCC_METHOD( TestGetObject )
	CFIXCC_METHOD( TestEnumObjects )	
CFIXCC_END_CLASS()