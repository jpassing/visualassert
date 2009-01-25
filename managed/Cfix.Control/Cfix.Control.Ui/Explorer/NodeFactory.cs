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
					Explorer.TestItemIconIndex,
					Explorer.TestItemIconSelectedIndex )
			{
			}
		}

		private class TestItemCollectionExplorerNode : AbstractExplorerCollectionNode
		{
			public TestItemCollectionExplorerNode( TreeView treeView, ITestItemCollection item )
				: base(
					item,
					Explorer.TestContainerIconIndex,
					Explorer.TestContainerIconSelectedIndex )
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
					Explorer.ModuleIconIndex,
					Explorer.ModuleIconSelectedIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren( treeView );
			}
		}

		private class GenericTestItemCollectionExplorerNode : AbstractExplorerCollectionNode
		{
			public GenericTestItemCollectionExplorerNode( TreeView treeView, GenericTestItemCollection item )
				: base(
					item,
					Explorer.ContainerIconIndex,
					Explorer.ContainerIconIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren( treeView );
			}

			public override void BeforeExpand()
			{
				this.ImageIndex = Explorer.ContainerIconExpandedIndex;
				this.SelectedImageIndex = Explorer.ContainerIconExpandedIndex;
			}

			public override void AfterCollapse()
			{
				this.ImageIndex = Explorer.ContainerIconIndex;
				this.SelectedImageIndex = Explorer.ContainerIconIndex;
			}
		}

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
