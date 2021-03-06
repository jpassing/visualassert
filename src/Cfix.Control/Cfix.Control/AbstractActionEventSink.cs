using System;
using System.Collections.Generic;
using System.Threading;

namespace Cfix.Control.RunControl
{
	public class AbstractActionEventSink : IActionEvents
	{
		public event EventHandler<NotificationEventArgs> Notification;
		public event EventHandler<HostEventArgs> HostSpawned;
		public event EventHandler<LogEventArgs> Log;
		public event EventHandler<ThreadEventArgs> ThreadStarted;
		public event EventHandler<ThreadEventArgs> ThreadFinished;
		public event EventHandler StatusChanged;
		public event EventHandler FailureOccured;

		private readonly IDispositionPolicy policy;

		private int itemsCompleted;
		private int itemsFailed;
		private int itemsInconclusive;
		private int itemsSucceeded;

		protected AbstractActionEventSink(
			IDispositionPolicy policy 
			)
		{
			this.policy = policy;
		}

		protected AbstractActionEventSink() : this( null )
		{
		}

		public uint ItemsCompleted
		{
			get { return ( uint ) this.itemsCompleted; }
		}

		public uint ItemsFailed
		{
			get { return ( uint ) this.itemsFailed; }
		}

		public uint ItemsInconclusive
		{
			get { return ( uint ) this.itemsInconclusive; }
		}

		public uint ItemsSucceeded
		{
			get { return ( uint ) this.itemsSucceeded; }
		}

		/*--------------------------------------------------------------
		 * IActionEvents.
		 */

		public IDispositionPolicy DispositionPolicy
		{
			get 
			{
				return this.policy;
			}
		}

		public void OnNotification( IResultItem item, int hr )
		{
			if ( Notification != null )
			{
				Notification( item, new NotificationEventArgs( hr ) );
			}
		}

		public void OnHostSpawned( uint processId )
		{
			if ( this.HostSpawned != null )
			{
				this.HostSpawned( this, new HostEventArgs( processId ) );
			}
		}

		public void OnLog( IResultItem item, String message )
		{
			if ( Log != null )
			{
				Log( item, new LogEventArgs( message ) );
			}
		}

		public void OnThreadStarted( IResultItem item, uint threadId )
		{
			if ( ThreadStarted != null )
			{
				ThreadStarted( item, new ThreadEventArgs( threadId ) );
			}
		}

		public void OnThreadFinished( IResultItem item, uint threadId )
		{
			if ( ThreadFinished != null )
			{
				ThreadFinished( item, new ThreadEventArgs( threadId ) );
			}
		}

		public void OnStatusChanged( IResultItem item )
		{
			if ( ! ( item is IResultItemCollection ) )
			{
				//
				// Leaf item - count.
				//
				if ( item.Completed )
				{
					if ( item.Status == ExecutionStatus.PostprocessingFailed )
					{
						//
						// Do not count these.
						//
					}
					else
					{
						Interlocked.Increment( ref this.itemsCompleted );
					}

					switch ( item.Status )
					{
						case ExecutionStatus.Failed:
							Interlocked.Increment( ref this.itemsFailed );
							break;

						case ExecutionStatus.Inconclusive:
							Interlocked.Increment( ref this.itemsInconclusive );
							break;

						case ExecutionStatus.Succeeded:
							Interlocked.Increment( ref this.itemsSucceeded );
							break;
					}
				}
			}

			if ( StatusChanged != null )
			{
				StatusChanged( item, EventArgs.Empty );
			}
		}

		public void OnFailureOccured( IResultItem item )
		{
			if ( FailureOccured != null )
			{
				FailureOccured( item, EventArgs.Empty );
			}
		}
	}
}
