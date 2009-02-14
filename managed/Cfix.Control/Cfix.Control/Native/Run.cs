using System;
using System.Collections.Generic;
using Cfixctl;

namespace Cfix.Control.Native
{
	public class Run : IRun, ICfixProcessEventSink, ICfixEventSink
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
		private readonly ITestItemCollection rootItem;
		private readonly IResultItemCollection rootResult;

		//
		// Rundown lock for avoiding disposal while async operations are
		// in progress.
		//
		private readonly RundownLock rundownLock = new RundownLock();

		//
		// Map between module paths and module result objects.
		//
		private readonly IDictionary<String, TestItemCollectionResult> modules =
			new Dictionary<String, TestItemCollectionResult>();

		public Run( 
			IDispositionPolicy policy,
			ITestItemCollection rootItem 
			)
		{
			this.dispositionPolicy = policy;
			this.rootItem = rootItem;
			this.rootResult = TestItemCollectionResult.CreateResult(
				this, rootItem, ExecutionStatus.Pending );
		}

		~Run()
		{
			Dispose( false );
		}

		protected void Dispose( bool disposing )
		{
			//
			// Wait for async operations to complete.
			//
			this.rundownLock.Rundown();

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

		private delegate void AsyncRunDelegate(
			IAction action
			);

		private void AsyncRun(
			IAction action
			)
		{
			if ( this.Started != null )
			{
				this.Started( this, EventArgs.Empty );
			}

			action.Run( this );
		}

		private void AsyncRunCompletionCallback( IAsyncResult ar )
		{
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

		/*--------------------------------------------------------------
		 * ICfixProcessEventSink.
		 */

		ICfixTestÌtemContainerEventSink ICfixProcessEventSink.GetTestÌtemContainerEventSink( 
			ICfixTestModule module, 
			uint fixtureOrdinal 
			)
		{
			return ( ICfixTestÌtemContainerEventSink )
				this.modules[ module.GetPath() ].GetItem( fixtureOrdinal );
		}

		void ICfixProcessEventSink.Notification( int hr )
		{
			OnNotification( this.rootResult, hr );
		}

		/*--------------------------------------------------------------
		 * ICfixEventSink.
		 */

		ICfixProcessEventSink ICfixEventSink.GetProcessEventSink( uint processId )
		{
			if ( this.HostSpawned != null )
			{
				this.HostSpawned( this, new HostEventArgs( processId ) );
			}

			return this;
		}

		/*--------------------------------------------------------------
		 * IRun.
		 */

		public ITestItemCollection RootItem 
		{ 
			get { return this.rootItem; }
		}

		public IResultItemCollection RootResult 
		{
			get { return this.rootResult; }
		}

		public void Start(
			SchedulingOptions schedulingOptions,
			CompositionOptions compositionOptions
			)
		{
			IAction action = null;

			if ( ( compositionOptions & CompositionOptions.NonComposite )
				== CompositionOptions.NonComposite )
			{
				IComponentActionSource compSrc =
					this.rootItem as IComponentActionSource;
				if ( compSrc == null )
				{
					throw new ArgumentException(
						"Not a component source" );
				}

				action = compSrc.CreateAction( schedulingOptions );
			}
			else
			{
				// TODO
				throw new NotImplementedException();
			}

			AsyncRunDelegate asyncRun = AsyncRun;
			this.rundownLock.Acquire();
			asyncRun.BeginInvoke(
				action,
				AsyncRunCompletionCallback,
				asyncRun );
		}

		/*--------------------------------------------------------------
		 * Internal.
		 */

		internal void OnItemAdded( IResultItem item )
		{
			//
			// Remember this module for use as event sink.
			//
			TestModule module = item.Item as TestModule;
			if ( module != null )
			{
				this.modules[ module.Path ] = ( TestItemCollectionResult ) item;
			}
		}

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

	}
}
