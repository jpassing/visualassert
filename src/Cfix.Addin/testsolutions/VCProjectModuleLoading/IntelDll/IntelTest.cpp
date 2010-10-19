#include <omp.h>
#include <cfixcc.h>
class IntelTest : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
		new char[ 1024 ];
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS(IntelTest)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

