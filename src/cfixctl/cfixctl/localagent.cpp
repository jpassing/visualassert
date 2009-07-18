/*----------------------------------------------------------------------
 * Purpose:
 *		Agent.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include "cfixctlp.h"
#include <shlwapi.h>

/*------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 */
struct RegistrationEntry
{
	DWORD Cookie;
	ICfixHost *Host;

	RegistrationEntry( 
		__in DWORD Cookie,
		__in ICfixHost *Host
		) 
		: Cookie ( Cookie )
		, Host( Host )
	{
		this->Host->AddRef();
	}

	~RegistrationEntry()
	{
		this->Host->Release();
	}
};

class LocalAgent : 
	public ComObjectBase, 
	public ICfixAgent
{
	DECLARE_NOT_COPYABLE( LocalAgent );

private:
	//
	// N.B. We currently allow one pending registration only.
	//
	CRITICAL_SECTION RegistrationLock;
	CRITICAL_SECTION SpawnLock;
	RegistrationEntry *Registration;
	HANDLE NewRegistrationEvent;

	volatile ULONG LicenseCookie;

	STDMETHOD( WaitForHostConnectionAndProcess )(
		__in DWORD Cookie,
		__in ULONG Timeout,
		__in_opt HANDLE ProcessHandle,
		__in BOOL IsCustomHost,
		__out ICfixHost** Host
		);

protected:
	LocalAgent();

public:
	virtual ~LocalAgent();

	STDMETHOD_( void, ClearRegistrations )();

	STDMETHOD( CreateProcessHost )(
		__in CfixTestModuleArch Arch,
		__in_opt PCWSTR CustomHostPath,
		__in_opt PCWSTR Environment,
		__in_opt PCWSTR CurrentDirectory,
		__in ULONG Flags,
		__in ULONG Timeout,
		__out ICfixHost **Result
		);

	STDMETHOD( FinalConstruct )();

	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )() PURE;
	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr );

	/*------------------------------------------------------------------
	 * ICfixHost methods.
	 */
	STDMETHOD( GetHostPath )(
		__in CfixTestModuleArch Arch,
		__out BSTR *Path
		);
	
	STDMETHOD( CreateHost )( 
		__in CfixTestModuleArch Arch,
		__in DWORD Clsctx,
		__in ULONG Flags,
		__in ULONG Timeout,
		__in const BSTR HostPath,
		__in const BSTR Environment,
		__in const BSTR CurrentDirectory,
		__out ICfixHost** Host
		);

	STDMETHOD( RegisterHost )(
		__in DWORD Cookie,
		__in ICfixHost *Host 
		);

	STDMETHOD( WaitForHostConnection )(
		__in DWORD Cookie,
		__in ULONG Timeout,
		__out ICfixHost** Host
		);

	STDMETHOD( CreateMessageResolver )(
		__out ICfixMessageResolver **Resolver
		);

	STDMETHOD( SetTrialLicenseCookie )(
		__in ULONG Cookie
		);
};

/*------------------------------------------------------------------
 * 
 * Factory.
 *
 */

IClassFactory& CfixctlpGetLocalAgentFactory()
{
	static ComClassFactory< ComMtaObject< LocalAgent >, CfixctlServerLock > Factory;
	return Factory;
}

/*------------------------------------------------------------------
 * 
 * Helpers.
 *
 */

static HRESULT CfixctlsCheckLicense(
	__in ULONG Cookie
	)
{
	CFIXCTL_LICENSE_INFO Info;
	Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
	HRESULT Hr = CfixctlpQueryLicenseInfo(
		TRUE,
		Cookie,
		&Info );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}
	
	if ( Info.Valid )
	{
		return S_OK;
	}
	else
	{
		if ( Info.Type == CfixctlTrial )
		{
			return CFIXCTL_E_LIC_TRIAL_EXPIRED;
		}
		else
		{
			return CFIXCTL_E_LIC_INVALID;
		}
	}
}

