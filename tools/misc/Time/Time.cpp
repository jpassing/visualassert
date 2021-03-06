// Time.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <windows.h>

static ULONG CfixctlsDaysFromFileTime( 
	__in FILETIME Ft 
	)
{
	ULARGE_INTEGER Int;
	Int.HighPart = Ft.dwHighDateTime;
	Int.LowPart = Ft.dwLowDateTime;

	return ( ULONG ) ( Int.QuadPart 
		/ 10	// us
		/ 1000	// ms
		/ 1000	// s
		/ 60	// m
		/ 60	// h
		/ 24 );	// d
}

int _tmain(int argc, _TCHAR* argv[])
{
	SYSTEMTIME t;
	ZeroMemory( &t, sizeof( SYSTEMTIME ) );
	t.wYear = 2010;
	t.wMonth = 2;
	t.wDay = 28;

	FILETIME Ft;
	SystemTimeToFileTime( &t, &Ft );

	ULONG Days = CfixctlsDaysFromFileTime( Ft );
	return 0;
}

