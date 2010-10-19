using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfix.Control.Native;

namespace Cfix.Control.RunControl
{
	/*++
	 * Run actions of a single host.
	 * 
	 * N.B. Do not Dispose() before the task has finished. 
	 --*/
	public class Task : IProcessTask
	{
		private const uint CFIX_E_SETUP_ROUTINE_FAILED = ( uint ) 0x8004800b;
		private const uint CFIX_E_TEARDOWN_ROUTINE_FAILED = ( uint ) 0x8004800c;
		private const uint CFIX_E_BEFORE_ROUTINE_FAILED = ( uint ) 0x80048011;
		private const uint CFIX_E_AFTER_ROUTINE_FAILED = ( uint ) 0x80048012;
		private const uint CFIX_E_TEST_ROUTINE_FAILED = ( uint ) 0x8004800e;
		private const uint RPC_S_CALL_FAILED = ( uint ) 0x800706BE;
		
		private const uint RPC_E_UNKNOWN_FAILURE = ( uint ) 0x80131500;

		public event EventHandler Started;
		public event EventHandler<FinishedEventArgs> Finished;
		
		private readonly IAgent agent;
		private readonly HostEnvironment hostEnv;
		private readonly List<IAction> actions = new List<IAction>();
		private uint itemCount;

		//
		// Host to run on. Lazily created, disposed and nulled eagerly.
		//
		// Do not access directly, call GetHost().
		//
		private volatile IHost __host;
		
		//
		// Save these attributes separately s.t. they remain available
		// after this.host has been reset.
		//
		// Not initialized before GetHost has been called at least once.
		//
		private volatile uint hostPid;
		private volatile Architecture hostArch;
		
		private volatile TaskStatus status = TaskStatus.Ready;

		//
		// Rundown lock for avoiding disposal while async operations are
		// in progress.
		//
		private readonly RundownLock rundownLock = new RundownLock();
		private readonly object actionLock = new object();

		private IHost GetHost( IAction action )
		{
			lock ( this )
			{
				if ( this.__host == null )
				{
					this.__host = action.CreateHost( this.agent, this.hostEnv );
					this.hostPid = this.__host.ProcessId;
					this.hostArch = this.__host.Architecture;
				}

				return this.__host;
			}
		}

		private void DisposeHost()
		{
			if ( this.__host != null )
			{
				IHost host = this.__host;
				this.__host = null;
				host.Dispose();
			}
		}

		private static bool IsTestRoutineFailureHr( uint hr )
		{
			switch ( hr )
			{
				case CFIX_E_TEST_ROUTINE_FAILED:
				case CFIX_E_SETUP_ROUTINE_FAILED:
				case CFIX_E_TEARDOWN_ROUTINE_FAILED:
				case CFIX_E_BEFORE_ROUTINE_FAILED:
				case CFIX_E_AFTER_ROUTINE_FAILED:
					return true;

				default:
					return false;
			}
		}

		public Task( 
			IAgent agent,
			HostEnvironment hostEnv
			)
		{
			Debug.Assert( agent != null );
			Debug.Assert( hostEnv != null );

			this.agent = agent;
			this.hostEnv = hostEnv;
		}

		~Task()
		{
			Dispose( false );
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing )
		{
			//
			// Wait for async operations to complete.
			//
			this.rundownLock.Rundown();

			DisposeHost();

			foreach ( IAction act in this.actions )
			{
				act.Dispose();
			}
		}

		internal void AddAction( IAction action )
		{
			lock ( this.actionLock )
			{
				this.actions.Add( action );
				this.itemCount += action.ItemCount;
			}
		}

		protected HostEnvironment Environment
		{
			get { return this.hostEnv; }
		}

		/*--------------------------------------------------------------
		 * Async run.
		 */

		private delegate void AsyncRunDelegate();

