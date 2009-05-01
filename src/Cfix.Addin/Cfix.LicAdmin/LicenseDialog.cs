using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace Cfix.LicAdmin
{
	public partial class LicenseDialog : Form
	{
		public enum Mode
		{
			//
			// Trial period expired.
			//
			TrialExpired,

			//
			// Trial not expired yet.
			//
			License,

			//
			// Valid license; change key.
			//
			ChangeKey
		}

		private string FormatKey( string key )
		{
			if ( key.Length > 29 )
			{
				key = key.Substring( 0, 29 );
			}

			key = key.Replace( "-", "" );

			int index = 5;
			while ( index < key.Length )
			{
				key = key.Insert( index, "-" );
				index += 6;
			}

			return key;
		}

		private bool IsElevated()
		{
			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			WindowsPrincipal principal = new WindowsPrincipal( identity );
			return principal.IsInRole( WindowsBuiltInRole.Administrator );
		}

		public LicenseDialog() : this( Mode.License )
		{
		}

		public LicenseDialog( Mode mode )
		{
			InitializeComponent();

			switch ( mode )
			{
				case Mode.TrialExpired:
					this.infoLabel.Text = Strings.InfoExpired;
					break;

				case Mode.License:
					this.infoLabel.Text = Strings.InfoLicense;
					break;

				case Mode.ChangeKey:
					this.infoLabel.Text = Strings.InfoChangeKey;
					break;

				default:
					Debug.Fail( "Invalid mode" );
					break;
			}

			this.elevationInfoLabel.Visible = !IsElevated();
		}

		/*----------------------------------------------------------------------
		 * Events.
		 */

		private void purchaseButton_Click( object sender, EventArgs e )
		{
			try
			{
				Process proc = new Process();
				proc.StartInfo.FileName = "http://www.cfix-studio.com/go/buy";
				proc.Start();
			}
			catch ( Exception x )
			{
				MessageBox.Show( 
					x.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private void cancelButton_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void licenseKeyTextBox_TextChanged( object sender, EventArgs e )
		{
			int pos = licenseKeyTextBox.SelectionStart;

			string keyPre = licenseKeyTextBox.Text;
			string keyPost = FormatKey( keyPre );

			licenseKeyTextBox.Text = keyPost.ToUpper();
			licenseKeyTextBox.SelectionStart = 
				Math.Max( 0, pos + ( keyPost.Length - keyPre.Length ) );


			//
			// If fully entered, validate.
			//
			this.okButton.Enabled =
				keyPost.Length == 29 &&
				Native.CfixctlValidateLicense( keyPost ) == 0;
		}

		private void okButton_Click( object sender, EventArgs e )
		{
			try
			{
				Process proc = new Process();
				proc.StartInfo.FileName = 
					Assembly.GetExecutingAssembly().Location;

				proc.StartInfo.Arguments = "install " + this.licenseKeyTextBox.Text;
				if ( !IsElevated() )
				{
					proc.StartInfo.Verb = "runas";
				}

				proc.Start();
				Close();
			}
			catch ( Exception x )
			{
				MessageBox.Show(
					x.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

	}
}