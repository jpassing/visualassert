/*----------------------------------------------------------------------
 * Purpose:
 *		Auxilary COM classes.
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

#include <ole2.h>

#include <crtdbg.h>
#define ASSERT _ASSERTE


#define DECLARE_NOT_COPYABLE( ClassName )								\
	private:															\
		ClassName( const ClassName& );									\
		const ClassName& operator = ( const ClassName& );

/*++
	Class Description:
		Utility class implementing thread-safe reference counting.
--*/
template< typename T >
class ComMtaObject : public T
{
	DECLARE_NOT_COPYABLE( ComMtaObject );

private:
	volatile LONG ReferenceCount;
	IClassFactory *Factory;

public:
	ComMtaObject( IClassFactory *Factory ) 
		: ReferenceCount( 1 )
		, Factory( Factory )
	{
		//
		// AddRef factory to lock server.
		//
		Factory->AddRef();
	}

	virtual ~ComMtaObject()
	{
		this->Factory->Release();
	}

public:
	STDMETHOD_( ULONG, AddRef )()
	{
		return InterlockedIncrement( &this->ReferenceCount );
	}

	STDMETHOD_( ULONG, Release )()
	{
		LONG Refs = InterlockedDecrement( &this->ReferenceCount );
		if ( Refs == 0 )
		{
			delete this;
		}

		return Refs;
	}
};

/*++
	Class Description:
		Default implementation for a class factory. Objects of this
		class support stack allocation only.

		LockT is a class that implements server locking. It has to
		look as follows:

			class ComServerLock
			{
			public:
				void LockServer(
					__in BOOL Lock
					);
			};
--*/
template< typename T, typename LockT >
struct ComClassFactory : public IClassFactory
{
	DECLARE_NOT_COPYABLE( ComClassFactory );

private:
	LockT ServerLock;

public:
	ComClassFactory()
	{}

	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )()
	{
		this->LockServer( TRUE );
		return 1;
	}

	STDMETHOD_( ULONG, Release )()
	{
		this->LockServer( FALSE );
		return 1;
	}

	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr )
	{
		HRESULT Hr;

		if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
			 InlineIsEqualGUID( Iid, IID_IClassFactory ) )
		{
			*Ptr = static_cast< IClassFactory* >( this );
			Hr = S_OK;

			this->AddRef();
		}
		else
		{
			*Ptr = NULL;
			Hr = E_NOINTERFACE;
		}

		return Hr;
	}

	/*------------------------------------------------------------------
	 * IClassFactory methods.
	 */

	STDMETHOD( CreateInstance )(
		__in IUnknown *UnkOuter,
		__in REFIID Iid,
		__out void **Object 
		)
	{
		//
		// No support for aggregation.
		//
		if ( UnkOuter )
		{
			*Object = NULL;
			return CLASS_E_NOAGGREGATION;
		}

		T* NewObject = new T( this );
		if ( Object == NULL )
		{
			*Object = NULL;
			return E_OUTOFMEMORY;
		}
		else
		{
			HRESULT Hr = NewObject->QueryInterface( Iid, Object );
			
			//
			// N.B. If QI succeeded, the refcount is 2, if it failed,
			// the refcount is 1. Either way, one release is required.
			//

			ULONG Refs = NewObject->Release();
			ASSERT( ( Refs == 1 ) == SUCCEEDED( Hr ) );
			UNREFERENCED_PARAMETER( Refs );

			return Hr;
		}
	}

	STDMETHOD( LockServer )( 
		__in BOOL Lock
		)
	{
		ServerLock.LockServer( Lock );
		return S_OK;
	}
};

inline HRESULT ComGetProperty(
	__in PCWSTR Property,
    __out BSTR *Result
	)
{
	if ( ! Result )
	{
		return E_POINTER;
	}

	if ( ! Property )
	{
		return E_UNEXPECTED;
	}

	*Result = SysAllocString( Property );
	if ( *Result )
	{
		return S_OK;
	}
	else
	{
		return E_OUTOFMEMORY;
	}
}

inline ULONG ComGetUnmarshalSizeMaxBSTR(
	__in const BSTR String
	)
{
	return
		sizeof( ULONG )	+	// Size of string.
		SysStringByteLen( String );
}

inline HRESULT ComMarshalBSTR(
	__in IStream *Stm,
	__in const BSTR String 
	)
{
	ULONG RawBstrLenCb = ComGetUnmarshalSizeMaxBSTR( String );
	PUCHAR RawBstr = ( ( PUCHAR ) String ) - 4;
	return Stm->Write( RawBstr, RawBstrLenCb, NULL );
}

inline HRESULT ComUnmarshalBSTR(
	__in IStream *Stm,
	__out BSTR *String 
	)
{
	ULONG StringLenCb;

	HRESULT Hr = Stm->Read( &StringLenCb, sizeof( ULONG ), NULL );
	if ( SUCCEEDED( Hr ) )
	{
		*String = SysAllocStringByteLen( NULL, StringLenCb );
		if ( *String == NULL )
		{
			Hr = E_OUTOFMEMORY;
		}
		else if ( StringLenCb == 0 )
		{
			Hr = S_OK;
		}
		else
		{
			Hr = Stm->Read( *String, StringLenCb, NULL );
		}
	}

	return Hr;
}

/*++
	Class Description:
		Utility class for storing COM interface pointers.
--*/
template< typename T >
class ComArray
{
	DECLARE_NOT_COPYABLE( ComArray );

private:
	T **Items;
	ULONG Count;

public:
	ComArray() : Items( NULL ), Count( 0 )
	{
	}

