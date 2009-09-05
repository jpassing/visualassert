/*----------------------------------------------------------------------
 * Purpose:
 *		Test licensing.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */
#include <cfixcc.h>
#include <cfixlic.h>
#include <cfixctlsvr.h>
#include <cfixctllic.h>
#include "testctlp.h"

static ULONG DaysFromFileTime( 
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

ULONG CurrentLicensingDate()
{
	FILETIME Date;
	CFIX_ASSERT_OK( CoFileTimeNow( &Date ) );
	return DaysFromFileTime( Date );
}

class TestLicensing : public cfixcc::TestFixture
{
private:
	
	void ResetBinFolderAge( LONGLONG AddDays )
	{
		FILETIME Date;
		CFIX_ASSERT_OK( CoFileTimeNow( &Date ) );

		ULARGE_INTEGER Int;
		Int.HighPart = Date.dwHighDateTime;
		Int.LowPart = Date.dwLowDateTime;
		
		Int.QuadPart = 
			( LONGLONG ) Int.QuadPart + AddDays * 10 * 1000 * 1000 * 60 * 60 * 24;

		Date.dwHighDateTime = Int.HighPart;
		Date.dwLowDateTime = Int.LowPart;
		
		WCHAR Path[ MAX_PATH ];
		CFIXCC_ASSERT_NOT_EQUALS( 0UL, GetModuleFileName(
			GetModuleHandle( L"cfixctl" ),
			Path,
			_countof( Path ) ) );

		PathRemoveFileSpec( Path );
		ASSERT( GetFileAttributes( Path ) != INVALID_FILE_ATTRIBUTES );
		
		HANDLE Dir = CreateFile( Path, FILE_WRITE_ATTRIBUTES, 0,
			NULL, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, NULL );
		CFIXCC_ASSERT( Dir != INVALID_HANDLE_VALUE );
		CFIXCC_ASSERT( SetFileTime( Dir, &Date, NULL, NULL ) );
		CloseHandle( Dir );
	}

	void InstallLicense( 
		__in PCFIXLIC_LICENSE_KEY LicKey
		)
	{
		HKEY Key;
		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegCreateKeyEx(
			HKEY_CURRENT_USER,
			CFIXCTL_LICENSE_REG_KEYPATH,
			0,
			NULL,
			0,
			KEY_WRITE,
			NULL,
			&Key,
			NULL ) );

		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegSetValueEx(
			Key,
			CFIXCTL_LICENSE_REG_KEY_NAME_LICENSE,
			0,
			REG_BINARY,
			( LPBYTE ) LicKey,
			sizeof( CFIXLIC_LICENSE_KEY ) ) );

		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegCloseKey( Key ) ); 
	}

	void DeleteRegistryDate()
	{
		HKEY Key;
		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegCreateKeyEx(
			HKEY_CURRENT_USER,
			CFIXCTL_LICENSE_REG_KEYPATH,
			0,
			NULL,
			0,
			KEY_WRITE,
			NULL,
			&Key,
			NULL ) );

		RegDeleteValue(
			Key,
			CFIXCTL_LICENSE_REG_KEY_NAME_LICENSE_DATE );

		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegCloseKey( Key ) ); 
	}

