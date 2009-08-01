using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Cfix.Control;
using Cfix.Control.Native;
using Cfix.Addin.Test;
using EnvDTE;
using EnvDTE80;

namespace Cfix.Addin.Windows
{
	internal static class Wizards
	{
		private const int VS_E_WIZARDBACKBUTTONPRESS = 
			unchecked( ( int ) 0x80041fff );

		public static bool CanAddFixture( ITestItem item )
		{
			if ( item is InvalidModule )
			{
				return false;
			}
			else if ( item is VCProjectTestCollection )
			{
				return true;
			}
			else if ( item.Parent != null &&
					item.Parent is VCProjectTestCollection )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void LaunchAddFixtureWizard(
			DTE2 dte,
			ITestItem parentItem
			)
		{
			VCProjectTestCollection project = parentItem as VCProjectTestCollection;
			if ( project == null && (
				 parentItem.Parent == null ||
				 ( project = parentItem.Parent as VCProjectTestCollection ) == null ) )
			{
				throw new ArgumentException( "Cannot add fixture to this node" );
			}

			Debug.Assert( project != null );
			LaunchAddFixtureWizard( dte, project.Project );
		}

		public static void LaunchAddFixtureWizard(
			DTE2 dte,
			Project project
			)
		{
			DirectoryInfo projectDir = new FileInfo( project.FullName ).Directory;
			
			string vszPath = Directories.GetVcAddUnitTestVszPath( dte );
			object[] wizardParams = new object[]
				{
					"{0F90E1D0-4999-11D1-B6D1-00A0C90F2744}",
					project.Name,
					projectDir.FullName, 
					Directories.GetVsDirectory( dte ), 
					false,
					"",
					false
				};

			try
			{
				dte.LaunchWizard(
					vszPath,
					ref wizardParams );
			}
			catch ( COMException x )
			{
				if ( x.ErrorCode == VS_E_WIZARDBACKBUTTONPRESS )
				{
					//
					// Cancelled.
					//
				}
				else
				{
					throw;
				}
			}
		}
	}
}
