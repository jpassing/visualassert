using System;
using Cfix.Addin.Dte;
using Cfix.Addin.Windows;

namespace Cfix.Addin
{
	internal class ToolWindows
	{
		private readonly DteConnect connect;
		private DteToolWindow<ExplorerWindow> explorer;

		internal ToolWindows( DteConnect connect )
		{
			this.connect = connect;
		}

		public void RestoreWindowState()
		{
			if ( this.connect.DTE.Globals.get_VariableExists( "Explorer" )
				&& this.connect.DTE.Globals[ "Explorer" ].ToString() == "1" )
			{
				Explorer.Visible = true;
			}
		}

		public void SaveWindowState()
		{
			if ( this.explorer != null && this.explorer.Visible )
			{
				this.connect.DTE.Globals[ "Explorer" ] = "1";
				this.connect.DTE.Globals.set_VariablePersists( "Explorer", true );
			}
				
		}

		public DteToolWindow<ExplorerWindow> Explorer
		{
			get
			{
				if ( this.explorer == null )
				{
					this.explorer = DteToolWindow<ExplorerWindow>.Create(
						this.connect,
						"Explorer",
						ExplorerWindow.Guid,
						Icons.Explorer );
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
