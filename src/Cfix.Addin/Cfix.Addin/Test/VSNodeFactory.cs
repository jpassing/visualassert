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

namespace Cfix.Addin.Test
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

				prj.Refreshed += new EventHandler( prj_Refreshed );
			}

			private void prj_Refreshed( object sender, EventArgs e )
			{
				VCProjectTestCollection prj = 
					( VCProjectTestCollection ) sender;

				this.TreeView.BeginInvoke( ( VoidDelegate ) delegate {
					if ( prj.ItemCount > 0 )
					{
						//
						// Has fixtures -> cfix DLL.
						//
						this.ForeColor = SystemColors.ControlText;
						this.ToolTipText = null;
					}
					else
					{
						//
						// No fixtures -> regular project.
						//
						this.ForeColor = Color.Gray;
						this.ToolTipText = Strings.NotATestProjectToolTip;
					}
				} );
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
