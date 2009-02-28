using System;
using System.Collections.Generic;

namespace Cfix.Control
{
	public abstract class AbstractRun : IRun
	{
		public event EventHandler Started;
		public event EventHandler Succeeded;
		public event EventHandler<FailEventArgs> Failed;
		public event EventHandler<HostEventArgs> HostSpawned;

		public event EventHandler<LogEventArgs> Log;
		public event EventHandler<ThreadEventArgs> ThreadStarted;
		public event EventHandler<ThreadEventArgs> ThreadFinished;

		public event EventHandler StatusChanged;
		public event EventHandler<NotificationEventArgs> Notification;

		private readonly IDispositionPolicy dispositionPolicy;
		private readonly SchedulingOptions schedulingOptions;
		private readonly CompositionOptions compositionOptions;

		private readonly ITestItemCollection rootItem;

		//
		// Rundown lock for avoiding disposal while async operations are
		// in progress.
		//
		private readonly RundownLock rundownLock = new RundownLock();

		private readonly object actionLock = new object();
		private IAction action;

		private volatile bool finished;

		public AbstractRun(
			IDispositionPolicy policy,
			SchedulingOptions schedulingOptions,
			CompositionOptions compositionOptions,
			ITestItemCollection rootItem
			)
		{
			this.dispositionPolicy = policy;
			this.schedulingOptions = schedulingOptions;
			this.compositionOptions = compositionOptions;
			this.rootItem = rootItem;
		}

		~AbstractRun()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			//
			// Wait for async operations to complete.
			//
			this.rundownLock.Rundown();

			if ( this.action != null )
			{
				this.action.Dispose();
			}

			//
			// Now that all async operations have completed, we can 
			// safely dispose the data structures.
			//
			this.rootItem.Dispose();
			//this.rootResult.Dispose();
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/*--------------------------------------------------------------
		 * Async run.
		 */

		protected abstract void RunAction( IAction action );

		private delegate void AsyncRunDelegate();

		private void AsyncRun()
		{
			if ( this.Started != null )
			{
				this.Started( this, EventArgs.Empty );
			}

			RunAction( this.action );
		}

		private void AsyncRunCompletionCallback( IAsyncResult ar )
		{
			this.finished = true;

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

		protected ITestItemCollection RootItem
		{
			get { return this.rootItem; }
		}

		/*--------------------------------------------------------------
		 * IRun.
		 */

		public abstract IResultItemCollection RootResult { get; }

		public bool IsStarted
		{
			get { return this.action != null; }
		}

		public bool IsFinished
		{
			get { return this.action != null && this.finished; }
		}

		public void Start()
		{
			lock ( this.actionLock )
			{
				//if ( this.IsStarted )
				//{
				//    throw new CfixException( "Already started" );
				//}

				//if ( ( compositionOptions & CompositionOptions.NonComposite )
				//    == CompositionOptions.NonComposite )
				//{
				//    IComponentActionSource compSrc =
				//        this.rootItem as IComponentActionSource;
				//    if ( compSrc == null )
				//    {
				//        throw new ArgumentException(
				//            "Not a component source" );
				//    }

				//    this.action = compSrc.CreateAction( schedulingOptions );
				//}
				//else
				//{
				//    // TODO
				//    throw new NotImplementedException();
				//}

				//AsyncRunDelegate asyncRun = AsyncRun;
				//this.rundownLock.Acquire();
				//asyncRun.BeginInvoke(
				//    AsyncRunCompletionCallback,
				//    asyncRun );
			}
		}

		public void Stop()
		{
			lock ( this.actionLock )
			{
				if ( this.action != null )
				{
					//
					// Will implicitly set finished to true.
					//
					this.action.Stop();
				}
			}
		}

		public void Terminate()
		{
			lock ( this.actionLock )
			{
				if ( this.action != null )
				{
					//IComponentAction compAction = this.action as IComponentAction;
					//if ( compAction != null )
					//{
					//    compAction.TerminateHost();

					//    //
					//    // Enforce state.
					//    //
					//    this.finished = true;
					//}
					//else
					//{
					//    // TODO: Terminate multiple hosts?
					//    Stop();
					//}
				}
			}
		}

		/*--------------------------------------------------------------
		 * Internal.
		 */

		internal void OnLog( IResultItem item, String message )
		{
			if ( Log != null )
			{
				Log( item, new LogEventArgs( message ) );
			}
		}

		internal void OnThreadStarted( IResultItem item, uint threadId )
		{
			if ( ThreadStarted != null )
			{
				ThreadStarted( item, new ThreadEventArgs( threadId ) );
			}
		}

		internal void OnThreadFinished( IResultItem item, uint threadId )
		{
			if ( ThreadFinished != null )
			{
				ThreadFinished( item, new ThreadEventArgs( threadId ) );
			}
		}

		internal void OnStatusChanged( IResultItem item )
		{
			if ( StatusChanged != null )
			{
				StatusChanged( item, EventArgs.Empty );
			}
		}

		internal void OnNotification( IResultItem item, int hr )
		{
			if ( Notification != null )
			{
				Notification( item, new NotificationEventArgs( hr ) );
			}
		}

		internal IDispositionPolicy DispositionPolicy
		{
			get { return this.dispositionPolicy; }
		}

		/*--------------------------------------------------------------
		 * Protected.
		 */

		protected void OnHostSpawned( uint processId )
		{
			if ( this.HostSpawned != null )
			{
				this.HostSpawned( this, new HostEventArgs( processId ) );
			}
		}
	}
}
