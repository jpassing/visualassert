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
	public partial class Explorer : UserControl
	{
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

		private ISession session;

		private readonly Object refreshLock = new Object();

		public delegate void ExplorerNodeEventHandler(
			Object sender,
			ExplorerNodeEventArgs args
			);

		public ExplorerNodeEventHandler AfterSelect;

		/*----------------------------------------------------------------------
		 * Events.
		 */

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


		/*----------------------------------------------------------------------
		 * Private.
		 */

		private void HandleException( Exception x )
		{
			MessageBox.Show( x.Message );
		}

		private delegate void AsyncVoidDelegate( bool async );

		private void AsyncVoidCompletionCallback( IAsyncResult ar )
		{
			try
			{
				AsyncVoidDelegate dlg = ( AsyncVoidDelegate ) ar.AsyncState;
				dlg.EndInvoke( ar );
			}
			catch ( Exception x )
			{
				HandleException( x );
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
			if ( this.AfterSelect != null )
			{
				this.AfterSelect(
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

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public Explorer()
		{
			InitializeComponent();

			this.treeView.AfterSelect += treeView_AfterSelect;
			this.treeView.BeforeExpand += treeView_BeforeExpand;
			this.treeView.AfterCollapse += treeView_AfterCollapse;
		}

		public ISession Session
		{
			get
			{
				return this.session;
			}
		}

		public void SetSession( ISession session, bool async )
		{
			if ( this.session != null )
			{
				this.session.Dispose();
			}
			this.treeView.Nodes.Clear();

			this.session = session;

			//
			// Populate tree.
			//
			RefreshSession( async );
		}

		public void RefreshSession( bool async )
		{
			if ( async )
			{
				//
				// N.B. Refreshing involves I/O and may block.
				//
				AsyncVoidDelegate refresh = RefreshSession;
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
						// Populate tree.
						//
						this.treeView.Nodes.Add(
							NodeFactory.CreateNode( session.Tests ) );
					}
					else
					{
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
				return curNode.Item;
			}
		}
	}

	

	
}
