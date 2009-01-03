using System;
using Cfixctl;

namespace Cfix.Control
{
    public interface ITestItem
    {
        String Name { get; }
        uint Ordinal { get; }
    }

	public delegate void TestItemChangedEventHandler(
			ITestItemContainer sender,
			ITestItem item
			);

    public interface ITestItemContainer : ITestItem
    {
		event TestItemChangedEventHandler ItemAdded;
		event TestItemChangedEventHandler ItemRemoved;

        ITestItem GetItem( uint ordinal );
        uint ItemCount { get; }
    }
    
}
