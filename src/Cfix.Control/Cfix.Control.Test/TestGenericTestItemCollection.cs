using System;
using NUnit.Framework;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Control.Test
{
	[TestFixture]
	public class TestGenericTestItemCollection
	{
		[Test]
		public void TestInvalidItemNotCountedAsRunnableItem()
		{
			GenericTestItemCollection coll = new GenericTestItemCollection( null, "test" );
			coll.Add( new InvalidModule( coll, "inv", new CfixException() ) );

			Assert.AreEqual( 1, coll.ItemCount );
			Assert.AreEqual( 1, coll.ItemCountRecursive );
			Assert.AreEqual( 0, coll.RunnableItemCount );
			Assert.AreEqual( 0, coll.RunnableItemCountRecursive );
		}

		[Test]
		public void TestInvalidItemNotCountedAsRunnableItemWhenNested()
		{
			GenericTestItemCollection coll = new GenericTestItemCollection( null, "test" );
			GenericTestItemCollection innerColl = new GenericTestItemCollection( coll, "inner" );
			coll.Add( innerColl );
			innerColl.Add( new InvalidModule( coll, "inv", new CfixException() ) );

			Assert.AreEqual( 1, coll.ItemCount );
			Assert.AreEqual( 1, coll.ItemCountRecursive );
			Assert.AreEqual( 1, coll.RunnableItemCount );
			Assert.AreEqual( 0, coll.RunnableItemCountRecursive );
		}
	}
}
