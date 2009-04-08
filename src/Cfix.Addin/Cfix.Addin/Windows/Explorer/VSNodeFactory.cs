/*----------------------------------------------------------------------
 * Purpose:
 *		Factory for text explorer nodes.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

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
				TreeView treeView,
				NodeFactory factory,
				SolutionTestCollection sln,
				int iconIndex,
				int iconSelectedIndex
				)
				: base(
					treeView,
					factory,
					sln,
					iconIndex,
					iconSelectedIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren();
			}
		}

		private class VCProjectNode : AbstractExplorerCollectionNode
		{
			public VCProjectNode(
				TreeView treeView,
				NodeFactory factory,
				VCProjectTestCollection prj,
				int iconIndex,
				int iconSelectedIndex
				)
				: base(
					treeView,
					factory,
					prj,
					iconIndex,
					iconSelectedIndex )
			{
				//
				// Children always available, so load them.
				//
				LoadChildren();
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
					treeView,
					this,
					( SolutionTestCollection ) item,
					this.SolutionIconIndex,
					this.SolutionIconSelectedIndex );
			}
			else if ( item is VCProjectTestCollection )
			{
				return new VCProjectNode(
					treeView,
					this,
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
