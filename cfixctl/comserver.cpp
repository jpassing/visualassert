/*----------------------------------------------------------------------
 * Purpose:
 *		COM Server.
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

static volatile LONG CfixctlsServerLocks = 0;



HRESULT CfixctlpAddRefServer()
{
	InterlockedIncrement( &CfixctlsServerLocks );
	return S_OK;
}

HRESULT CfixctlpReleaseServer()
{
	if ( CfixctlsServerLocks == 0 )
	{
		return E_UNEXPECTED;
	}
	else
	{
		InterlockedDecrement( &CfixctlsServerLocks );
		return S_OK;
	}
}

/*----------------------------------------------------------------------
 *
 * COM Server Exports.
 *
 */

HRESULT STDMETHODCALLTYPE DllCanUnloadNow()
{
	return CfixctlsServerLocks == 0 ? S_OK : S_FALSE;
}

HRESULT STDMETHODCALLTYPE DllGetClassObject(
	__in REFCLSID Clsid,
	__in REFIID Iid,
	__out PVOID *ClassObject 
	)
{
	if ( ! ClassObject )
	{
		return E_INVALIDARG;
	}

	if ( InlineIsEqualGUID( Clsid, CLSID_TestCase ) )
	{
		return CfixctlpGetTestCaseFactory().QueryInterface( Iid, ClassObject );
	}
	else if ( InlineIsEqualGUID( Clsid, CLSID_TestFixture ) )
	{
		return CfixctlpGetTestFixtureFactory().QueryInterface( Iid, ClassObject );
	}
	else if ( InlineIsEqualGUID( Clsid, CLSID_TestModule ) )
	{
		return CfixctlpGetTestModuleFactory().QueryInterface( Iid, ClassObject );
	}
	else if ( InlineIsEqualGUID( Clsid, CLSID_Host ) )
	{
		return CfixctlpGetHostFactory().QueryInterface( Iid, ClassObject );
	}
	else
	{
		return CLASS_E_CLASSNOTAVAILABLE;
	}
}
