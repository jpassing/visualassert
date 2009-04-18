/*----------------------------------------------------------------------
 * Purpose:
 *		Key verification. 
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfixlic.h>
#include "crc16.h"

#define CfixlicsXorFromSerial( Serial )				\
	( ( ( USHORT ) Serial ) ^ 0xBABE )
#define CfixlicsRotateFromSerial( Serial )		\
	( ( UCHAR ) ( ( ( ( Serial >> 8 ) ^ Serial ) & 0xFF ) | 0x1 ) )

HRESULT CfixlicVerifyKey(
	__in CONST PCFIXLIC_LICENSE_KEY Key,
	__out PBOOL Valid
	)
{
	USHORT BaseInfoChecksum;
	USHORT Checksum;

	if ( ! Key || ! Valid || Key->Scrambled )
	{
		return E_INVALIDARG;
	}

	BaseInfoChecksum = crcsum( 
		( PUCHAR ) Key,
		2,
		0 );
	if ( BaseInfoChecksum != Key->u.Fields.BaseInfo.Checksum )
	{
		*Valid = FALSE;
		return S_OK;
	}

	if ( Key->u.Fields.Reserved != 0 || 
		 Key->u.Fields.Serial == 0 ||
		 Key->u.Fields.XorMask != CfixlicsXorFromSerial( Key->u.Fields.Serial ) )
	{
		*Valid = FALSE;
		return S_OK;
	}

	Checksum = crcsum( 
		( PUCHAR ) Key,
		FIELD_OFFSET( CFIXLIC_LICENSE_KEY, u.Fields.Checksum ),
		0 );
	if ( Checksum != Key->u.Fields.Checksum )
	{
		*Valid = FALSE;
		return S_OK;
	}

	if ( Key->u.Fields.Rotate == 0 ||
		 Key->u.Fields.Rotate != CfixlicsRotateFromSerial( Key->u.Fields.Serial ) ||
		 Key->u.Fields.Unused != 0 )
	{
		*Valid = FALSE;
		return S_OK;
	}

	*Valid = TRUE;
	return S_OK;
}

HRESULT CfixlicCreateKey(
	__in ULONG Product,
	__in ULONG SubProduct,
	__in ULONG MajorVersion,
	__in ULONG MinorVersion,
	__in ULONG Serial,
	__out PCFIXLIC_LICENSE_KEY Key
	)
{
	if ( Product > 0xF ||
		 SubProduct > 0xF ||
		 MajorVersion > 0xF ||
		 MinorVersion > 0xF ||
		 Serial == 0 ||
		 Serial > 1000000000 ||
		 ! Key )
	{
		return E_INVALIDARG;
	}

	//
	// Slightly increase differences between consecutive keys.
	//
	Serial *= 3;

	Key->u.Fields.BaseInfo.Product		= Product;
	Key->u.Fields.BaseInfo.SubProduct	= SubProduct;
	Key->u.Fields.BaseInfo.MajorVersion	= MajorVersion;
	Key->u.Fields.BaseInfo.MinorVersion	= MinorVersion;
	
	Key->u.Fields.BaseInfo.Checksum = crcsum(
		( PUCHAR ) Key,
		2,
		0 );

	Key->u.Fields.Serial		= Serial;
	Key->u.Fields.Unused		= 0;
	Key->u.Fields.Rotate		= CfixlicsRotateFromSerial( Key->u.Fields.Serial );
	Key->u.Fields.XorMask		= CfixlicsXorFromSerial( Serial );
	
	Key->u.Fields.Reserved		= 0;

	Key->u.Fields.Checksum	= crcsum( 
		( PUCHAR ) Key,
		FIELD_OFFSET( CFIXLIC_LICENSE_KEY, u.Fields.Checksum ),
		0 );

	Key->Scrambled			= FALSE;

	return S_OK;
}
	