static HRESULT CfixctlsFindHostImage(
	__in CfixTestModuleArch Arch,
	__in SIZE_T PathCch,
	__out_ecount( PathCch ) PWSTR Path
	)
{
	//
	// Starting point is the directory this module was loaded from.
	//
	WCHAR OwnModulePath[ MAX_PATH ];

	if ( ! CfixcrlpIsValidArch( Arch ) )
	{
		return E_INVALIDARG;
	}

	if ( 0 == GetModuleFileName(
		CfixctlpGetModule(),
		OwnModulePath,
		_countof( OwnModulePath ) ) )
	{
		return HRESULT_FROM_WIN32( GetLastError() );
	}

	if ( ! PathRemoveFileSpec( OwnModulePath ) )
	{
		return CFIXCTL_E_HOST_IMAGE_NOT_FOUND;
	}

	//
	// Get system information to decide which module (32/64) we need
	// to load - due to WOW64 the driver image bitness may not be the
	// same as the bitness of this module.
	//
	PWSTR HostImageName;
	PWSTR HostImageNameWithDirectory;

	switch ( Arch )
	{
	case CfixTestModuleArchAmd64:
		HostImageName = L"cfixhs64.exe";
		HostImageNameWithDirectory = L"..\\amd64\\cfixhs64.exe";
		break;

	case CfixTestModuleArchI386:
		HostImageName = L"cfixhs32.exe";
		HostImageNameWithDirectory = L"..\\i386\\cfixhs32.exe";
		break;

	default:
		return E_INVALIDARG;
	}

	WCHAR HostModulePath[ MAX_PATH ];

	//
	// Try .\cfixhsXX.exe
	//
	if ( ! PathCombine( HostModulePath, OwnModulePath, HostImageName ) )
	{
		return CFIXCTL_E_HOST_IMAGE_NOT_FOUND;
	}

	if ( INVALID_FILE_ATTRIBUTES != GetFileAttributes( HostModulePath ) )
	{
		return StringCchCopy(
			Path,
			PathCch,
			HostModulePath );
	}

	//
	// Try ..\<arch>\cfixhsXX.exe.
	//
	if ( ! PathCombine( HostModulePath, OwnModulePath, HostImageNameWithDirectory ) )
	{
		return CFIXCTL_E_HOST_IMAGE_NOT_FOUND;
	}

	if ( INVALID_FILE_ATTRIBUTES != GetFileAttributes( HostModulePath ) )
	{
		return StringCchCopy(
			Path,
			PathCch,
			HostModulePath );
	}

	return CFIXCTL_E_HOST_IMAGE_NOT_FOUND;
}

static HRESULT CfixctlsGetObjrefMonikerString(
	__in ICfixAgent *Agent,
	__out LPOLESTR *DisplayName
	)
{
	IMoniker *AgentMk = NULL;
	IBindCtx *BindCtx = NULL;

	HRESULT Hr = CreateObjrefMoniker( Agent, &AgentMk );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	Hr = CreateBindCtx( 0, &BindCtx );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}
	
	Hr = AgentMk->GetDisplayName( BindCtx, NULL, DisplayName );

Cleanup:
	if ( AgentMk != NULL )
	{
		AgentMk->Release();
	}

	if ( BindCtx != NULL )
	{
		BindCtx->Release();
	}

	return Hr;
}

#define CFIXCTLP_EMB_INIT_ENVVAR \
		CFIX_EMB_INIT_ENVVAR_NAME L"=cfixctl.dll!CfixctlServeHost"

