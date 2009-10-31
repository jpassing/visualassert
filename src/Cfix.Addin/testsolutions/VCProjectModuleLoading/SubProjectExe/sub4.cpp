#include "StdAfx.h"
#include <cfixcc.h>
class sub4 : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(sub4)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

