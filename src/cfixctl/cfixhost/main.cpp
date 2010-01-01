/*----------------------------------------------------------------------
 * Purpose:
 *		Entry Point.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <windows.h>
#include <ole2.h>
#include <cfixctl.h>
#include <cfixctlsvr.h>
#include <crtdbg.h>

/*----------------------------------------------------------------------
 *
 * Heap compatibility routines.
 *
 */

#ifndef _WIN64

#ifndef HeapEnableTerminationOnCorruption
#   define HeapEnableTerminationOnCorruption ( HEAP_INFORMATION_CLASS ) 1
#endif

typedef BOOL ( * HEAPSETINFORMATION_ROUTINE )(
	__in HANDLE HeapHandle,
	__in HEAP_INFORMATION_CLASS HeapInformationClass,
	__in_opt PVOID HeapInformation,
	__in SIZE_T HeapInformationLength
	);

static BOOL CfixctlsHeapSetInformation(
	__in HANDLE HeapHandle,
	__in HEAP_INFORMATION_CLASS HeapInformationClass,
	__in_opt PVOID HeapInformation,
	__in SIZE_T HeapInformationLength
	)
{
	HMODULE Kernel32Module = GetModuleHandle( L"kernel32" );
	_ASSERTE( Kernel32Module != NULL );
	__assume( Kernel32Module != NULL );

	HEAPSETINFORMATION_ROUTINE Routine = ( HEAPSETINFORMATION_ROUTINE ) 
		GetProcAddress( 
			Kernel32Module,
			"HeapSetInformation" );

	if ( Routine != NULL )
	{
		//
		// Windows XP or above.
		//
		return ( Routine )( 
			HeapHandle, 
			HeapInformationClass,
			HeapInformation,
			HeapInformationLength );
	}
	else
	{
		//
		// Windows 2000 or below.
		//
		return TRUE;
	}
}

#endif

int wWinMain(
	__in HINSTANCE Instance,
    __in HINSTANCE PrevInstance,
    __in PWSTR CmdLine,
    __in int CmdShow
	)
{
	UNREFERENCED_PARAMETER( Instance );
	UNREFERENCED_PARAMETER( PrevInstance );
	UNREFERENCED_PARAMETER( CmdLine );
	UNREFERENCED_PARAMETER( CmdShow );

	//
	// No 'drive not ready'-dialogs, please.
	//
	SetErrorMode( SetErrorMode( 0 ) | SEM_FAILCRITICALERRORS );

#ifndef _WIN64
	//
	// Fail early on heap corruptions.
	//
	( void ) CfixctlsHeapSetInformation(
		GetProcessHeap(), 
		HeapEnableTerminationOnCorruption, 
		NULL, 
		0 );
#endif
	
	return CfixctlServeHost();
}