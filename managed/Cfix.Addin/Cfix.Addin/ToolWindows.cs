using System;
using Cfix.Addin.Dte;
using Cfix.Addin.Windows.Explorer;

namespace Cfix.Addin
{
	internal class ToolWindows
	{
		private readonly CfixPlus addin;
		private DteToolWindow<ExplorerWindow> explorer;

		internal ToolWindows( CfixPlus addin )
		{
			this.addin = addin;
		}

		public void RestoreWindowState()
		{
			if ( this.addin.Workspace.Configuration.ExplorerWindowVisible )
			{
				Explorer.Visible = true;
			}
		}

		public void SaveWindowState()
		{
			if ( this.explorer != null && this.explorer.Visible )
			{
				this.addin.Workspace.Configuration.ExplorerWindowVisible = true;
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

		public void CloseAll()
		{
			if ( this.explorer != null )
			{
				this.explorer.Close();
				this.explorer.UserControl.Dispose();
			}
		}
	}
}
