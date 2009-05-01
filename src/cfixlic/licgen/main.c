/*----------------------------------------------------------------------
 * Purpose:
 *		License generation tool.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfixlic.h>
#include <stdlib.h>
#include <stdio.h>

static HRESULT Generate(
	__in ULONG ProductId,
	__in ULONG SubProductId,
	__in ULONG MajorVersion,
	__in ULONG MinorVersion,
	__in ULONG Count 
	)
{
	HRESULT Hr;
	ULONG Index;

	for ( Index = 0; Index < Count; Index++ )
	{
		CFIXLIC_LICENSE_KEY Key;
		WCHAR KeyBuffer[ 30 ];

		Hr = CfixlicCreateKey(
			ProductId,
			SubProductId,
			MajorVersion,
			MinorVersion,
			1,
			&Key );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		Hr = CfixlicScrambleKey( &Key );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		Hr = CfixlicEncode( &Key, _countof( KeyBuffer ), KeyBuffer );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		wprintf( L"%s\n", KeyBuffer );
	}

	return S_OK;
}

INT __cdecl wmain( INT Argc, PCWSTR* Argv )
{
	HRESULT Hr;

	ULONG Count = 1;
	ULONG ProductId;
	ULONG SubProductId;

	ULONG MajorVersion;
	ULONG MinorVersion;
	
	if ( Argc < 4 )
	{
		wprintf( L"Usage: %s <product> <subproduct> <major> <minor> [<count>]\n", Argv[ 0 ] );
		return 1;
	}

	ProductId = _wtol( Argv[ 1 ] );
	SubProductId = _wtol( Argv[ 2 ] );
	
	MajorVersion = _wtol( Argv[ 3 ] );
	MinorVersion = _wtol( Argv[ 4 ] );

	if ( Argc > 5 )
	{
		Count = _wtol( Argv[ 5 ] );
	}

	wprintf( L"Product:     %u\n"
			 L"SubProduct:  %u\n" 
			 L"Major ver:   %u\n" 
			 L"Minor ver:   %u\n" 
			 L"Count:       %u\n\n", 
			 ProductId,
			 SubProductId,
			 MajorVersion,
			 MinorVersion,
			 Count );

	Hr = Generate( 
		ProductId,
		SubProductId,
		MajorVersion,
		MinorVersion,
		Count );

	if ( FAILED( Hr ) )
	{
		wprintf( L"Error 0x%08X\n", Hr );
		return Hr;
	}
	else
	{
		return 0;
	}
}