#include "StdAfx.h"
#include <cfixcc.h>
#include  <stdlib.h>

class A : public cfixcc::TestFixture
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
		CFIX_ASSERT(!"hjhj");
	}

	void Test()
	{
		//abort();
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(A)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

class B : public cfixcc::TestFixture
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

CFIXCC_BEGIN_CLASS(B)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

