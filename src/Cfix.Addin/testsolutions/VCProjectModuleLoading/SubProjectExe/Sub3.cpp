#include "StdAfx.h"
#include <cfixcc.h>
class Sub3 : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(Sub3)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

