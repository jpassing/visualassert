using System;
using System.Collections.Generic;

namespace Cfix.Control
{
	public class TestItemEventArgs : EventArgs
	{
		private ITestItem item;

		public TestItemEventArgs( ITestItem item )
		{
			this.item = item;
		}

		public ITestItem Item
		{
			get { return this.item; }
		}
	}

	public interface ITestItemCollection : ITestItem, IEnumerable<ITestItem>
	{
		event EventHandler< TestItemEventArgs > ItemAdded;
		event EventHandler< TestItemEventArgs > ItemRemoved;

		ITestItem GetItem( uint ordinal );
		uint ItemCount { get; }
		uint ItemCountRecursive { get; }

		void Refresh();
	}


	public interface IRunnableTestItemCollection
		: ITestItemCollection, IRunnableTestItem
	{
		/*++
		 * Number of immediate children that are runnable, i.e. 
		 * implement IRunnableItem.
		 --*/
		uint RunnableItemCount { get; }

		/*++
		 * Number of all children that are runnable, i.e. 
		 * implement IRunnableItem.
		 --*/
		uint RunnableItemCountRecursive { get; }

		/*++
		 * Indicates whether this collection is worth running. If
		 * RunnableItemCountRecursive == 0, it depends on the nature of
		 * the items (invalid or not) whether it can still be run or not.
		 * 
		 * If RunnableItemCountRecursive > 0, IsRunnable is always true.
		 --*/
		bool IsRunnable { get; }
	}

	/*++
	 * TestItemCollection that supports refreshs to be aborted.
	 --*/
	public interface IAbortableTestItemCollection : ITestItemCollection
	{
		void AbortRefresh();
	}
}
