using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cfix.Control.RunControl
{
	/*++
	 * Run actions of a single host.
	 * 
	 * N.B. Do not Dispose() before the task has finished. 
	 --*/
	internal class Task : ITask
	{
		public event EventHandler Started;
		public event EventHandler<FinishedEventArgs> Finished;

		private readonly IAgent agent;
		private readonly List<IAction> actions = new List<IAction>();

		//
		// Host to run on. Disposed and nulled eagerly.
		//
		private volatile IHost host;
		
		private volatile TaskStatus status = TaskStatus.Ready;

		//
		// Rundown lock for avoiding disposal while async operations are
		// in progress.
		//
		private readonly RundownLock rundownLock = new RundownLock();
		private readonly object actionLock = new object();

		public Task( 
			IAgent agent,
			IHost host
			)
		{
			Debug.Assert( agent != null );
			Debug.Assert( host != null );

			this.agent = agent;
			this.host = host;
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

			if ( this.host != null )
			{
				this.host.Dispose();
			}

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
			}
		}


		/*--------------------------------------------------------------
		 * Async run.
		 */

		private delegate void AsyncRunDelegate();

		private void AsyncRun()
		{
			Debug.Assert( this.host != null );

			if ( this.Started != null )
			{
				this.Started( this, EventArgs.Empty );
			}

			this.status = TaskStatus.Running;
			try
			{
				foreach ( IAction act in this.actions )
				{
					act.Run( this.host );
				}
			}
			catch ( COMException x )
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
					IResultItem actionResult = action.Result;
					actionResult.ForceCompletion( true );
				}

				throw new CfixException(
					this.agent.ResolveMessage( x.ErrorCode ) );
			}
		}

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
				this.rundownLock.Release();

				//
				// We are done with the host. Dispose it s.t. the 
				// process can terminate.
				//
				this.host.Dispose();
				this.host = null;
			}
		}

		/*----------------------------------------------------------------------
		 * ITask.
		 */

		public TaskStatus Status
		{
			get { return this.status; }
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
				this.rundownLock.Acquire();
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
				
				if ( this.host != null )
				{
					this.host.Terminate();
					this.status = TaskStatus.Terminated;
				}
			}
		}
	}
}
