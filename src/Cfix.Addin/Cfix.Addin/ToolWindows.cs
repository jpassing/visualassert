/*----------------------------------------------------------------------
 * Purpose:
 *		Tool Window Collection.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Diagnostics;
using Cfix.Control;
using Cfix.Addin.Dte;
using Cfix.Addin.Windows.Explorer;
using Cfix.Addin.Windows.Run;
using EnvDTE;
namespace Cfix.Addin
{
	internal class ToolWindows : IDisposable
	{
		private readonly CfixPlus addin;
		private readonly string extraCaption;
		private readonly bool disableControls;

		private DteToolWindow<ExplorerWindow> explorer;
		private DteToolWindow<RunWindow> run;
		private OutputWindowPane logOutputWindow;

		internal ToolWindows( 
			CfixPlus addin,
			Workspace ws
			)
		{
			this.addin = addin;

			try
			{
				LicenseInfo licInfo = ws.QueryLicenseInfo();

				if ( licInfo.IsTrial )
				{
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

			if ( this.addin.Workspace.Configuration.ExplorerWindowVisible )
			{
				Explorer.Visible = true;
			}

			if ( this.addin.Workspace.Configuration.RunWindowVisible )
			{
				Run.Visible = true;
			}
		}

		public void SaveWindowState()
		{
			if ( this.explorer != null )
			{
				this.addin.Workspace.Configuration.ExplorerWindowVisible = 
					this.explorer.Visible;
			}

			if ( this.run != null )
			{
				this.addin.Workspace.Configuration.RunWindowVisible =
					this.run.Visible;
			}
		}

		public DteToolWindow<ExplorerWindow> Explorer
		{
			get
			{
				if ( this.explorer == null )
				{
					this.explorer = DteToolWindow<ExplorerWindow>.Create(
						this.addin,
						Strings.ExplorerWindowCaption + " " + this.extraCaption,
						ExplorerWindow.Guid,
						Icons.cfix );
					this.explorer.UserControl.Initialize( 
						this.addin.Workspace,
						this.addin.DTE );

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
						this.addin,
						Strings.RunWindowCaption + " " + this.extraCaption,
						RunWindow.Guid,
						Icons.cfix );
					this.run.UserControl.Initialize(
						this.addin.Workspace,
						this.addin.DTE );

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
						this.addin.DTE.ToolWindows.OutputWindow.OutputWindowPanes;
					try
					{
						this.logOutputWindow = panes.Item( "cfix" );
					}
					catch ( Exception )
					{ }

					//
					// Create new if no existing found.
					//
					if ( this.logOutputWindow == null )
					{
						this.logOutputWindow = panes.Add( "cfix" );
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
				CfixPlus.HandleError( x );
			}
		}

	}
}
