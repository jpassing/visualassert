/*----------------------------------------------------------------------
 * Purpose:
 *		Tool Window Collection.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using Cfix.Addin.Dte;
using Cfix.Addin.Windows.Explorer;
using Cfix.Addin.Windows.Run;
using EnvDTE;
namespace Cfix.Addin
{
	internal class ToolWindows : IDisposable
	{
		private readonly CfixPlus addin;
		private DteToolWindow<ExplorerWindow> explorer;
		private DteToolWindow<RunWindow> run;
		private OutputWindowPane logOutputWindow;

		internal ToolWindows( CfixPlus addin )
		{
			this.addin = addin;
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
			Explorer.Visible = this.addin.Workspace.Configuration.ExplorerWindowVisible;
			Run.Visible = this.addin.Workspace.Configuration.RunWindowVisible;
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
						Strings.ExplorerWindowCaption,
						ExplorerWindow.Guid,
						Icons.cfix );
					this.explorer.UserControl.Initialize( 
						this.addin.Workspace,
						this.addin.DTE );

					this.explorer.DefaultHeight = 400;
					this.explorer.DefaultWidth = 350;
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
						Strings.RunWindowCaption,
						RunWindow.Guid,
						Icons.cfix );
					this.run.UserControl.Initialize(
						this.addin.Workspace,
						this.addin.DTE );

					this.run.DefaultHeight = 300;
					this.run.DefaultWidth = 700;
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
	}
}
