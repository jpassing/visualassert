/*----------------------------------------------------------------------
 * Purpose:
 *		Stack Trace Frame.
 *
 * Copyright:
 *		2009, Johannes Passing (passing at users.sourceforge.net)
 *
 */

#include "cfixctlp.h"
#include <cdiag.h>

// {6157CFFD-3680-49ce-8BBC-2C82F3CE3FB0}
const GUID IID_ICfixStackTraceFrameInternal = 
	{ 0x6157cffd, 0x3680, 0x49ce, { 0x8b, 0xbc, 0x2c, 0x82, 0xf3, 0xce, 0x3f, 0xb0 } };

/*------------------------------------------------------------------
 * 
 * Class.
 *
 */

class StackTraceFrame :
	public ComObjectBase, 
	public ICfixStackTraceFrameInternal,
	public IMarshal
{
	DECLARE_NOT_COPYABLE( StackTraceFrame );

	BSTR ModuleName;
	BSTR FunctionName;
	BSTR SourceFile;
	ULONG Displacement;
	ULONG SourceLine;
	
protected:
	StackTraceFrame()
		: ModuleName( NULL )
		, FunctionName( NULL )
		, SourceFile( NULL )
		, Displacement( 0 )
		, SourceLine( 0 )
	{
	}

public:
	virtual ~StackTraceFrame()
	{
		if ( this->ModuleName )
		{
			SysFreeString( this->ModuleName );
		}

		if ( this->FunctionName )
		{
			SysFreeString( this->FunctionName );
		}

		if ( this->SourceFile )
		{
			SysFreeString( this->SourceFile );
		}
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
			 InlineIsEqualGUID( Iid, IID_ICfixStackTraceFrame ) )
		{
			*Ptr = static_cast< ICfixStackTraceFrame* >( this );
			Hr = S_OK;

		}
		else if ( InlineIsEqualGUID( Iid, IID_IMarshal ) )
		{
			*Ptr = static_cast< IMarshal* >( this );
			Hr = S_OK;
		}
		else if ( InlineIsEqualGUID( Iid, IID_ICfixStackTraceFrameInternal ) )
		{
			*Ptr = static_cast< ICfixStackTraceFrameInternal* >( this );
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

	STDMETHOD( GetModuleName )( 
		__out BSTR *Module 
		)
	{
		return ComGetProperty( this->ModuleName, Module );
	}

	STDMETHOD( GetFunctionName )( 
		__out BSTR *Function 
		)
	{
		return ComGetProperty( this->FunctionName, Function );
	}

	STDMETHOD( GetDisplacement )( 
		__out ULONG *Displacement 
		)
	{
		if ( ! Displacement )
		{
			return E_POINTER;
		}

		*Displacement = this->Displacement;
		return S_OK;
	}

	STDMETHOD( GetSourceFile )( 
		__out BSTR *Path 
		)
	{
		return ComGetProperty( this->SourceFile, Path );
	}

	STDMETHOD( GetSourceLine )( 
		__out ULONG *Line 
		)
	{
		if ( ! Line )
		{
			return E_POINTER;
		}

		*Line = this->SourceLine;
		return S_OK;
	}

	STDMETHOD( GetParameterCount )( 
		__out ULONG *Count 
		)
	{
		if ( ! Count )
		{
			return E_POINTER;
		}
		else
		{
			*Count = 0;
		}

		return E_NOTIMPL;
	}

	STDMETHOD( GetParameter )( 
		__in ULONG Index,
		__out BSTR *Name,
		__out BSTR *TypeName,
		__out BSTR *Value
		)
	{
		UNREFERENCED_PARAMETER( Index );

		if ( ! Name || ! TypeName || ! Value )
		{
			return E_POINTER;
		}
		else
		{
			*Name = NULL;
			*TypeName = NULL;
			*Value = NULL;
		}

		return E_NOTIMPL;
	}

	/*------------------------------------------------------------------
	 * ICfixStackTraceInternal methods.
	 */

	#pragma warning( push )
	#pragma warning( disable: 6054 )
	STDMETHOD( Initialize )(
		__in ULONGLONG FramePc,
		__in CFIX_GET_INFORMATION_STACKFRAME_ROUTINE GetInfFrameRoutine
		) 
	{
		if ( FramePc == 0 || ! GetInfFrameRoutine )
		{
			return E_INVALIDARG;
		}

		WCHAR ModuleName[ 64 ];
		WCHAR FunctionName[ 128];
		WCHAR SourceFile[ MAX_PATH ];

		HRESULT Hr = ( GetInfFrameRoutine )(
			FramePc,
			_countof( ModuleName ),
			ModuleName,
			_countof( FunctionName ),
			FunctionName,
			&this->Displacement,
			_countof( SourceFile ),
			SourceFile,
			&this->SourceLine );
		if ( Hr == HRESULT_FROM_WIN32( ERROR_INVALID_ADDRESS ) )
		{
			//
			// This can occur frequently when resolving failed.
			//
			// Provide pseudo values.
			//
			this->ModuleName = SysAllocString( L"[unknown]" );
			this->FunctionName = SysAllocString( L"[unknown]" );
			this->SourceFile = SysAllocString( L"[unknown]" );
			this->Displacement = 0;
			this->SourceLine = 0;
		}
		else if ( FAILED( Hr ) )
		{
			return Hr;
		}
		else
		{
			this->ModuleName = SysAllocString( ModuleName );
			this->FunctionName = SysAllocString( FunctionName );
			this->SourceFile = SysAllocString( SourceFile );
		}

		if ( ! this->ModuleName ||
			 ! this->FunctionName ||
			 ! this->SourceFile  )
		{
			return E_OUTOFMEMORY;
		}

		return S_OK;
	}
	#pragma warning( pop )

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
		*Clsid = CLSID_StackTraceFrame;
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
			*Size = 
				ComGetUnmarshalSizeMaxBSTR( this->ModuleName ) +
				ComGetUnmarshalSizeMaxBSTR( this->FunctionName ) +
				ComGetUnmarshalSizeMaxBSTR( this->SourceFile ) +
				sizeof( ULONG ) +	// Displacement.
				sizeof( ULONG );	// SourceLine
		
			return S_OK;
		}
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

		HRESULT Hr = ComMarshalBSTR( Stm, this->ModuleName );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		Hr = ComMarshalBSTR( Stm, this->FunctionName );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		Hr = ComMarshalBSTR( Stm, this->SourceFile );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		Hr = Stm->Write( &this->Displacement, sizeof( ULONG ), NULL );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		Hr = Stm->Write( &this->SourceLine, sizeof( ULONG ), NULL );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		return S_OK;
	}

	STDMETHOD( UnmarshalInterface )( 
		__in IStream *Stm,
		__in REFIID Iid,
		__out PVOID *Object
		)
	{
		ASSERT( this->ModuleName == NULL );
		ASSERT( this->FunctionName == NULL );
		ASSERT( this->SourceFile == NULL );

		if ( ! Object )
		{
			return E_POINTER;
		}
		else
		{
			*Object = NULL;
		}

		HRESULT Hr = ComUnmarshalBSTR( Stm, &this->ModuleName );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}
		ASSERT( this->ModuleName != NULL );

		Hr = ComUnmarshalBSTR( Stm, &this->FunctionName );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}
		ASSERT( this->FunctionName != NULL );

		Hr = ComUnmarshalBSTR( Stm, &this->SourceFile );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}
		ASSERT( this->SourceFile != NULL );

		Hr = Stm->Read( &this->Displacement, sizeof( ULONG ), NULL );
		if ( FAILED( Hr ) )
		{
			return Hr;
		}

		Hr = Stm->Read( &this->SourceLine, sizeof( ULONG ), NULL );
		if ( FAILED( Hr ) )
		{
			return Hr;
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

IClassFactory& CfixctlpGetStackTraceFrameFactory()
{
	static ComClassFactory< ComMtaObject< StackTraceFrame >, CfixctlServerLock > Factory;
	return Factory;
}