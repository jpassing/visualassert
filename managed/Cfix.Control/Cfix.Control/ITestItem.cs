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
	}

	[Flags]
	public enum ThreadingOptions
	{
		None = 0,

		//
		// Run tests on threads that are COM-neutral, i.e. have not
		// joined any apartment.
		//
		ComNeutralThreading = ( int ) Native.NativeAction.CFIXCTL_ACTION_COM_NEUTRAL
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
			IActionEvents events,
			SchedulingOptions schedOpts,
			ThreadingOptions threadingOptions
			);

		IResultItem CreateResultItem(
			IResultItemCollection parentResult,
			IActionEvents events,
			ExecutionStatus interimStatus
			);
	}
}
