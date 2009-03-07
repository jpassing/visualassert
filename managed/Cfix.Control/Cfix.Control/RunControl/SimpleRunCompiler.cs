using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.RunControl
{
	// Non-threadsafe
	public class SimpleRunCompiler : IRunCompiler
	{
		private readonly IAgent agent;
		private readonly SchedulingOptions schedulingOptions;
		private readonly ThreadingOptions threadingOptions;
		private readonly Run run;

		private IList<IAction> actions = new List<IAction>();
		private IResultItem result;

		private static IResultItem GetRootAncestor( IResultItem item )
		{
			IResultItem parent = item.Parent;
			if ( parent != null )
			{
				return GetRootAncestor( parent );
			}
			else
			{
				return item;
			}
		}

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

		/*--------------------------------------------------------------
		 * IRunCompiler.
		 */

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
			if ( this.result == null )
			{
				this.result = GetRootAncestor( action.Result );
			}
			else
			{
				//
				// The action's result must belong to the existing
				// tree of results.
				//
				if ( ReferenceEquals(
					GetRootAncestor( action.Result ),
					GetRootAncestor( this.result ) ) )
				{
					//
					// Ok.
					//
				}
				else
				{
					//
					// Forests not supported.
					//
					throw new ArgumentException(
						"The action's result does not integrate with " +
						"the existing result tree" );

				}
			}

			this.actions.Add( action );
		}

		public void Add( IRunnableTestItem item )
		{
			item.Add(
				this,
				this.run,
				item.CreateResultItem(
					null,
					this.run,
					ExecutionStatus.Pending ) );
		}

		public IRun Compile()
		{
			if ( this.actions.Count > 0 )
			{
				Task task = new Task( this.agent, this.agent.CreateHost() );
				
				foreach ( IAction act in this.actions )
				{
					task.AddAction( act );
				}

				this.run.RootResult = ( IResultItemCollection ) this.result;
				this.run.AddTask( task );
			}

			return this.run;
		}

	}
}
