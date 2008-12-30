/*----------------------------------------------------------------------
 * Purpose:
 *		Test TestCase class.
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
		CFIXCC_ASSERT_OK( Exports.GetClassObject( 
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
		CFIXCC_ASSERT_OK( this->Factory->CreateInstance( 
			NULL, IID_IUnknown, ( PVOID* ) &Item ) );

		TestComUnknown( Item, IID_IUnknown, IID_ICfixTestItem );

		CFIXCC_ASSERT_EQUALS( 0UL, Item->Release() );
	}

	void TestMarshal( __in PCWSTR Name )
	{
		ICfixTestCaseInternal *InitItem;
		CFIXCC_ASSERT_OK( this->Factory->CreateInstance( 
			NULL, IID_ICfixTestCaseInternal, ( PVOID* ) &InitItem ) );

		//
		// Initialize.
		//
		StubActionFactory MockFactory;
		CFIXCC_ASSERT_OK( InitItem->Initialize( Name, 0, 0, &MockFactory ) );
		CFIXCC_ASSERT_EQUALS( 
			E_UNEXPECTED, InitItem->Initialize( Name, 0, 0, &MockFactory ) );

		//
		// Marshal.
		//
		IMarshal *Mrsh;
		CFIXCC_ASSERT_OK( InitItem->QueryInterface( 
			IID_IMarshal, ( PVOID* ) &Mrsh ) );

		InitItem->Release();

		IStream *Stm;
		CFIXCC_ASSERT_OK( CreateStreamOnHGlobal( NULL, TRUE, &Stm ) );

		//
		// Note: Requires TLB to be registered.
		//
		CFIXCC_ASSERT_OK( Mrsh->MarshalInterface(
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
		CFIXCC_ASSERT_OK( Stm->Seek( Zero, STREAM_SEEK_SET, NULL ) );

		//
		// Unmarshal.
		//
		IMarshal *ItemUnmrsh;
		CFIXCC_ASSERT_OK( this->Factory->CreateInstance( 
			NULL, IID_IMarshal, ( PVOID* ) &ItemUnmrsh ) );

		ICfixTestItem *Item2;
		CFIXCC_ASSERT_OK( ItemUnmrsh->UnmarshalInterface(
			Stm,
			IID_ICfixTestItem,
			( PVOID* ) &Item2 ) );

		ItemUnmrsh->Release();
		Stm->Release();
		
		BSTR Name2;
		CFIXCC_ASSERT_OK( Item2->GetName( &Name2 ) );
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