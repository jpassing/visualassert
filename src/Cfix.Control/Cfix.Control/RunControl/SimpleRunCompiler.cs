using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.RunControl
{
	// Non-threadsafe
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable" )]
	public class SimpleRunCompiler : RunCompilerBase
	{
		private readonly bool allowIncompatibleModules;

		//
		// One bucket per architecture; one for embedded modules.
		//
		private IList<IAction>[] actionBuckets = new IList<IAction>[ 3 ];

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

		public SimpleRunCompiler(
			AgentSet agentSet,
			IDispositionPolicy policy,
			ExecutionOptions executionOptions,
			EnvironmentOptions environmentOptions,
			bool allowIncompatibleModules
			) : base(
				agentSet,
				policy,
				executionOptions,
				environmentOptions )
		{
			for ( int i = 0; i < this.actionBuckets.Length; i++ )
			{
				this.actionBuckets[ i ] = new List<IAction>();
			}

			this.allowIncompatibleModules = allowIncompatibleModules;
		}

		public SimpleRunCompiler(
			IAgent agent,
			IDispositionPolicy policy,
			ExecutionOptions executionOptions,
			EnvironmentOptions environmentOptions
			)
			: this(
				CreateSingleArchitectureAgentSet( agent ),
				policy,
				executionOptions,
				environmentOptions,
				true )
		{ }

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		protected override void AddAction( IAction action )
		{
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

		public override IRun Compile()
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
							this.Environment );
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
	}
}
