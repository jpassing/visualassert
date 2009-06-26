using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.RunControl
{
	// Non-threadsafe
	public class SimpleRunCompiler : IRunCompiler
	{
		private readonly AgentSet agentSet;
		private readonly ExecutionOptions executionOptions;
		private readonly ThreadingOptions threadingOptions;
		private readonly EnvironmentOptions envOptions;
		private readonly Run run;
		private readonly bool allowMultipleArchitectures;

		private readonly HostEnvironment env = new HostEnvironment();

		//
		// One action list per architecture.
		//
		private IList<IAction>[] actions =
			new IList<IAction>[ ( int ) Architecture.Max + 1 ];

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

		private static AgentSet CreateSingleArchitectureAgentSet(
			IAgent agent
			)
		{
			AgentSet set = new AgentSet();
			set.AddArchitecture( agent );
			return set;
		}

		public SimpleRunCompiler(
			AgentSet agentSet,
			IDispositionPolicy policy,
			ExecutionOptions executionOptions,
			ThreadingOptions threadingOptions,
			EnvironmentOptions envOptions,
			bool allowMultipleArchitectures
			)
		{
			for ( int i = 0; i < this.actions.Length; i++ )
			{
				this.actions[ i ] = new List<IAction>();
			}

			this.agentSet = agentSet;
			this.executionOptions = executionOptions;
			this.threadingOptions = threadingOptions;
			this.envOptions = envOptions;

			this.allowMultipleArchitectures = allowMultipleArchitectures;
			this.run = new Run( policy );
		}

		internal SimpleRunCompiler(
			IAgent agent,
			IDispositionPolicy policy,
			ExecutionOptions executionOptions,
			ThreadingOptions threadingOptions,
			EnvironmentOptions envOptions
			)
			: this(
				CreateSingleArchitectureAgentSet( agent ),
				policy,
				executionOptions,
				threadingOptions,
				envOptions,
				true )
		{ }

		/*--------------------------------------------------------------
		 * IRunCompiler.
		 */

		public ExecutionOptions ExecutionOptions
		{
			get { return this.executionOptions; }
		}

		public ThreadingOptions ThreadingOptions
		{
			get { return this.threadingOptions; }
		}

		public EnvironmentOptions EnvironmentOptions
		{
			get { return this.envOptions; }
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

			int actionsIndex = ( int ) action.Architecture;
			if ( !this.allowMultipleArchitectures )
			{
				for ( int i = 0; i < this.actions.Length; i++ )
				{
					if ( i != actionsIndex && this.actions[ i ].Count > 0 )
					{
						//
						// Architecture differs from previously added
						// actions. This would result in another host
						// having to be created.
						//
						throw new ArchitectureMismatchException();
					}
				}
			}
			this.actions[ actionsIndex ].Add( action );
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
			int tasks = 0;
			for ( int i = 0; i < this.actions.Length; i++ )
			{
				if ( this.actions[ i ].Count > 0 )
				{
					IAgent agent = this.agentSet.GetAgent( ( Architecture ) i );
					Task task = new Task( 
						agent, 
						agent.CreateHost( this.env ) );

					foreach ( IAction act in this.actions[ i ] )
					{
						task.AddAction( act );
					}

					this.run.AddTask( task );
					tasks++;
				}
			}

			if ( tasks == 0 )
			{
				throw new EmptyRunException();
			}

			this.run.RootResult = ( IResultItemCollection ) this.result;

			return this.run;
		}

		public HostEnvironment Environment
		{
			get { return this.env; }
		}
	}
}
