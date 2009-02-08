using System;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Control.Ui.Explorer
{
	internal static class NodeFactory
	{
		private class TestItemExplorerNode : AbstractExplorerNode
		{
			public TestItemExplorerNode( ITestItem item )
				: base(
					item,
					TestExplorer.TestItemIconIndex,
					TestExplorer.TestItemIconSelectedIndex )
			{
			}
		}

		private class InvalidModuleExplorerNode : AbstractExplorerNode
		{
			public InvalidModuleExplorerNode( InvalidModule item )
				: base(
					item,
					TestExplorer.InvalidModuleIconIndex,
					TestExplorer.InvalidModuleIconSelectedIndex )
			{
				this.ToolTipText = item.InvalidityCause.Message;
			}
		}

		private class TestItemCollectionExplorerNode : AbstractExplorerCollectionNode
		{
			public TestItemCollectionExplorerNode( TreeView treeView, ITestItemCollection item )
				: base(
					item,
					TestExplorer.TestContainerIconIndex,
					TestExplorer.TestContainerIconSelectedIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren( treeView );
			}
		}

		private class ModuleExplorerNode : AbstractExplorerCollectionNode
		{
			public ModuleExplorerNode( TreeView treeView, TestModule item )
				: base(
					item,
					TestExplorer.ModuleIconIndex,
					TestExplorer.ModuleIconSelectedIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren( treeView );

				this.ToolTipText = item.Path;
			}
		}

		private class GenericTestItemCollectionExplorerNode : AbstractExplorerCollectionNode
		{
			private delegate void VoidDelegate();

			public GenericTestItemCollectionExplorerNode( TreeView treeView, GenericTestItemCollection item )
				: base(
					item,
					TestExplorer.ContainerIconIndex,
					TestExplorer.ContainerIconIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren( treeView );

				item.ItemAdded += new EventHandler< TestItemEventArgs >( item_ItemAdded );
				item.ItemRemoved += new EventHandler< TestItemEventArgs >( item_ItemRemoved );
			}

			private void item_ItemAdded( 
				object sender, 
				TestItemEventArgs e 
				)
			{
				if ( this.TreeView.InvokeRequired )
				{
					this.TreeView.Invoke( ( VoidDelegate ) delegate()
					{
						this.Nodes.Add( NodeFactory.CreateNode( this.TreeView, e.Item ) );
						this.Expand();
					} );
				}
				else
				{
					this.Nodes.Add( NodeFactory.CreateNode( this.TreeView, e.Item ) );
					this.Expand();
				}
			}

			private void item_ItemRemoved(
				object sender, 
				TestItemEventArgs e 
				)
			{
				if ( this.TreeView.InvokeRequired )
				{
					this.TreeView.Invoke( ( VoidDelegate ) delegate()
					{
						this.Nodes.RemoveByKey( e.Item.Name );
					} );
				}
				else
				{
					this.Nodes.RemoveByKey( e.Item.Name );
				}
			}

			public override void BeforeExpand()
			{
				this.ImageIndex = TestExplorer.ContainerIconExpandedIndex;
				this.SelectedImageIndex = TestExplorer.ContainerIconExpandedIndex;
			}

			public override void AfterCollapse()
			{
				this.ImageIndex = TestExplorer.ContainerIconIndex;
				this.SelectedImageIndex = TestExplorer.ContainerIconIndex;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1800:DoNotCastUnnecessarily" )]
		internal static AbstractExplorerNode CreateNode( 
			TreeView treeView, 
			ITestItem item )
		{
			if ( item is TestModule )
			{
				return new ModuleExplorerNode( 
					treeView,
					( TestModule ) item );
			}
			else if ( item is InvalidModule )
			{
				return new InvalidModuleExplorerNode(
					( InvalidModule ) item );
			}
			else if ( item is GenericTestItemCollection )
			{
				return new GenericTestItemCollectionExplorerNode(
					treeView,
					( GenericTestItemCollection ) item );
			}
			else if ( item is ITestItemCollection )
			{
				return new TestItemCollectionExplorerNode(
					treeView,
					( ITestItemCollection ) item );
			}
			else if ( item is ITestItem )
			{
				return new TestItemExplorerNode( 
					( ITestItem ) item );
			}
			else
			{
				throw new ArgumentException( "Unrecognized node type" );
			}
		}
	}
}