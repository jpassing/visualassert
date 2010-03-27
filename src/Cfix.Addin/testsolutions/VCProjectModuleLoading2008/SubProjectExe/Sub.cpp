#include "StdAfx.h"
#include <cfixcc.h>
class Sub : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(Sub)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

