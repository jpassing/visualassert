// SubProjectExe.cpp : Defines the entry point for the console application.
//
 
#include "stdafx.h"

#include <cfixcc.h>

class My1 : public cfixcc::TestFixture
{
private:

public:
	static void SetUp()
	{
	}

	static void TearDown()
	{
	}

	void Before()
	{
	}

	void After()
	{
	}

	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(My1)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

int _tmain(int argc, _TCHAR* argv[])
{
	return 0;
}

