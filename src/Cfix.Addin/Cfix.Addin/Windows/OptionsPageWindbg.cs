using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using System.Diagnostics;
using System.IO;

namespace Cfix.Addin.Windows
{
	public partial class OptionsPageWindbg : UserControl, IDTToolsOptionsPage
	{
		private DTE2 dte;
		private Configuration configuration;

		private static string BrowseWinDbgInstallFolder( string suggestedPath )
		{
			using ( FolderBrowserDialog dlg = new FolderBrowserDialog() )
			{
				if ( suggestedPath != null )
				{
					dlg.SelectedPath = suggestedPath;
				}

				dlg.Description = Strings.WinDbgInstallFolder;
				if ( dlg.ShowDialog() == DialogResult.OK )
				{
					if ( IsValidWindbgFolder( dlg.SelectedPath ) )
					{
						return dlg.SelectedPath;
					}
					else
					{
						VisualAssert.ShowError( 
							String.Format( 
								Strings.InvalidWindbgInstallFolder,
								dlg.SelectedPath ) );

						return null;
					}
				}
				else
				{
					return null;
				}
			}
		}

		private static bool IsValidWindbgFolder( string path )
		{
			return Directory.Exists( path ) &&
				File.Exists( Path.Combine( path, "windbg.exe" ) );
		}

		public OptionsPageWindbg()
		{
			InitializeComponent();
		}

		/*----------------------------------------------------------------------
		 * Events.
		 */

		private void windbg32Browse_Click( object sender, EventArgs e )
		{
			string path = BrowseWinDbgInstallFolder( this.windbg32Path.Text );
			if ( path != null )
			{
				this.windbg32Path.Text = path;
			}
		}

		private void windbg64Browse_Click( object sender, EventArgs e )
		{
			string path = BrowseWinDbgInstallFolder( this.windbg64Path.Text );
			if ( path != null )
			{
				this.windbg64Path.Text = path;
			}
		}

		/*----------------------------------------------------------------------
		 * IDTToolsOptionsPage Members.
		 */

		public void GetProperties( ref object propertiesObject )
		{
			Debug.Assert( this.configuration != null );

			propertiesObject = this.configuration;
		}

		public void OnAfterCreated( DTE dte )
		{
			this.dte = ( DTE2 ) dte;
			this.configuration = Configuration.Load( this.dte );

			bool enable64 = ArchitectureUtil.Is64bitSupported;
			if ( !enable64 )
			{
				this.windbg64Browse.Enabled = false;
				this.windbg64Path.Enabled = false;
				this.windbg64Label.Enabled = false;
			}

			this.windbg32Path.Text = this.configuration.WinDbgInstallFolder32;
			this.windbg64Path.Text = this.configuration.WinDbgInstallFolder64;
			this.windbgAdditionalOpts.Text = this.configuration.WinDbgAdditionalOptions;
		}

		public void OnCancel()
		{
			Debug.Assert( this.configuration != null );
		}

		public void OnHelp()
		{
			Debug.Assert( this.configuration != null );
		}

		public void OnOK()
		{
			Debug.Assert( this.configuration != null );

			this.configuration.WinDbgInstallFolder32 = this.windbg32Path.Text.Trim();
			this.configuration.WinDbgInstallFolder64 = this.windbg64Path.Text.Trim();
			this.configuration.WinDbgAdditionalOptions = this.windbgAdditionalOpts.Text.Trim();
		}
	}
}
