/*----------------------------------------------------------------------
 * Purpose:
 *		Licensing and trial age-related stuff.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <windows.h>
#include <shlwapi.h>
#include <cfixctllic.h>
#include "cfixctlp.h"

static ULONG CfixctlsDaysFromFileTime( 
	__in FILETIME Ft 
	)
{
	ULARGE_INTEGER Int;
	Int.HighPart = Ft.dwHighDateTime;
	Int.LowPart = Ft.dwLowDateTime;

	return ( ULONG ) ( Int.QuadPart 
		/ 10	// us
		/ 1000	// ms
		/ 1000	// s
		/ 60	// m
		/ 60	// h
		/ 24 );	// d
}

static ULONG CfixctlsDaysBetween(
	__in FILETIME Early,
	__in FILETIME Late
	)
{
	return CfixctlsDaysFromFileTime( Late ) - CfixctlsDaysFromFileTime( Early );
}

static HRESULT CfixctlsGetBinFolderCreationDate(
	__out ULONG *DaysSince
	)
{
	WCHAR Path[ MAX_PATH ];
	if ( 0 == GetModuleFileName(
		CfixctlpGetModule(),
		Path,
		_countof( Path ) ) )
	{
		return HRESULT_FROM_WIN32( GetLastError() );
	}

	PathRemoveFileSpec( Path );
	ASSERT( GetFileAttributes( Path ) != INVALID_FILE_ATTRIBUTES );

	WIN32_FILE_ATTRIBUTE_DATA Attr;
	if ( ! GetFileAttributesEx(
		Path,
		GetFileExInfoStandard,
		&Attr ) )
	{
		return HRESULT_FROM_WIN32( GetLastError() );
	}

	*DaysSince = CfixctlsDaysFromFileTime( Attr.ftCreationTime );
	return S_OK;
}

static HRESULT CfixctlsLazyCreateAndGetDateFromRegistry(
	__out ULONG *DaysSince
	)
{
	HKEY Key;
	LONG Result = RegCreateKeyEx(
		HKEY_CURRENT_USER,
		CFIXCTL_LICENSE_REG_KEYPATH,
		0,
		NULL,
		0,
		KEY_READ | KEY_WRITE,
		NULL,
		&Key,
		NULL );
	if ( Result != ERROR_SUCCESS )
	{
		return HRESULT_FROM_WIN32( Result );
	}

	DWORD Type;
	DWORD Value;
	DWORD CbRead = sizeof( DWORD );
	Result = RegQueryValueEx(
		Key,
		CFIXCTL_LICENSE_REG_KEY_NAME_LICENSE_DATE,
		0,
		&Type,
		( LPBYTE ) &Value,
		&CbRead );

	HRESULT Hr;
	if ( Result == ERROR_SUCCESS )
	{
		if ( Type != REG_DWORD || CbRead != sizeof( DWORD ) )
		{
			Hr = CFIXCTL_E_LIC_TAMPERED;
		}
		else
		{
			*DaysSince = Value ^ 'CFIX';
			Hr = S_OK;
		}
	}
	else if ( Result == ERROR_FILE_NOT_FOUND ||
		      Result == ERROR_PATH_NOT_FOUND )
	{
		//
		// No date in registry -- create.
		//
		FILETIME Now;
		Hr = CoFileTimeNow( &Now );
		if ( SUCCEEDED( Hr ) )
		{
			Value = CfixctlsDaysFromFileTime( Now );
	
			DWORD ValueXored = Value  ^ 'CFIX';

			Result = RegSetValueEx(
				Key,
				CFIXCTL_LICENSE_REG_KEY_NAME_LICENSE_DATE,
				0,
				REG_DWORD,
				( LPBYTE ) &ValueXored,
				sizeof( DWORD ) );
			if ( Result == ERROR_SUCCESS )
			{
				*DaysSince = Value;
				Hr = S_OK;
			}
			else
			{
				Hr = HRESULT_FROM_WIN32( Result );
			}
		}
	}
	else
	{
		Hr = HRESULT_FROM_WIN32( Result );
	}

	VERIFY( ERROR_SUCCESS == RegCloseKey( Key ) );
	return Hr;
}

HRESULT CfixctlpGetTrialInstallationAge(
	__in ULONG ExternalDate,
	__out ULONG *DaysInstalled
	)
{
	if ( ! DaysInstalled )
	{
		return E_INVALIDARG;
	}

	//
	// N.B. dates are relative to base date.
	//
	ULONG BinFolderDaysSince;
	HRESULT Hr = CfixctlsGetBinFolderCreationDate( &BinFolderDaysSince );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	ULONG RegistryDaysSince;
	Hr = CfixctlsLazyCreateAndGetDateFromRegistry( &RegistryDaysSince );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	FILETIME Now;
	Hr = CoFileTimeNow( &Now );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	ULONG NowDaysSince = CfixctlsDaysFromFileTime( Now );

	if ( NowDaysSince < BinFolderDaysSince ||
		NowDaysSince < RegistryDaysSince ||
		NowDaysSince < ExternalDate )
	{
		return CFIXCTL_E_LIC_TAMPERED;
	}
	
	*DaysInstalled = NowDaysSince -
		min( min( BinFolderDaysSince, RegistryDaysSince ), ExternalDate );
	return S_OK;
}

HRESULT CfixctlpIsTrialPeriodActive(
	__in ULONG DaysInstalled,
	__out BOOL *Active,
	__out ULONG *DaysLeft
	)
{
	if ( ! Active )
	{
		return E_INVALIDARG;
	}

	FILETIME Now;
	HRESULT Hr = CoFileTimeNow( &Now );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

#ifdef CFIXCTL_LIC_HARD_EXPIRY_DATE
	ULONG NowDaysSince = CfixctlsDaysFromFileTime( Now );
	if ( NowDaysSince > CFIXCTL_LIC_HARD_EXPIRY_DATE )
	{

		//
		// Age-wise, the installation may be ok, but the hard expiry
		// date is over.
		//
		*Active		= FALSE;
		*DaysLeft	= 0;
	}
	else
#endif
	{
		*Active = ( DaysInstalled <= CFIXCTL_LIC_TRIAL_PERIOD );
		if ( ! *Active )
		{
			*DaysLeft = 0;
		}
		else
		{
#ifdef CFIXCTL_LIC_HARD_EXPIRY_DATE
			*DaysLeft	= min( 
				CFIXCTL_LIC_HARD_EXPIRY_DATE - NowDaysSince, 
				CFIXCTL_LIC_TRIAL_PERIOD - DaysInstalled );
#else
			*DaysLeft	= CFIXCTL_LIC_TRIAL_PERIOD - DaysInstalled;
#endif
		}
	}
	
	return S_OK;
}

