#include <cfixcc.h>
class a : public cfixcc::TestFixture
{
private:

public:
	void Test1()
	{
	}
	void Test2()
	{
	}
};

CFIXCC_BEGIN_CLASS(a)
	CFIXCC_METHOD(Test1)
	CFIXCC_METHOD(Test2)
CFIXCC_END_CLASS()

