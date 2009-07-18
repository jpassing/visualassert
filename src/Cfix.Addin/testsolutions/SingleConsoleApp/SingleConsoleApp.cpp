// ConsExe.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <cfix.h>

static void __stdcall Dummy()
{
	CFIX_ASSERT( !"fail" );
}

CFIX_BEGIN_FIXTURE( Cons )
	CFIX_FIXTURE_ENTRY( Dummy )
CFIX_END_FIXTURE()

int _tmain(int argc, _TCHAR* argv[])
{
	return 0;
}

