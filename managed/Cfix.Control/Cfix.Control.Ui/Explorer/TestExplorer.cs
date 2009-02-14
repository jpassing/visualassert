using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Control.Ui.Explorer
{
	public partial class TestExplorer : UserControl
	{
		private delegate void VoidDelegate();
		
		//
		// ImageList indexes.
		//
		internal const int ContainerIconIndex = 0;
		internal const int ContainerIconSelectedIndex = 0;
		internal const int ContainerIconExpandedIndex = 1;
		internal const int ModuleIconIndex = 2;
		internal const int ModuleIconSelectedIndex = 2;
		internal const int TestContainerIconIndex = 3;
		internal const int TestContainerIconSelectedIndex = 3;
		internal const int TestItemIconIndex = 4;
		internal const int TestItemIconSelectedIndex = 4;
		internal const int InvalidModuleIconIndex = 5;
		internal const int InvalidModuleIconSelectedIndex = 5;

		private NodeFactory nodeFactory = new NodeFactory();
		private ISession session;

		private readonly Object refreshLock = new Object();

		//
		// Lock that needs to be acquired by async operations to protect
		// from the control being torn down prematurely.
		//
		private RundownLock rundownLock = new RundownLock();

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1713:EventsShouldNotHaveBeforeOrAfterPrefix" )]
		public event EventHandler<ExplorerNodeEventArgs> AfterSelected;
		public event EventHandler<ExceptionEventArgs> ExceptionRaised;
		public event EventHandler RefreshStarted;
		public event EventHandler RefreshFinished;
		public event EventHandler<ExplorerNodeEventArgs> BeforeContextMenuPopup;
		
		private ContextMenuStrip nodeContextMenu;

		/*----------------------------------------------------------------------
		 * Private.
		 */

		private static void HandleException( Exception x )
		{
			MessageBox.Show( x.Message );
		}

		private delegate void RefreshDelegate( bool async, bool selectionOnly );

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes" )]
		private void AsyncVoidCompletionCallback( IAsyncResult ar )
		{
			try
			{
				RefreshDelegate dlg = ( RefreshDelegate ) ar.AsyncState;
				dlg.EndInvoke( ar );
			}
			catch ( Exception x )
			{
				if ( ExceptionRaised != null )
				{
					ExceptionRaised( this, new ExceptionEventArgs( x ) );
				}
			}
			finally
			{
				if ( this.Disposing || this.treeView.Disposing )
				{
					//
					// Do not touch UI.
					//
				}
				else
				{
					this.treeView.Invoke( ( VoidDelegate ) delegate()
					{
						if ( this.RefreshFinished != null )
						{
							this.RefreshFinished( this, EventArgs.Empty );
						}
					} );
				}

				this.rundownLock.Release();
			}
		}

		/*----------------------------------------------------------------------
		 * Events.
		 */

		private void treeView_AfterSelect(
			Object sender,
			TreeViewEventArgs e
			)
		{
			if ( this.AfterSelected != null )
			{
				this.AfterSelected(
					this,
					new ExplorerNodeEventArgs(
						( ( AbstractExplorerNode ) e.Node ).Item ) );
			}
		}

		private void treeView_BeforeExpand(
			Object sender,
			TreeViewCancelEventArgs e
			)
		{
			( ( AbstractExplorerNode ) e.Node ).BeforeExpand();
		}

		private void treeView_AfterCollapse(
			Object sender,
			TreeViewEventArgs e
			)
		{
			( ( AbstractExplorerNode ) e.Node ).AfterCollapse();
		}

		private void this_Disposed(
			Object sender,
			EventArgs e
			)
		{
			//
			// Wait for async operations to complete.
			//

			AbortRefreshSession();
			this.rundownLock.Rundown();

			//
			// Now that async operations have been completed, it is safe
			// to tear down the data structure.
			//

			if ( this.session != null )
			{
				this.session.Dispose();
			}
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public TestExplorer()
		{
			InitializeComponent();

			this.treeView.AfterSelect += treeView_AfterSelect;
			this.treeView.BeforeExpand += treeView_BeforeExpand;
			this.treeView.AfterCollapse += treeView_AfterCollapse;
			this.treeView.MouseDown += new MouseEventHandler( treeView_MouseDown );

			this.Disposed += this_Disposed;
		}

		public ImageList ImageList
		{
			get { return this.imageList; }
		}

		public NodeFactory NodeFactory
		{
			get { return nodeFactory; }
			set { nodeFactory = value; }
		}

		/*----------------------------------------------------------------------
		 * Context Menu.
		 */

		public ContextMenuStrip NodeContextMenu
		{
			get { return this.nodeContextMenu; }
			set { this.nodeContextMenu = value; }
		}

		private void treeView_MouseDown( object sender, MouseEventArgs e )
		{
			if ( this.nodeContextMenu == null || e.Button != MouseButtons.Right )
			{
				return;
			}

			TreeNode nodeHit = this.treeView.HitTest( e.X, e.Y ).Node;
			if ( nodeHit != null )
			{
				AbstractExplorerNode explNode = nodeHit as AbstractExplorerNode;
				if ( explNode != null && this.BeforeContextMenuPopup != null )
				{
					this.BeforeContextMenuPopup( 
						this,
						new ExplorerNodeEventArgs( explNode.Item ) );
				}

				this.nodeContextMenu.Show( this.treeView, new Point( e.X, e.Y ) );
			}
		}
		
		/*----------------------------------------------------------------------
		 * Session switching.
		 */

		public ISession Session
		{
			get
			{
				return this.session;
			}
		}

		public void SetSession( ISession sess, bool async )
		{
			if ( this.session != null )
			{
				this.session.Dispose();
			}

			this.treeView.Nodes.Clear();

			this.session = sess;

			sess.BeforeSetTests += new EventHandler( sess_BeforeSetTests );
			sess.AfterSetTests += new EventHandler( sess_AfterSetTests );

			//
			// Populate tree.
			//
			if ( sess.Tests != null )
			{
				RefreshSession( async, false );
			}
		}

		private void sess_AfterSetTests( object sender, EventArgs e )
		{
			//
			// Load new.
			//
			RefreshSession( true, false );
		}

		private void sess_BeforeSetTests( object sender, EventArgs e )
		{
			//
			// Clear old.
			//
			this.treeView.Nodes.Clear();
		}

		/*----------------------------------------------------------------------
		 * Session refreshing.
		 */

		public void AbortRefreshSession()
		{
			IAbortableTestItemCollection abort =
				this.session.Tests as IAbortableTestItemCollection;
			if ( abort != null )
			{
				abort.AbortRefresh();
			}
		}

		public void RefreshSession( bool async, bool selectionOnly )
		{
			if ( async )
			{
				//
				// N.B. Refreshing involves I/O and may block.
				//
				RefreshDelegate refresh = RefreshSession;
				this.rundownLock.Acquire();
				refresh.BeginInvoke( 
					false,
					selectionOnly,
					AsyncVoidCompletionCallback,
					refresh );
			}
			else
			{
				lock ( this.refreshLock )
				{
					if ( this.treeView.Nodes.Count == 0 )
					{
						//
						// Create initial tree mode -- children will
						// follow via events.
						//
						AbstractExplorerNode childNode =
							NodeFactory.CreateNode(
								this.treeView,
								session.Tests );

						if ( this.treeView.InvokeRequired )
						{
							this.treeView.Invoke( ( VoidDelegate ) delegate()
							{
								this.treeView.Nodes.Add( childNode );
							} );
						}
						else
						{
							this.treeView.Nodes.Add( childNode );
						}
					}

					ITestItem selectedItem = null;

					if ( this.treeView.InvokeRequired )
					{
						if ( this.RefreshStarted != null )
						{
							this.treeView.Invoke( ( VoidDelegate ) delegate()
							{
								this.RefreshStarted( this, EventArgs.Empty );

								selectedItem = this.SelectedItem;
							} );
						}
					}

					//
					// (Re-) load children.
					//
					if ( selectionOnly && selectedItem != null )
					{
						//
						// Refresh currently selected node only.
						//
						ITestItemCollection selectedItemColl =
							selectedItem as ITestItemCollection;

						if ( selectedItemColl != null )
						{
							selectedItemColl.Refresh();
						}
					}
					else
					{
						//
						// Refresh entire tree.
						//
						this.session.Tests.Refresh();
					}
				}
			}
		}

		public ITestItem SelectedItem
		{
			get
			{
				AbstractExplorerNode curNode =
					( AbstractExplorerNode ) this.treeView.SelectedNode;
				if ( curNode == null )
				{
					return null;
				}
				else
				{
					return curNode.Item;
				}
			}
		}
	}

	public class ExplorerNodeEventArgs : EventArgs
	{
		private readonly ITestItem item;

		internal ExplorerNodeEventArgs( ITestItem item )
		{
			this.item = item;
		}

		public ITestItem Item
		{
			get { return item; }
		}
	}

	public class ExceptionEventArgs : EventArgs
	{
		private readonly Exception exception;

		internal ExceptionEventArgs( Exception exception )
		{
			this.exception = exception;
		}

		public Exception Exception
		{
			get { return exception; }
		}
	}



}
