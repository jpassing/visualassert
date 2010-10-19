using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.RunControl
{
	// Non-threadsafe
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable" )]
	public abstract class RunCompilerBase : IRunCompiler
	{
		private readonly ExecutionOptions executionOptions;
		private readonly EnvironmentOptions environmentOptions;

		protected internal readonly AgentSet agentSet;
		protected internal readonly Run run;

		private readonly HostEnvironment env = new HostEnvironment();

		//
		// Result root node.
		//
		protected internal IResultItem result;

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

		protected static AgentSet CreateSingleArchitectureAgentSet(
			IAgent agent
			)
		{
			AgentSet set = new AgentSet();
			set.AddArchitecture( agent );
			return set;
		}

		/*--------------------------------------------------------------
		 * Ctor.
		 */

		protected RunCompilerBase(
			AgentSet agentSet,
			IDispositionPolicy policy,
			ExecutionOptions executionOptions,
			EnvironmentOptions environmentOptions
			)
		{
			this.agentSet = agentSet;
			this.executionOptions = executionOptions;
			this.environmentOptions = environmentOptions;

			this.run = new Run( policy );
		}

		/*--------------------------------------------------------------
		 * Abstracts.
		 */

		/*++
		 * Add action. The action has passed all checks and its result
		 * belongs to a common tree of results.
		--*/
		protected abstract void AddAction( IAction action );

		/*++
		 * Compile a run based on all actions added through AddAction.
		--*/
		public abstract IRun Compile();

		/*--------------------------------------------------------------
		 * IRunCompiler.
		 */

		public ExecutionOptions ExecutionOptions
		{
			get { return this.executionOptions; }
		}

		public EnvironmentOptions EnvironmentOptions
		{
			get { return this.environmentOptions; }
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

			AddAction( action );
		}

		public virtual void Add( IRunnableTestItem item )
		{
			item.Add(
				this,
				this.run,
				item.CreateResultItem(
					null,
					this.run,
					ExecutionStatus.Pending ) );
		}

		public HostEnvironment Environment
		{
			get { return this.env; }
		}
	}
}
