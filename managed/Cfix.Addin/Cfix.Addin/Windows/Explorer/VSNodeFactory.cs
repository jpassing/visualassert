using System;
using System.Drawing;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.Ui.Explorer;

namespace Cfix.Addin.Windows.Explorer
{
	public class VSNodeFactory : NodeFactory
	{
		private readonly int SolutionIconIndex;
		private readonly int SolutionIconSelectedIndex;
		private readonly int ProjectIconIndex;
		private readonly int ProjectIconSelectedIndex;

		private class SolutionNode : AbstractExplorerCollectionNode
		{
			public SolutionNode(
				NodeFactory factory,
				TreeView treeView,
				SolutionTestCollection sln,
				int iconIndex,
				int iconSelectedIndex
				)
				: base(
					factory,
					sln,
					iconIndex,
					iconSelectedIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren( treeView );
			}
		}

		private class VCProjectNode : AbstractExplorerCollectionNode
		{
			public VCProjectNode(
				NodeFactory factory,
				TreeView treeView,
				VCProjectTestCollection prj,
				int iconIndex,
				int iconSelectedIndex
				)
				: base(
					factory,
					prj,
					iconIndex,
					iconSelectedIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren( treeView );
			}
		}
		
		public VSNodeFactory( TestExplorer explorer )
		{
			//
			// Add icons for VS object nodes.
			//
			this.SolutionIconIndex = explorer.ImageList.Images.Add(
				Icons.VSObject_Solution, Color.Magenta );
			this.ProjectIconIndex = explorer.ImageList.Images.Add(
				Icons.VSObject_VCProject, Color.Magenta );

			this.SolutionIconSelectedIndex = SolutionIconIndex;
			this.ProjectIconSelectedIndex = ProjectIconIndex;
		}

		public override AbstractExplorerNode CreateNode(
			TreeView treeView,
			ITestItem item )
		{
			if ( item is SolutionTestCollection )
			{
				return new SolutionNode(
					this,
					treeView,
					( SolutionTestCollection ) item,
					this.SolutionIconIndex,
					this.SolutionIconSelectedIndex );
			}
			else if ( item is VCProjectTestCollection )
			{
				return new VCProjectNode(
					this,
					treeView,
					( VCProjectTestCollection ) item,
					this.ProjectIconIndex,
					this.ProjectIconSelectedIndex );
			}
			else
			{
				return base.CreateNode( treeView, item );
			}
		}
	}
	
}