public:
	virtual void Before()
	{
		CFIX_ASSERT_OK( CfixctlInstallLicense( FALSE, NULL ) );
		CFIX_ASSERT_OK( CfixctlInstallLicense( FALSE, NULL ) );
		DeleteRegistryDate();
	}

	virtual void After()
	{
	}

	void InstallMessedLicense()
	{
		CFIXCC_ASSERT_EQUALS( E_INVALIDARG, CfixctlInstallLicense( FALSE, L"" ) );
	}

	void InstallValidLicense()
	{
		CFIXLIC_LICENSE_KEY Key;
		CFIX_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			1,
			&Key ) );
		CFIX_ASSERT_OK( CfixlicScrambleKey( &Key ) );

		WCHAR KeyString[ CFIXLIC_ENCODED_KEY_LENGTH + 1 ];
		CFIX_ASSERT_OK( CfixlicEncode(
			&Key, _countof( KeyString ), KeyString ) );

		CFIXCC_ASSERT_EQUALS( E_INVALIDARG, CfixctlValidateLicense( NULL ) );
		CFIX_ASSERT_OK( CfixctlValidateLicense( KeyString ) );
		CFIX_ASSERT_OK( CfixctlInstallLicense( FALSE, KeyString ) );

		CFIXCTL_LICENSE_INFO Info;
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlLicensed, Info.Type );
		CFIXCC_ASSERT_EQUALS( KeyString, Info.Key );
		CFIXCC_ASSERT_EQUALS( 1UL, ( ULONG ) Info.Product );
		CFIXCC_ASSERT_EQUALS( 2UL, ( ULONG ) Info.SubProduct );
		CFIXCC_ASSERT_EQUALS( TRUE, Info.Valid );
		CFIXCC_ASSERT_EQUALS( 0UL, Info.DaysLeft );

		//
		// Overwrite previous key.
		//
		CFIX_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			2,
			&Key ) );
		CFIX_ASSERT_OK( CfixlicScrambleKey( &Key ) );

		CFIX_ASSERT_OK( CfixlicEncode(
			&Key, _countof( KeyString ), KeyString ) );

		CFIX_ASSERT_OK( CfixctlInstallLicense( FALSE, KeyString ) );

		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlLicensed, Info.Type );
		CFIXCC_ASSERT_EQUALS( KeyString, Info.Key );
		CFIXCC_ASSERT_EQUALS( 1UL, ( ULONG ) Info.Product );
		CFIXCC_ASSERT_EQUALS( 2UL, ( ULONG ) Info.SubProduct );
		CFIXCC_ASSERT_EQUALS( TRUE, Info.Valid );
		CFIXCC_ASSERT_EQUALS( 0UL, Info.DaysLeft );
	}

	void InstallInconsistentLicense()
	{	
		CFIXLIC_LICENSE_KEY Key;
		CFIX_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			1,
			&Key ) );
		CFIX_ASSERT_OK( CfixlicScrambleKey( &Key ) );

		WCHAR KeyString[ CFIXLIC_ENCODED_KEY_LENGTH + 1 ];
		CFIX_ASSERT_OK( CfixlicEncode(
			&Key, _countof( KeyString ), KeyString ) );

		KeyString[ 0 ] = 'X';
		KeyString[ 1 ] = 'X';
		KeyString[ 2 ] = 'X';

		CFIXCC_ASSERT_EQUALS( 
			CFIXCTL_E_LIC_INVALID, CfixctlInstallLicense( FALSE, KeyString ) );

		//
		// No license installed -> trial.
		//
		CFIXCTL_LICENSE_INFO Info;
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlTrial, Info.Type );
	}

	void InstallFutureLicense()
	{	
		CFIXLIC_LICENSE_KEY Key;
		CFIX_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MAX_ALLOWED_VERSION_MAJOR + 1,	// Wrong.
			CFIXCTL_LIC_MAX_ALLOWED_VERSION_MINOR,
			1,
			&Key ) );
		CFIX_ASSERT_OK( CfixlicScrambleKey( &Key ) );

		WCHAR KeyString[ CFIXLIC_ENCODED_KEY_LENGTH + 1 ];
		CFIX_ASSERT_OK( CfixlicEncode(
			&Key, _countof( KeyString ), KeyString ) );

		CFIXCC_ASSERT_EQUALS( 
			CFIXCTL_E_LIC_INVALID, CfixctlInstallLicense( FALSE, KeyString ) );

		//
		// No license installed -> trial.
		//
		CFIXCTL_LICENSE_INFO Info;
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlTrial, Info.Type );
	}

	void InstallOutdatedLicense()
	{	
		CFIXLIC_LICENSE_KEY Key;
		CFIX_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR - 1,	// Wrong.
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			1,
			&Key ) );
		CFIX_ASSERT_OK( CfixlicScrambleKey( &Key ) );

		WCHAR KeyString[ CFIXLIC_ENCODED_KEY_LENGTH + 1 ];
		CFIX_ASSERT_OK( CfixlicEncode(
			&Key, _countof( KeyString ), KeyString ) );

		CFIXCC_ASSERT_EQUALS( 
			CFIXCTL_E_LIC_INVALID, CfixctlInstallLicense( FALSE, KeyString ) );

		//
		// No license installed -> trial.
		//
		CFIXCTL_LICENSE_INFO Info;
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlTrial, Info.Type );
	}

	void QueryInconsistentLicense()
	{
		CFIXLIC_LICENSE_KEY LicKey;
		CFIX_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			1,
			&LicKey ) );
		LicKey.u.Fields.Checksum -= 1;
		CFIX_ASSERT_OK( CfixlicScrambleKey( &LicKey ) );

		InstallLicense( &LicKey );

		CFIXCTL_LICENSE_INFO Info;
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );
		CFIXCC_ASSERT_EQUALS( FALSE, Info.Valid );
	}

	void QueryOutdatedLicense()
	{
		CFIXLIC_LICENSE_KEY LicKey;
		CFIX_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR - 1,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			1,
			&LicKey ) );
		CFIX_ASSERT_OK( CfixlicScrambleKey( &LicKey ) );

		InstallLicense( &LicKey );
		
		CFIXCTL_LICENSE_INFO Info;
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );
		CFIXCC_ASSERT_EQUALS( FALSE, Info.Valid );
	}

	void QueryFutureLicense()
	{
		CFIXLIC_LICENSE_KEY LicKey;
		CFIX_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MAX_ALLOWED_VERSION_MAJOR + 1,
			CFIXCTL_LIC_MAX_ALLOWED_VERSION_MINOR,
			1,
			&LicKey ) );
		CFIX_ASSERT_OK( CfixlicScrambleKey( &LicKey ) );

		InstallLicense( &LicKey );
		
		CFIXCTL_LICENSE_INFO Info;
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );
		CFIXCC_ASSERT_EQUALS( FALSE, Info.Valid );
	}

	void QueryTamperedLicense()
	{
		HKEY Key;
		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegCreateKeyEx(
			HKEY_CURRENT_USER,
			CFIXCTL_LICENSE_REG_KEYPATH,
			0,
			NULL,
			0,
			KEY_WRITE,
			NULL,
			&Key,
			NULL ) );

		DWORD Junk = 'Junk';
		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegSetValueEx(
			Key,
			CFIXCTL_LICENSE_REG_KEY_NAME_LICENSE,
			0,
			REG_BINARY,
			( LPBYTE ) &Junk,
			sizeof( DWORD ) ) );

		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegCloseKey( Key ) ); 
		
		CFIXCTL_LICENSE_INFO Info;
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIXCC_ASSERT_EQUALS( CFIXCTL_E_LIC_TAMPERED, CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );
	}

	void QueryAge()
	{
		FILETIME Now;
		CFIX_ASSERT_OK( CoFileTimeNow( &Now ) );
		ULONG NowDays = DaysFromFileTime ( Now );

		CFIXCTL_LICENSE_INFO Info;
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, NowDays, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlTrial, Info.Type );
		CFIXCC_ASSERT_EQUALS( 0UL, ( ULONG ) Info.Product );
		CFIXCC_ASSERT_EQUALS( 0UL, ( ULONG ) Info.SubProduct );

		//
		// Again (now with registry).
		//
		ULONG Installed = Info.DaysInstalled;
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, NowDays, &Info ) );
		CFIXCC_ASSERT_EQUALS( Installed, Info.DaysInstalled );

		//
		// Let external date be significant.
		//
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, NowDays - Installed - 1, &Info ) );

		CFIXCC_ASSERT_EQUALS( Installed + 1, Info.DaysInstalled );
	}

	void QueryDaysLeft()
	{
		FILETIME Now;
		CFIX_ASSERT_OK( CoFileTimeNow( &Now ) );
		ULONG NowDays = DaysFromFileTime ( Now );

		ResetBinFolderAge( 1 );	// tomorrow.

		CFIXCTL_LICENSE_INFO Info;
		Info.SizeOfStruct = sizeof( CFIXCTL_LICENSE_INFO );
		CFIXCC_ASSERT_EQUALS( CFIXCTL_E_LIC_TAMPERED, CfixctlQueryLicenseInfo(
			FALSE, NowDays + 1, &Info ) );

		ResetBinFolderAge( -1 );	// yesterday.

		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, NowDays, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlTrial, Info.Type );
		CFIXCC_ASSERT_EQUALS( 0UL, ( ULONG ) Info.Product );
		CFIXCC_ASSERT_EQUALS( 0UL, ( ULONG ) Info.SubProduct );
		CFIXCC_ASSERT( Info.Valid );
		CFIXCC_ASSERT_EQUALS( 1UL, Info.DaysInstalled );	// folder prevails.

