FIXTURE([!output FIXTURE_NAME]);

[!if GENERATE_BEFORE]
SETUP([!output FIXTURE_NAME])
{
}

[!endif]
[!if GENERATE_AFTER]
TEARDOWN([!output FIXTURE_NAME])
{
}

[!endif]

BEGIN_TESTF(Test, [!output FIXTURE_NAME])
{
	CFIX_INCONCLUSIVE("Not implemented");
}
END_TESTF