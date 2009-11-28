using System;
using System.Collections.Generic;
using System.Text;
using Cfix.Control;

namespace Cfix.Addin
{
	internal static class ArchitectureUtil
	{
		public static Architecture NativeArchitecture
		{
			get
			{
				//
				// N.B. Use CfixklGetNativeSystemInfo for downlevel compat.
				//
				Native.SYSTEM_INFO info = new Native.SYSTEM_INFO();
				Native.CfixklGetNativeSystemInfo( ref info );

				switch ( info.processorArchitecture )
				{
					case Native.PROCESSOR_ARCHITECTURE_AMD64:
						return Architecture.Amd64;

					case Native.PROCESSOR_ARCHITECTURE_INTEL:
						return Architecture.I386;

					default:
						throw new CfixAddinException(
							Strings.UnsupportedArchitecture );
				}
			}
		}

		public static bool Is64bitSupported
		{
			get
			{
				return NativeArchitecture == Architecture.Amd64;
			}
		}
	}
}
