/*----------------------------------------------------------------------
 * Purpose:
 *		Compatibility routines.
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