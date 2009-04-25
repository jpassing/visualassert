/*----------------------------------------------------------------------
 * Purpose:
 *		Key/string encoding. 
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfixlic.h>
#include <stdlib.h>

static WCHAR CfixlicsCharTable[] = {
	L'A', L'B', L'C', L'D', L'E', L'F', L'G', L'H', 
	L'I', L'K', L'L', L'M', L'N', L'P', L'Q', L'R', 
	L'S', L'T', L'U', L'V', L'W', L'X', L'Y', L'Z', 
	L'2', L'3', L'4', L'5', L'6', L'7', L'8', L'9' };

static ULONG CfixlicsDecodeChar( 
	__in WCHAR Char
	)
{
	ULONG Index;

	//
	// CfixlicsCharTable has gaps, so use sequential scan.
	//
	for ( Index = 0; Index < _countof( CfixlicsCharTable ); Index++ )
	{
		if ( Char == CfixlicsCharTable[ Index ] )
		{
			return Index;
		}
	}

	return ( ULONG ) -1;
}


HRESULT CfixlicEncode(
	__in CONST PCFIXLIC_LICENSE_KEY Key,
	__in SIZE_T BufferCch,
	__out_ecount( BufferCch ) PWSTR Buffer
	)
{
	if ( ! Key || 
		 BufferCch == 0 || 
		 BufferCch < 30 || 
		 ! Buffer ||
		 ! Key->Scrambled )
	{
		return E_INVALIDARG;
	}

	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 -  5 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 - 10 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 - 15 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 - 20 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 - 25 ) ) & 0x1F ];
	*Buffer++ = L'-';
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 - 30 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 - 35 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 - 40 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 - 45 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 - 50 ) ) & 0x1F ];
	*Buffer++ = L'-';
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 - 55 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 0 ] >> ( 64 - 60 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[   Key->u.Qwords[ 0 ] & 0xF ];	// 1 bit spare

	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 -  5 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 - 10 ) ) & 0x1F ];
	*Buffer++ = L'-';
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 - 15 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 - 20 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 - 25 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 - 30 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 - 35 ) ) & 0x1F ];
	*Buffer++ = L'-';
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 - 40 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 - 45 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 - 50 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 - 55 ) ) & 0x1F ];
	*Buffer++ = CfixlicsCharTable[ ( Key->u.Qwords[ 1 ] >> ( 64 - 60 ) ) & 0x1F ];
	*Buffer = L'\0';

	return S_OK;
}

HRESULT CfixlicDecode(
	__in PCWSTR KeyString,
	__out CONST PCFIXLIC_LICENSE_KEY Key
	)
{
	if ( ! KeyString ||
		 wcslen( KeyString ) != CFIXLIC_ENCODED_KEY_LENGTH ||
		 ! Key )
	{
		return E_INVALIDARG;
	}

	ZeroMemory( Key, sizeof( CFIXLIC_LICENSE_KEY ) );

	Key->u.Qwords[ 0 ]  = ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 -  5 );
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 10 );
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 15 );
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 20 );
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 25 );
	KeyString++;
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 30 );
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 35 );
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 40 );
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 45 );
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 50 );
	KeyString++;
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 55 );
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 60 );
	Key->u.Qwords[ 0 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) );

	Key->u.Qwords[ 1 ]  = ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 -  5 );
	Key->u.Qwords[ 1 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 10 );
	KeyString++;
	Key->u.Qwords[ 1 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 15 );
	Key->u.Qwords[ 1 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 20 );
	Key->u.Qwords[ 1 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 25 );
	Key->u.Qwords[ 1 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 30 );
	Key->u.Qwords[ 1 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 35 );
	KeyString++;
	Key->u.Qwords[ 1 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 40 );
	Key->u.Qwords[ 1 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 45 );
	Key->u.Qwords[ 1 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 50 );
	Key->u.Qwords[ 1 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 55 );
	Key->u.Qwords[ 1 ] |= ( ( ULONGLONG ) CfixlicsDecodeChar( *KeyString++ ) ) << ( 64 - 60 );

	Key->Scrambled = TRUE;

	return S_OK;
}