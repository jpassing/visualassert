using System;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace Cfix.Control.Ui.Result
{
	internal delegate string GetToolTipDelegate( TreeNodeAdv node );

	internal class ToolTipProvider : IToolTipProvider
	{
		private readonly GetToolTipDelegate dlg;

		public ToolTipProvider( GetToolTipDelegate dlg )
		{
			this.dlg = dlg;
		}

		public string GetToolTip( 
			TreeNodeAdv node, 
			NodeControl nodeControl 
			)
		{
			return dlg( node );
		}
	}
}