#ifdef CFIXCTL_LIC_HARD_EXPIRY_DATE
		CFIXCC_ASSERT_EQUALS( 
			min( 
				CFIXCTL_LIC_HARD_EXPIRY_DATE - CurrentLicensingDate(), 
				CFIXCTL_LIC_TRIAL_PERIOD - 1 ),
			Info.DaysLeft );
#else
		CFIXCC_ASSERT_EQUALS( CFIXCTL_LIC_TRIAL_PERIOD - 1, Info.DaysLeft );
#endif

		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, NowDays - 2, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlTrial, Info.Type );
		CFIXCC_ASSERT_EQUALS( 0UL, ( ULONG ) Info.Product );
		CFIXCC_ASSERT_EQUALS( 0UL, ( ULONG ) Info.SubProduct );
		CFIXCC_ASSERT( Info.Valid );
		CFIXCC_ASSERT_EQUALS( 2UL, Info.DaysInstalled );	// external date prevails.

#ifdef CFIXCTL_LIC_HARD_EXPIRY_DATE
		CFIXCC_ASSERT_EQUALS( 
			min( 
				CFIXCTL_LIC_HARD_EXPIRY_DATE - CurrentLicensingDate(), 
				CFIXCTL_LIC_TRIAL_PERIOD - 2 ),
			Info.DaysLeft );
