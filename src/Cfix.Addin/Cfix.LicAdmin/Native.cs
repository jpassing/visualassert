/*----------------------------------------------------------------------
 * Purpose:
 *		Native wrappers.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Runtime.InteropServices;

namespace Cfix.LicAdmin
{
	internal static class Native
	{
		public enum CFIXCTL_LICENSE_TYPE : uint
		{
			CfixctlLicensed = 0,
			CfixctlTrial = 1
		}

		[StructLayout( LayoutKind.Sequential, CharSet=CharSet.Unicode )]
		public struct CFIXCTL_LICENSE_INFO
		{
			public uint SizeOfStruct;
			public CFIXCTL_LICENSE_TYPE Type;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 30 )]
			public string Key;

			public byte Product;
			public byte SubProduct;
			public bool Valid;
			public uint DaysInstalled;
			public uint DaysLeft;
		}

		[DllImport( "cfixctl.dll", CharSet = CharSet.Unicode )]
		public static extern int CfixctlQueryLicenseInfo(
			bool MachineWide,
			uint ExternalDateOfInstallation,
			ref CFIXCTL_LICENSE_INFO Info
			);

		[DllImport( "cfixctl.dll", CharSet = CharSet.Unicode )]
		public static extern int CfixctlValidateLicense(
			string Key
			);

		[DllImport( "cfixctl.dll", CharSet = CharSet.Unicode )]
		public static extern int CfixctlInstallLicense(
			bool MachineWide,
			string Key
			);
	}
}
