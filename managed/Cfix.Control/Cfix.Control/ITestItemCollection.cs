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

	public delegate void TestItemChangedEventHandler(
			object sender,
			TestItemEventArgs args
			);

	public interface ITestItemCollection : ITestItem, IEnumerable<ITestItem>
	{
		event TestItemChangedEventHandler ItemAdded;
		event TestItemChangedEventHandler ItemRemoved;

		ITestItem GetItem( uint ordinal );
		uint ItemCount { get; }

		void Refresh();
	}

	/*++
	 * TestItemCollection that supports refreshs to be aborted.
	 --*/
	public interface IAbortableTestItemCollection : ITestItemCollection
	{
		void AbortRefresh();
	}
}
