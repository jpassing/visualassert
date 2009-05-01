using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Cfix.LicAdmin
{
	static class Program
	{
		private static void InstallLicense( string key )
		{
			int hr = Native.CfixctlInstallLicense( true, key );
			if ( hr != 0 )
			{
				throw new Win32Exception( hr );
			}
		}

		[STAThread]
		static void Main( string[] argv )
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			
			if ( argv.Length == 2 && argv[ 0 ] == "install" )
			{
				try
				{
					InstallLicense( argv[ 1 ] );
					Application.Run( new ThankYouDialog() );
				}
				catch ( Exception x )
				{
					MessageBox.Show(
						String.Format( Strings.InstallFailed, x.Message ), 
						Strings.InstallFailedCaption, 
						MessageBoxButtons.OK, 
						MessageBoxIcon.Error );
				}
			}
			else
			{
				LicenseDialog.Mode mode;
				if ( argv.Length == 0 )
				{
					mode = LicenseDialog.Mode.License;
				}
				else
				{
					switch ( argv[ 0 ] )
					{
						case "expired":
							mode = LicenseDialog.Mode.TrialExpired;
							break;

						case "license":
							mode = LicenseDialog.Mode.License;
							break;

						case "changekey":
							mode = LicenseDialog.Mode.ChangeKey;
							break;

						default:
							return;
					}
				}

				Application.Run( new LicenseDialog( mode ) );
			}
		}
	}
}