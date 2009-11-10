#include <cfix.h>
#include <crtdbg.h>

static void __stdcall Fail()
{
	CFIX_FAIL( NULL );
}

static void __stdcall Succeed()
{
}

CFIX_BEGIN_FIXTURE( Fail_FailInSetup )
	CFIX_FIXTURE_SETUP( Fail )
	CFIX_FIXTURE_TEARDOWN( Succeed )
	CFIX_FIXTURE_BEFORE( Succeed )
	CFIX_FIXTURE_AFTER( Succeed )
	CFIX_FIXTURE_ENTRY( Succeed )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( Fail_FailInTeardown )
	CFIX_FIXTURE_SETUP( Succeed )
	CFIX_FIXTURE_TEARDOWN( Fail )
	CFIX_FIXTURE_BEFORE( Succeed )
	CFIX_FIXTURE_AFTER( Succeed )
	CFIX_FIXTURE_ENTRY( Succeed )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( Fail_FailInBefore )
	CFIX_FIXTURE_SETUP( Succeed )
	CFIX_FIXTURE_TEARDOWN( Succeed )
	CFIX_FIXTURE_BEFORE( Fail )
	CFIX_FIXTURE_AFTER( Succeed )
	CFIX_FIXTURE_ENTRY( Succeed )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( Fail_FailInAfter )
	CFIX_FIXTURE_SETUP( Succeed )
	CFIX_FIXTURE_TEARDOWN( Succeed )
	CFIX_FIXTURE_BEFORE( Succeed )
	CFIX_FIXTURE_AFTER( Fail )
	CFIX_FIXTURE_ENTRY( Succeed )
CFIX_END_FIXTURE()

CFIX_BEGIN_FIXTURE( Fail_FailInTest )
	CFIX_FIXTURE_SETUP( Succeed )
	CFIX_FIXTURE_TEARDOWN( Succeed )
	CFIX_FIXTURE_BEFORE( Succeed )
	CFIX_FIXTURE_AFTER( Succeed )
	CFIX_FIXTURE_ENTRY( Fail )
CFIX_END_FIXTURE()
