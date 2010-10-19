#include "StdAfx.h"
#include <cfixcc.h>
class My1 : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(My1)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

