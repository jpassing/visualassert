/*----------------------------------------------------------------------
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#include <windows.h>
#include <stdlib.h>

int __cdecl wmain( int Argc, LPWSTR *Args )
{
	if ( Argc < 2 )
	{
		return EXIT_FAILURE;
	}

	if ( Argc == 3 && 0 == wcscmp( Args[ 1 ], L"run" ) )
	{
		STARTUPINFO StartupInfo;
		ZeroMemory( &StartupInfo, sizeof( StartupInfo ) );
		StartupInfo.cb = sizeof( STARTUPINFO );

		PROCESS_INFORMATION ProcessInfo;

		if ( ! CreateProcess(
			NULL,
			Args[ 2 ],
			NULL,
			NULL,
			FALSE,
			0,
			NULL,
			NULL,
			&StartupInfo,
			&ProcessInfo ) )
		{
			return GetLastError();
		}

		WaitForSingleObject( ProcessInfo.hProcess, INFINITE );

		DWORD ExitCode;
		GetExitCodeProcess( ProcessInfo.hProcess, &ExitCode );

		CloseHandle( ProcessInfo.hProcess );
		CloseHandle( ProcessInfo.hThread );

		return ExitCode;
	}
	else
	{
		return EXIT_FAILURE;
	}
}