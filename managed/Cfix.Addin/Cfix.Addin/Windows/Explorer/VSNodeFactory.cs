using System;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.Ui.Explorer;

namespace Cfix.Addin.Windows.Explorer
{
	public class VSNodeFactory : NodeFactory
	{
		public override AbstractExplorerNode CreateNode(
			TreeView treeView,
			ITestItem item )
		{
			return base.CreateNode( treeView, item );
		}
	}
	//internal class VSSolutionNode : AbstractExplorerNode
	//{
	//    public VSSolutionNode( 
	//        int iconIndex,
	//        int iconSelectedIndex
	//        )
	//        : base(
	//            iconIndex,
	//            iconSelectedIndex )
	//    {
	//    }
	//}
}
