using System;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace Cfix.Control.Ui.Result
{
	class TextNodeControl : NodeTextBox
	{
		private readonly ResultExplorer explorer;

		private void RequestContextMenu( IResultNode node, Point pt )
		{
			this.explorer.OnContextMenuRequested( node, pt );
		}

		public TextNodeControl( ResultExplorer explorer )
		{
			this.explorer = explorer;
		}

		/*----------------------------------------------------------------------
		 * Overrides.
		 */

		public override void MouseDown( TreeNodeAdvMouseEventArgs args )
		{
			if ( args.Button == MouseButtons.Right )
			{
				RequestContextMenu(
					( IResultNode ) args.Node.Tag, 
					args.Location );
			}
		}

		public override void KeyDown( KeyEventArgs e )
		{
			if ( e.KeyCode == Keys.Apps ||
				 ( e.KeyCode == Keys.F10 && e.Shift ) )
			{
				TreeNodeAdv node = this.explorer.Tree.CurrentNode;
				if ( node != null )
				{
					Point pt = this.explorer.Tree.GetNodeLocation( node, true );
					RequestContextMenu(
						( IResultNode ) node.Tag,
						new Point( 100, pt.Y ) );
				}
			}
		}
	}
}
