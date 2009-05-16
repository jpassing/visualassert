/*----------------------------------------------------------------------
 * Purpose:
 *		Test COM rules
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <testctlp.h>
#include <shlwapi.h>

static const GUID IID_INonExisting = 
{ 0x3e645fcd, 0xc550, 0x4027, { 0xad, 0xee, 0xe9, 0x5e, 0xce, 0xb9, 0x18, 0xe2 } };

static void TestUnknownQiIsReflexive(
	__in IUnknown *Unk,
	__in REFIID Iid
	)
{
	IUnknown *UnkCopy = NULL;
	CFIX_ASSERT_OK( Unk->QueryInterface( Iid, ( PVOID* ) &UnkCopy ) );
	CFIXCC_ASSERT_EQUALS( Unk, UnkCopy );
	UnkCopy->Release();
}

static void TestUnknownQiIsSymmetric(
	__in IUnknown *Unk,
	__in REFIID Iid1,
	__in REFIID Iid2
	)
{
	IUnknown *Unk1 = NULL;
	IUnknown *Unk2 = NULL;
	IUnknown *Unk1Copy = NULL;
	
	CFIX_ASSERT_OK( Unk->QueryInterface( Iid1, ( PVOID* ) &Unk1 ) );
	CFIXCC_ASSERT( Unk1 );
	__assume( Unk1 != NULL );
	
	CFIX_ASSERT_OK( Unk1->QueryInterface( Iid2, ( PVOID* ) &Unk2 ) );
	CFIXCC_ASSERT( Unk2 );
	__assume( Unk2 != NULL );
	
	CFIX_ASSERT_OK( Unk2->QueryInterface( Iid1, ( PVOID* ) &Unk1Copy ) );
	CFIXCC_ASSERT( Unk1Copy );
	__assume( Unk1Copy != NULL );

	CFIXCC_ASSERT_EQUALS( Unk1, Unk1Copy );

	Unk1->Release();
	Unk2->Release();
	Unk1Copy->Release();
}

static void TestUnknownQueryNonExistingInterface(
	__in IUnknown *Unk
	)
{
	IUnknown *Unk1 = NULL;
	CFIXCC_ASSERT_EQUALS( E_NOINTERFACE, Unk->QueryInterface( 
		IID_INonExisting, ( PVOID* ) &Unk1 ) );
	CFIXCC_ASSERT( ! Unk1 );
}

void TestComClassFactory(
	__in IClassFactory *Factory,
	__in REFIID Iid
	)
{
	//
	// CreateInstance.
	//
	IUnknown *Unk1 = NULL;
	CFIXCC_ASSERT_EQUALS( CLASS_E_NOAGGREGATION, 
		Factory->CreateInstance( Factory, Iid, ( PVOID* ) &Unk1 ) );
	CFIXCC_ASSERT( ! Unk1 );

	CFIX_ASSERT_FAILED(
		Factory->CreateInstance( NULL, IID_INonExisting, ( PVOID* ) &Unk1 ) );
	CFIXCC_ASSERT( ! Unk1 );

	CFIX_ASSERT_SUCCEEDED( Factory->CreateInstance( NULL, Iid, ( PVOID* ) &Unk1 ) );
	CFIXCC_ASSERT( Unk1 );
	__assume( Unk1 != NULL );
	CFIXCC_ASSERT_EQUALS( 0UL, Unk1->Release() );

	//
	// LockServer.
	//
	CFIX_ASSERT_OK( Factory->LockServer( TRUE ) );
	CFIX_ASSERT_OK( Factory->LockServer( FALSE ) );
}


void TestComUnknown(
	__in IUnknown *Unk,
	__in REFIID Iid1,
	__in REFIID Iid2
	)
{	
	TestUnknownQiIsReflexive( Unk, IID_IUnknown );
	TestUnknownQiIsReflexive( Unk, Iid1 );
	TestUnknownQiIsSymmetric( Unk, Iid1, Iid2 );
	TestUnknownQueryNonExistingInterface( Unk );
}

void TestComServer(
	__in PCOM_EXPORTS Exports,
	__in REFCLSID Clsid
	)
{
	CFIXCC_ASSERT_EQUALS( S_OK, Exports->CanUnloadNow() );

	//
	// Invalid CLSID/IID.
	//
	IClassFactory *Factory = NULL;
	CFIXCC_ASSERT_EQUALS( CLASS_E_CLASSNOTAVAILABLE,
		Exports->GetClassObject( 
			IID_INonExisting, IID_INonExisting, ( PVOID* ) &Factory ) );
	CFIXCC_ASSERT( ! Factory );
	CFIXCC_ASSERT_EQUALS( S_OK, Exports->CanUnloadNow() );

	CFIXCC_ASSERT_EQUALS( E_NOINTERFACE,
		Exports->GetClassObject( 
			Clsid, IID_INonExisting, ( PVOID* ) &Factory ) );
	CFIXCC_ASSERT_EQUALS( S_OK, Exports->CanUnloadNow() );

	//
	// Valid.
	//
	CFIX_ASSERT_OK( Exports->GetClassObject( 
			Clsid, IID_IClassFactory, ( PVOID* ) &Factory ) );
	CFIXCC_ASSERT_EQUALS( S_FALSE, Exports->CanUnloadNow() );
	__assume( Factory != NULL );

	Factory->Release();
	Factory = NULL;
	CFIXCC_ASSERT_EQUALS( S_OK, Exports->CanUnloadNow() );

	//
	// Test locking.
	//
	CFIX_ASSERT_OK( Exports->GetClassObject( 
			Clsid, IID_IClassFactory, ( PVOID* ) &Factory ) );
	CFIXCC_ASSERT_EQUALS( S_FALSE, Exports->CanUnloadNow() );
	Factory->LockServer( TRUE );
	Factory->Release();
	Factory = NULL;
	CFIXCC_ASSERT_EQUALS( S_FALSE, Exports->CanUnloadNow() );

	CFIX_ASSERT_OK( Exports->GetClassObject( 
			Clsid, IID_IClassFactory, ( PVOID* ) &Factory ) );
	CFIXCC_ASSERT_EQUALS( S_FALSE, Exports->CanUnloadNow() );
	Factory->LockServer( FALSE );
	
	Factory->Release();
	Factory = NULL;
	CFIXCC_ASSERT_EQUALS( S_OK, Exports->CanUnloadNow() );
}

void GetComExports( 
	__in PCWSTR ModuleName,
	__out PCOM_EXPORTS Exports
	)
{
	HMODULE Module = GetModuleHandle( ModuleName );
	if ( Module == NULL )
	{
		WCHAR Path[ MAX_PATH ];
		GetModuleFileName( GetModuleHandle( L"testctl" ), Path, _countof( Path ) );
		PathRemoveFileSpec( Path );
		PathAppend( Path, ModuleName );

		Module = LoadLibrary( Path );
		CFIX_ASSERT( Module );
	}

	Exports->GetClassObject = ( GETCLASSOBJ_PROC ) 
		GetProcAddress( Module, "DllGetClassObject" );
	CFIXCC_ASSERT( Exports->GetClassObject );

	Exports->CanUnloadNow = ( CANUNLOADNOW_PROC ) 
		GetProcAddress( Module, "DllCanUnloadNow" );
	CFIXCC_ASSERT( Exports->CanUnloadNow );

	Exports->RegisterServer = ( SELFREGISTER_PROC ) 
		GetProcAddress( Module, "DllRegisterServer" );
	CFIXCC_ASSERT( Exports->RegisterServer );

	Exports->UnregisterServer = ( SELFREGISTER_PROC ) 
		GetProcAddress( Module, "DllUnregisterServer" );
	CFIXCC_ASSERT( Exports->UnregisterServer );

	CFIX_ASSERT( Exports->GetClassObject );
	CFIX_ASSERT( Exports->CanUnloadNow );
}