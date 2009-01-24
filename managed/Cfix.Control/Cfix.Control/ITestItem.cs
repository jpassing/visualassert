using System;

namespace Cfix.Control
{
    public interface ITestItem
    {
        String Name { get; }
        uint Ordinal { get; }

		IAction CreateAction( SchedulingOptions flags );
	}
}
