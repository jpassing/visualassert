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

namespace Cfix.Addin.Windows
{
	public partial class AboutWindow : Form
	{
		private string licadminArg = "";
		private readonly Workspace workspace;

		private void PopulateFileVersionsList( Architecture arch )
		{
			foreach ( string file in Directories.GetBinaries( arch ) )
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

		private void PopulateFileVersionsList()
		{
			PopulateFileVersionsList( Architecture.I386 );
			PopulateFileVersionsList( Architecture.Amd64 );
		}

		private void PopulateLicenseInfo( Workspace workspace )
		{
			try
			{
				LicenseInfo licInfo = workspace.QueryLicenseInfo();

#if BETA
				if ( licInfo.IsTrial )
				{
					if ( licInfo.Valid )
					{
						this.licenseValueLabel.Text = String.Format(
							Strings.BetaLicenseValidWithExp, licInfo.TrialDaysLeft );
						this.licadminArg = "license";
					}
					else
					{
						this.licenseValueLabel.Text = Strings.BetaLicenseInvalid;
						this.licadminArg = "expired";
					}
				}
#else
				if ( licInfo.IsTrial )
				{
					if ( licInfo.Valid )
					{
						this.licenseValueLabel.Text = String.Format(
							Strings.TrialLicenseValid, licInfo.TrialDaysLeft );
						this.licadminArg = "license";
					}
					else
					{
						this.licenseValueLabel.Text =
							Strings.TrialLicenseInalid;
						this.licadminArg = "expired";
					}
				}
#endif
				else
				{
					if ( licInfo.Valid )
					{
						this.licenseValueLabel.Text = licInfo.Key;
						this.licadminArg = "changekey";
					}
					else
					{
						this.licenseValueLabel.Text = "(Invalid)";
						this.licadminArg = "license";
					}
				}
			}
			catch 
			{
				this.licenseValueLabel.Text = "(Invalid)";
				this.licadminArg = "license";
			}
		}

		private void linkLabel_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			CommonUiOperations.OpenHomepage();
		}

		public AboutWindow() : this( null )
		{
		}

		public AboutWindow( Workspace ws )
		{
			InitializeComponent();

#if BETA
			this.licenseLabel.Text = "Info:";
			this.enterLicenseButton.Visible = false;
#elif FREEWARE
			this.licenseLabel.Visible = false;
			this.licenseValueLabel.Visible = false;
			this.enterLicenseButton.Visible = false;
#endif

			this.workspace = ws;

			this.versionLabel.Text += VisualAssert.Version;

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
					VisualAssert.HandleError( x );
				}
			}
		}

		private void enterLicenseButton_Click( object sender, EventArgs e )
		{
			Debug.Assert( this.workspace != null );
			this.workspace.ToolWindows.LaunchLicenseAdmin( this.licadminArg );
		}
	}
}