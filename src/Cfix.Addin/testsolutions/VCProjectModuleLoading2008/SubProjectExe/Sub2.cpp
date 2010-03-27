#include "StdAfx.h"
#include <cfixcc.h>
class Sub21 : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(Sub21)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

