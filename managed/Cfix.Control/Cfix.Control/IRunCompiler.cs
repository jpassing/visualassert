using System;
using System.Collections.Generic;

namespace Cfix.Control
{
	public interface IRunCompiler
	{
		SchedulingOptions SchedulingOptions { get; }
		ThreadingOptions ThreadingOptions { get; }
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
		void Add( ITestItem item );
	}
}
