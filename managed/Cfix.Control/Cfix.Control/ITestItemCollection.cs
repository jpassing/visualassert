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
		uint RunnableItemCount { get; }
	}

	/*++
	 * TestItemCollection that supports refreshs to be aborted.
	 --*/
	public interface IAbortableTestItemCollection : ITestItemCollection
	{
		void AbortRefresh();
	}
}
