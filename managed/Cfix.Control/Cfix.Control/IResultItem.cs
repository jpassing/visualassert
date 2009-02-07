using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public interface IResultItem
	{
		IResultItemCollection Parent { get; }

		//
		// TestItem this result corresponds to - may or may not be
		// still available.
		//
		ITestItem Item { get; }

		String Name { get; }
		ExecutionStatus Status { get; }

		IRun Run { get; }
	}
}
