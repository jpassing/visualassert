using System;
using System.Diagnostics;
using System.Collections;
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

		private IRun run;

		private IEnumerable GetRoot()
		{
			yield return new ResultItemNode( this.run.RootResult );
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
