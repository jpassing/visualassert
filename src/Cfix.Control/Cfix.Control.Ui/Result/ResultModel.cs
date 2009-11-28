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

		private delegate void VoidDelegate();

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
		private bool autoScroll;
		private ResultNodeFilter filter;

		/*----------------------------------------------------------------------
		 * Events.
		 */

		private static TreePath GetTreePath( IResultNode n )
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

		private void Reload()
		{
			if ( this.StructureChanged != null )
			{
				this.StructureChanged(
					this,
					new TreePathEventArgs( TreePath.Empty ) );
			}
		}

		private void run_StatusChangedOrFailureOccured( object sender, EventArgs e )
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

					if ( this.NodesRemoved != null )
					{
						//
						// To avoid adding duplicates, we have to 
						// clear the list of existing failures first.
						//
						// N.B. In case of a fixture, there may be non-failure
						// children -- make sure to leave these in place.
						//

						TreeNodeAdv node = this.tree.FindNode( nodePath );
						Debug.Assert( node.Tag == affectedNode );

						List<object> nodesToDelete = new List<object>();
						
						foreach ( TreeNodeAdv childNode in node.Children )
						{
							FailureNode failure = childNode.Tag as FailureNode;
							if ( failure != null )
							{
								nodesToDelete.Add( failure );
							}
						}

						if ( nodesToDelete.Count > 0 )
						{
							this.NodesRemoved(
								this,
								new TreeModelEventArgs(
									nodePath,
									null,
									nodesToDelete.ToArray() ) );
						}
					}

					if ( this.NodesInserted != null )
					{
						object[] children = new object[ affectedNode.Failures.Count ];
						int[] indices = new int[ children.Length ];
						int index = 0;
						foreach ( Failure f in affectedNode.Failures )
						{
							indices[ index ] = index;
							children[ index++ ] = FailureNode.Create( 
								f, 
								this.iconsList,
								affectedNode );
						}

						this.NodesInserted(
							this,
							new TreeModelEventArgs(
								nodePath,
								indices,
								children ) );
					}
				}

				if ( item.Status > ExecutionStatus.Running )
				{
					IResultItemCollection itemColl =
							item as IResultItemCollection;
					if ( item.Status == ExecutionStatus.Succeeded &&
						 itemColl != null )
					{
						Collapse( nodePath );
					}

					//
					// We do not expect further changes for this node.
					//
					this.nodeTable.Remove( item );
				}

				if ( this.autoScroll && this.run.TaskCount == 1 )
				{
					//
					// Try scrolling to this node.
					//
					// N.B. When more than 1 task is active, scrolling
					// must be avoided.
					//
					TreeNodeAdv node = this.tree.FindNode( nodePath );
					if ( node != null )
					{
						this.tree.BeginInvoke( ( VoidDelegate ) delegate
						{
							this.tree.ScrollTo( node );
						} );
					}
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
		 * Internal helpers.
		 */

		internal static bool MatchesFilter( IResultItem item, ResultNodeFilter filter )
		{
			switch ( item.Status )
			{
				case ExecutionStatus.Pending:
				case ExecutionStatus.Running:
				case ExecutionStatus.Skipped:
				case ExecutionStatus.Stopped:

				case ExecutionStatus.Succeeded:
				case ExecutionStatus.SucceededWithSkippedParts:
					return ( filter & ResultNodeFilter.NonFailureNodes ) != 0;

				case ExecutionStatus.SucceededWithInconclusiveParts:
				case ExecutionStatus.Failed:
				case ExecutionStatus.Inconclusive:
					return ( filter & ResultNodeFilter.FailureNodes ) != 0;

				default:
					Debug.Fail( "Unrecognized status: " + item.Status );
					return false;
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

		public bool AutoScroll
		{
			get { return this.autoScroll; }
			set { this.autoScroll = value; }
		}

		public ResultNodeFilter Filter
		{
			get { return this.filter; }
			set 
			{
				this.filter = value;
				Reload();
			}
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
						this.run.StatusChanged -= new EventHandler( run_StatusChangedOrFailureOccured );
						this.run.FailureOccured -= new EventHandler( run_StatusChangedOrFailureOccured );
					}

					//
					// (Re-) initialize.
					//

					this.run = value;
					this.rootExpanded = false;

					//
					// N.B. Filter must be reset because the model does
					// not supported being updated dynamically while having
					// a filter applied.
					//
					this.filter = ResultNodeFilter.All;

					if ( this.run != null )
					{
						Debug.Assert( value.Status == TaskStatus.Ready );

						this.root = new ResultItemNode(
							this.run.RootResult,
							this.iconsList,
							null );
						this.nodeTable = new Dictionary<IResultItem, ResultItemNode>();
						this.nodeTable[ this.run.RootResult ] = this.root;

						this.run.StatusChanged += new EventHandler( run_StatusChangedOrFailureOccured );
						this.run.FailureOccured += new EventHandler( run_StatusChangedOrFailureOccured );
					}

					Reload();
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
				if ( MatchesFilter( this.root.ResultItem, this.filter ) )
				{
					yield return this.root;
				}
				else
				{
					yield break;
				}
			}
			else
			{
				IResultNode node = ( IResultNode ) treePath.LastNode;
				foreach ( IResultNode child in node.GetChildren( this.filter ) )
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

		public void ExpandAll()
		{
			Expand( TreePath.Empty, true );
		}
	}
}
