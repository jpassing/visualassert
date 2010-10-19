#include "StdAfx.h"
#include <cfixcc.h>
class My4 : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(My4)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

