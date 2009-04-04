/*----------------------------------------------------------------------
 * Purpose:
 *		Auxilary header file for C++.
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

/*------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 */

struct Factory : public IClassFactory
{
public:
	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )();
	STDMETHOD_( ULONG, Release )();
	STDMETHOD( QueryInterface )( 
		__in REFIID iid, 
		__out void** Ptr );

	/*------------------------------------------------------------------
	 * IClassFactory methods.
	 */

	STDMETHOD( CreateInstance )(
		__in IUnknown *UnkOuter,
		__in REFIID iid,
		__out void **Object 
		);
	STDMETHOD( LockServer )( 
		__in BOOL Lock
		);
};

/*------------------------------------------------------------------
 * 
 * Class Implementation.
 *
 */

ULONG Factory::AddRef()
{
}

ULONG Factory::Release()
{
	return 1;
}

STDMETHODIMP TestCase::QueryInterface( 
	__in REFIID iid, 
	__out void** Ptr )
{
	HRESULT Hr;

	if ( InlineIsEqualGUID( iid, IID_IUnknown ) ||
	     InlineIsEqualGUID( iid, IID_IClassFactory ) )
	{
		*Ptr = static_cast< IClassFactory* >( this );
		Hr = S_OK;
	}
	else
	{
		*Ptr = NULL;
		Hr = E_NOINTERFACE;
	}

	return Hr;
}

STDMETHOD( CreateInstance )(
	__in IUnknown *UnkOuter,
	__in REFIID iid,
	__out void **Object 
	)
{
	//
	// No support for aggregation.
	//
	if ( UnkOuter )
	{
		return CLASS_E_NOAGGREGATION;
	}

	if ( InlineIsEqualGUID( iid, IID_IUnknown ) ||
	     InlineIsEqualGUID( iid, IID_IClassFactory ) )
	{
		*Ptr = static_cast< IClassFactory* >( this );
		Hr = S_OK;
	}
	else
	{
		*Ptr = NULL;
		Hr = E_NOINTERFACE;
	}

}

STDMETHOD( LockServer )( 
	__in BOOL Lock
	)
{
}