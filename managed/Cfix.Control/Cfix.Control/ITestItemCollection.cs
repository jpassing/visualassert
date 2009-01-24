using System;
using System.Collections.Generic;

namespace Cfix.Control
{
	public delegate void TestItemChangedEventHandler(
			ITestItemCollection sender,
			ITestItem item
			);

	public interface ITestItemCollection : ITestItem, IEnumerable<ITestItem>
	{
		event TestItemChangedEventHandler ItemAdded;
		event TestItemChangedEventHandler ItemRemoved;

		ITestItem GetItem( uint ordinal );
		uint ItemCount { get; }

		void Refresh();
	}

}
