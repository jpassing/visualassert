using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.Ui.Explorer
{
	internal class AbstractExplorerCollectionNode : AbstractExplorerNode
	{
		private readonly ITestItemCollection testItemCollection;

		protected void LoadChildren()
		{
			foreach ( ITestItem child in testItemCollection )
			{
				if ( child != null )
				{
					this.Nodes.Add(
						NodeFactory.CreateNode( child ) );
				}
			}
		}

		public AbstractExplorerCollectionNode(
			ITestItemCollection testItemCollection,
			int imageIndex,
			int selectedImageIndex
			)
			: base( testItemCollection, imageIndex, selectedImageIndex )
		{
			this.testItemCollection = testItemCollection;
		}

		public ITestItemCollection ItemCollection
		{
			get { return this.testItemCollection; }
		}
	}
}
