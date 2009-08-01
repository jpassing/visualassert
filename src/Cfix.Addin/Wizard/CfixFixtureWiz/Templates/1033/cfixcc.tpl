class [!output FIXTURE_NAME] : public cfixcc::TestFixture
{
private:

public:
[!if GENERATE_SETUP]
	static void SetUp()
	{
	}

[!endif]
[!if GENERATE_TEARDOWN]
	static void TearDown()
	{
	}

[!endif]
[!if GENERATE_BEFORE]
	void Before()
	{
	}

[!endif]
[!if GENERATE_AFTER]
	void After()
	{
	}

[!endif]
	void Test()
	{
		CFIX_INCONCLUSIVE(__TEXT("Not implemented"));
	}
};

CFIXCC_BEGIN_CLASS([!output FIXTURE_NAME])
	CFIXCC_METHOD(Test)
CFIXCC_END_CLASS()

