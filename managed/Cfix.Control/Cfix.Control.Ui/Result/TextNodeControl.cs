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

		private Brush FailBrush = new SolidBrush(
			Color.FromArgb( 255, 229, 229 ) );

		private void RequestContextMenu( IResultNode node, Point pt )
		{
			this.explorer.OnContextMenuRequested( node, pt );
		}

		public TextNodeControl( ResultExplorer explorer )
		{
			this.explorer = explorer;

			this.DrawText += new EventHandler<DrawEventArgs>( TextNodeControl_DrawText );
		}

		private void TextNodeControl_DrawText( object sender, DrawEventArgs args )
		{
			IResultNode node = args.Node.Tag as IResultNode;
			if ( node != null )
			{
				Color? txtColor = node.TextColor;
				if ( txtColor != null )
				{
					args.TextColor = ( Color ) txtColor;
				}

				ResultItemNode resNode = node as ResultItemNode;
				if ( resNode != null && 
				     resNode.ResultItem.Status == ExecutionStatus.Failed )
				{
					args.BackgroundBrush = FailBrush;
				}
			}
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
			TreeNodeAdv node = this.explorer.Tree.CurrentNode;
			if ( node != null )
			{
				if ( e.KeyCode == Keys.Apps ||
					( e.KeyCode == Keys.F10 && e.Shift ) )
				{
					Point pt = this.explorer.Tree.GetNodeLocation( node, true );
					RequestContextMenu(
						( IResultNode ) node.Tag,
						new Point( 100, pt.Y ) );
				}
				else
				{
					this.explorer.OnTreeKeyDown( 
						( IResultNode ) node.Tag, 
						e );
				}

				e.Handled = true;
			}
		}

		public override void MouseDoubleClick( TreeNodeAdvMouseEventArgs args )
		{
			this.explorer.OnTreeDoubleClick(
				( IResultNode ) args.Node.Tag, 
				args );
			args.Handled = true;
		}
	}
}
