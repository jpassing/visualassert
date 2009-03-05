using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cfix.Control.RunControl
{
	/*++
	 * Run actions of a single host.
	 --*/
	internal class Task : ITask
	{
		public event EventHandler Started;
		public event EventHandler<FinishedEventArgs> Finished;

		private readonly IAgent agent;
		private readonly IHost host;
		private readonly List<IAction> actions = new List<IAction>();

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
			if ( this.host != null )
			{
				this.host.Dispose();
			}

			//
			// Wait for async operations to complete.
			//
			this.rundownLock.Rundown();

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

		internal void AddActionRange( IEnumerable<IAction> actions )
		{
			lock ( this.actionLock )
			{
				foreach ( IAction action in actions )
				{
					this.actions.Add( action );
				}
			}
		}

		internal IHost Host
		{
			get { return this.host; }
		}

		/*--------------------------------------------------------------
		 * Async run.
		 */

		private delegate void AsyncRunDelegate();

		private void AsyncRun()
		{
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
