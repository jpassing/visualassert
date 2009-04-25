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

class TestLicensing : public cfixcc::TestFixture
{
private:
	
public:
	virtual void Before()
	{
		CFIXCC_ASSERT_OK( CfixctlInstallLicense( FALSE, NULL ) );
		CFIXCC_ASSERT_OK( CfixctlInstallLicense( FALSE, NULL ) );
	}

	virtual void After()
	{
	}

	void InstallLicense( 
		__in PCFIXLIC_LICENSE_KEY LicKey
		)
	{
		HKEY Key;
		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegCreateKeyEx(
			HKEY_CURRENT_USER,
			CFIXCTL_LICNESE_REG_KEYPATH,
			0,
			NULL,
			0,
			KEY_WRITE,
			NULL,
			&Key,
			NULL ) );

		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegSetValueEx(
			Key,
			CFIXCTL_LICNESE_REG_KEY_NAME,
			0,
			REG_BINARY,
			( LPBYTE ) &LicKey,
			sizeof( CFIXLIC_LICENSE_KEY ) ) );

		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegCloseKey( Key ) ); 
	}

	void InstallMessedLicense()
	{
		CFIXCC_ASSERT_EQUALS( E_INVALIDARG, CfixctlInstallLicense( FALSE, L"" ) );
	}

	void InstallValidLicense()
	{
		CFIXLIC_LICENSE_KEY Key;
		CFIXCC_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			1,
			&Key ) );
		CFIXCC_ASSERT_OK( CfixlicScrambleKey( &Key ) );

		WCHAR KeyString[ CFIXLIC_ENCODED_KEY_LENGTH + 1 ];
		CFIXCC_ASSERT_OK( CfixlicEncode(
			&Key, _countof( KeyString ), KeyString ) );

		CFIXCC_ASSERT_OK( CfixctlInstallLicense( FALSE, KeyString ) );

		CFIXCTL_LICENSE_INFO Info;
		CFIXCC_ASSERT_OK( CfixctlQueryLicenseInfo(
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
		CFIXCC_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			2,
			&Key ) );
		CFIXCC_ASSERT_OK( CfixlicScrambleKey( &Key ) );

		CFIXCC_ASSERT_OK( CfixlicEncode(
			&Key, _countof( KeyString ), KeyString ) );

		CFIXCC_ASSERT_OK( CfixctlInstallLicense( FALSE, KeyString ) );

		CFIXCC_ASSERT_OK( CfixctlQueryLicenseInfo(
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
		CFIXCC_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			1,
			&Key ) );
		CFIXCC_ASSERT_OK( CfixlicScrambleKey( &Key ) );

		WCHAR KeyString[ CFIXLIC_ENCODED_KEY_LENGTH + 1 ];
		CFIXCC_ASSERT_OK( CfixlicEncode(
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
		CFIXCC_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlTrial, Info.Type );
	}

	void InstallFutureLicense()
	{	
		CFIXLIC_LICENSE_KEY Key;
		CFIXCC_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MAX_ALLOWED_VERSION_MAJOR + 1,	// Wrong.
			CFIXCTL_LIC_MAX_ALLOWED_VERSION_MINOR,
			1,
			&Key ) );
		CFIXCC_ASSERT_OK( CfixlicScrambleKey( &Key ) );

		WCHAR KeyString[ CFIXLIC_ENCODED_KEY_LENGTH + 1 ];
		CFIXCC_ASSERT_OK( CfixlicEncode(
			&Key, _countof( KeyString ), KeyString ) );

		CFIXCC_ASSERT_EQUALS( 
			CFIXCTL_E_LIC_INVALID, CfixctlInstallLicense( FALSE, KeyString ) );

		//
		// No license installed -> trial.
		//
		CFIXCTL_LICENSE_INFO Info;
		CFIXCC_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlTrial, Info.Type );
	}

	void InstallOutdatedLicense()
	{	
		CFIXLIC_LICENSE_KEY Key;
		CFIXCC_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR - 1,	// Wrong.
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			1,
			&Key ) );
		CFIXCC_ASSERT_OK( CfixlicScrambleKey( &Key ) );

		WCHAR KeyString[ CFIXLIC_ENCODED_KEY_LENGTH + 1 ];
		CFIXCC_ASSERT_OK( CfixlicEncode(
			&Key, _countof( KeyString ), KeyString ) );

		CFIXCC_ASSERT_EQUALS( 
			CFIXCTL_E_LIC_INVALID, CfixctlInstallLicense( FALSE, KeyString ) );

		//
		// No license installed -> trial.
		//
		CFIXCTL_LICENSE_INFO Info;
		CFIXCC_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );

		CFIXCC_ASSERT_EQUALS( CfixctlTrial, Info.Type );
	}

	void QueryInconsistentLicense()
	{
		CFIXLIC_LICENSE_KEY LicKey;
		CFIXCC_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			1,
			&LicKey ) );
		LicKey.u.Fields.Checksum -= 1;
		CFIXCC_ASSERT_OK( CfixlicScrambleKey( &LicKey ) );

		InstallLicense( &LicKey );

		CFIXCTL_LICENSE_INFO Info;
		CFIXCC_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );
		CFIXCC_ASSERT_EQUALS( FALSE, Info.Valid );
	}

	void QueryOutdatedLicense()
	{
		CFIXLIC_LICENSE_KEY LicKey;
		CFIXCC_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR - 1,
			CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR,
			1,
			&LicKey ) );
		CFIXCC_ASSERT_OK( CfixlicScrambleKey( &LicKey ) );

		InstallLicense( &LicKey );
		
		CFIXCTL_LICENSE_INFO Info;
		CFIXCC_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );
		CFIXCC_ASSERT_EQUALS( FALSE, Info.Valid );
	}

	void QueryFutureLicense()
	{
		CFIXLIC_LICENSE_KEY LicKey;
		CFIXCC_ASSERT_OK( CfixlicCreateKey(
			1,
			2,
			CFIXCTL_LIC_MAX_ALLOWED_VERSION_MAJOR + 1,
			CFIXCTL_LIC_MAX_ALLOWED_VERSION_MINOR,
			1,
			&LicKey ) );
		CFIXCC_ASSERT_OK( CfixlicScrambleKey( &LicKey ) );

		InstallLicense( &LicKey );
		
		CFIXCTL_LICENSE_INFO Info;
		CFIXCC_ASSERT_OK( CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );
		CFIXCC_ASSERT_EQUALS( FALSE, Info.Valid );
	}

	void QueryTamperedLicense()
	{
		HKEY Key;
		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegCreateKeyEx(
			HKEY_CURRENT_USER,
			CFIXCTL_LICNESE_REG_KEYPATH,
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
			CFIXCTL_LICNESE_REG_KEY_NAME,
			0,
			REG_BINARY,
			( LPBYTE ) &Junk,
			sizeof( DWORD ) ) );

		CFIXCC_ASSERT_EQUALS( ERROR_SUCCESS, RegCloseKey( Key ) ); 
		
		CFIXCTL_LICENSE_INFO Info;
		CFIXCC_ASSERT_EQUALS( CFIXCTL_E_LIC_TAMPERED, CfixctlQueryLicenseInfo(
			FALSE, 1, &Info ) );
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
CFIXCC_END_CLASS()