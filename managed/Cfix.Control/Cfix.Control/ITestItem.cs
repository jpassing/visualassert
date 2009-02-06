using System;

namespace Cfix.Control
{
	[Flags]
	public enum SchedulingOptions
	{
		None = 0
	}

	[Flags]
	public enum CompositionOptions
	{
		//
		// Prefer fine-grained components i.o. to allow more flexible
		// scheduling.
		//
		FineGrained = 1
	}

	public interface ITestItem : IDisposable
    {
		event EventHandler Disposed;
		bool IsDisposed { get; }

        String Name { get; }
        uint Ordinal { get; }

		ITestItemCollection Parent { get; }

		IAction CreateAction( 
			SchedulingOptions schedulingOptions 
			);

		void CreateAction( 
			ICompositeAction actionToComposeWith,
			SchedulingOptions schedulingOptions,
			CompositionOptions compositionOptions
			);
	}
}
