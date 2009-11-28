#include "StdAfx.h"
#include <cfixcc.h>
class ba : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_FAIL(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(ba)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

