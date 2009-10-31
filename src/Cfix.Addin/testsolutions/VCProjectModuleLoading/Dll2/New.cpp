#include "StdAfx.h"
#include <cfixcc.h>
class New : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(New)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

