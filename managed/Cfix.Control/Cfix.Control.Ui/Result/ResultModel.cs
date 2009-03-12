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

		private void run_StatusChanged( object sender, EventArgs e )
		{
			IResultItem item = sender as IResultItem;
			Debug.Assert( item != null );

			if ( this.NodesChanged != null )
			{
				ResultItemNode affectedNode;
				if ( this.nodeTable.TryGetValue( item, out affectedNode ) )
				{
					//
					// Corresponding node has been requested --> issue update.
					//

					this.NodesChanged(
						this,
						new TreeModelEventArgs(
							GetTreePath( affectedNode.Parent ),
							new object[] { affectedNode } ) );

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
								children[ index++ ] = f;
							}

							this.NodesInserted(
								this,
								new TreeModelEventArgs(
									GetTreePath( affectedNode ),
									indices,
									children ) );
						}
					}

					//
					// We do not expect further changes for this node.
					//
					this.nodeTable.Remove( item );
				}
				else
				{
					//
					// Corresponding node not requested/vivible yet.
					//
				}
			}
		}
		
		/*----------------------------------------------------------------------
		 * Public.
		 */

		public ResultModel( ImageList iconsList )
		{
			this.iconsList = iconsList;
		}

		public IRun Run
		{
			get { return this.run; }
			set
			{
				lock ( this.runLock )
				{
					if ( value == null )
					{
						return;
					}

					Debug.Assert( value.Status == TaskStatus.Ready );

					if ( this.run != null )
					{
						this.run.StatusChanged -= new EventHandler( run_StatusChanged );
					}

					//
					// (Re-) initialize.
					//

					this.run = value;
					this.root = new ResultItemNode(
						this.run.RootResult,
						this.iconsList,
						null );
					this.nodeTable = new Dictionary<IResultItem, ResultItemNode>();
					this.nodeTable[ this.run.RootResult ] = this.root;

					this.run.StatusChanged += new EventHandler( run_StatusChanged );

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
