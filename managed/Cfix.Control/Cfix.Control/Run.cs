using System;
using Cfix.Control.Native;

namespace Cfix.Control
{
	public class Run : IRun
	{
		public event EventHandler<LogEventArgs> Log;
		public event EventHandler<ThreadEventArgs> ThreadStarted;
		public event EventHandler<ThreadEventArgs> ThreadFinished;
		public event EventHandler StatusChanged;
		public event EventHandler<NotificationEventArgs> Notification;

		private readonly IDispositionPolicy dispositionPolicy;
		private readonly ITestItemCollection rootItem;
		private readonly IResultItemCollection rootResult;

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
	}
}
