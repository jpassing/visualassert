using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public interface IRunCompiler
	{
		SchedulingOptions SchedulingOptions { get; }
		ThreadingOptions ThreadingOptions { get; }
		IRun Compile();
		void Add( IAction action );
		void Add( ITestItem item );
	}
}
