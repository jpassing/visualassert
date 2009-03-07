using System;
using System.Diagnostics;
using System.Windows.Forms;
using Cfix.Control;

namespace Cfix.Control.Ui.Explorer
{
	public abstract class AbstractExplorerNode : TreeNode
	{
		private readonly ITestItem testItem;

		protected AbstractExplorerNode(
			ITestItem testItem,
			int imageIndex,
			int selectedImageIndex
			)
			: base( testItem.Name, imageIndex, selectedImageIndex )
		{
			Debug.Assert( testItem != null );

			this.testItem = testItem;

			//
			// Assign name to make RemoveByKey work.
			//
			this.Name = testItem.Name;
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
