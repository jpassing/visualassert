/*----------------------------------------------------------------------
 * Purpose:
 *		Test Module searching.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#define TESTDATA_FOLDER L"..\\..\\..\\src\\cfixctl\\testctl\\testdata"

#define E_FNF HRESULT_FROM_WIN32( ERROR_FILE_NOT_FOUND )

#include <testctlp.h>

class SearchCallback : public ICfixSearchModulesCallback
{
public:
	ULONG I386Files;
	ULONG Amd64Files;
	ULONG UserFiles;
	ULONG KernelFiles;
	ULONG DirEntries;
	ULONG DirLeaves;

	ULONG MaxFiles;

	SearchCallback( ULONG MaxFiles ) 
		: I386Files( 0 )
		, Amd64Files( 0 )
		, UserFiles( 0 )
		, KernelFiles( 0 )
		, DirEntries( 0 )
		, DirLeaves( 0 )
		, MaxFiles( MaxFiles )
	{
	}

	SearchCallback() 
		: I386Files( 0 )
		, Amd64Files( 0 )
		, UserFiles( 0 )
		, KernelFiles( 0 )
		, DirEntries( 0 )
		, DirLeaves( 0 )
		, MaxFiles( 0xFFFFFFFF )
	{
	}

	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )()
	{
		return 1;
	}

	STDMETHOD_( ULONG, Release )()
	{
		return 1;
	}

	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr )
	{
		HRESULT Hr;

		if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
			 InlineIsEqualGUID( Iid, IID_ICfixSearchModulesCallback ) )
		{
			*Ptr = static_cast< ICfixSearchModulesCallback* >( this );
			Hr = S_OK;

		}
		else
		{
			*Ptr = NULL;
			Hr = E_NOINTERFACE;
		}

		return Hr;
	}

	/*------------------------------------------------------------------
	 * ICfixSearchModulesCallback methods.
	 */

	STDMETHOD( EnterDirectory )(
		__in BSTR Path
		)
	{
		CFIXCC_ASSERT_NOT_EQUALS( 
			INVALID_FILE_ATTRIBUTES,
			GetFileAttributes( Path ) );
		CFIXCC_ASSERT( FILE_ATTRIBUTE_DIRECTORY & GetFileAttributes( Path ) );
		
		this->DirEntries++;
		return S_OK;
	}

	STDMETHOD( FoundModule )(
		__in BSTR Path,
		__in CfixTestModuleType Type,
		__in CfixTestModuleArch Arch
		)
	{
		CFIXCC_ASSERT_NOT_EQUALS( 
			INVALID_FILE_ATTRIBUTES,
			GetFileAttributes( Path ) );
		CFIXCC_ASSERT( INVALID_FILE_ATTRIBUTES != GetFileAttributes( Path ) );

		if ( Arch == CfixTestModuleArchI386 )
		{
			this->I386Files++;
		}
		else
		{
			this->Amd64Files++;
		}

		if ( Type == CfixTestModuleTypeUser )
		{
			this->UserFiles++;
		}
		else
		{
			this->KernelFiles++;
		}


		return UserFiles + KernelFiles < MaxFiles ? S_OK : E_FAIL;
	}

	STDMETHOD( LeaveDirectory )(
		__in BSTR Path
		)
	{
		CFIXCC_ASSERT_NOT_EQUALS( 
			INVALID_FILE_ATTRIBUTES,
			GetFileAttributes( Path ) );
		CFIXCC_ASSERT( FILE_ATTRIBUTE_DIRECTORY & GetFileAttributes( Path ) );

		this->DirLeaves++;
		return S_OK;
	}
};

class TestSearch : public cfixcc::TestFixture
{
private:
	static COM_EXPORTS Exports;
	ICfixHost *Host;
	WCHAR BinPath[ MAX_PATH ];
	
public:
	TestSearch() : Host( NULL )
	{
	}

	static void SetUp()
	{
		CoInitialize( NULL );
		GetComExports( L"cfixctl.dll", &Exports );
	}

	static void TearDown()
	{
		CoUninitialize();
	}

