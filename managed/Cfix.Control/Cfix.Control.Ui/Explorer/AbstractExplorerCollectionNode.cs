using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cfix.Control.Ui.Explorer
{
	internal class AbstractExplorerCollectionNode : AbstractExplorerNode
	{
		protected readonly NodeFactory nodeFactory;
		private readonly ITestItemCollection testItemCollection;
		private delegate void VoidDelegate();

		protected void LoadChildren( TreeView treeView )
		{
			foreach ( ITestItem child in testItemCollection )
			{
				if ( child != null )
				{
					AbstractExplorerNode childNode =
						this.nodeFactory.CreateNode( treeView, child );

					if ( treeView.InvokeRequired )
					{
						treeView.Invoke( ( VoidDelegate ) delegate()
						{
							this.Nodes.Add( childNode );
						} );
					}
					else
					{
						this.Nodes.Add( childNode );
					}
				}
			}
		}

		public AbstractExplorerCollectionNode(
			NodeFactory factory,
			ITestItemCollection testItemCollection,
			int imageIndex,
			int selectedImageIndex
			)
			: base( testItemCollection, imageIndex, selectedImageIndex )
		{
			this.nodeFactory = factory;
			this.testItemCollection = testItemCollection;
		}

		//public ITestItemCollection ItemCollection
		//{
		//    get { return this.testItemCollection; }
		//}
	}
}
