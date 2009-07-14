// ConsExe.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <cfix.h>


class Test
{
public:
	ULONG var;

	Test()
	{
		this->var = 0xBABE;
	}
};

Test test;

static void __stdcall Dummy1()
{
	CFIX_ASSERT_EQUALS_ULONG( 0xBABE, test.var );
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
	CFIX_ASSERT( !"fail" );
	return 0;
}

