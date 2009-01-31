using System;

namespace Cfix.Control
{
    public interface ITestItem : IDisposable
    {
        String Name { get; }
        uint Ordinal { get; }

		ITestItemCollection Parent { get; }

		IAction CreateAction( SchedulingOptions flags );
	}
}
