using System;

namespace Cfix.Control
{
	[Flags]
	public enum ExecutionOptions
	{
		None = 0,

		ShortCircuitFixtureOnFailure = ( int ) Native.NativeAction.CFIX_FIXTURE_EXECUTION_SHORTCIRCUIT_FIXTURE_ON_FAILURE,
		ShurtCircuitRunOnSetupFailure = ( int ) Native.NativeAction.CFIX_FIXTURE_EXECUTION_SHORTCIRCUIT_RUN_ON_SETUP_FAILURE,
		ShortCircuitRunOnFailure = ( int ) Native.NativeAction.CFIX_FIXTURE_EXECUTION_SHORTCIRCUIT_RUN_ON_FAILURE,

		CaptureStackTraces = ( int ) Native.NativeAction.CFIX_FIXTURE_EXECUTION_CAPTURE_STACK_TRACES
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
