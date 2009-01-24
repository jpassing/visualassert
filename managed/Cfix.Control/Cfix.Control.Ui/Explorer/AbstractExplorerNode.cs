using System;
using System.Windows.Forms;
using Cfix.Control;

namespace Cfix.Control.Ui.Explorer
{
	internal abstract class AbstractExplorerNode : TreeNode
	{
		private readonly ITestItem testItem;
		private int defaultImageIndex;

		public AbstractExplorerNode(
			ITestItem testItem,
			int imageIndex,
			int selectedImageIndex
			)
			: base( testItem.Name, imageIndex, selectedImageIndex )
		{
			this.defaultImageIndex = imageIndex;
			this.testItem = testItem;
		}

		public ITestItem Item
		{
			get { return this.testItem; }
		}
		
		public virtual void BeforeExpand()
		{
		}

		public virtual void AfterCollapse()
		{
		}
	}
}