		private void ForceComplete()
		{
			//
			// Run did not complete -- either because of a severe
			// error or because the run hsa been shortcut.
			//
			// In either case, we have to update the results of 
			// not-yet-run actions.
			//
			foreach ( IAction action in this.actions )
			{
				if ( !ReferenceEquals( action.Result.Item, action.Item ) )
				{
					Debug.Assert( action.Result is IResultItemCollection );
					Debug.Assert( action.Item is TestCase );

					//
					// Special case: single test case run or test case is
					// run in separate process -- in either case, the 
					// result belongs to the item's parent.
					//
					// To avoid interfering with concurrent tasks, make
					// sure to only force-complete the one item that 
					// was covered by the action.
					//
					IResultItemCollection parentResult = 
						( IResultItemCollection ) action.Result;
					IResultItem testCaseResult;

					if ( action.Item.Ordinal < parentResult.ItemCount )
					{
						testCaseResult = parentResult.GetItem( action.Item.Ordinal );
					}
					else
					{
						//
						// Partial result.
						//
						Debug.Assert( parentResult.ItemCount == 1 );
						testCaseResult = parentResult.GetItem( 0 );
					}

					testCaseResult.ForceCompletion( false, ExecutionStatus.Failed );
				}
				else
				{
					action.Result.ForceCompletion( true );
				}
			}
		}

		private void AsyncRun()
		{
			if ( this.Started != null )
			{
				this.Started( this, EventArgs.Empty );
			}

			this.status = TaskStatus.Running;


			try
			{
				this.rundownLock.Acquire();
				try
				{
					foreach ( IAction act in this.actions )
					{
						IHost host = GetHost( act );
						Debug.Assert( host != null );

						act.Run( host );
					}
				}
				finally
				{
					//
					// N.B. Run down lock needs to be released before
					// ForceComplete is called - otherwise, a deadlock
					// occurs.
					//
					this.rundownLock.Release();
				}
			}
			catch ( COMException x )
			{
				Logger.LogError( "Task", "AsnycRun aborted", x );

				ForceComplete();

				if ( ( uint ) x.ErrorCode == RPC_S_CALL_FAILED ||
					 ( uint ) x.ErrorCode == RPC_E_UNKNOWN_FAILURE )
				{
					//
					// Remote procedure call failed -- this is expected
					// after termination.
					//
				}
				else if ( IsTestRoutineFailureHr( ( uint ) x.ErrorCode ) )
				{
					//
					// Expected cfix failures.
					//
				}
				else
				{
					throw new CfixException(
						this.agent.ResolveMessage( x.ErrorCode ) );
				}
			}
			catch ( CfixException )
			{
				ForceComplete();

				throw;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes" )]
		private void AsyncRunCompletionCallback( IAsyncResult ar )
		{
			try
			{
				AsyncRunDelegate dlg = ( AsyncRunDelegate ) ar.AsyncState;
				dlg.EndInvoke( ar );

				this.status = TaskStatus.Suceeded;

				if ( this.Finished != null )
				{
					this.Finished( this, FinishedEventArgs.Empty );
				}
			}
			catch ( Exception x )
			{
				this.status = TaskStatus.Failed;

				if ( this.Finished != null )
				{
					this.Finished( this, new FinishedEventArgs( x ) );
				}
			}
			finally
			{
				//
				// We are done with the host. Dispose it s.t. the 
				// process can terminate.
				//
				DisposeHost();
			}
		}

		/*----------------------------------------------------------------------
		 * IProcessTask.
		 */

		public uint ProcessId
		{
			get { return this.hostPid; }
		}

		public Architecture Architecture
		{
			get { return this.hostArch; }
		}

		/*----------------------------------------------------------------------
		 * ITask.
		 */

		public TaskStatus Status
		{
			get { return this.status; }
		}

		public uint ItemCount
		{
			get { return this.itemCount; }
		}

		public void Start()
		{
			lock ( this.actionLock )
			{
				if ( this.status != TaskStatus.Ready )
				{
					throw new CfixException( "Already started" );
				}

				AsyncRunDelegate asyncRun = AsyncRun;
				asyncRun.BeginInvoke(
					AsyncRunCompletionCallback,
					asyncRun );
			}
		}

		public void Stop()
		{
			lock ( this.actionLock )
			{
				if ( this.status != TaskStatus.Running )
				{
					return;
				}

				foreach ( IAction act in this.actions )
				{
					//
					// Will implicitly set finished to true.
					//
					act.Stop();
				}

				this.status = TaskStatus.Stopped;
			}
		}

		public void Terminate()
		{
			lock ( this.actionLock )
			{
				if ( this.status != TaskStatus.Running )
				{
					return;
				}

				IHost host = this.__host;
				if ( host != null )
				{
					host.Terminate();
					this.status = TaskStatus.Terminated;
				}
			}
		}
	}
}