	~ComArray()
	{
		Clear();
	}

	HRESULT Initialize( ULONG Count )
	{
		if ( this->Count != 0 )
		{
			return E_UNEXPECTED;
		}

		this->Items = new T*[ Count ];
		if ( this->Items == NULL )
		{
			return E_OUTOFMEMORY;
		}

		ZeroMemory( this->Items, Count * sizeof( T* ) );

		this->Count = Count;
		return S_OK;
	}

	void Clear()
	{
		if ( this->Items != NULL )
		{
			for ( ULONG Index = 0; Index < this->Count; Index++ )
			{
				if ( this->Items[ Index ] != NULL )
				{
					this->Items[ Index ]->Release();
				}
			}

			delete [] this->Items;
			
			this->Items = NULL;
			this->Count = 0;
		}
	}

	ULONG GetCount() const
	{
		return this->Count;
	}

	void Set( ULONG Index, T* Item )
	{
		ASSERT( Index < this->Count );

		if ( this->Items[ Index ] != NULL )
		{
			this->Items[ Index ]->Release();
		}

		Item->AddRef();
		this->Items[ Index ] = Item;
	}

	T*& Get( ULONG Index )
	{
		ASSERT( Index < this->Count );
		return this->Items[ Index ];
	}
};

/*++
	Class Description:
		Non-threadsafe implementation of IEnumUnknown.
--*/
template< typename T >
class ComAbstractEnumerator : public IEnumUnknown
{
	DECLARE_NOT_COPYABLE( ComAbstractEnumerator );

private:
	ULONG Offset;
	ULONG ReferenceCount;

protected:
	IUnknown *Parent;

	/*++
		Method Description:
			Get addref'ed element.
	--*/
	STDMETHOD( GetElement )( 
		__in ULONG Index,
		__out T** Object
		) PURE;
	
	/*++
		Method Description:
			Get element count.
	--*/
	STDMETHOD_( ULONG, GetCount )() PURE;
	
public:
	ComAbstractEnumerator( IUnknown *Parent ) 
		: Offset( 0 )
		, ReferenceCount( 1 )
		, Parent( Parent )
	{
		//
		// Keep alive parent.
		//
		Parent->AddRef();
	}

	virtual ~ComAbstractEnumerator()
	{
		Parent->Release();
	}

	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )()
	{
		return ++this->ReferenceCount;
	}

	STDMETHOD_( ULONG, Release )()
	{
		LONG Refs = --this->ReferenceCount;
		if ( Refs == 0 )
		{
			delete this;
		}

		return Refs;
	}

	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr )
	{
		HRESULT Hr;

		if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
			 InlineIsEqualGUID( Iid, IID_IEnumUnknown ) )
		{
			*Ptr = static_cast< IEnumUnknown* >( this );
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
	 * IEnumUnknown methods.
	 */

	STDMETHOD( Next )( 
		__in ULONG Count,
		__out IUnknown **Elements,
		__out PULONG Fetched
		)
	{
		*Fetched = 0;

		for ( ULONG Index = 0; 
			  Index < Count && this->Offset + Index < GetCount(); 
			  Index++ )
		{
			T* Item;
			
			HRESULT Hr = GetElement( this->Offset +  Index, &Item );
			if ( FAILED( Hr ) )
			{
				//
				// State has not been modified yet - bail out.
				//
				return Hr;
			}

			Elements[ Index ] = static_cast< IUnknown* >( Item );
			( *Fetched )++;
		}

		this->Offset += *Fetched;
		ASSERT( this->Offset <= GetCount() );

		return ( *Fetched == Count ) ? S_OK : S_FALSE;
	}

	STDMETHOD( Skip )( 
		__in ULONG Count
		)
	{
		if ( Count == 0 )
		{
			return S_OK;
		}
		else if ( this->Offset + Count >= GetCount() )
		{
			return S_FALSE;
		}
		else
		{
			this->Offset += Count;
			return S_OK;
		}
	}

	STDMETHOD( Reset )()
	{
		this->Offset = 0;
		return S_OK;
	}
};

/*++
	Class Description:
		Non-threadsafe implementation of IEnumUnknown for enumerating
		ComArray objects.
--*/
template< typename T >
class ComArrayEnumerator : public ComAbstractEnumerator< T >
{
	DECLARE_NOT_COPYABLE( ComArrayEnumerator );

private:
	ComArray< T >& Array;

public:
	ComArrayEnumerator( ComArray< T >& Arr, IUnknown *Parent ) 
		: Array( Arr )
		, ComAbstractEnumerator( Parent )
	{
	}

	virtual ~ComArrayEnumerator()
	{
	}

	/*------------------------------------------------------------------
	 * Overrides.
	 */
	STDMETHODIMP GetElement( 
		__in ULONG Index,
		__out T** Element
		)
	{
		ASSERT( Element );
		ASSERT( Index < this->Array.GetCount() );

		*Element = this->Array.Get( Index );
		( *Element )->AddRef();
		return S_OK;
	}
	
	STDMETHODIMP_( ULONG ) GetCount()
	{
		return this->Array.GetCount();
	}

	STDMETHOD( Clone )( 
		__out IEnumUnknown **Enum
		)
	{
		*Enum = new ComArrayEnumerator< T >( this->Array, this->Parent );
		if ( *Enum == NULL )
		{
			return E_OUTOFMEMORY;
		}
		else
		{
			return S_OK;
		}
	}
};