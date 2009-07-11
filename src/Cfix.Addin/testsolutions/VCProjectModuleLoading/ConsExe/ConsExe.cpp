// ConsExe.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <cfix.h>

static void __stdcall Dummy1()
{
	//CFIX_ASSERT( !"fail" );
}

static void __stdcall Dummy2()
{
	//CFIX_ASSERT( !"fail" );
}

CFIX_BEGIN_FIXTURE( Cons )
	CFIX_FIXTURE_ENTRY( Dummy1 )
	CFIX_FIXTURE_ENTRY( Dummy2 )
CFIX_END_FIXTURE()

int _tmain(int argc, _TCHAR* argv[])
{
	return 0;
}

