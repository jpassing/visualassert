using System;
using System.Diagnostics;
using System.Collections.Generic;
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
		private readonly TreeViewAdv tree;
		
		//
		// The current run -- may be changed at any time.
		//
		private IRun run;

		//
		// For the current run, map between result and node. 
		//
		private IDictionary<IResultItem, ResultItemNode> nodeTable;

		private ResultItemNode root;

		private readonly object runLock = new object();

		private bool rootExpanded;

		/*----------------------------------------------------------------------
		 * Events.
		 */

		private static TreePath GetTreePath( ResultItemNode n )
		{
			if ( n == null )
			{
				return TreePath.Empty;
			}
			else
			{
				Stack<object> stack = new Stack<object>();
				while ( n != null )
				{
					stack.Push( n );
					n = n.Parent;
				}

				return new TreePath( stack.ToArray() );
			}
		}

		private void Expand( TreePath path, bool recursive )
		{
			this.tree.FindNode( path ).Expand( !recursive );
		}

		private void Collapse( TreePath path )
		{
			this.tree.FindNode( path ).Collapse();
		}

		private void run_StatusChanged( object sender, EventArgs e )
		{
			IResultItem item = sender as IResultItem;
			Debug.Assert( item != null );
			Debug.Assert( this.NodesChanged != null );
			
			ResultItemNode affectedNode;
			if ( this.nodeTable.TryGetValue( item, out affectedNode ) )
			{
				//
				// Corresponding node has been expanded and loaded 
				// before --> issue update.
				//

				this.NodesChanged(
					this,
					new TreeModelEventArgs(
						GetTreePath( affectedNode.Parent ),
						new object[] { affectedNode } ) );

				TreePath nodePath = GetTreePath( affectedNode );

				if ( affectedNode.HasFailures )
				{
					//
					// New children for this node.
					//

					if ( this.NodesInserted != null )
					{
						object[] children = new object[ affectedNode.Failures.Count ];
						int[] indices = new int[ children.Length ];
						int index = 0;
						foreach ( Failure f in affectedNode.Failures )
						{
							indices[ index ] = index;
							children[ index++ ] = FailureNode.Create( f, this.iconsList );
						}

						this.NodesInserted(
							this,
							new TreeModelEventArgs(
								nodePath,
								indices,
								children ) );
					}
				}

				//if ( item.Status == ExecutionStatus.Running )
				//{
				//    Expand( nodePath, false );
				//}
				//else 
				if ( item.Status > ExecutionStatus.Running )
				{
					IResultItemCollection itemColl =
							item as IResultItemCollection;
					if ( item.Status == ExecutionStatus.Succeeded &&
						 itemColl != null )
					{
						//if ( itemColl.ItemCount > 0 &&
						//    !( itemColl.GetItem( 0 ) is IResultItemCollection ) )
						//{
						//    //
						//    // Node has leaves as children.
						//    //
						//    Collapse( nodePath );
						//}
						Collapse( nodePath );
					}

					//
					// We do not expect further changes for this node.
					//
					this.nodeTable.Remove( item );
				}
			}
			else
			{
				//
				// Corresponding node not been requested/expanded yet.
				//
			}

			if ( !this.rootExpanded )
			{
				Expand( new TreePath( this.root ), true );
				this.rootExpanded = true;
			}
		}
		
		/*----------------------------------------------------------------------
		 * Public.
		 */

		public ResultModel( ImageList iconsList, TreeViewAdv tree )
		{
			this.iconsList = iconsList;
			this.tree = tree;
		}

		public IRun Run
		{
			get { return this.run; }
			set
			{
				lock ( this.runLock )
				{
					if ( this.run != null )
					{
						this.run.StatusChanged -= new EventHandler( run_StatusChanged );
					}

					//
					// (Re-) initialize.
					//

					this.run = value;
					this.rootExpanded = false;

					if ( this.run != null )
					{
						Debug.Assert( value.Status == TaskStatus.Ready );

						this.root = new ResultItemNode(
							this.run.RootResult,
							this.iconsList,
							null );
						this.nodeTable = new Dictionary<IResultItem, ResultItemNode>();
						this.nodeTable[ this.run.RootResult ] = this.root;

						this.run.StatusChanged += new EventHandler( run_StatusChanged );
					}

					if ( this.StructureChanged != null )
					{
						this.StructureChanged(
							this,
							new TreePathEventArgs( TreePath.Empty ) );
					}
				}
			}
		}

		public System.Collections.IEnumerable GetChildren( TreePath treePath )
		{
			if ( this.run == null )
			{
				yield break;
			}
			else if ( treePath.IsEmpty() )
			{
				yield return this.root;
			}
			else
			{
				IResultNode node = ( IResultNode ) treePath.LastNode;
				foreach ( IResultNode child in node.GetChildren() )
				{
					ResultItemNode resultChild = child as ResultItemNode;
					if ( resultChild != null )
					{
						//
						// Remember mapping for status changes.
						//
						this.nodeTable[ resultChild.ResultItem ] = resultChild;
					}

					yield return child;
				}
			}
		}

		public bool IsLeaf( TreePath treePath )
		{
			IResultNode node = ( IResultNode ) treePath.LastNode;
			return node.IsLeaf;
		}
	}
}
