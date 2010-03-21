using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Cfix.Control.Native;

namespace Cfix.Control.RunControl
{
	// Non-threadsafe
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable" )]
	public class ProcessPerTestRunCompiler : RunCompilerBase
	{
		//
		// List of actions. For each action, a separate host process
		// will be created.
		//
		private ICollection<IAction> actions = new LinkedList<IAction>();

		public ProcessPerTestRunCompiler(
			AgentSet agentSet,
			IDispositionPolicy policy,
			ExecutionOptions executionOptions,
			EnvironmentOptions environmentOptions
			)
			: base(
				agentSet,
				policy,
				executionOptions,
				environmentOptions )
		{
		}

		public ProcessPerTestRunCompiler(
			IAgent agent,
			IDispositionPolicy policy,
			ExecutionOptions executionOptions,
			EnvironmentOptions environmentOptions
			)
			: this(
				CreateSingleArchitectureAgentSet( agent ),
				policy,
				executionOptions,
				environmentOptions )
		{ }

		protected virtual Task CreateTask( IAction action )
		{
			return new Task(
				this.agentSet.GetAgent( action.Architecture ),
				this.Environment );
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		protected override void AddAction( IAction action )
		{
			this.actions.Add( action );
		}

		public override IRun Compile()
		{
			int tasks = 0;
			foreach ( IAction action in this.actions )
			{
				Task task = CreateTask( action );
				this.run.AddTask( task );
				task.AddAction( action );

				tasks++;
			}

			if ( tasks == 0 )
			{
				throw new EmptyRunException();
			}

			this.run.RootResult = ( IResultItemCollection ) this.result;

			return this.run;
		}

		public void Add( IRunnableTestItem item, IResultItem result )
		{
			ITestItemCollection coll = item as ITestItemCollection;
			if ( coll != null )
			{
				//
				// Traverse in order to avoid creating actions for non-leaves.
				//

				foreach ( ITestItem childItem in coll )
				{
					IRunnableTestItem runnableItem = childItem as IRunnableTestItem;
					if ( runnableItem != null )
					{
						Add( 
							runnableItem, 
							( ( IResultItemCollection ) result ).GetItem( runnableItem ) );
					}
				}
			}
			else // leaf.
			{
				Debug.Assert( item is TestCase );

				//
				// Create action.
				//
				// N.B. Because this action is on test case level, we have
				// a situation similar to single-testcase runs. That is, 
				// although the action references a test case, the associated
				// result item has to be on fixture-level.
				//

				if ( result.Parent != null )
				{
					result = result.Parent;
				}

				item.Add(
					this,
					this.run,
					result );
			}
		}
		
		public override void Add( IRunnableTestItem item )
		{
			//
			// Provide root result item to avoid forests.
			//
			IResultItem rootResult = item.CreateResultItem(
				null,
				this.run,
				ExecutionStatus.Pending );
			if ( this.result == null )
			{
				this.result = rootResult;
			}

			Add( item, rootResult );
		}
	}
}