static HRESULT CfixctlsSpawnHost(
	__in ICfixAgent *Agent,
	__in CfixTestModuleArch Arch,
	__in_opt PCWSTR CustomHostPath,
	__in_opt PCWSTR CustomEnvironment,
	__in_opt PCWSTR CurrentDirectory,
	__in BOOL Suspend,
	__out PPROCESS_INFORMATION ProcessInfo
	)
{
	if ( ! Agent || ! ProcessInfo )
	{
		return E_INVALIDARG;
	}

	LPOLESTR AgentMkDisplayName = NULL;
	HRESULT Hr = CfixctlsGetObjrefMonikerString( Agent, &AgentMkDisplayName );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	//
	// See which host image we have to use.
	//
	WCHAR HostPathBuffer[ MAX_PATH ];
	if ( CustomHostPath != NULL )
	{
		//
		// Custom host image.
		//
		if ( INVALID_FILE_ATTRIBUTES == GetFileAttributes( CustomHostPath ) )
		{
			return CFIXCTL_E_HOST_IMAGE_NOT_FOUND;
		}

		//
		// Copy to make string non-const.
		//
		Hr = StringCchCopy(
			HostPathBuffer,
			_countof( HostPathBuffer ),
			CustomHostPath );
	}
	else
	{
		//
		// Use default host image.
		//
		Hr = CfixctlsFindHostImage(
			Arch, 
			_countof( HostPathBuffer ), 
			HostPathBuffer );
	}

	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	//
	// Prepare environment.
	//
	// The environment consists of the custom environment (if any)
	// and a special variable that contains the moniker string.
	//

	SIZE_T FullEnvironmentCch = 
		//
		// Custom-environment
		//
		( CustomEnvironment == NULL ? 0 : wcslen( CustomEnvironment ) ) +

		//
		// Embedding\n.
		//
		( CustomHostPath == NULL ? 0 : wcslen( CFIXCTLP_EMB_INIT_ENVVAR ) + 1 ) +

		//
		// name=value\n
		//
		wcslen( CFIXCTLP_MONIKER_ENVVAR_NAME ) + 1 +
		wcslen( AgentMkDisplayName ) + 1 +
		
		//
		// Terminator.
		//
		1;

	PWSTR FullEnvironment = new WCHAR[ FullEnvironmentCch ];
	if ( FullEnvironment == NULL )
	{
		return E_OUTOFMEMORY;
	}

	ASSERT( 
		CustomEnvironment == NULL || 
		wcslen( CustomEnvironment ) == 0 ||
		CustomEnvironment[ wcslen( CustomEnvironment ) - 1 ] == L'\n' );

	Hr = StringCchPrintf(
		FullEnvironment,
		FullEnvironmentCch,
		L"%s%s" CFIXCTLP_MONIKER_ENVVAR_NAME L"=%s\n",
		CustomEnvironment ? CustomEnvironment : L"",
		CustomHostPath ? CFIXCTLP_EMB_INIT_ENVVAR L"\n" : L"",
		AgentMkDisplayName
		);
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	//
	// Replace all \n by \0 so that the buffer becomes a valid
	// environment string.
	//

	PWSTR Newline;
	while ( ( Newline = wcsrchr( FullEnvironment, L'\n' ) ) != NULL )
	{
		*Newline = UNICODE_NULL;
	}

	ASSERT( FullEnvironment[ FullEnvironmentCch - 2 ] == UNICODE_NULL );
	ASSERT( FullEnvironment[ FullEnvironmentCch - 1 ] == UNICODE_NULL );

	//
	// Spawn.
	//
	STARTUPINFO StartupInfo;
	ZeroMemory( &StartupInfo, sizeof( STARTUPINFO ) );
	StartupInfo.cb = sizeof( STARTUPINFO );

	if ( ! CreateProcess(
		HostPathBuffer,
		NULL,
		NULL,
		NULL,
		FALSE,
		CREATE_NO_WINDOW 
			| CREATE_UNICODE_ENVIRONMENT 
			| ( Suspend ? CREATE_SUSPENDED : 0 ),
		( PVOID ) FullEnvironment,
		CurrentDirectory,
		&StartupInfo,
		ProcessInfo ) )
	{
		Hr = HRESULT_FROM_WIN32( GetLastError() );
		goto Cleanup;
	}

	Hr = S_OK;

Cleanup:
	if ( AgentMkDisplayName != NULL )
	{
		CoTaskMemFree( AgentMkDisplayName );
	}

	if ( FullEnvironment != NULL )
	{
		delete [] FullEnvironment;
	}

	return Hr;
}

