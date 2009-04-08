/*----------------------------------------------------------------------
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <cfixcc.h>

class TestExecActionDummy : public cfixcc::TestFixture
{
public:
	void One()
	{
		CFIX_LOG( L"one" );
	}

	void Two()
	{
		CFIX_LOG( L"two" );
	}
};

CFIXCC_BEGIN_CLASS( TestExecActionDummy )
	CFIXCC_METHOD( One )
	CFIXCC_METHOD( Two )
CFIXCC_END_CLASS()