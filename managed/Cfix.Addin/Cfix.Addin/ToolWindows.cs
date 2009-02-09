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
			if ( this.addin.DTE.Globals.get_VariableExists( "Explorer" )
				&& this.addin.DTE.Globals[ "Explorer" ].ToString() == "1" )
			{
				Explorer.Visible = true;
			}
		}

		public void SaveWindowState()
		{
			if ( this.explorer != null && this.explorer.Visible )
			{
				this.addin.DTE.Globals[ "Explorer" ] = "1";
				this.addin.DTE.Globals.set_VariablePersists( "Explorer", true );
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
						Icons.Explorer );
					this.explorer.UserControl.Initialize( 
						this.addin.Workspace,
						this.addin.DTE );
					this.explorer.Visible = true;
					this.explorer.Height = 400;
					this.explorer.Width = 300;
				}

				return this.explorer;
			}
		}

		public void CloseAll()
		{
			if ( this.explorer != null )
			{
				this.explorer.Close();
			}
		}
	}
}
