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
		private readonly SchedulingOptions schedulingOptions;
		private readonly CompositionOptions compositionOptions;

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

		private readonly object actionLock = new object();
		private IAction action;
		
		private volatile bool finished;

		public Run( 
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

		private delegate void AsyncRunDelegate();

		private void AsyncRun()
		{
			if ( this.Started != null )
			{
				this.Started( this, EventArgs.Empty );
			}

			this.action.Run( this );
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

		/*--------------------------------------------------------------
		 * ICfixProcessEventSink.
		 */

		void ICfixProcessEventSink.AfterRunFinish()
		{
			//
			// Not used.
			//
		}

		void ICfixProcessEventSink.BeforeRunStart()
		{ 
			//
			// Not used.
			//
		}

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

		public IResultItemCollection RootResult 
		{
			get { return this.rootResult; }
		}

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
				if ( this.IsStarted )
				{
					throw new CfixException( "Already started" );
				}

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

					this.action = compSrc.CreateAction( schedulingOptions );
				}
				else
				{
					// TODO
					throw new NotImplementedException();
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
					IComponentAction compAction = this.action as IComponentAction;
					if ( compAction != null )
					{
						compAction.TerminateHost();

						//
						// Enforce state.
						//
						this.finished = true;
					}
					else
					{
						// TODO: Terminate multiple hosts?
						Stop();
					}
				}
			}
		}

		/*--------------------------------------------------------------
		 * Internal.
		 */

		internal void OnItemAdded( IResultItem item )
		{
			//
			// Remember the item's module for use as event sink.
			//
			TestModule module = item.Item as TestModule;
			if ( module != null )
			{
				this.modules[ module.Path ] = ( TestItemCollectionResult ) item;
				return;
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
