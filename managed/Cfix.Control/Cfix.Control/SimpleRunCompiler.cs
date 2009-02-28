using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public class SimpleRunCompiler : IRunCompiler
	{
		private readonly ITestItemCollection item;
		private readonly IDispositionPolicy dispositionPolicy;
		private readonly SchedulingOptions schedulingOptions;

		public SimpleRunCompiler(
			IDispositionPolicy policy,
			SchedulingOptions schedulingOptions,
			ITestItemCollection item 
			)
		{
			this.item = item;
			this.dispositionPolicy = policy;
			this.schedulingOptions = schedulingOptions;
		}

		public IRun Compile()
		{
			return new Run(
				this.dispositionPolicy,
				this.schedulingOptions,
				this.item );
		}

	}
}
