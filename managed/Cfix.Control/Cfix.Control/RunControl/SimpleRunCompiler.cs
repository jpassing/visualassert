using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.RunControl
{
	public class SimpleRunCompiler : IRunCompiler
	{
		private readonly IAgent agent;
		private readonly SchedulingOptions schedulingOptions;
		private readonly ThreadingOptions threadingOptions;
		private readonly Run run;
		private IAction action;

		public SimpleRunCompiler(
			IAgent agent,
			IDispositionPolicy policy,
			SchedulingOptions schedulingOptions,
			ThreadingOptions threadingOptions
			)
		{
			this.agent = agent;
			this.schedulingOptions = schedulingOptions;
			this.threadingOptions = threadingOptions;
			this.run = new Run( policy );
		}

		public SchedulingOptions SchedulingOptions
		{
			get { return this.schedulingOptions; }
		}

		public ThreadingOptions ThreadingOptions
		{
			get { return this.threadingOptions; }
		}

		public void Add( IAction action )
		{
			if ( this.action != null )
			{
				throw new InvalidOperationException();
			}

			this.action = action;
		}

		public void Add( ITestItem item )
		{
			item.Add(
				this,
				null,
				this.run );
		}

		public IRun Compile()
		{
			Task task = new Task( this.agent.CreateHost() );
			if ( this.action != null )
			{
				task.AddAction( this.action );

				this.run.RootResult = ( IResultItemCollection ) this.action.Result;
				this.run.AddTask( task );
			}

			return this.run;
		}

	}
}
