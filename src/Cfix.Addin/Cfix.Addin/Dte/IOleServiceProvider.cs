using System;
using System.Runtime.InteropServices;

namespace Cfix.Addin.Dte
{
	[
		ComImport,
		Guid("6D5140C1-7436-11CE-8034-00AA006009FA"),
		InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)
	]
	internal interface IOleServiceProvider
	{
	   [PreserveSig]
	   int QueryService(
			[In]ref Guid guidService, 
			[In]ref Guid riid, 
			[MarshalAs(UnmanagedType.Interface)] out System.Object obj);
	}
}
