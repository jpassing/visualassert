using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		
		private ISession session;

		private readonly Object refreshLock = new Object();

		//
		// Lock that needs to be acquired by async operations to protect
		// from the control being torn down prematurely.
		//
		private RundownLock rundownLock = new RundownLock();

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1713:EventsShouldNotHaveBeforeOrAfterPrefix" )]
		public event EventHandler< ExplorerNodeEventArgs > AfterSelected;
		public event EventHandler<ExceptionEventArgs> ExceptionRaised;

		/*----------------------------------------------------------------------
		 * Private.
		 */

		private static void HandleException( Exception x )
		{
			MessageBox.Show( x.Message );
		}

		private delegate void AsyncVoidDelegate( bool async );

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes" )]
		private void AsyncVoidCompletionCallback( IAsyncResult ar )
		{
			try
			{
				AsyncVoidDelegate dlg = ( AsyncVoidDelegate ) ar.AsyncState;
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
			if ( this.session != null )
			{
				this.session.Dispose();
			}

			this.rundownLock.Rundown();
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

			this.Disposed += this_Disposed;
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
				RefreshSession( async );
			}
		}

		private void sess_AfterSetTests( object sender, EventArgs e )
		{
			//
			// Load new.
			//
			RefreshSession( true );
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

		public void RefreshSession( bool async )
		{
			if ( async )
			{
				//
				// N.B. Refreshing involves I/O and may block.
				//
				AsyncVoidDelegate refresh = RefreshSession;
				this.rundownLock.Acquire();
				refresh.BeginInvoke( 
					false,
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

					//
					// (Re-) load children.
					//
					this.session.Tests.Refresh();
				}
			}
		}

		public ITestItem SelectedItem
		{
			get
			{
				AbstractExplorerNode curNode =
					( AbstractExplorerNode ) this.treeView.SelectedNode;
				return curNode.Item;
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