static HRESULT CfixctlsSpawnHostAndPutInJobIfRequired(
	__in ICfixAgent *Agent,
	__in CfixTestModuleArch Arch,
	__in_opt PCWSTR CustomHostPath,
	__in_opt PCWSTR Environment,
	__in_opt PCWSTR CurrentDirectory,
	__in BOOL PutInJob,
	__out HANDLE *ProcessOrJob,
	__out PBOOL InJob,
	__out DWORD *Cookie
	)
{
	ASSERT( Agent );
	ASSERT( ProcessOrJob );

	//
	// Spawn the process.
	//
	PROCESS_INFORMATION ProcessInfo;
	ZeroMemory( &ProcessInfo, sizeof( PROCESS_INFORMATION ) );

	BOOL SuspendInitialThread = PutInJob;

	HRESULT Hr = CfixctlsSpawnHost( 
		Agent,
		Arch,
		CustomHostPath,
		Environment,
		CurrentDirectory, 
		SuspendInitialThread,
		&ProcessInfo );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	ASSERT( ProcessInfo.hProcess );
	ASSERT( ProcessInfo.hThread );

	//
	// N.B. The process ID is the cookie.
	//
	*Cookie = ProcessInfo.dwProcessId;

	//
	// Reconsider decision of assigning the process to a new job based on
	// whether it already belongs to some otehr job.
	//
	if ( PutInJob )
	{
		BOOL AlreadyInJob;
		if ( ! CfixctlpIsProcessInJob( ProcessInfo.hProcess, NULL, &AlreadyInJob ) )
		{
			AlreadyInJob = TRUE;
		}

		PutInJob = PutInJob && ! AlreadyInJob;
	}

	*InJob = PutInJob;

	if ( ! PutInJob )
	{
		*ProcessOrJob = ProcessInfo.hProcess;

		Hr = S_OK;
	}
	else
	{
		HANDLE Job = CreateJobObject( NULL, NULL );
		if ( Job )
		{
			if ( AssignProcessToJobObject( Job, ProcessInfo.hProcess ) )
			{
				*ProcessOrJob = Job;
				Hr = S_OK;
			}
			else
			{
				Hr = HRESULT_FROM_WIN32( GetLastError() );
				VERIFY( CloseHandle( Job ) );
			}
		}
		else
		{
			Hr = HRESULT_FROM_WIN32( GetLastError() );
		}

		if ( FAILED( Hr ) )
		{
			( VOID ) TerminateProcess( ProcessInfo.hProcess, 0 );
		}

		VERIFY( CloseHandle( ProcessInfo.hProcess ) );
	}
	
	if ( SuspendInitialThread )
	{
		( VOID ) ResumeThread( ProcessInfo.hThread );
	}

	VERIFY( CloseHandle( ProcessInfo.hThread ) );

	return Hr;
}

/*------------------------------------------------------------------
 * 
 * Class Implementation.
 *
 */

LocalAgent::LocalAgent() 
	: Registration( NULL )
	, NewRegistrationEvent( NULL )
	, LicenseCookie( 0 )
{
}

LocalAgent::~LocalAgent()
{
	ClearRegistrations();
	DeleteCriticalSection( &this->RegistrationLock );
	DeleteCriticalSection( &this->SpawnLock );

	if ( this->NewRegistrationEvent != NULL )
	{
		VERIFY( CloseHandle( this->NewRegistrationEvent ) );
	}
}

