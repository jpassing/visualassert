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

		private void PopupContextMenu( IResultNode node, Point pt )
		{
			ContextMenuStrip strip = null;

			ResultItemNode resNode = node as ResultItemNode;
			if ( resNode != null )
			{
				strip = this.explorer.ResultNodeContextMenu;
			}
			else
			{
				FailureNode failNode = node as FailureNode;
				if ( failNode != null )
				{
					strip = this.explorer.FailureNodeContextMenu;
				}
			}

			if ( strip != null )
			{
				strip.Show( this.explorer, pt );
			}
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
				PopupContextMenu(
					( IResultNode ) args.Node.Tag, 
					args.Location );
			}
		}

		public override void KeyDown( KeyEventArgs e )
		{
			if ( e.KeyCode == Keys.Apps ||
				 ( e.KeyCode == Keys.F10 && e.Shift ) )
			{
				TreeNodeAdv node = this.explorer.CurrentNode;
				if ( node != null )
				{
					PopupContextMenu(
						( IResultNode ) node.Tag,
						GetBounds( node, new DrawContext() ).Location );
				}
			}
		}
	}
}
