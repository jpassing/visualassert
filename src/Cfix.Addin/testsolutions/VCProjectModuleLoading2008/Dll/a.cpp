#include <cfixcc.h>
class a : public cfixcc::TestFixture
{
private:

public:
	void Test()
	{
	}
};

CFIXCC_BEGIN_CLASS(a)
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

