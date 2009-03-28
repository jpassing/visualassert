/*----------------------------------------------------------------------
 * Purpose:
 *		Stack Trace.
 *
 * Copyright:
 *		2009, Johannes Passing (passing at users.sourceforge.net)
 *
 */

#include "cfixctlp.h"
#include <cdiag.h>

// {6DCA5F7C-AB9A-4262-8F11-A4DE271EC152}
const GUID IID_ICfixStackTraceInternal = 
	{ 0x6dca5f7c, 0xab9a, 0x4262, { 0x8f, 0x11, 0xa4, 0xde, 0x27, 0x1e, 0xc1, 0x52 } };

/*------------------------------------------------------------------
 * 
 * Class.
 *
 */

class StackTrace :
	public ComObjectBase, 
	public ICfixStackTraceInternal,
	public IMarshal
{
	DECLARE_NOT_COPYABLE( StackTrace );

	ComArray< ICfixStackTraceFrame > Frames;

protected:
	StackTrace()
	{
	}

public:
	virtual ~StackTrace()
	{
	}

	STDMETHOD( FinalConstruct )()
	{
		return S_OK;
	}

	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )() PURE;
	STDMETHOD_( ULONG, Release )() PURE;
	
	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr )
	{
		HRESULT Hr;

		if ( ! Ptr )
		{
			return E_POINTER;
		}

		if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
			 InlineIsEqualGUID( Iid, IID_ICfixStackTrace ) )
		{
			*Ptr = static_cast< ICfixStackTrace* >( this );
			Hr = S_OK;

		}
		else if ( InlineIsEqualGUID( Iid, IID_IMarshal ) )
		{
			*Ptr = static_cast< IMarshal* >( this );
			Hr = S_OK;
		}
		else if ( InlineIsEqualGUID( Iid, IID_ICfixStackTraceInternal ) )
		{
			*Ptr = static_cast< ICfixStackTraceInternal* >( this );
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
	 * ICfixStackTrace methods.
	 */

	STDMETHOD( GetFrameCount )(
		__out ULONG *Count
		)
	{
		if ( ! Count )
		{
			return E_POINTER;
		}
		else
		{
			*Count = this->Frames.GetCount();
			return S_OK;
		}
	}

	STDMETHOD( GetFrame )(
		__in ULONG Index,
		__out ICfixStackTraceFrame **Frame
		)
	{
		if ( ! Frame )
		{
			return E_POINTER;
		}
		else
		{
			*Frame = NULL;
		}

		if ( Index >= this->Frames.GetCount() )
		{
			return E_INVALIDARG;
		}

		*Frame = this->Frames.Get( Index );
		( *Frame )->AddRef();
		return S_OK;
	}

	/*------------------------------------------------------------------
	 * ICfixStackTraceInternal methods.
	 */

	STDMETHOD( Initialize )(
		__in PCFIX_STACKTRACE StackTrace,
		__in CFIX_GET_INFORMATION_STACKFRAME_ROUTINE GetInfFrameRoutine
		) 
	{
		if ( ! StackTrace || ! GetInfFrameRoutine )
		{
			return E_INVALIDARG;
		}

		this->Frames.Initialize( StackTrace->FrameCount );
		
		HRESULT Hr;

		IClassFactory& Factory = CfixctlpGetStackTraceFrameFactory();
		for ( ULONG Index = 0; Index < StackTrace->FrameCount; Index++ )
		{
			ICfixStackTraceFrameInternal *Init;
			Hr = Factory.CreateInstance(
				NULL,
				IID_ICfixStackTraceFrameInternal,
				( PVOID* ) &Init );
			if ( FAILED( Hr ) )
			{
				goto Cleanup;
			}

			Hr = Init->Initialize( 
				StackTrace->Frames[ Index ], 
				GetInfFrameRoutine );
			if ( SUCCEEDED( Hr ) )
			{
				this->Frames.Set( Index, Init );
			}

			Init->Release();

			if ( FAILED( Hr ) )
			{
				goto Cleanup;
			}
		}

		Hr = S_OK;
		
	Cleanup:
		if ( FAILED( Hr ) )
		{
			this->Frames.Clear();
		}

		return Hr;
	}

	
	/*------------------------------------------------------------------
	 * IMarshal methods.
	 */

	STDMETHOD( GetUnmarshalClass )( 
		__in REFIID Iid,
		__in PVOID Itf,
		__in DWORD DestContext,
		__in PVOID Reserved,
		__in DWORD Mshlflags,
		__out CLSID *Clsid
		)
	{
		UNREFERENCED_PARAMETER( Iid );
		UNREFERENCED_PARAMETER( Itf );
		UNREFERENCED_PARAMETER( DestContext );
		UNREFERENCED_PARAMETER( Reserved );
		UNREFERENCED_PARAMETER( Mshlflags );

		//
		// Marshal by value, i.e. use own CLSID.
		//
		*Clsid = CLSID_StackTrace;
		return S_OK;
	}

	STDMETHOD( GetMarshalSizeMax )( 
		__in REFIID Iid,
		__in PVOID Itf,
		__in DWORD DestContext,
		__in PVOID Reserved,
		__in DWORD Mshlflags,
		__out DWORD *Size
		)
	{
		UNREFERENCED_PARAMETER( Iid );
		UNREFERENCED_PARAMETER( Itf );
		UNREFERENCED_PARAMETER( DestContext );
		UNREFERENCED_PARAMETER( Reserved );
		UNREFERENCED_PARAMETER( Mshlflags );

		if ( ! Size )
		{
			return E_POINTER;
		}
		else
		{
			*Size = 0;
		}

		ULONG SizeNeeded = sizeof( ULONG );	// Count.

		ULONG ItfSize;
		for ( ULONG Index = 0; Index < this->Frames.GetCount(); Index++ )
		{
			HRESULT Hr = CoGetMarshalSizeMax(
				&ItfSize,
				IID_ICfixStackTraceFrame,
				this->Frames.Get( Index ),
				DestContext,
				Reserved,
				Mshlflags );
			if ( FAILED( Hr ) )
			{
				return Hr;
			}

			SizeNeeded += ItfSize;
		}

		*Size = SizeNeeded;
		return S_OK;
	}


	STDMETHOD( MarshalInterface )( 
		__in IStream *Stm,
		__in REFIID Iid,
		__in PVOID Itf,
		__in DWORD DestContext,
		__in PVOID Reserved,
		__in DWORD Mshlflags
		)
	{
		UNREFERENCED_PARAMETER( Iid );
		UNREFERENCED_PARAMETER( Itf );
		UNREFERENCED_PARAMETER( DestContext );
		UNREFERENCED_PARAMETER( Reserved );
		UNREFERENCED_PARAMETER( Mshlflags );

		ULONG Count = this->Frames.GetCount();
		HRESULT Hr = Stm->Write( &Count, sizeof( ULONG ), NULL );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}
		
		for ( ULONG Index = 0; Index < this->Frames.GetCount(); Index++ )
		{
			Hr = CoMarshalInterface(
				Stm,
				IID_ICfixStackTraceFrame,
				this->Frames.Get( Index ),
				DestContext,
				Reserved,
				Mshlflags );
			if ( FAILED( Hr ) )
			{
				return Hr;
			}
		}

		return Hr;
	}

	STDMETHOD( UnmarshalInterface )( 
		__in IStream *Stm,
		__in REFIID Iid,
		__out PVOID *Object
		)
	{
		if ( ! Object )
		{
			return E_POINTER;
		}
		else
		{
			*Object = NULL;
		}

		ULONG Count;
		HRESULT Hr = Stm->Read( &Count, sizeof( ULONG ), NULL );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		Hr = this->Frames.Initialize( Count );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		for ( ULONG Index = 0; Index < this->Frames.GetCount(); Index++ )
		{
			ICfixStackTraceFrame *Item;
			Hr = CoUnmarshalInterface(
				Stm,
				IID_ICfixStackTraceFrame,
				( PVOID* ) &Item );
			if ( FAILED( Hr ) )
			{
				return Hr;
			}

			this->Frames.Set( Index, Item );
			Item->Release();
		}

		return QueryInterface( Iid, Object );
	}

	STDMETHOD( ReleaseMarshalData )( 
		__in IStream *Stm
		)
	{
		UNREFERENCED_PARAMETER( Stm );
		return S_OK;
	}

	STDMETHOD( DisconnectObject )( 
		__in DWORD Reserved
		)
	{
		UNREFERENCED_PARAMETER( Reserved );
		return S_OK;
	}
};


/*------------------------------------------------------------------
 * 
 * Factory.
 *
 */

IClassFactory& CfixctlpGetStackTraceFactory()
{
	static ComClassFactory< ComMtaObject< StackTrace >, CfixctlServerLock > Factory;
	return Factory;
}