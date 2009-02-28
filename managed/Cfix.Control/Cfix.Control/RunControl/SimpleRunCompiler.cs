using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.RunControl
{
	public class SimpleRunCompiler : IRunCompiler
	{
		private readonly IAgent agent;
		private readonly ITestItemCollection item;
		private readonly IDispositionPolicy dispositionPolicy;
		private readonly SchedulingOptions schedulingOptions;
		private readonly ThreadingOptions threadingOptions;

		public SimpleRunCompiler(
			IAgent agent,
			IDispositionPolicy policy,
			SchedulingOptions schedulingOptions,
			ThreadingOptions threadingOptions,
			ITestItemCollection item 
			)
		{
			this.agent = agent;
			this.item = item;
			this.dispositionPolicy = policy;
			this.schedulingOptions = schedulingOptions;
			this.threadingOptions = threadingOptions;
		}

		public IRun Compile()
		{
			IHost host = this.agent.CreateHost();
			Task task = new Task( host, this.dispositionPolicy );
			IAction action = this.item.CreateAction(
				host,
				task,
				this.schedulingOptions,
				this.threadingOptions );
			Run r = new Run(
				this.dispositionPolicy,
				( IResultItemCollection ) action.Result );
			task.SetParent( r );
			return r;
		}

	}
}
