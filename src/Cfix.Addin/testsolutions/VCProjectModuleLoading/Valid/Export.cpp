#include <stdio.h>
#include <windows.h>

EXTERN_C
__declspec( dllexport )
BOOL ValidExport()
{
	printf( "ValidExport\n" );
	return TRUE;
}