	virtual void Before()
	{
		IClassFactory *AgentFactory;

		CFIXCC_ASSERT_OK( Exports.GetClassObject( 
			CLSID_LocalAgent, IID_IClassFactory, ( PVOID* ) &AgentFactory ) );
		CFIXCC_ASSERT( AgentFactory );
		__assume( AgentFactory );

		ICfixAgent *Agent;
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		CFIXCC_ASSERT_OK( Agent->SetTrialLicenseCookie(
			CurrentLicensingDate() ) );

		CFIXCC_ASSERT_OK( Agent->CreateHost( 
			TESTCTLP_OWN_ARCHITECTURE,
			CLSCTX_INPROC_SERVER,
			0,
			0,
			NULL,
			NULL,
			&Host ) );

		Agent->Release();
		AgentFactory->Release();
		
		CFIXCC_ASSERT( GetModuleFileName( 
			GetModuleHandle( L"cfixctl.dll" ), 
			this->BinPath,
			_countof( this->BinPath ) ) );
		PathRemoveFileSpec( this->BinPath );
	}

	virtual void After()
	{
		if ( Host )
		{
			Host->Release();
		}
	}

	void SearchMissingFile()
	{
		ULONG FlagSets[] = { 0, CFIXCTL_SEARCH_FLAG_RECURSIVE };

		for ( ULONG Index = 0; Index < _countof( FlagSets ); Index++ )
		{
			SearchCallback Cb;

			WCHAR PathBuffer[ MAX_PATH ];
			CFIXCC_ASSERT( PathCombine( 
				PathBuffer, 
				this->BinPath,
				TESTDATA_FOLDER L"\\amd64\\__.sys" ) );

			BSTR Path = SysAllocString( PathBuffer );
			CFIXCC_ASSERT_EQUALS( E_FNF, this->Host->SearchModules(
				Path,
				FlagSets[ Index ],
				( ULONG ) -1,
				( ULONG ) -1,
				&Cb ) );
			SysFreeString( Path );

			CFIXCC_ASSERT( PathCombine( 
				PathBuffer, 
				this->BinPath,
				TESTDATA_FOLDER L"\\amd64__" ) );
			Path = SysAllocString( PathBuffer  );
			CFIXCC_ASSERT_EQUALS( E_FNF, this->Host->SearchModules(
				Path,
				FlagSets[ Index ],
				( ULONG ) -1,
				( ULONG ) -1,
				&Cb ) );
			SysFreeString( Path );

			CFIXCC_ASSERT_EQUALS( 0UL, Cb.Amd64Files );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.I386Files );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.UserFiles );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.KernelFiles );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.DirEntries );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.DirLeaves );
		}
	}

	void SearchFile()
	{
		ULONG FlagSets[] = { 0, CFIXCTL_SEARCH_FLAG_RECURSIVE };

		for ( ULONG Index = 0; Index < _countof( FlagSets ); Index++ )
		{
			SearchCallback Cb;

			WCHAR PathBuffer[ MAX_PATH ];
			CFIXCC_ASSERT( PathCombine( 
				PathBuffer, 
				this->BinPath,
				TESTDATA_FOLDER L"\\amd64\\testklib5.sys" ) );
			
			BSTR Path = SysAllocString( PathBuffer );
			CFIXCC_ASSERT_EQUALS( E_INVALIDARG, this->Host->SearchModules(
				Path,
				FlagSets[ Index ],
				( ULONG ) -1,
				0,
				&Cb ) );
			CFIXCC_ASSERT_EQUALS( E_INVALIDARG, this->Host->SearchModules(
				Path,
				FlagSets[ Index ],
				( ULONG ) -1,
				32,
				&Cb ) );
			CFIXCC_ASSERT_OK( this->Host->SearchModules(
				Path,
				FlagSets[ Index ],
				( ULONG ) -1,
				CfixTestModuleArchI386 | CfixTestModuleArchAmd64,
				&Cb ) );
			SysFreeString( Path );

			CFIXCC_ASSERT_EQUALS( 1UL, Cb.Amd64Files );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.I386Files );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.UserFiles );
			CFIXCC_ASSERT_EQUALS( 1UL, Cb.KernelFiles );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.DirEntries );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.DirLeaves );
		}
	}

	void SearchFileWithNoFixtures()
	{
		ULONG FlagSets[] = { 0, CFIXCTL_SEARCH_FLAG_RECURSIVE };

		for ( ULONG Index = 0; Index < _countof( FlagSets ); Index++ )
		{
			SearchCallback Cb;

			WCHAR PathBuffer[ MAX_PATH ];
			CFIXCC_ASSERT( PathCombine( 
				PathBuffer, 
				this->BinPath,
				TESTDATA_FOLDER L"\\amd64\\testklib0.sys" ) );
			
			BSTR Path = SysAllocString( PathBuffer );
			CFIXCC_ASSERT_OK( this->Host->SearchModules(
				Path,
				FlagSets[ Index ],
				( ULONG ) -1,
				( ULONG ) -1,
				&Cb ) );
			SysFreeString( Path );

			CFIXCC_ASSERT_EQUALS( 0UL, Cb.Amd64Files );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.I386Files );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.UserFiles );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.KernelFiles );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.DirEntries );
			CFIXCC_ASSERT_EQUALS( 0UL, Cb.DirLeaves );
		}
	}

	void SearchDirNonRecursive()
	{
		SearchCallback Cb;

		WCHAR PathBuffer[ MAX_PATH ];
		CFIXCC_ASSERT( PathCombine( 
			PathBuffer, 
			this->BinPath,
			TESTDATA_FOLDER L"\\i386" ) );
		
		BSTR Path = SysAllocString( PathBuffer );
		CFIXCC_ASSERT_OK( this->Host->SearchModules(
			Path,
			0,
			( ULONG ) -1,
			( ULONG ) -1,
			&Cb ) );
		SysFreeString( Path );

		CFIXCC_ASSERT_EQUALS( 0UL, Cb.Amd64Files );
		CFIXCC_ASSERT_EQUALS( 2UL, Cb.I386Files );
		CFIXCC_ASSERT_EQUALS( 2UL, Cb.UserFiles );
		CFIXCC_ASSERT_EQUALS( 0UL, Cb.KernelFiles );
		CFIXCC_ASSERT_EQUALS( 1UL, Cb.DirEntries );
		CFIXCC_ASSERT_EQUALS( 1UL, Cb.DirLeaves );
	}

	void SearchDirRecursive()
	{
		SearchCallback Cb;

		WCHAR PathBuffer[ MAX_PATH ];
		CFIXCC_ASSERT( PathCombine( 
			PathBuffer, 
			this->BinPath,
			TESTDATA_FOLDER ) );
		
		BSTR Path = SysAllocString( PathBuffer );
		CFIXCC_ASSERT_OK( this->Host->SearchModules(
			Path,
			CFIXCTL_SEARCH_FLAG_RECURSIVE,
			( ULONG ) -1,
			( ULONG ) -1,
			&Cb ) );
		SysFreeString( Path );

		CFIXCC_ASSERT_EQUALS( 1UL, Cb.Amd64Files );
		CFIXCC_ASSERT_EQUALS( 2UL, Cb.I386Files );
		CFIXCC_ASSERT_EQUALS( 2UL, Cb.UserFiles );
		CFIXCC_ASSERT_EQUALS( 1UL, Cb.KernelFiles );
		CFIXCC_ASSERT_EQUALS( Cb.DirEntries, Cb.DirLeaves );
	}

	void SearchDirRecursiveStop()
	{
		SearchCallback Cb( 1 );

		WCHAR PathBuffer[ MAX_PATH ];
		CFIXCC_ASSERT( PathCombine( 
			PathBuffer, 
			this->BinPath,
			TESTDATA_FOLDER ) );
		
		BSTR Path = SysAllocString( PathBuffer );
		CFIXCC_ASSERT_EQUALS( E_FAIL, this->Host->SearchModules(
			Path,
			CFIXCTL_SEARCH_FLAG_RECURSIVE,
			( ULONG ) -1,
			( ULONG ) -1,
			&Cb ) );
		SysFreeString( Path );

		CFIXCC_ASSERT_EQUALS( 1UL, Cb.Amd64Files + Cb.I386Files );
		CFIXCC_ASSERT_EQUALS( Cb.DirEntries, Cb.DirLeaves );
	}
};

COM_EXPORTS TestSearch::Exports;

CFIXCC_BEGIN_CLASS( TestSearch )
	CFIXCC_METHOD( SearchMissingFile )
	CFIXCC_METHOD( SearchFile )
	CFIXCC_METHOD( SearchFileWithNoFixtures )
	CFIXCC_METHOD( SearchDirNonRecursive )
	CFIXCC_METHOD( SearchDirRecursive )
	CFIXCC_METHOD( SearchDirRecursiveStop )
CFIXCC_END_CLASS()