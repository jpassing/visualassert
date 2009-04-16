using System;
using System.Collections.Generic;

namespace Cfix.Control
{
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
	public enum ExecutionOptions
	{
		None = 0,
		AutoAdjustCurrentDirectory = 1
	}

	public interface IRunCompiler
	{
		SchedulingOptions SchedulingOptions { get; }
		ThreadingOptions ThreadingOptions { get; }
		ExecutionOptions ExecutionOptions { get; }

		IRun Compile();

		/*++
		 * Method Description:
		 *		For use by implementors of ITestItems only.
		 --*/
		void Add( IAction action );
		
		/*++
		 * Method Description:
		 *		Convenience method for use by client.
		 --*/
		void Add( IRunnableTestItem item );

		HostEnvironment Environment { get; }
	}
}
