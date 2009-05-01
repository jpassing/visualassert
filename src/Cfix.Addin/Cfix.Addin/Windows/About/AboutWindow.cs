using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Addin.Windows;
using System.Runtime.InteropServices;

namespace Cfix.Addin.Windows.About
{
	public partial class AboutWindow : Form
	{
		private string licadminArg = "";

		private void PopulateFileVersionsList( Architecture arch )
		{
			string dir = Directories.GetBinDirectory( arch );
			if ( !Directory.Exists( dir ) )
			{
				return;
			}

			foreach ( string file in Directory.GetFiles( dir ) )
			{
				if ( file.EndsWith( ".sys", StringComparison.OrdinalIgnoreCase ) ||
					 file.EndsWith( ".dll", StringComparison.OrdinalIgnoreCase ) ||
					 file.EndsWith( ".exe", StringComparison.OrdinalIgnoreCase ) )
				{
					Native.CDIAG_MODULE_VERSION version =
						new	Native.CDIAG_MODULE_VERSION();
					string versionString = String.Empty;
					if ( 0 == Native.CdiagGetModuleVersion(
						file,
						ref version ) )
					{
						versionString = version.ToString();
					}

					this.fileVersionsList.Items.Add(
						new ListViewItem(
							new string[] {
								new FileInfo( file ).Name,
								arch.ToString(),
								versionString } ) );
				}
			}
		}

		private void PopulateFileVersionsList()
		{
			PopulateFileVersionsList( Architecture.I386 );
			PopulateFileVersionsList( Architecture.Amd64 );
		}

		private void PopulateLicenseInfo( Workspace workspace )
		{
			Native.CFIXCTL_LICENSE_INFO license = workspace.License;

			switch ( license.Type )
			{
				case Native.CFIXCTL_LICENSE_TYPE.CfixctlLicensed:
					this.licenseValueLabel.Text = license.Key;
					this.licadminArg = "changekey";
					break;

				case Native.CFIXCTL_LICENSE_TYPE.CfixctlTrial:
					if ( license.Valid )
					{
						this.licenseValueLabel.Text = String.Format(
							Strings.TrialLicenseValid, license.DaysLeft );
						this.licadminArg = "license";
					}
					else
					{
						this.licenseValueLabel.Text =
							Strings.TrialLicenseInalid;
						this.licadminArg = "expired";
					}
					break;

				default:
					Debug.Fail( "Invalid license type" );
					break;
			}
		}

		private void linkLabel_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			try
			{
				Process proc = new Process();
				proc.StartInfo.FileName = "http://www.cfix-studio.com/";
				proc.Start();
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
			}
		}

		public AboutWindow() : this( null )
		{
		}

		public AboutWindow( Workspace ws )
		{
			InitializeComponent();

			Version version = GetType().Assembly.GetName().Version;
			this.versionLabel.Text += String.Format(
				"{0}.{1}.{2} Build {3}",
				version.Major,
				version.Minor,
				version.MajorRevision,
				version.MinorRevision );

			try
			{
				PopulateFileVersionsList();
			}
			catch
			{ }

			if ( ws != null )
			{
				try
				{
					PopulateLicenseInfo( ws );
				}
				catch ( Exception x )
				{
					CfixPlus.HandleError( x );
				}
			}
		}

		private void enterLicenseButton_Click( object sender, EventArgs e )
		{
			try
			{
				Process proc = new Process();
				proc.StartInfo.FileName =
					Directories.GetBinDirectory( Architecture.I386 ) +
					"\\licadmin.exe";
				proc.StartInfo.Arguments = this.licadminArg;
				proc.Start();
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
			}
		}
	}
}