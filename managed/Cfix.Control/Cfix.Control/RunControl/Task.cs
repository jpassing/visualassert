using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cfix.Control.RunControl
{
	/*++
	 * Run actions of a single host.
	 --*/
	internal class Task : AbstractActionEventSink, ITask
	{
		public event EventHandler Started;
		public event EventHandler Succeeded;
		public event EventHandler<FailEventArgs> Failed;
		
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
			IHost host,
			IDispositionPolicy policy
			)
			: base( policy )
		{
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
			Debug.Assert( action.Host == this.host );
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
			if ( this.Started != null )
			{
				this.Started( this, EventArgs.Empty );
			}

			this.status = TaskStatus.Running;
			foreach ( IAction act in this.actions )
			{
				act.Run();
			}
		}

		private void AsyncRunCompletionCallback( IAsyncResult ar )
		{
			this.status = TaskStatus.Finished;

			try
			{
				AsyncRunDelegate dlg = ( AsyncRunDelegate ) ar.AsyncState;
				dlg.EndInvoke( ar );
				
				if ( this.Succeeded != null )
				{
					this.Succeeded( this, EventArgs.Empty );
				}
			}
			catch ( Exception x )
			{
				if ( this.Failed != null )
				{
					this.Failed( this, new FailEventArgs( x ) );
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
