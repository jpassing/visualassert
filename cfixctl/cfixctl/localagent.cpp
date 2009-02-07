/*----------------------------------------------------------------------
 * Purpose:
 *		Agent.
 *
 * Copyright:
 *		2008, Johannes Passing (passing at users.sourceforge.net)
 *
 * This file is part of cfix.
 *
 * cfix is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * cfix is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with cfix.  If not, see <http://www.gnu.org/licenses/>.
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

	STDMETHOD( WaitForHostConnectionAndProcess )(
		__in DWORD Cookie,
		__in ULONG Timeout,
		__in_opt HANDLE ProcessHandle,
		__out ICfixHost** Host
		);

protected:
	LocalAgent();

public:
	virtual ~LocalAgent();

	STDMETHOD_( void, ClearRegistrations )();

	STDMETHOD( CreateProcessHost )(
		__in CfixTestModuleArch Arch,
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
		__in BSTR CurrentDirectory,
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

static HRESULT CfixctlsSpawnHost(
	__in CfixTestModuleArch Arch,
	__in ICfixAgent *Agent,
	__in_opt PCWSTR CurrentDirectory,
	__in BOOL Suspend,
	__out PPROCESS_INFORMATION ProcessInfo
	)
{
	if ( ! Agent || ! ProcessInfo )
	{
		return E_INVALIDARG;
	}

	//
	// Find image to load.
	//
	WCHAR HostPath[ MAX_PATH ];
	HRESULT Hr = CfixctlsFindHostImage(
		Arch, _countof( HostPath ), HostPath );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	LPOLESTR AgentMkDisplayName = NULL;
	Hr = CfixctlsGetObjrefMonikerString( Agent, &AgentMkDisplayName );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	//
	// Prepare command line.
	//
	SIZE_T CommandLineCch = wcslen( AgentMkDisplayName ) + 32;
	PWSTR CommandLine = new WCHAR[ CommandLineCch ];
	if ( CommandLine == NULL )
	{
		goto Cleanup;
	}

	Hr = StringCchPrintf(
		CommandLine,
		CommandLineCch,
		L"cfixhost %s",
		AgentMkDisplayName );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Spawn.
	//
	STARTUPINFO StartupInfo;
	ZeroMemory( &StartupInfo, sizeof( STARTUPINFO ) );
	StartupInfo.cb = sizeof( STARTUPINFO );

	if ( ! CreateProcess(
		HostPath,
		CommandLine,
		NULL,
		NULL,
		FALSE,
		Suspend ? CREATE_SUSPENDED : 0,
		NULL,
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

	if ( CommandLine != NULL )
	{
		delete [] CommandLine;
	}

	return Hr;
}

static HRESULT CfixctlsSpawnHostAndPutInJobIfRequired(
	__in CfixTestModuleArch Arch,
	__in ICfixAgent *Agent,
	__in_opt PCWSTR CurrentDirectory,
	__in BOOL PutInJob,
	__out HANDLE *ProcessOrJob,
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
		Arch, 
		Agent, 
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
		BOOL InJob;
		if ( ! CfixctlpIsProcessInJob( ProcessInfo.hProcess, NULL, &InJob ) )
		{
			InJob = TRUE;
		}

		PutInJob = PutInJob && ! InJob;
	}

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

LocalAgent::LocalAgent() : Registration( NULL ), NewRegistrationEvent( NULL )
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

	if ( ! CfixcrlpIsValidArch( Arch ) )
	{
		return E_INVALIDARG;
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
		Arch, 
		this, 
		CurrentDirectory, 
		UseJob,
		&ProcessOrJob,
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
 * ICfixHost methods.
 */

	
STDMETHODIMP LocalAgent::CreateHost( 
	__in CfixTestModuleArch Arch,
	__in DWORD Clsctx,
	__in ULONG Flags,
	__in ULONG Timeout,
	__in BSTR CurrentDirectory,
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
		HRESULT Hr = CfixctlpGetLocalHostFactory().CreateInstance(
			NULL, IID_ICfixHost, ( PVOID* ) Host );

		if ( SUCCEEDED( Hr ) && 
			 CurrentDirectory != NULL &&
			 SysStringByteLen( CurrentDirectory ) > 0 )
		{
			if ( ! SetCurrentDirectory( CurrentDirectory ) )
			{
				( *Host )->Release();
				*Host = NULL;
				Hr = HRESULT_FROM_WIN32( GetLastError() );
			}
		}

		return Hr;
	}
	else if ( Clsctx & CLSCTX_LOCAL_SERVER )
	{
		// 
		// Spawn process.
		//
		return CreateProcessHost(
			Arch,
			CurrentDirectory,
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

	return Hr;
}

STDMETHODIMP LocalAgent::WaitForHostConnectionAndProcess(
	__in DWORD Cookie,
	__in ULONG Timeout,
	__in_opt HANDLE ProcessHandle,
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
						return ( HRESULT ) ExitCode;
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