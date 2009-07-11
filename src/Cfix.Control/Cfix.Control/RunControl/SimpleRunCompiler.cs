using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.RunControl
{
	// Non-threadsafe
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable" )]
	public class SimpleRunCompiler : IRunCompiler
	{
		private readonly AgentSet agentSet;
		private readonly ExecutionOptions executionOptions;
		private readonly ThreadingOptions threadingOptions;
		private readonly EnvironmentOptions envOptions;
		private readonly Run run;
		private readonly bool allowIncompatibleModules;

		private readonly HostEnvironment env = new HostEnvironment();

		//
		// One bucket per architecture; one for embedded modules.
		//
		private IList<IAction>[] actionBuckets = new IList<IAction>[ 3 ];
		private IResultItem result;

		private const int BucketEmbedded = 0;
		private const int BucketAmd64 = 1;
		private const int BucketI386 = 2;

		private static int GetBucket( IAction action )
		{
			if ( action.ModuleType == ModuleType.UserEmbedded )
			{
				return BucketEmbedded;
			}
			else
			{
				switch ( action.Architecture )
				{
					case Architecture.Amd64:
						return BucketAmd64;

					case Architecture.I386:
						return BucketI386;

					default:
						Debug.Fail( "Unrecognized architecture" );
						return -1;
				}
			}
		}

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
			bool allowIncompatibleModules
			)
		{
			for ( int i = 0; i < this.actionBuckets.Length; i++ )
			{
				this.actionBuckets[ i ] = new List<IAction>();
			}

			this.agentSet = agentSet;
			this.executionOptions = executionOptions;
			this.threadingOptions = threadingOptions;
			this.envOptions = envOptions;

			this.allowIncompatibleModules = allowIncompatibleModules;
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

			int bucket = GetBucket( action );
			if ( !this.allowIncompatibleModules )
			{
				for ( int i = 0; i < this.actionBuckets.Length; i++ )
				{
					if ( i != bucket && this.actionBuckets[ i ].Count > 0 )
					{
						//
						// Architecture/module type differs from previously 
						// added actions. This would result in another host
						// having to be created.
						//
						throw new IncompatibleModulesException();
					}
				}
			}

			this.actionBuckets[ bucket ].Add( action );
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
			for ( int i = 0; i < this.actionBuckets.Length; i++ )
			{
				Task task = null;
				foreach ( IAction act in this.actionBuckets[ i ] )
				{
					if ( task == null )
					{
						IAgent agent = this.agentSet.GetAgent( act.Architecture );
						task = new Task( 
							agent,
							act.CreateHost( agent, env ) );
						this.run.AddTask( task );
						tasks++;
					}

					task.AddAction( act );

					if ( i == BucketEmbedded )
					{
						//
						// Create a fresh task/host for each action.
						//
						task = null;
					}
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
