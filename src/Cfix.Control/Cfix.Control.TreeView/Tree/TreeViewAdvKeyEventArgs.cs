using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Aga.Controls.Tree.NodeControls;

namespace Aga.Controls.Tree
{
	public class TreeViewAdvKeyEventArgs 
	{
		private KeyEventArgs _args;
		public KeyEventArgs Args
		{
			get { return _args; }
		}

		private TreeNodeAdv _node;
		public TreeNodeAdv Node
		{
			get { return _node; }
			internal set { _node = value; }
		}

		public TreeViewAdvKeyEventArgs(
			TreeNodeAdv node,
			KeyEventArgs args )
		{
			_args = args;
			_node = node;
		}
	}
}
