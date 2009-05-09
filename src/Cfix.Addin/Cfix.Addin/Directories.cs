/*----------------------------------------------------------------------
 * Purpose:
 *		Common directory lookup code.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.IO;
using System.Reflection;
using Cfix.Control;

namespace Cfix.Addin
{
	internal static class Directories
	{
		public static string InstallationDirectory
		{
			get
			{
				string imagePath = typeof( Directories ).Assembly.Location;
				return new DirectoryInfo( imagePath ).Parent.Parent.Parent.FullName;
			}
		}

		public static string IncludeDirectory
		{
			get
			{
				return InstallationDirectory + "\\include";
			}
		}

		public static string DocDirectory
		{
			get
			{
				return InstallationDirectory + "\\doc";
			}
		}

		public static string GetLibDirectory( Architecture arch )
		{
			switch ( arch )
			{
				case Architecture.I386:
					return InstallationDirectory + "\\lib\\i386";

				case Architecture.Amd64:
					return InstallationDirectory + "\\lib\\amd64";

				default:
					throw new ArgumentException();
			}
		}

		public static string GetBinDirectory( Architecture arch )
		{
			switch ( arch )
			{
				case Architecture.I386:
					return InstallationDirectory + "\\bin\\i386";

				case Architecture.Amd64:
					return InstallationDirectory + "\\bin\\amd64";

				default:
					throw new ArgumentException();
			}
		}
	}
}
