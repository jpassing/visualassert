/*----------------------------------------------------------------------
 * Purpose:
 *		Test TestCase class.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <testctlp.h>

const GUID IID_ICfixStackTraceInternal = 
	{ 0x6dca5f7c, 0xab9a, 0x4262, { 0x8f, 0x11, 0xa4, 0xde, 0x27, 0x1e, 0xc1, 0x52 } };

static HRESULT CFIXCALLTYPE PseudoGetInformationStackframe(
	__in ULONGLONG Frame,
	__in SIZE_T ModuleNameCch,
	__out_ecount(ModuleNameCch) PWSTR ModuleName,
	__in SIZE_T FunctionNameCch,
	__out_ecount(FunctionNameCch) PWSTR FunctionName,
	__out PDWORD Displacement,
	__in SIZE_T SourceFileCch,
	__out_ecount(SourceFileCch) PWSTR SourceFile,
	__out PDWORD SourceLine 
	)
{
	HRESULT Hr;
	if ( FAILED( Hr = StringCchCopy( ModuleName, ModuleNameCch, L"mod" ) ) )
	{
		return Hr;
	}

	if ( FAILED( Hr = StringCchCopy( FunctionName, FunctionNameCch, L"func" ) ) )
	{
		return Hr;
	}

	if ( FAILED( Hr = StringCchCopy( SourceFile, SourceFileCch, L"f.c" ) ) )
	{
		return Hr;
	}

	*SourceLine = ( ULONG ) Frame;
	*Displacement = ( ULONG ) Frame * 2;

	return S_OK;
}

class TestStackTrace : public cfixcc::TestFixture
{
private:
	static COM_EXPORTS Exports;
	IClassFactory *Factory;

public:
	static void SetUp()
	{
		CoInitialize( NULL );
		GetComExports( L"cfixctl.dll", &Exports );
	}

	static void TearDown()
	{
		CoUninitialize();
	}

	virtual void Before()
	{
		CFIX_ASSERT_OK( Exports.GetClassObject( 
			CLSID_StackTrace, IID_IClassFactory, ( PVOID* ) &this->Factory ) );
		CFIXCC_ASSERT( this->Factory );
	}

	virtual void After()
	{
		if ( this->Factory )
		{
			this->Factory->Release();
		}
	}

	void MarshalEmptyTrace()
	{
		ICfixStackTraceInternal *Trace;
		CFIX_ASSERT_OK( this->Factory->CreateInstance( 
			NULL, IID_ICfixStackTraceInternal, ( PVOID* ) &Trace ) );

		CFIX_STACKTRACE StackTrace = { 0 };

		CFIX_ASSERT_OK( Trace->Initialize( 
			&StackTrace,
			PseudoGetInformationStackframe ) );

		ULONG Count;
		CFIX_ASSERT_OK( Trace->GetFrameCount( &Count ) );
		CFIXCC_ASSERT_EQUALS( 0UL, Count );

		//
		// Marshal.
		//
		IMarshal *Mrsh;
		CFIX_ASSERT_OK( Trace->QueryInterface( 
			IID_IMarshal, ( PVOID* ) &Mrsh ) );

		IStream *Stm;
		CFIX_ASSERT_OK( CreateStreamOnHGlobal( NULL, TRUE, &Stm ) );

		//
		// Note: Requires TLB to be registered.
		//
		CFIX_ASSERT_OK( Mrsh->MarshalInterface(
			Stm,
			IID_ICfixStackTrace,
			Trace,
			0,
			NULL,
			0 ) );

		Trace->Release();
		CFIXCC_ASSERT_EQUALS( 0UL, Mrsh->Release() );

		//
		// Reposition stream.
		//
		LARGE_INTEGER Zero;
		Zero.QuadPart = 0;
		CFIX_ASSERT_OK( Stm->Seek( Zero, STREAM_SEEK_SET, NULL ) );

		//
		// Unmarshal.
		//
		IMarshal *Unmrsh;
		CFIX_ASSERT_OK( this->Factory->CreateInstance( 
			NULL, IID_IMarshal, ( PVOID* ) &Unmrsh ) );

		ICfixStackTrace *Trace2;
		CFIX_ASSERT_OK( Unmrsh->UnmarshalInterface(
			Stm,
			IID_ICfixStackTrace,
			( PVOID* ) &Trace2 ) );

		Unmrsh->Release();
		Stm->Release();

		CFIX_ASSERT_OK( Trace2->GetFrameCount( &Count ) );
		CFIXCC_ASSERT_EQUALS( 0UL, Count );
		CFIXCC_ASSERT_EQUALS( 0UL, Trace2->Release() );
	}

	void MarshalNonEmptyTrace()
	{
		ICfixStackTraceInternal *Trace;
		CFIX_ASSERT_OK( this->Factory->CreateInstance( 
			NULL, IID_ICfixStackTraceInternal, ( PVOID* ) &Trace ) );

		PCFIX_STACKTRACE StackTrace = ( PCFIX_STACKTRACE ) malloc(
			RTL_SIZEOF_THROUGH_FIELD(
				CFIX_STACKTRACE, Frames[ 3 ] ) );
		CFIX_ASSUME( StackTrace );

		StackTrace->FrameCount = 3;
		StackTrace->Frames[ 0 ] = 10;
		StackTrace->Frames[ 1 ] = 100;
		StackTrace->Frames[ 2 ] = 1000;

		CFIX_ASSERT_OK( Trace->Initialize( 
			StackTrace,
			PseudoGetInformationStackframe ) );

		ULONG Count;
		CFIX_ASSERT_OK( Trace->GetFrameCount( &Count ) );
		CFIXCC_ASSERT_EQUALS( 3UL, Count );

		//
		// Marshal.
		//
		IMarshal *Mrsh;
		CFIX_ASSERT_OK( Trace->QueryInterface( 
			IID_IMarshal, ( PVOID* ) &Mrsh ) );

		IStream *Stm;
		CFIX_ASSERT_OK( CreateStreamOnHGlobal( NULL, TRUE, &Stm ) );

		//
		// Note: Requires TLB to be registered.
		//
		CFIX_ASSERT_OK( Mrsh->MarshalInterface(
			Stm,
			IID_ICfixStackTrace,
			Trace,
			0,
			NULL,
			0 ) );

		Trace->Release();
		CFIXCC_ASSERT_EQUALS( 0UL, Mrsh->Release() );

		//
		// Reposition stream.
		//
		LARGE_INTEGER Zero;
		Zero.QuadPart = 0;
		CFIX_ASSERT_OK( Stm->Seek( Zero, STREAM_SEEK_SET, NULL ) );

		//
		// Unmarshal.
		//
		IMarshal *Unmrsh;
		CFIX_ASSERT_OK( this->Factory->CreateInstance( 
			NULL, IID_IMarshal, ( PVOID* ) &Unmrsh ) );

		ICfixStackTrace *Trace2;
		CFIX_ASSERT_OK( Unmrsh->UnmarshalInterface(
			Stm,
			IID_ICfixStackTrace,
			( PVOID* ) &Trace2 ) );

		Unmrsh->Release();
		Stm->Release();

		CFIX_ASSERT_OK( Trace2->GetFrameCount( &Count ) );
		CFIXCC_ASSERT_EQUALS( 3UL, Count );

		for ( ULONG Index = 0; Index < Count; Index++ )
		{
			ICfixStackTraceFrame *Frame;
			CFIX_ASSERT_OK( Trace2->GetFrame( Index, &Frame ) );

			BSTR Module;
			CFIX_ASSERT_OK( Frame->GetModuleName( &Module ) );
			CFIXCC_ASSERT_EQUALS( L"mod", ( PCWSTR ) Module );
			SysFreeString( Module );

			BSTR Func;
			CFIX_ASSERT_OK( Frame->GetFunctionName( &Func ) );
			CFIXCC_ASSERT_EQUALS( L"func", ( PCWSTR ) Func );
			SysFreeString( Func );

			BSTR File;
			CFIX_ASSERT_OK( Frame->GetSourceFile( &File ) );
			CFIXCC_ASSERT_EQUALS( L"f.c", ( PCWSTR ) File );
			SysFreeString( File );

			ULONG Line;
			CFIX_ASSERT_OK( Frame->GetSourceLine( &Line ) );
			CFIXCC_ASSERT_EQUALS( ( ULONG ) StackTrace->Frames[ Index ], Line );
			
			ULONG Disp;
			CFIX_ASSERT_OK( Frame->GetDisplacement( &Disp ) );
			CFIXCC_ASSERT_EQUALS( ( ULONG ) StackTrace->Frames[ Index ] * 2, Disp );
			
			Frame->Release();
		}

		free( StackTrace );

		CFIXCC_ASSERT_EQUALS( 0UL, Trace2->Release() );
	}
};


COM_EXPORTS TestStackTrace::Exports;

CFIXCC_BEGIN_CLASS( TestStackTrace )
	CFIXCC_METHOD( MarshalEmptyTrace )
	CFIXCC_METHOD( MarshalNonEmptyTrace )
CFIXCC_END_CLASS()