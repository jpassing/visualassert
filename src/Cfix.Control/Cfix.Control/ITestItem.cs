using System;

namespace Cfix.Control
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags" )]
	public enum SchedulingOptions
	{
		None = 0,

		ShortCircuitFixtureOnFailure = ( int ) Native.NativeAction.CFIX_FIXTURE_EXECUTION_SHORTCIRCUIT_FIXTURE_ON_FAILURE,
		ShurtCircuitRunOnSetupFailure = ( int ) Native.NativeAction.CFIX_FIXTURE_EXECUTION_SHORTCIRCUIT_RUN_ON_SETUP_FAILURE,
		ShortCircuitRunOnFailure = ( int ) Native.NativeAction.CFIX_FIXTURE_EXECUTION_SHORTCIRCUIT_RUN_ON_FAILURE,
	}

	public interface ITestItem : IDisposable
    {
		event EventHandler Disposed;
		bool IsDisposed { get; }

        String Name { get; }

		//
		// Full name of function backing this test, e.g. Foo::Bar. Max be null.
		//
		String FullName { get; }

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