STDMETHODIMP LocalAgent::FinalConstruct() 
{
	InitializeCriticalSection( &this->RegistrationLock );
	InitializeCriticalSection( &this->SpawnLock );

	this->NewRegistrationEvent = CreateEvent( NULL, FALSE, FALSE, NULL );
	if ( ! this->NewRegistrationEvent )
	{
		return HRESULT_FROM_WIN32( GetLastError() );
	}
	else
	{
		return S_OK;
	}
}

STDMETHODIMP_( void ) LocalAgent::ClearRegistrations()
{
	EnterCriticalSection( &this->RegistrationLock );

	if ( this->Registration != NULL )
	{
		delete this->Registration;
		this->Registration = NULL;
	}

	LeaveCriticalSection( &this->RegistrationLock );
}


/*------------------------------------------------------------------
 * 
 * ICfixAgent Implementation.
 *
 */

STDMETHODIMP LocalAgent::GetHostPath(
	__in CfixTestModuleArch Arch,
	__out BSTR *Path
	)
{
	if ( ! Path )
	{
		return E_INVALIDARG;
	}
	else
	{
		*Path = NULL;
	}

	WCHAR Buffer[ MAX_PATH ];
	HRESULT Hr = CfixctlsFindHostImage(
		Arch,
		_countof( Buffer ),
		Buffer );
	if ( SUCCEEDED( Hr ) )
	{
		*Path = SysAllocString( Buffer );
		if ( *Path == NULL )
		{
			return E_OUTOFMEMORY;
		}
	}

	return Hr;
}

STDMETHODIMP LocalAgent::CreateProcessHost(
	__in CfixTestModuleArch Arch,
	__in_opt PCWSTR CustomHostPath,
	__in_opt PCWSTR Environment,
	__in_opt PCWSTR CurrentDirectory,
	__in ULONG Flags,
	__in ULONG Timeout,
	__out ICfixHost **Result
	)
{
	if ( ! Result )
	{
		return E_POINTER;
	}
	else
	{
		*Result = NULL;
	}

	BOOL UseJob = ( Flags & CFIXCTL_AGENT_FLAG_USE_JOB );

	//
	// Spawn the process.
	//
	HANDLE ProcessOrJob = NULL;
	DWORD Cookie;

	ICfixHost *RemoteHost = NULL;
	ICfixProcessHostInternal *ProcessHost = NULL;

	//
	// Guard this by a lock to prevent multiple interleaving
	// spawns which are currently ot supported.
	//
	EnterCriticalSection( &this->SpawnLock );

	HRESULT Hr = CfixctlsSpawnHostAndPutInJobIfRequired( 
		this,
		Arch,
		CustomHostPath,
		Environment,
		CurrentDirectory, 
		UseJob,
		&ProcessOrJob,
		&UseJob,			// Just because we want a job, we may still not get one.
		&Cookie );
	if ( SUCCEEDED( Hr ) )
	{
		//
		// Wait for it to register and obtain its Host object.
		//
		Hr = WaitForHostConnectionAndProcess(
			Cookie,
			Timeout,
			ProcessOrJob,
			CustomHostPath != NULL,
			&RemoteHost );
	}

	LeaveCriticalSection( &this->SpawnLock );

	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Wrap by a ProcessHost object. Ownership of the process handle
	// is transfered to this object.
	//
	Hr = CfixctlpGetProcessHostFactory().CreateInstance( 
		NULL, IID_ICfixProcessHostInternal, ( PVOID* ) &ProcessHost );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	Hr = ProcessHost->Initialize( 
		RemoteHost, 
		ProcessOrJob,
		UseJob );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	ProcessHost->AddRef();
	*Result = ProcessHost;
	Hr = S_OK;

Cleanup:
	if ( RemoteHost )
	{
		RemoteHost->Release();
	}

	if  ( ProcessHost )
	{
		ProcessHost->Release();
	}

	if ( FAILED( Hr ) )
	{
		if ( ProcessOrJob )
		{
			VERIFY( CloseHandle( ProcessOrJob ) );
		}
	}

	return Hr;
}

