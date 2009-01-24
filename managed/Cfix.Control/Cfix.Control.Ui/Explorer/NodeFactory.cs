using System;
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
			public TestItemCollectionExplorerNode( ITestItemCollection item )
				: base(
					item,
					Explorer.TestContainerIconIndex,
					Explorer.TestContainerIconSelectedIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren();
			}
		}

		private class ModuleExplorerNode : AbstractExplorerCollectionNode
		{
			public ModuleExplorerNode( TestModule item )
				: base(
					item,
					Explorer.ModuleIconIndex,
					Explorer.ModuleIconSelectedIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren();
			}
		}

		private class GenericTestItemCollectionExplorerNode : AbstractExplorerCollectionNode
		{
			public GenericTestItemCollectionExplorerNode( GenericTestItemCollection item )
				: base(
					item,
					Explorer.ContainerIconIndex,
					Explorer.ContainerIconIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren();
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

		internal static AbstractExplorerNode CreateNode( ITestItem item )
		{
			if ( item is TestModule )
			{
				return new ModuleExplorerNode( 
					( TestModule ) item );
			}
			else if ( item is GenericTestItemCollection )
			{
				return new GenericTestItemCollectionExplorerNode( 
					( GenericTestItemCollection ) item );
			}
			else if ( item is ITestItemCollection )
			{
				return new TestItemCollectionExplorerNode( 
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
