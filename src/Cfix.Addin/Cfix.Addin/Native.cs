/*----------------------------------------------------------------------
 * Purpose:
 *		Native wrappers.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

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

		[StructLayout( LayoutKind.Sequential )]
		public struct CDIAG_MODULE_VERSION
		{
			public ushort Major;
			public ushort Minor;
			public ushort Revision;
			public ushort Build;

			public override string ToString()
			{
				return String.Format(
					"{0}.{1}.{2}.{3}",
					this.Major,
					this.Minor,
					this.Revision,
					this.Build );
			}
		}

		[DllImport( "cdiag.dll", CharSet=CharSet.Unicode )]
		public static extern uint CdiagGetModuleVersion(
			string modulePath,
			ref CDIAG_MODULE_VERSION Version
			);
	}
}
