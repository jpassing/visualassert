/*----------------------------------------------------------------------
 * Purpose:
 *		Licensing and trial age-related stuff.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <windows.h>
#include <shlwapi.h>
#include "cfixctlp.h"

static ULONG CfixctlsDaysFromFileTime( 
	__in FILETIME Ft 
	)
{
	ULARGE_INTEGER Int;
	Int.HighPart = Ft.dwHighDateTime;
	Int.LowPart = Ft.dwLowDateTime;

	return ( ULONG ) Int.QuadPart 
		/ 10	// us
		/ 1000	// ms
		/ 1000	// s
		/ 60	// m
		/ 60	// h
		/ 24;	// d
}

static VOID CfixctlsGetBaseDate(
	__out FILETIME *BaseDate
	)
{
	SYSTEMTIME Time;
	Time.wYear = 2007;
	Time.wMonth = 1;
	Time.wDayOfWeek = 0;
	Time.wHour = 0;
	Time.wMinute = 0;
	Time.wSecond = 0;
	Time.wMilliseconds = 0;

	( VOID ) SystemTimeToFileTime( &Time, BaseDate );
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
		CFIXCTLP_LICNESE_REG_KEYPATH,
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
		L"State", // Be unspecific
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
	else if ( Result == ERROR_PATH_NOT_FOUND )
	{
		//
		// No date in registry -- create.
		//
		FILETIME Now;
		Hr = CoFileTimeNow( &Now );
		if ( SUCCEEDED( Hr ) )
		{
			FILETIME Base;
			CfixctlsGetBaseDate( &Base );
			Value = CfixctlsDaysBetween( Base, Now );
	
			DWORD ValueXored = Value  ^ 'CFIX';

			Result = RegSetValueEx(
				Key,
				L"State",
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

	RegCloseKey( Key );
	return Hr;
}

HRESULT CfixctlpGetTrialInstallationAge(
	__in ULONG ExternalDate,
	__out ULONG *DaysInstalled
	)
{
	if ( ExternalDate == 0 || ! DaysInstalled )
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
	Hr = CoFileTimeNow( &Now );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	ULONG NowDaysSince = CfixctlsDaysFromFileTime( Now );

#ifdef CFIXCTL_LIC_HARD_EXPIRY_DATE
	if ( NowDaysSince > CFIXCTL_HARD_EXPIRY_DATE )
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
				CFIXCTL_HARD_EXPIRY_DATE - NowDaysSince, 
				CFIXCTL_LIC_TRIAL_PERIOD - DaysInstalled );
#else
			*DaysLeft	= CFIXCTL_LIC_TRIAL_PERIOD - DaysInstalled;
#endif
		}
	}
	
	return S_OK;
}

