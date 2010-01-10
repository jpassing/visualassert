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

		private void ReadCurrentVersionInfoCallback( IAsyncResult ar )
		{
			ReadCurrentVersionInfoDelegate dlg =
				( ReadCurrentVersionInfoDelegate ) ar.AsyncState;

			Debug.Assert( dlg != null );

			this.Invoke( ( VoidDelegate ) delegate
			{
				try
				{
					VersionInfo currentVersion = dlg.EndInvoke( ar );

					if ( currentVersion.IsNewer )
					{
						DialogResult result = MessageBox.Show(
							this,
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
							this,
							Strings.NoNewVersionAvailable,
							Strings.UpdateCheckCaption,
							MessageBoxButtons.OK );
					}
				}
				catch ( Exception x )
				{
					Logger.LogError( "UpdateCheck", x );

					DialogResult result = MessageBox.Show(
						this,
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

		public static void CheckForUpdate()
		{
			ReadCurrentVersionInfoDelegate checkDelegate = 
				UpdateCheck.ReadCurrentVersionInfo;

			using ( UpdateCheckWindow window = new UpdateCheckWindow() )
			{
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