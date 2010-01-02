using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Cfix.Addin;
using Cfix.Control;

namespace Cfix.Addin.Windows
{
	public partial class UpdateCheckWindow : Form
	{
		private delegate VersionInfo ReadCurrentVersionInfoDelegate();
		private delegate void VoidDelegate();

		private IWin32Window parentWindow = null;

		public IWin32Window ParentWindow
		{
			set { this.parentWindow = value; }
			get { return this.parentWindow; }
		}

		private void ReadCurrentVersionInfoCallback( IAsyncResult ar )
		{
			ReadCurrentVersionInfoDelegate dlg =
				( ReadCurrentVersionInfoDelegate ) ar.AsyncState;

			Debug.Assert( dlg != null );

			this.Invoke( ( VoidDelegate ) delegate
			{
				try
				{
					this.Hide();

					VersionInfo currentVersion = dlg.EndInvoke( ar );

					if ( currentVersion.IsNewer )
					{
						DialogResult result = MessageBox.Show(
							this.ParentWindow,
							String.Format(
								Strings.NewVersionAvailable,
								currentVersion.Version.ToString() ),
							Strings.UpdateCheckCaption,
							MessageBoxButtons.YesNo );
						if ( result == DialogResult.Yes )
						{
							CommonUiOperations.OpenBrowser(
								currentVersion.DownloadUrl );
						}
						else
						{
							//
							// Cancelled.
							//
						}
					}
					else
					{
						MessageBox.Show(
							this.ParentWindow,
							Strings.NoNewVersionAvailable,
							Strings.UpdateCheckCaption,
							MessageBoxButtons.OK );
					}
				}
				catch ( Exception x )
				{
					Logger.LogError( "UpdateCheck", x );

					DialogResult result = MessageBox.Show(
						this.ParentWindow,
						Strings.UpdateCheckFailed,
						Strings.UpdateCheckCaption,
						MessageBoxButtons.YesNo );
					if ( result == DialogResult.Yes )
					{
						CommonUiOperations.OpenHomepage();
					}
				}
				finally
				{
					this.Close();
				}
			} );
		}

		public UpdateCheckWindow()
		{
			InitializeComponent();
		}

		public static void CheckForUpdate( IWin32Window parentWnd )
		{
			ReadCurrentVersionInfoDelegate checkDelegate = 
				UpdateCheck.ReadCurrentVersionInfo;

			using ( UpdateCheckWindow window = new UpdateCheckWindow() )
			{
				window.parentWindow = parentWnd;

				//
				// Perform check asynchronously s.t. UI stays responsive.
				//
				checkDelegate.BeginInvoke(
					window.ReadCurrentVersionInfoCallback,
					checkDelegate );

				window.ShowDialog();
			}
		}
	
	}
}