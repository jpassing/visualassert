/*----------------------------------------------------------------------
 * Purpose:
 *		Compatibility routines.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include "cfixctlp.h"

typedef BOOL ( * ISPROCESSINJOB_ROUTINE )(
	__in HANDLE ProcessHandle,
	__in HANDLE JobHandle,
	__out PBOOL Result
	);

BOOL CfixctlpIsProcessInJob(
	__in HANDLE ProcessHandle,
	__in HANDLE JobHandle,
	__out PBOOL Result
	)
{
	HMODULE Kernel32Module = GetModuleHandle( L"kernel32" );
	ASSERT( Kernel32Module != NULL );

	ISPROCESSINJOB_ROUTINE Routine = ( ISPROCESSINJOB_ROUTINE ) GetProcAddress( 
		Kernel32Module,
		"IsProcessInJob" );

	if ( Routine != NULL )
	{
		//
		// Windows XP or above.
		//
		return ( Routine )( ProcessHandle, JobHandle, Result );
	}
	else
	{
		//
		// Windows 2000 or below - assume FALSE
		//
		*Result = FALSE;
		return TRUE;
	}
}