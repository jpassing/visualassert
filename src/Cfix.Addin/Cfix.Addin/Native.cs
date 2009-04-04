using System;
using System.Runtime.InteropServices;

namespace Cfix.Addin
{
	internal static class Native
	{
		[StructLayout( LayoutKind.Sequential )]
		public struct SYSTEM_INFO
		{
			public ushort processorArchitecture;
			ushort reserved;
			public uint pageSize;
			public IntPtr minimumApplicationAddress;
			public IntPtr maximumApplicationAddress;
			public IntPtr activeProcessorMask;
			public uint numberOfProcessors;
			public uint processorType;
			public uint allocationGranularity;
			public ushort processorLevel;
			public ushort processorRevision;
		}

		public const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
		public const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;

		[DllImport( "cfixkl.dll" )]
		public static extern void CfixklGetNativeSystemInfo( 
			ref SYSTEM_INFO SystemInfo 
			);
	}
}
