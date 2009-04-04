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

		ICollection<Failure> Failures { get; }

		String Name { get; }
		ExecutionStatus Status { get; set; }
		TimeSpan Duration { get; }
		bool Completed { get; }

		//
		// Enforce completion:
		// Mark pending sub-results skipped and calculate status.
		//
		void ForceCompletion( bool propagateToParent );
	}
}
