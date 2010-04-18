using System;
using NUnit.Framework;
using Cfix.Control;
using Cfix.Control.Native;
using System.Collections.Generic;

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

		[Test]
		public void TestCollectionMaintainsOrderInWhichItemsWereAdded()
		{
			GenericTestItemCollection coll = new GenericTestItemCollection( null, "test" );
			coll.Add( new InvalidModule( coll, "C", new CfixException() ) );
			coll.Add( new InvalidModule( coll, "A", new CfixException() ) );
			coll.Add( new InvalidModule( coll, "B", new CfixException() ) );

			IEnumerator<ITestItem> en = coll.GetEnumerator();
			en.MoveNext();
			Assert.AreEqual( "C", en.Current.Name );
			en.MoveNext();
			Assert.AreEqual( "A", en.Current.Name );
			en.MoveNext();
			Assert.AreEqual( "B", en.Current.Name );
		}

		[Test]
		public void TestSortedCollectionEnumeratedInPropOrder()
		{
			SortedTestItemCollection coll = new SortedTestItemCollection( 
				null, 
				"test",
				new SortedTestItemCollection.ByNameComparer() );
			coll.Add( new InvalidModule( coll, "C", new CfixException() ) );
			coll.Add( new InvalidModule( coll, "A", new CfixException() ) );
			coll.Add( new InvalidModule( coll, "B", new CfixException() ) );

			IEnumerator<ITestItem> en = coll.GetEnumerator();
			en.MoveNext();
			Assert.AreEqual( "A", en.Current.Name );
			en.MoveNext();
			Assert.AreEqual( "B", en.Current.Name );
			en.MoveNext();
			Assert.AreEqual( "C", en.Current.Name );
		}
	}
}
