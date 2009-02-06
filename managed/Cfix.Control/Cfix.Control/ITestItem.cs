using System;

namespace Cfix.Control
{
    public interface ITestItem : IDisposable
    {
		event EventHandler Disposed;
		bool IsDisposed { get; }

        String Name { get; }
        uint Ordinal { get; }

		ITestItemCollection Parent { get; }

		IAction CreateAction( SchedulingOptions flags );
	}
}