#else
		CFIXCC_ASSERT_EQUALS( CFIXCTL_LIC_TRIAL_PERIOD - 2, Info.DaysLeft );
#endif

		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, NowDays - CFIXCTL_LIC_TRIAL_PERIOD, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlTrial, Info.Type );
		CFIXCC_ASSERT_EQUALS( 0UL, ( ULONG ) Info.Product );
		CFIXCC_ASSERT_EQUALS( 0UL, ( ULONG ) Info.SubProduct );
		CFIXCC_ASSERT( Info.Valid );
		CFIXCC_ASSERT_EQUALS( CFIXCTL_LIC_TRIAL_PERIOD, Info.DaysInstalled );
		CFIXCC_ASSERT_EQUALS( 0UL, Info.DaysLeft );

		CFIX_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, NowDays - CFIXCTL_LIC_TRIAL_PERIOD - 1, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlTrial, Info.Type );
		CFIXCC_ASSERT_EQUALS( 0UL, ( ULONG ) Info.Product );
		CFIXCC_ASSERT_EQUALS( 0UL, ( ULONG ) Info.SubProduct );
		CFIXCC_ASSERT( ! Info.Valid );
		CFIXCC_ASSERT_EQUALS( CFIXCTL_LIC_TRIAL_PERIOD + 1, Info.DaysInstalled );
		CFIXCC_ASSERT_EQUALS( 0UL, Info.DaysLeft );
	}
};

CFIXCC_BEGIN_CLASS( TestLicensing )
	CFIXCC_METHOD( InstallMessedLicense )
	CFIXCC_METHOD( InstallValidLicense )
	CFIXCC_METHOD( InstallInconsistentLicense )
	CFIXCC_METHOD( InstallFutureLicense )
	CFIXCC_METHOD( InstallOutdatedLicense )
	CFIXCC_METHOD( QueryInconsistentLicense )
	CFIXCC_METHOD( QueryOutdatedLicense )
	CFIXCC_METHOD( QueryFutureLicense )
	CFIXCC_METHOD( QueryTamperedLicense )
	CFIXCC_METHOD( QueryAge )
	CFIXCC_METHOD( QueryDaysLeft )
CFIXCC_END_CLASS()