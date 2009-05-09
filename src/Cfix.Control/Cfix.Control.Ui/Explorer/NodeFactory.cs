using System;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Control.Ui.Explorer
{
	public class NodeFactory
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
			public TestItemCollectionExplorerNode(
				TreeView treeView,
				NodeFactory factory,
				ITestItemCollection item 
				)
				: base(
					treeView,
					factory,
					item,
					TestExplorer.TestContainerIconIndex,
					TestExplorer.TestContainerIconSelectedIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren();
			}
		}

		private class ModuleExplorerNode : AbstractExplorerCollectionNode
		{
			public ModuleExplorerNode(
				TreeView treeView,
				NodeFactory factory,
				TestModule item 
				)
				: base(
					treeView,
					factory,
					item,
					TestExplorer.ModuleIconIndex,
					TestExplorer.ModuleIconSelectedIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren();

				this.ToolTipText = String.Format(
					"{0} ({1})",
					item.Path,
					item.Architecture );
			}
		}

		private class GenericTestItemCollectionExplorerNode : AbstractExplorerCollectionNode
		{
			public GenericTestItemCollectionExplorerNode(
				TreeView treeView,
				NodeFactory factory,
				GenericTestItemCollection item 
				)
				: base(
					treeView,
					factory,
					item,
					TestExplorer.ContainerIconIndex,
					TestExplorer.ContainerIconIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren();
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
		public virtual AbstractExplorerNode CreateNode( 
			TreeView treeView, 
			ITestItem item )
		{
			if ( item is TestModule )
			{
				return new ModuleExplorerNode(
					treeView,
					this,
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
					this,
					( GenericTestItemCollection ) item );
			}
			else if ( item is ITestItemCollection )
			{
				return new TestItemCollectionExplorerNode(
					treeView,
					this, 
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
