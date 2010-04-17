#include "StdAfx.h"
#include <cfixcc.h>
class Cc : public cfixcc::TestFixture
{
private:

public:
	void TestCc()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(Cc)
	CFIXCC_METHOD(TestCc)
	CFIXCC_METHOD(TestCc)
CFIXCC_END_CLASS()

