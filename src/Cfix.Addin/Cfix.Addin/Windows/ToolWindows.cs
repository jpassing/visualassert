/*----------------------------------------------------------------------
 * Purpose:
 *		Tool Window Collection.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Diagnostics;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Addin.Dte;
using Cfix.Addin.Windows.Explorer;
using Cfix.Addin.Windows.Run;
using EnvDTE;
using EnvDTE80;

namespace Cfix.Addin.Windows
{
	internal class ToolWindows : IDisposable
	{
		private const string LogWindowName = "Visual Assert Log";

		private readonly DTE2 dte;
		private AddIn addin;
		private readonly Workspace workspace;

		private readonly string extraCaption;
		private readonly bool disableControls;

		private DteToolWindow<ExplorerWindow> explorer;
		private DteToolWindow<RunWindow> run;
		private OutputWindowPane logOutputWindow;

		internal ToolWindows( 
			DTE2 dte,
			AddIn addin,
			Workspace ws
			)
		{
			this.dte = dte;
			this.addin = addin;
			this.workspace = ws;

			try
			{
				LicenseInfo licInfo = ws.QueryLicenseInfo();

				if ( licInfo.IsTrial )
				{
#if BETA
					if ( licInfo.Valid )
					{
						this.extraCaption = "(" + Strings.BetaLicenseValid + ")";
					}
					else
					{
						this.extraCaption = "(" + Strings.BetaLicenseInvalid + ")";

						this.disableControls = true;
						LaunchLicenseAdmin( "expired" );
					}
#else
					if ( licInfo.Valid )
					{
						this.extraCaption = "(" + String.Format(
							Strings.TrialLicenseValid, 
							licInfo.TrialDaysLeft ) + ")";
					}
					else
					{
						this.extraCaption = "(" +
							Strings.TrialLicenseInalid + ")";

						this.disableControls = true;
						LaunchLicenseAdmin( "expired" );
					}
#endif
				}
				else
				{
					this.extraCaption = "";
				}
			}
			catch
			{ }
		}

		~ToolWindows()
		{
			Dispose( false );
		}

		public void Dispose()
		{
			GC.SuppressFinalize( this );
			Dispose( true );
		}

		public void Dispose( bool disposing )
		{
			if ( this.explorer != null )
			{
				this.explorer.Dispose();
				this.explorer = null;
			}

			if ( this.run != null )
			{
				this.run.Dispose();
				this.run = null;
			}
		}

		public void RestoreWindowState()
		{
			//
			// N.B. Avoid creating the windows unless we intend to 
			// activate them.
			//

			if ( this.workspace.Configuration.ExplorerWindowVisible )
			{
				Explorer.Activate();
			}

			if ( this.workspace.Configuration.RunWindowVisible )
			{
				Run.Activate();
			}
		}

		public void SaveWindowState()
		{
			if ( this.explorer != null )
			{
				this.workspace.Configuration.ExplorerWindowVisible =
					this.explorer.Visible;
			}

			if ( this.run != null )
			{
				this.workspace.Configuration.RunWindowVisible =
					this.run.Visible;
			}
		}

		public bool IsExplorerLoaded
		{
			get { return this.explorer != null; }
		}

		public DteToolWindow<ExplorerWindow> Explorer
		{
			get
			{
				if ( this.explorer == null )
				{
					this.explorer = DteToolWindow<ExplorerWindow>.Create(
						this.dte,
						this.addin,
						Strings.ExplorerWindowCaption + " " + this.extraCaption,
						ExplorerWindow.Guid,
                        Windows.Chrome.CfixIcon );
					this.explorer.UserControl.Initialize( 
						this.workspace,
						this.dte,
						this.explorer.Window );

					this.explorer.DefaultHeight = 400;
					this.explorer.DefaultWidth = 350;
				}

				if ( this.disableControls )
				{
					this.explorer.UserControl.Enabled = false;
				}

				return this.explorer;
			}
		}

		public DteToolWindow<RunWindow> Run
		{
			get
			{
				if ( this.run == null )
				{
					this.run= DteToolWindow<RunWindow>.Create(
						this.dte,
						this.addin,
						Strings.RunWindowCaption + " " + this.extraCaption,
						RunWindow.Guid,
                        Windows.Chrome.CfixIcon);
					this.run.UserControl.Initialize(
						this.workspace,
						this.dte,
						this.run.Window );

					this.run.DefaultHeight = 300;
					this.run.DefaultWidth = 700;
				}

				if ( this.disableControls )
				{
					this.run.UserControl.Enabled = false;
				}

				return this.run;
			}
		}

		public OutputWindowPane LogWindow
		{
			get
			{
				if ( this.logOutputWindow == null )
				{
					//
					// Try using existing.
					//
					OutputWindowPanes panes = 
						this.dte.ToolWindows.OutputWindow.OutputWindowPanes;
					try
					{
						this.logOutputWindow = panes.Item( LogWindowName );
					}
					catch ( Exception )
					{ }

					//
					// Create new if no existing found.
					//
					if ( this.logOutputWindow == null )
					{
						this.logOutputWindow = panes.Add( LogWindowName );
					}
				}

				return this.logOutputWindow;
			}
		}

		public void CloseAll()
		{
			if ( this.explorer != null )
			{
				this.explorer.Close();
				this.explorer.UserControl.Dispose();
			}

			if ( this.run != null )
			{
				this.run.Close();
				this.run.UserControl.Dispose();
			}
		}

		public void LaunchLicenseAdmin( string cmdArgs )
		{
#if BETA
			if ( DialogResult.Yes == 
				MessageBox.Show(
					"Your Visual Assert Beta installation has expired. Please visit the "+
					"Visual Assert website to obtain the current version "+
					"of Visual Assert.\r\n\r\n" +
					"Would you like to open the Visual Assert website now?",
					"Visual Assert",
					MessageBoxButtons.YesNo ) )
			{
				Cfix.Addin.Windows.CommonUiOperations.OpenHomepage();
			}

#else
			try
			{
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo.FileName =
					Directories.GetBinDirectory( Architecture.I386 ) +
					"\\licadmin.exe";
				proc.StartInfo.Arguments = cmdArgs;
				proc.Start();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
#endif
		}
	}
}
