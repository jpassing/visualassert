using System;

namespace Cfix.Control
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags" ), Flags]
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

	public interface ITestItem : IDisposable
    {
		event EventHandler Disposed;
		bool IsDisposed { get; }

        String Name { get; }
        uint Ordinal { get; }

		ITestItemCollection Parent { get; }

		void Add( 
			IRunCompiler compiler,
			IActionEvents events,
			IResultItem result
			);
	}

	public interface IRunnableTestItem : ITestItem
	{
		IResultItem CreateResultItem(
			IResultItemCollection parentResult,
			IActionEvents events,
			ExecutionStatus interimStatus
			);
	}

}