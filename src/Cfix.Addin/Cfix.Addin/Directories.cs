/*----------------------------------------------------------------------
 * Purpose:
 *		Common directory lookup code.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using Cfix.Control;
using EnvDTE80;

namespace Cfix.Addin
{
	internal static class Directories
	{
		private static string BuildEnvironment
		{
			get
			{
				string imagePath = typeof( Directories ).Assembly.Location;

				string dirName = new DirectoryInfo( imagePath ).Parent.Parent.Name;
				if ( dirName == "chk" || dirName == "fre" )
				{
					return dirName;
				}
				else
				{
					return null;
				}
			}
		}

		public static string InstallationDirectory
		{
			get
			{
				string imagePath = typeof( Directories ).Assembly.Location;

				DirectoryInfo dir = new DirectoryInfo( imagePath ).Parent.Parent.Parent;
				if ( dir.Name == "bin" )
				{
					//
					// Dev environment.
					//
					dir = dir.Parent;
				}

				return dir.FullName;
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
			string env = BuildEnvironment;
			if ( env != null )
			{
				return GetBinDirectory( arch );
			}
			else
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
		}

		public static string GetBinDirectory( Architecture arch )
		{
			string env = BuildEnvironment;
			if ( env == null )
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
			else
			{
				switch ( arch )
				{
					case Architecture.I386:
						return InstallationDirectory + "\\bin\\" + env +"\\i386";

					case Architecture.Amd64:
						return InstallationDirectory + "\\bin\\" + env + "\\amd64";

					default:
						throw new ArgumentException();
				}
			}
		}

		public static string GetVcDirectory( DTE2 dte )
		{
			using ( RegistryKey key = Registry.LocalMachine.OpenSubKey( 
				dte.RegistryRoot + "\\Setup\\VC" ) )
			{
				return ( string ) key.GetValue( "ProductDir" );
			}
		}

		public static string GetVsDirectory( DTE2 dte )
		{
			using ( RegistryKey key = Registry.LocalMachine.OpenSubKey(
				dte.RegistryRoot ) )
			{
				string ideDir = ( string ) key.GetValue( "InstallDir" );
				return new DirectoryInfo( ideDir ).Parent.Parent.FullName;
			}
		}

		public static string GetVcAddUnitTestVszPath( DTE2 dte )
		{
			return GetVcDirectory( dte ) + "VcAddClass\\cfix\\fixture.vsz";
		}

		public static string LogDirectory
		{
			get
			{
				return Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ) +
					"\\cfixstudio\\log";
			}
		}

		public static string QuickStartPage
		{
			get
			{
				return DocDirectory + "\\QuickStart\\index.html";
			}
		}
	}
}
