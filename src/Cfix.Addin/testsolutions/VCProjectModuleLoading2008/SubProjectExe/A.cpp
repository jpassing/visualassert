#include "StdAfx.h"
#include <cfixcc.h>
#include  <stdlib.h>

class A : public cfixcc::TestFixture
{
private:

public:
	static void SetUp()
	{
		//CFIX_ASSERT(!"hjhj");
	}

	static void TearDown()
	{
	}

	void Before()
	{
	}

	void After()
	{
//		CFIX_ASSERT(!"hjhj");
	}

	void Test1()
	{
		new char[213];
		//abort();
		CFIX_LOG(__TEXT("Not implemented"));
	}

	void Test2()
	{
		//abort();
		CFIX_LOG(__TEXT("Not implemented"));
	}

	void Test3()
	{
		//abort();
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(A)
	CFIXCC_METHOD(Test1)
	CFIXCC_METHOD(Test2)
	CFIXCC_METHOD(Test3)
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
	}
};

CFIXCC_BEGIN_CLASS(B)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

