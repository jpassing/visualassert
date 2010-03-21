using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public enum ExecutionStatus
	{
		Pending,
		Running,
		Skipped,
		Stopped,
		Succeeded,
		SucceededWithInconclusiveParts,
		SucceededWithSkippedParts,
		Failed,
		Inconclusive,
		PostprocessingFailed
	}
}
