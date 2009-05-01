#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Licensing. 
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <windows.h>

#ifdef _WIN64
#define CFIXCTLCALLTYPE
#else
#define CFIXCTLCALLTYPE __stdcall
#endif

#pragma warning( push )
#pragma warning( disable: 4214 ) // ULONG bitfields.

#define CFIXLIC_ENCODED_KEY_LENGTH 29

typedef struct _CFIXLIC_LICENSE_KEY
{
	union
	{
		ULONGLONG Qwords[ 2 ];
		DWORD Dwords[ 4 ];
		WORD Words[ 8 ];

		struct
		{
			struct
			{
				ULONG Product : 4;
				ULONG SubProduct : 4;
				ULONG MajorVersion : 4;
				ULONG MinorVersion : 4;

				//
				// CRC-16 checksum over fields above.
				//
				ULONG Checksum : 16;
			} BaseInfo;

			//
			// Serial number, auto-incremented.
			//
			ULONG Serial;

			//
			// Last byte (mind the byte order) cannot be fully used as 
			// it is cropped. Must be 0.
			//
			UCHAR Unused;

			//
			// Bit count by which Dword{0,1,3} are rotated to the left.
			// Equals (non-XORed) lower halfword of serial.
			//
			UCHAR Rotate;

			//
			// Mask used to XOR Word{0,1,2,3,6,7} with.
			// Equals lower word of serial.
			//
			USHORT XorMask;

			//
			// Reserved, must be 0.
			//
			USHORT Reserved;

			//
			// CRC-16 checksum over all fields above
			//
			USHORT Checksum;
		} Fields;
	} u;

	//
	// Indicates whether rotation and XOR has been applied.
	//
	BOOL Scrambled;
} CFIXLIC_LICENSE_KEY, *PCFIXLIC_LICENSE_KEY;

#pragma warning( pop )

/*++
	Routine Description:
		Verify given unscrambled key by checking its consistency.
--*/
EXTERN_C HRESULT CfixlicVerifyKey(
	__in CONST PCFIXLIC_LICENSE_KEY Key,
	__out PBOOL Valid
	);

/*++
	Routine Description:
		Create a new, unscrambled key.
--*/
EXTERN_C HRESULT CfixlicCreateKey(
	__in ULONG Product,
	__in ULONG SubProduct,
	__in ULONG MajorVersion,
	__in ULONG MinorVersion,
	__in ULONG Serial,
	__out PCFIXLIC_LICENSE_KEY Key
	);

/*++
	Routine Description:
		Scramble key by applying XOR and rotation.
--*/
EXTERN_C HRESULT CfixlicScrambleKey(
	__inout PCFIXLIC_LICENSE_KEY Key
	);

/*++
	Routine Description:
		Unscramble key by applying XOR and rotation.
--*/
EXTERN_C HRESULT CfixlicUnscrambleKey(
	__inout PCFIXLIC_LICENSE_KEY Key
	);

/*++
	Routine Description:
		Encode key into a human-readable string.
--*/
EXTERN_C HRESULT CfixlicEncode(
	__in CONST PCFIXLIC_LICENSE_KEY Key,
	__in SIZE_T BufferCch,
	__out_ecount( BufferCch ) PWSTR Buffer
	);

/*++
	Routine Description:
		Decode key from human-readable string.
--*/
EXTERN_C HRESULT CfixlicDecode(
	__in PCWSTR KeyString,
	__out CONST PCFIXLIC_LICENSE_KEY Key
	);