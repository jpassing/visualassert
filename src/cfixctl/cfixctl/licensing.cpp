/*----------------------------------------------------------------------
 * Purpose:
 *		Licensing.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#define CFIXCTLAPI

#include <windows.h>
#include <cfixlic.h>
#include "cfixctlp.h"

static HRESULT CfixctlsStoreLicenseKey(
	__in BOOL MachineWide,
	__in CONST PCFIXLIC_LICENSE_KEY LicKey
	)
{
	HKEY Key;
	LONG Result = RegCreateKeyEx(
		MachineWide ? HKEY_LOCAL_MACHINE : HKEY_CURRENT_USER,
		CFIXCTLP_LICNESE_REG_KEYPATH,
		0,
		NULL,
		0,
		KEY_WRITE,
		NULL,
		&Key,
		NULL );
	if ( Result != ERROR_SUCCESS )
	{
		return HRESULT_FROM_WIN32( Result );
	}

	Result = RegSetValueEx(
		Key,
		L"License",
		0,
		REG_BINARY,
		( LPBYTE ) LicKey,
		sizeof( CFIXLIC_LICENSE_KEY ) );

	RegCloseKey( Key );
	return HRESULT_FROM_WIN32( Result );
}

static HRESULT CfixctlsLoadLicenseKey(
	__in BOOL MachineWide,
	__out PCFIXLIC_LICENSE_KEY LicKey
	)
{
	HKEY Key;
	LONG Result = RegCreateKeyEx(
		MachineWide ? HKEY_LOCAL_MACHINE : HKEY_CURRENT_USER,
		CFIXCTLP_LICNESE_REG_KEYPATH,
		0,
		NULL,
		0,
		KEY_READ,
		NULL,
		&Key,
		NULL );
	if ( Result != ERROR_SUCCESS )
	{
		return HRESULT_FROM_WIN32( Result );
	}

	DWORD Type;
	DWORD CbRead = sizeof( CFIXLIC_LICENSE_KEY );
	Result = RegQueryValueEx(
		Key,
		L"License", 
		0,
		&Type,
		( LPBYTE ) LicKey,
		&CbRead );

	HRESULT Hr;
	if ( Result == ERROR_SUCCESS &&
		 ( Type != REG_BINARY || CbRead != sizeof( CFIXLIC_LICENSE_KEY ) ) )
	{
		Hr = CFIXCTL_E_LIC_TAMPERED;
	}
	else
	{
		Hr = HRESULT_FROM_WIN32( Result );
	}

	RegCloseKey( Key );
	return Hr;
}

static HRESULT CfixctlsValidateScrambledLicenseKey(
	__in CONST PCFIXLIC_LICENSE_KEY Key
	)
{
	ASSERT( Key->Scrambled );

	//
	// Unscramble.
	//
	CFIXLIC_LICENSE_KEY KeyUnscrambled;
	KeyUnscrambled = *Key;
	HRESULT Hr = CfixlicUnscrambleKey( &KeyUnscrambled );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	//
	// Check the key's consistency.
	//
	BOOL Valid;
	Hr = CfixlicVerifyKey( &KeyUnscrambled, &Valid );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	if ( ! Valid )
	{
		return CFIXCTL_E_LIC_INVALID;
	}

	//
	// Check that the key matches the product/version.
	//
	if ( KeyUnscrambled.u.Fields.BaseInfo.Product != CFIXCTL_LIC_PRODUCT ||
	     KeyUnscrambled.u.Fields.BaseInfo.SubProduct != CFIXCTL_LIC_SUBPRODUCT )
	{
		//
		// Wrong product.
		//
		return CFIXCTL_E_LIC_INVALID;
	}

	UCHAR MinVersionAllowed = 
		CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR << 4 |
		CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR;

	UCHAR MaxVersionAllowed = 
		CFIXCTL_LIC_MAX_ALLOWED_VERSION_MAJOR << 4 |
		CFIXCTL_LIC_MAX_ALLOWED_VERSION_MINOR;

	UCHAR LicVersion = ( UCHAR ) (
		KeyUnscrambled.u.Fields.BaseInfo.MajorVersion << 4 |
		KeyUnscrambled.u.Fields.BaseInfo.MinorVersion );

	ASSERT( MinVersionAllowed > 0xF );
	ASSERT( MaxVersionAllowed > 0xF );
	ASSERT( LicVersion > 0xF );

	if ( LicVersion < MinVersionAllowed ||
		 LicVersion > MaxVersionAllowed )
	{
		//
		// Key not applicable for this release.
		//
		return CFIXCTL_E_LIC_INVALID;
	}
		
	return S_OK;
}

HRESULT CfixctlInstallLicense(
	__in BOOL MachineWide,
	__in PCWSTR KeyString
	)
{
	if ( ! KeyString || wcslen( KeyString ) != CFIXLIC_ENCODED_KEY_LENGTH )
	{
		return E_INVALIDARG;
	}

	CFIXLIC_LICENSE_KEY Key;
	HRESULT Hr = CfixlicDecode( KeyString, &Key );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	//
	// Check that the key is valid -- otherwise, installing is mute.
	//
	Hr = CfixctlsValidateScrambledLicenseKey( &Key );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	return CfixctlsStoreLicenseKey( MachineWide, &Key );
}

HRESULT CfixctlQueryLicenseInfo(
	__in BOOL MachineWide,
	__in ULONG ExternalDateOfInstallation,
	__out PCFIXCTL_LICENSE_INFO Info
	)
{
	if ( ! Info )
	{
		return E_INVALIDARG;
	}

	HRESULT Hr = CfixctlpGetTrialInstallationAge(
		ExternalDateOfInstallation,
		&Info->DaysInstalled );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	CFIXLIC_LICENSE_KEY Key;
	Hr = CfixctlsLoadLicenseKey( MachineWide, &Key );
	if ( Hr == HRESULT_FROM_WIN32( ERROR_PATH_NOT_FOUND ) )
	{
		//
		// No key installed -> trial.
		//
		Info->Type = CfixctlTrial;

		Info->Key[ 0 ]	= L'\0';

		Hr = CfixctlpIsTrialPeriodActive(
			Info->DaysInstalled,
			&Info->Valid );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		if ( Info->Valid )
		{
			Info->DaysLeft = CFIXCTL_LIC_TRIAL_PERIOD - Info->DaysInstalled;
		}
		else
		{
			Info->DaysLeft = 0;
		}
	}
	else if ( SUCCEEDED( Hr ) )
	{
		//
		// License installed.
		//
		Info->Type = CfixctlLicensed;

		Hr = CfixlicEncode(
			&Key,
			_countof( Info->Key ),
			Info->Key );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		Info->Valid = ( S_OK == CfixctlsValidateScrambledLicenseKey( &Key ) );
		Info->DaysLeft = 0;
	}
	
	return Hr;
}