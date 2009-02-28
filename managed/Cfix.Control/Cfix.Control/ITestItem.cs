using System;

namespace Cfix.Control
{
	[Flags]
	public enum SchedulingOptions
	{
		None = 0,

		ShortcutFixtureOnFailure = ( int ) Native.NativeAction.CFIX_FIXTURE_EXECUTION_SHORTCUT_FIXTURE_ON_FAILURE,
		ShurtcutRunOnSetupFailure = ( int ) Native.NativeAction.CFIX_FIXTURE_EXECUTION_SHORTCUT_RUN_ON_SETUP_FAILURE,
		ShurtcutRunOnFailure = ( int ) Native.NativeAction.CFIX_FIXTURE_EXECUTION_SHORTCUT_RUN_ON_FAILURE,

		//
		// Run tests on threads that are COM-neutral, i.e. have not
		// joined any apartment.
		//
		ComNeutralThreading = 0x1000
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

		IAction CreateAction( 
			IHost hostToRunOn, 
			SchedulingOptions schedOpts 
			);
	}
}
