using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cfix.Control.Ui.Explorer
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2229:ImplementSerializationConstructors" ), Serializable]
	public class AbstractExplorerCollectionNode : AbstractExplorerNode
	{
		private readonly TreeView treeView;
		private readonly ITestItemCollection testItemCollection;
		private readonly NodeFactory nodeFactory;

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix" )]
		protected delegate void VoidDelegate();

		private void AddNode( AbstractExplorerNode node )
		{
			if ( this.treeView.InvokeRequired )
			{
				this.treeView.BeginInvoke( ( VoidDelegate ) delegate()
				{
					this.Nodes.Add( node );
					if ( node is AbstractExplorerCollectionNode )
					{
						this.Expand();
					}
				} );
			}
			else
			{
				this.Nodes.Add( node );
				this.Expand();
			}
		}

		private void testItemCollection_ItemRemoved( 
			object sender, 
			TestItemEventArgs e 
			)
		{
			if ( this.treeView.InvokeRequired )
			{
				this.treeView.BeginInvoke( ( VoidDelegate ) delegate()
				{
					this.Nodes.RemoveByKey( e.Item.Name );
				} );
			}
			else
			{
				this.Nodes.RemoveByKey( e.Item.Name );
			}
		}

		private void testItemCollection_ItemAdded( 
			object sender, 
			TestItemEventArgs e 
			)
		{
			AddNode( this.nodeFactory.CreateNode( this.treeView, e.Item ) );
		}

		protected void LoadChildren()
		{
			foreach ( ITestItem child in testItemCollection )
			{
				if ( child != null )
				{
					AbstractExplorerNode childNode =
						this.nodeFactory.CreateNode( this.treeView, child );

					AddNode( childNode );
				}
			}
		}

		public AbstractExplorerCollectionNode(
			TreeView treeView,
			NodeFactory factory,
			ITestItemCollection testItemCollection,
			int imageIndex,
			int selectedImageIndex
			)
			: base( testItemCollection, imageIndex, selectedImageIndex )
		{
			this.nodeFactory = factory;
			this.treeView = treeView;
			this.testItemCollection = testItemCollection;

			testItemCollection.ItemAdded += new EventHandler<TestItemEventArgs>( testItemCollection_ItemAdded );
			testItemCollection.ItemRemoved += new EventHandler<TestItemEventArgs>( testItemCollection_ItemRemoved );
		}

		//public ITestItemCollection ItemCollection
		//{
		//    get { return this.testItemCollection; }
		//}
	}
}
