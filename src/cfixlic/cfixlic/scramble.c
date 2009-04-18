/*----------------------------------------------------------------------
 * Purpose:
 *		Key scrambling. 
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfixlic.h>

HRESULT CfixlicScrambleKey(
	__inout PCFIXLIC_LICENSE_KEY Key
	)
{
	if ( ! Key || Key->Scrambled )
	{
		return E_INVALIDARG;
	}

	//
	// Apply XOR.
	//
	Key->u.Words[ 0 ] ^= Key->u.Fields.XorMask;
	Key->u.Words[ 1 ] ^= Key->u.Fields.XorMask;
	Key->u.Words[ 2 ] ^= Key->u.Fields.XorMask;
	Key->u.Words[ 3 ] ^= Key->u.Fields.XorMask;
	Key->u.Words[ 6 ] ^= Key->u.Fields.XorMask;
	Key->u.Words[ 7 ] ^= Key->u.Fields.XorMask;

	Key->u.Fields.Checksum ^= Key->u.Fields.XorMask;

	//
	// Rotate.
	//
	Key->u.Dwords[ 0 ] = _lrotl( 
		Key->u.Dwords[ 0 ],
		Key->u.Fields.Rotate );
	Key->u.Dwords[ 1 ] = _lrotl( 
		Key->u.Dwords[ 1 ],
		Key->u.Fields.Rotate );
	Key->u.Dwords[ 3 ] = _lrotl( 
		Key->u.Dwords[ 3 ],
		Key->u.Fields.Rotate );

	Key->Scrambled = TRUE;

	return S_OK;
}

HRESULT CfixlicUnscrambleKey(
	__inout PCFIXLIC_LICENSE_KEY Key
	)
{
	if ( ! Key || ! Key->Scrambled )
	{
		return E_INVALIDARG;
	}

	//
	// Rotate.
	//
	Key->u.Dwords[ 0 ] = _lrotr( 
		Key->u.Dwords[ 0 ],
		Key->u.Fields.Rotate );
	Key->u.Dwords[ 1 ] = _lrotr( 
		Key->u.Dwords[ 1 ],
		Key->u.Fields.Rotate );
	Key->u.Dwords[ 3 ] = _lrotr( 
		Key->u.Dwords[ 3 ],
		Key->u.Fields.Rotate );

	//
	// Apply XOR.
	//
	Key->u.Words[ 0 ] ^= Key->u.Fields.XorMask;
	Key->u.Words[ 1 ] ^= Key->u.Fields.XorMask;
	Key->u.Words[ 2 ] ^= Key->u.Fields.XorMask;
	Key->u.Words[ 3 ] ^= Key->u.Fields.XorMask;
	Key->u.Words[ 6 ] ^= Key->u.Fields.XorMask;
	Key->u.Words[ 7 ] ^= Key->u.Fields.XorMask;

	Key->u.Fields.Checksum ^= Key->u.Fields.XorMask;

	Key->Scrambled = FALSE;

	return S_OK;
}