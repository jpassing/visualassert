[!if GENERATE_SETUP]
static void CFIXCALLTYPE Setup[!output FIXTURE_NAME]()
{
}

[!endif]
[!if GENERATE_TEARDOWN]
static void CFIXCALLTYPE Teardown[!output FIXTURE_NAME]()
{
}

[!endif]
[!if GENERATE_BEFORE]
static void CFIXCALLTYPE Before[!output FIXTURE_NAME]()
{
}

[!endif]
[!if GENERATE_AFTER]
static void CFIXCALLTYPE After[!output FIXTURE_NAME]()
{
}

[!endif]
static void CFIXCALLTYPE Test()
{
	CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
}

CFIX_BEGIN_FIXTURE([!output FIXTURE_NAME])
[!if GENERATE_SETUP]
	CFIX_FIXTURE_SETUP(Setup[!output FIXTURE_NAME])
[!endif]
[!if GENERATE_TEARDOWN]
	CFIX_FIXTURE_TEARDOWN(Teardown[!output FIXTURE_NAME])
[!endif]
[!if GENERATE_BEFORE]
	CFIX_FIXTURE_BEFORE(Before[!output FIXTURE_NAME])
[!endif]
[!if GENERATE_AFTER]
	CFIX_FIXTURE_AFTER(After[!output FIXTURE_NAME])
[!endif]
	CFIX_FIXTURE_ENTRY(Test)
CFIX_END_FIXTURE()

