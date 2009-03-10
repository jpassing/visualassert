using System;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using Cfix.Control;
using Aga.Controls.Tree;

namespace Cfix.Control.Ui.Result
{
	internal class ResultModel : ITreeModel
	{
		public event EventHandler<TreeModelEventArgs> NodesChanged;
		public event EventHandler<TreeModelEventArgs> NodesInserted;
		public event EventHandler<TreeModelEventArgs> NodesRemoved;
		public event EventHandler<TreePathEventArgs> StructureChanged;

		private readonly ImageList iconsList;

		private IRun run;

		public ResultModel( ImageList iconsList )
		{
			this.iconsList = iconsList;
		}

		private IEnumerable GetRoot()
		{
			yield return new ResultItemNode( 
				this.run.RootResult,
				this.iconsList );
		}

		public IRun Run
		{
			get { return this.run; }
			set
			{
				this.run = value;
				if ( this.StructureChanged != null )
				{
					this.StructureChanged(
						this,
						new TreePathEventArgs( TreePath.Empty ) );
				}
			}
		}

		public IEnumerable GetChildren( TreePath treePath )
		{
			if ( this.run == null )
			{
				return null;
			}
			else if ( treePath.IsEmpty() )
			{
				return GetRoot();
			}
			else
			{
				IResultNode node = ( IResultNode ) treePath.LastNode;
				return node.GetChildren();
			}
		}

		public bool IsLeaf( TreePath treePath )
		{
			IResultNode node = ( IResultNode ) treePath.LastNode;
			return node.IsLeaf;
		}
	}
}
