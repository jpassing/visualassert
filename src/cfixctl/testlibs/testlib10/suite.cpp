/*----------------------------------------------------------------------
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfixcc.h>

class TestExecActionDummy : public cfixcc::TestFixture
{
public:
	void Log()
	{
		CFIX_LOG( L"test" );
	}
};

CFIXCC_BEGIN_CLASS( TestExecActionDummy )
	CFIXCC_METHOD( Log )
	CFIXCC_METHOD( Log )
CFIXCC_END_CLASS()