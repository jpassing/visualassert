using System;
using System.Collections.Generic;
using Cfixctl;

namespace Cfix.Control
{
    public interface ITestItem
    {
        String Name { get; }
        uint Ordinal { get; }

		IAction CreateAction( SchedulingOptions flags );
	}

	public delegate void TestItemChangedEventHandler(
			ITestItemCollection sender,
			ITestItem item
			);

	public interface ITestItemCollection : ITestItem, IEnumerable< ITestItem >
    {
		event TestItemChangedEventHandler ItemAdded;
		event TestItemChangedEventHandler ItemRemoved;

        ITestItem GetItem( uint ordinal );
        uint ItemCount { get; }
    }
    
}