/*------------------------------------------------------------------
 * IUnknown methods.
 */

STDMETHODIMP LocalAgent::QueryInterface( 
	__in REFIID Iid, 
	__out PVOID* Ptr )
{
	HRESULT Hr;

	if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
		 InlineIsEqualGUID( Iid, IID_ICfixAgent ) )
	{
		*Ptr = static_cast< ICfixAgent* >( this );
		Hr = S_OK;

	}
	else
	{
		*Ptr = NULL;
		Hr = E_NOINTERFACE;
	}

	if ( SUCCEEDED( Hr ) )
	{
		this->AddRef();
	}

	return Hr;
}

/*------------------------------------------------------------------
 * ICfixAgent methods.
 */

	
STDMETHODIMP LocalAgent::CreateHost( 
	__in CfixTestModuleArch Arch,
	__in DWORD Clsctx,
	__in ULONG Flags,
	__in ULONG Timeout,
	__in const BSTR CustomHostPath,
	__in const BSTR Environment,
	__in const BSTR CurrentDirectory,
	__out ICfixHost** Host
	)
{
	if ( ! Host )
	{
		return E_POINTER;
	}
	else
	{
		*Host = NULL;
	}

	if ( Arch > CfixTestModuleArchMax ||
		( ( Clsctx & ~( CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER ) ) != 0 ) )
	{
		return E_INVALIDARG;
	}

	if ( ( Clsctx & CLSCTX_INPROC_SERVER ) && Arch == CFIXCTL_OWN_ARCHITECTURE )
	{
		if ( CustomHostPath != NULL || Environment != NULL )
		{
			return E_INVALIDARG;
		}

		return CfixctlpGetLocalHostFactory().CreateInstance(
			NULL, IID_ICfixHost, ( PVOID* ) Host );
	}
	else if ( Clsctx & CLSCTX_LOCAL_SERVER )
	{
		//
		// Check license (this method is a choke point).
		//
		// N.B. It is important to only check the license here --
		// otherwise, the host process would check the license
		// again and, due to a missing cookie, may fail.
		//
		HRESULT Hr = CfixctlsCheckLicense( this->LicenseCookie );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}
		
		// 
		// Spawn process.
		//
		return CreateProcessHost(
			Arch,
			CustomHostPath != NULL && SysStringByteLen( CustomHostPath ) > 0
				? CustomHostPath
				: NULL,
			Environment != NULL && SysStringByteLen( Environment ) > 0
				? Environment
				: NULL,
			CurrentDirectory != NULL && SysStringByteLen( CurrentDirectory ) > 0
				? CurrentDirectory
				: NULL,
			Flags,
			Timeout,
			Host );
	}
	else
	{
		return E_INVALIDARG;
	}
}

STDMETHODIMP LocalAgent::RegisterHost(
	__in DWORD Cookie,
	__in ICfixHost *Host 
	)
{
	if ( Cookie == 0 || Host == NULL )
	{
		return E_INVALIDARG;
	}

	if ( this->NewRegistrationEvent == NULL )
	{
		return E_UNEXPECTED;
	}

	HRESULT Hr;
	EnterCriticalSection( &this->RegistrationLock );

	if ( this->Registration != NULL )
	{
		Hr = E_UNEXPECTED;
	}
	else
	{
		this->Registration = new RegistrationEntry( Cookie, Host );
		if ( this->Registration == NULL )
		{
			Hr = E_OUTOFMEMORY;
		}
		else
		{
			VERIFY( SetEvent( this->NewRegistrationEvent ) );
			Hr = S_OK;
		}
	}

	LeaveCriticalSection( &this->RegistrationLock );

	CFIXCTLP_TRACE( ( L"LocalAgent: Registered %d: %x", Cookie, Hr ) );

	return Hr;
}

