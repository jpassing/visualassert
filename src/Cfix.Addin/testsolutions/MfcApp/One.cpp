#include "StdAfx.h"
#include <cfixcc.h>
class One : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(One)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

