/*----------------------------------------------------------------------
 * Purpose:
 *		COM Self Registration.
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

#include <selfreg.h>
#include <stdlib.h>
#include <shlwapi.h>

#pragma warning( push )
#pragma warning( disable: 6011; disable: 6387 )
#include <strsafe.h>
#pragma warning( pop )

static HRESULT CfixctlsSetValueInKey(
	__in HKEY BaseKey,
	__in PCWSTR KeyName, 
	__in PCWSTR ValueName, 
	__in PCWSTR Value
	)
{
	HRESULT Hr;
	HKEY Key;
	LONG Result;

	Result = RegCreateKeyEx(
		BaseKey,
		KeyName,
		0,
		NULL,
		0,
		KEY_WRITE,
		NULL,
		&Key,
		NULL );
	if ( ERROR_SUCCESS != Result )
	{
		return HRESULT_FROM_WIN32( Result );
	}

	Result = RegSetValueEx(
		Key, 
		ValueName, 
		0, 
		REG_SZ, 
		( PUCHAR ) Value, 
		( DWORD ) ( ( wcslen( Value ) + 1 ) * sizeof( WCHAR ) ) );
	Hr = HRESULT_FROM_WIN32( Result );

	RegCloseKey( Key );
	return Hr;
}

static HRESULT CfixctlsCreateSubKeyAndSetValue(
	__in HKEY BaseKey,
	__in PCWSTR BaseKeyName, 
	__in PCWSTR SubKeyName, 
	__in PCWSTR ValueName, 
	__in PCWSTR Value
	)
{
	WCHAR KeyPath[ 200 ];
	
	HRESULT Hr = StringCchPrintf(
		KeyPath,
		_countof( KeyPath ),
		L"%s\\%s",
		BaseKeyName,
		SubKeyName );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = CfixctlsSetValueInKey(
		BaseKey,
		KeyPath,
		ValueName,
		Value );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	return S_OK;
}

static HRESULT CfixctlsRegisterServer(
	__in HKEY BaseKey,
	__in PCWSTR ModulePath,
	__in CFIXCTLP_SERVER_TYPE Type,
	__in PCWSTR ClsidString,
	__in PCWSTR ClsidKeyPath,
	__in PCWSTR FriendlyName,
	__in PCWSTR VersionIndependentProgId,
	__in PCWSTR ProgId,
	__in PCWSTR ThreadingModel
	)
{
	HRESULT Hr;

	Hr = CfixctlsSetValueInKey(
		BaseKey,
		ClsidKeyPath,
		L"",	// Default value.
		FriendlyName );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Server path and threading model.
	//
	if( Type == CfixctlpServerTypeInproc )
	{
		Hr = CfixctlsCreateSubKeyAndSetValue(
			BaseKey,
			ClsidKeyPath,
			L"InprocServer32",
			L"",	// Default value.
			ModulePath );
		if ( FAILED( Hr ) )
		{
			goto Cleanup;
		}

		Hr = CfixctlsCreateSubKeyAndSetValue(
			BaseKey,
			ClsidKeyPath,
			L"InprocServer32",
			L"ThreadingModel",
			ThreadingModel );
		if ( FAILED( Hr ) )
		{
			goto Cleanup;
		}
	}
	else
	{
		Hr = CfixctlsCreateSubKeyAndSetValue(
			BaseKey,
			ClsidKeyPath,
			L"LocalServer32",
			L"",	// Default value.
			ModulePath );
		if ( FAILED( Hr ) )
		{
			goto Cleanup;
		}
	}

	//
	// Add the ProgID subkey under the CLSID key.
	//
	Hr = CfixctlsCreateSubKeyAndSetValue(
		BaseKey,
		ClsidKeyPath,
		L"ProgID",
		L"",
		ProgId );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Add the version-independent ProgID subkey under CLSID key.
	//
	Hr = CfixctlsCreateSubKeyAndSetValue(
		BaseKey,
		ClsidKeyPath,
		L"VersionIndependentProgID",
		L"",
		VersionIndependentProgId );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Add the version-independent ProgID subkey under HKEY_CLASSES_ROOT.
	//
	Hr = CfixctlsSetValueInKey(
		BaseKey,
		VersionIndependentProgId,
		L"",	// Default value.
		FriendlyName );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	Hr = CfixctlsCreateSubKeyAndSetValue(
		BaseKey,
		VersionIndependentProgId,
		L"CLSID",
		L"",
		ClsidString );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	Hr = CfixctlsCreateSubKeyAndSetValue(
		BaseKey,
		VersionIndependentProgId,
		L"CurVer",
		L"",
		ProgId );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Add the versioned ProgID subkey under HKEY_CLASSES_ROOT.
	//
	Hr = CfixctlsSetValueInKey(
		BaseKey,
		ProgId,
		L"",	// Default value.
		FriendlyName );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	Hr = CfixctlsCreateSubKeyAndSetValue(
		BaseKey,
		ProgId,
		L"CLSID",
		L"",
		ClsidString );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	Hr = S_OK;

Cleanup:
	if ( FAILED( Hr ) )
	{
		SHDeleteKey( HKEY_CLASSES_ROOT, ClsidKeyPath );
		SHDeleteKey( HKEY_CLASSES_ROOT, ProgId );
		SHDeleteKey( HKEY_CLASSES_ROOT, VersionIndependentProgId );
	}

	return S_OK;
}

HRESULT CfixctlpRegisterServer(
	__in HMODULE Module,
	__in CFIXCTLP_SERVER_TYPE Type,
	__in CFIXCTLP_SERVER_REGSCOPE RegScope,
	__in REFCLSID Clsid,
	__in PCWSTR FriendlyName,
	__in PCWSTR VersionIndependentProgId,
	__in PCWSTR ProgId,
	__in PCWSTR ThreadingModel,
	__in BOOL Register
	)
{
	HKEY BaseKey;
	WCHAR ClsidString[ 40 ];
	WCHAR ClsidKeyPath[ 100 ];
	WCHAR ModulePath[ MAX_PATH ];

	HRESULT Hr;

	if ( ! GetModuleFileName(
		Module,
		ModulePath,
		_countof( ModulePath ) ) )
	{
		return HRESULT_FROM_WIN32( GetLastError() );
	}

	Hr = StringFromGUID2(
		Clsid,
		ClsidString,
		_countof( ClsidString ) );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = StringCchPrintf(
		ClsidKeyPath,
		_countof( ClsidKeyPath ),
		L"CLSID\\%s",
		ClsidString );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	if ( RegScope == CfixctlpServerRegScopeUser )
	{
		LONG Result = RegOpenKeyEx(
			HKEY_CURRENT_USER,
			L"Software\\Classes",
			0,
			KEY_WRITE,
			&BaseKey );

		if ( ERROR_SUCCESS != Result )
		{
			return HRESULT_FROM_WIN32( Result );
		}
	}
	else
	{
		BaseKey = HKEY_CLASSES_ROOT;
	}

	if ( Register )
	{
		Hr = CfixctlsRegisterServer(
			BaseKey,
			ModulePath,
			Type,
			ClsidString,
			ClsidKeyPath,
			FriendlyName,
			VersionIndependentProgId,
			ProgId,
			ThreadingModel );
	}
	else
	{
		if ( FAILED( Hr = SHDeleteKey( BaseKey, ClsidKeyPath ) ) )
		{
			goto Cleanup;
		}

		if ( FAILED( Hr = SHDeleteKey( BaseKey, ProgId ) ) )
		{
			goto Cleanup;
		}

		if ( FAILED( Hr = SHDeleteKey( BaseKey, VersionIndependentProgId ) ) )
		{
			goto Cleanup;
		}

		Hr = S_OK;
	}

Cleanup:
	if ( RegScope == CfixctlpServerRegScopeUser )
	{
		RegCloseKey( BaseKey );
	}

	return Hr;
}