STDMETHODIMP LocalAgent::WaitForHostConnectionAndProcess(
	__in DWORD Cookie,
	__in ULONG Timeout,
	__in_opt HANDLE ProcessHandle,
	__in BOOL IsCustomHost,
	__out ICfixHost** Host
	)
{
	if ( Host == NULL )
	{
		return E_POINTER;
	}
	else
	{
		*Host = NULL;
	}

	if ( Cookie == 0  )
	{
		return E_INVALIDARG;
	}

	CFIXCTLP_TRACE( ( L"LocalAgent: Waiting for %d", Cookie ) );

	HRESULT Hr = E_FAIL;
	BOOL KeepSpinning = TRUE;
	do
	{
		EnterCriticalSection( &this->RegistrationLock );
	
		if ( this->Registration != NULL )
		{
			if ( this->Registration->Cookie == Cookie )
			{
				*Host = Registration->Host;
				( *Host )->AddRef();

				delete this->Registration;
				this->Registration = NULL;
				Hr = S_OK;
			}
			else
			{
				Hr = CFIXCTL_E_HOST_NOT_FOUND;
			}

			KeepSpinning = FALSE;
		}

		LeaveCriticalSection( &this->RegistrationLock );

		if ( KeepSpinning && *Host == NULL )
		{
			if ( Timeout == 0 )
			{
				Hr = CFIXCTL_E_HOST_NOT_FOUND;
				break;
			}
			else
			{
				//
				// Wait for a new registration.
				//
				DWORD Index;
				HANDLE WaitObjects[] = { 
					this->NewRegistrationEvent, ProcessHandle };
				Hr = CoWaitForMultipleHandles(
					0,	// Wait any.
					Timeout,
					ProcessHandle == NULL ? 1 : 2,
					WaitObjects, 
					&Index );
				if ( Hr == RPC_S_CALLPENDING  )
				{
					return HRESULT_FROM_WIN32( ERROR_TIMEOUT );
				}
				else if ( FAILED( Hr ) )
				{
					return Hr;
				}
				else if ( Index == 0 )
				{
					//
					// New regsitration - Search again.
					//
				}
				else if ( Index == 1 )
				{
					ASSERT( ProcessHandle );

					//
					// Process has died. The exit code should be the
					// failure HRESULT.
					//
					DWORD ExitCode;
					if ( ! GetExitCodeProcess( ProcessHandle, &ExitCode ) )
					{
						return CFIXCTL_E_HOST_DIED_PREMATURELY;
					}
					else
					{
						HRESULT ExitHr = ( HRESULT ) ExitCode;
						
						if ( IsCustomHost )
						{
							return CFIXCTL_E_CUSTOM_HOST_EXITED_PREMATURELY;
						}
						else if ( FAILED( ExitHr ) )
						{
							return Hr;
						}
						else
						{
							return CFIXCTL_E_HOST_EXITED_PREMATURELY;
						}
					}
				}
				else
				{
					ASSERT( !"Unexpected wait result" );
				}
			}
		}
	}
	while ( KeepSpinning );

	ASSERT( SUCCEEDED( Hr ) == ( *Host != NULL ) );
	return Hr;
}

STDMETHODIMP LocalAgent::WaitForHostConnection(
	__in DWORD Cookie,
	__in ULONG Timeout,
	__out ICfixHost** Host
	)
{
	return WaitForHostConnectionAndProcess(
		Cookie,
		Timeout,
		NULL,
		FALSE, // Do not know - assume.
		Host );
}


STDMETHODIMP LocalAgent::CreateMessageResolver(
	__out ICfixMessageResolver **Resolver
	)
{
	return CfixctlpGetMessageResolverFactory().CreateInstance(
		NULL,
		IID_ICfixMessageResolver,
		( PVOID* ) Resolver );
}

STDMETHODIMP LocalAgent::SetTrialLicenseCookie(
	__in ULONG Cookie
	)
{
	this->LicenseCookie = Cookie;
	return S_OK;
}