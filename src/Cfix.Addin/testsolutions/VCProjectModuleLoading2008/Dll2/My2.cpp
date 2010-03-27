#include "StdAfx.h"
#include <cfixcc.h>
class My2 : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(My2)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

