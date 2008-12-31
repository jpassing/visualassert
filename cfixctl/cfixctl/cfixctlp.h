#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Internal declarations.
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

#include <cfix.h>
#include <cfixapi.h>
#include <ole2.h>
#include <cfixctl.h>
#include <cfixctlmsg.h>
#include <crtdbg.h>

#pragma warning( push )
#pragma warning( disable: 6011; disable: 6387 )
#include <strsafe.h>
#pragma warning( pop )

#define ASSERT _ASSERTE

#ifndef VERIFY
	#if defined( DBG ) || defined( _DEBUG )
		#define VERIFY ASSERT
	#else
		#define VERIFY( x ) ( VOID ) ( x )
	#endif
#endif

#define DECLARE_NOT_COPYABLE( ClassName )								\
	private:															\
		ClassName( const ClassName& );									\
		const ClassName& operator = ( const ClassName& );

HRESULT CfixctlpAddRefServer();
HRESULT CfixctlpReleaseServer();

/*----------------------------------------------------------------------
 *
 * Class factories.
 *
 */
IClassFactory& CfixctlpGetTestCaseFactory();
IClassFactory& CfixctlpGetTestFixtureFactory();
IClassFactory& CfixctlpGetTestModuleFactory();
IClassFactory& CfixctlpGetHostFactory();

/*----------------------------------------------------------------------
 *
 * Internal interfaces.
 *
 */

DEFINE_GUID( IID_ICfixTestCaseInternal, 
	0x221f2d4b, 0xdf5, 0x4755, 0x9c, 0xf4, 0xcd, 0x6b, 0x56, 0x6b, 0x8, 0x74 );

interface
DECLSPEC_UUID( "221F2D4B-0DF5-4755-9CF4-CD6B566B0874" ) 
ICfixTestCaseInternal : public ICfixTestItem
{
	STDMETHOD( Initialize )(
		__in PCWSTR Name,
		__in ULONG FixtureOrdinal,
		__in ULONG TestCaseOrdinal,
		__in ICfixActionFactory *ActionFactory
		) PURE;

	/*++
		Method Description:
			Return pointer to the name, valid only as long as the
			object itself remains valid.
	--*/
	STDMETHOD_( BSTR, GetNameInternal )() PURE;
};

DEFINE_GUID( IID_ICfixTestFixtureInternal, 
	0xfdac652, 0x27d6, 0x4283, 0x8b, 0x53, 0x97, 0x5a, 0x99, 0x28, 0xb, 0x97);

interface
DECLSPEC_UUID( "0FDAC652-27D6-4283-8B53-975A99280B97" ) 
ICfixTestFixtureInternal : public ICfixTestItem
{
	STDMETHOD( Initialize )(
		__in PCFIX_FIXTURE Fixture,
		__in ULONG FixtureOrdinal,
		__in ICfixActionFactory *ActionFactory
		) PURE;

	/*++
		Method Description:
			Return pointer to the name, valid only as long as the
			object itself remains valid.
	--*/
	STDMETHOD_( BSTR, GetNameInternal )() PURE;
};


DEFINE_GUID( IID_ICfixTestModuleInternal, 
	0x2e8f211, 0xe29b, 0x40e0, 0xb2, 0xba, 0x39, 0xba, 0x4c, 0x56, 0xb8, 0x39);

interface
DECLSPEC_UUID( "02E8F211-E29B-40e0-B2BA-39BA4C56B839" ) 
ICfixTestModuleInternal : public ICfixTestModule
{
	STDMETHOD( Initialize )(
		__in PCWSTR Path,
		__in CfixTestModuleType Type,
		__in CfixTestModuleArch Architecture,
		__in PCFIX_TEST_MODULE Module
		) PURE;
};