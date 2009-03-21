using System;
using Cfix.Addin.Dte;
using Cfix.Addin.Windows.Explorer;
using Cfix.Addin.Windows.Run;

namespace Cfix.Addin
{
	internal class ToolWindows : IDisposable
	{
		private readonly CfixPlus addin;
		private DteToolWindow<ExplorerWindow> explorer;
		private DteToolWindow<RunWindow> run;

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
			if ( this.addin.Workspace.Configuration.ExplorerWindowVisible )
			{
				Explorer.Visible = true;
				Run.Visible = true;
			}
		}

		public void SaveWindowState()
		{
			if ( this.explorer != null && this.explorer.Visible )
			{
				this.addin.Workspace.Configuration.ExplorerWindowVisible = true;
				this.addin.Workspace.Configuration.RunWindowVisible = true;
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
					this.explorer.Visible = true;

					try
					{
						this.explorer.Height = 400;
						this.explorer.Width = 300;
					}
					catch ( Exception )
					{ }
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
						this.addin.Workspace );
					this.run.Visible = true;

					try
					{
						this.run.Height = 300;
						this.run.Width = 700;
					}
					catch ( Exception )
					{ }
				}

				return this.run;
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
