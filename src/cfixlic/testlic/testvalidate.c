/*----------------------------------------------------------------------
 * Purpose:
 *		Test Licensing. 
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfix.h>
#include <cfixlic.h>
#include <stdlib.h>

#define CFIX_ASSERT_OK( Expr ) CFIX_ASSERT_EQUALS_ULONG( S_OK, ( Expr ) )

static void CreateAndVerify()
{
	CFIXLIC_LICENSE_KEY Key;
	ULONG Index;

	for ( Index = 1; Index < 0xffff; Index++ )
	{
		BOOL Valid;

		CFIX_ASSERT_OK( CfixlicCreateKey( 
			Index & 1, 
			Index & 2, 
			Index & 3, 
			Index & 4, 
			Index, 
			&Key ) );

		CFIX_ASSERT_OK( CfixlicVerifyKey( &Key, &Valid ) );
		CFIX_ASSERT( Valid );
	}
	
	CFIX_ASSERT( E_INVALIDARG == CfixlicCreateKey( 
		17, 
		0,
		0,
		0,
		1, 
		&Key ) );
	CFIX_ASSERT( E_INVALIDARG == CfixlicCreateKey( 
		16, 
		0,
		0,
		0,
		0, 
		&Key ) );
}

static void Scramble()
{
	CFIXLIC_LICENSE_KEY Key;
	CFIXLIC_LICENSE_KEY KeyUnscrambled;
	ULONG Index;

	for ( Index = 1; Index < 0xffff; Index++ )
	{
		BOOL Valid;

		CFIX_ASSERT_OK( CfixlicCreateKey( 
			Index & 1, 
			Index & 2, 
			Index & 3, 
			Index & 4, 
			Index, 
			&Key ) );

		memcpy( &KeyUnscrambled, &Key, sizeof( CFIXLIC_LICENSE_KEY ) );

		CFIX_ASSERT_OK( CfixlicScrambleKey( &Key ) );
		CFIX_ASSERT_OK( CfixlicUnscrambleKey( &Key ) );
		
		CFIX_ASSERT( 0 == memcmp( &KeyUnscrambled, &Key, sizeof( CFIXLIC_LICENSE_KEY ) ) );

		CFIX_ASSERT_OK( CfixlicVerifyKey( &Key, &Valid ) );
		CFIX_ASSERT( Valid );
	}
}

static void EnDecode()
{
	CFIXLIC_LICENSE_KEY Key;
	CFIXLIC_LICENSE_KEY KeyUnscrambled;
	CFIXLIC_LICENSE_KEY KeyScrambled;
	ULONG Index;

	for ( Index = 1; Index < 0xffff; Index++ )
	{
		BOOL Valid;
		WCHAR Buffer[ 30 ];

		CFIX_ASSERT_OK( CfixlicCreateKey( 
			Index & 1, 
			Index & 2, 
			Index & 3, 
			Index & 4, 
			Index, 
			&Key ) );

		//Key.Scrambled = TRUE;
		memcpy( &KeyUnscrambled, &Key, sizeof( CFIXLIC_LICENSE_KEY ) );

		CFIX_ASSERT_OK( CfixlicScrambleKey( &Key ) );
		memcpy( &KeyScrambled, &Key, sizeof( CFIXLIC_LICENSE_KEY ) );

		CfixlicEncode( &Key, _countof( Buffer ), Buffer );
		CFIX_LOG( Buffer );
		CfixlicDecode( Buffer, &Key );

		CFIX_ASSERT( Key.u.Qwords[ 0 ] == KeyScrambled.u.Qwords[ 0 ] );
		CFIX_ASSERT( Key.u.Qwords[ 1 ] == KeyScrambled.u.Qwords[ 1 ] );

		CFIX_ASSERT_OK( CfixlicUnscrambleKey( &Key ) );
		
		CFIX_ASSERT( Key.u.Qwords[ 0 ] == KeyUnscrambled.u.Qwords[ 0 ] );
		CFIX_ASSERT( Key.u.Qwords[ 1 ] == KeyUnscrambled.u.Qwords[ 1 ] );

		CFIX_ASSERT_OK( CfixlicVerifyKey( &Key, &Valid ) );
		CFIX_ASSERT( Valid );
	}
}

CFIX_BEGIN_FIXTURE( Validate )
	CFIX_FIXTURE_ENTRY( CreateAndVerify )
	CFIX_FIXTURE_ENTRY( Scramble )
	CFIX_FIXTURE_ENTRY( EnDecode )
CFIX_END_FIXTURE()