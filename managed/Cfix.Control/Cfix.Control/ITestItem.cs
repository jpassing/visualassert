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
		// Disallow composition.
		//
		NonComposite = 1,

		//
		// Prefer fine-grained components i.o. to allow more flexible
		// scheduling.
		//
		FineGrained = 2
	}

	public interface ITestItem : IDisposable
    {
		event EventHandler Disposed;
		bool IsDisposed { get; }

        String Name { get; }
        uint Ordinal { get; }

		ITestItemCollection Parent { get; }

		void CreateAction( 
			ICompositeAction actionToComposeWith,
			SchedulingOptions schedulingOptions,
			CompositionOptions compositionOptions
			);
	}
}
