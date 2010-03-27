// ExportingDll.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"

EXTERN_C
__declspec( dllexport )
BOOL CallMe()
{
	return TRUE;
}
