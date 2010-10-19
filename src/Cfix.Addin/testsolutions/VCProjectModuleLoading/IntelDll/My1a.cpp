#include "StdAfx.h"
#include <cfixcc.h>
#include "StdAfx.h"
#include <cfixcc.h>
class My1a : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(My1a)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

class My1b : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(My1b)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

