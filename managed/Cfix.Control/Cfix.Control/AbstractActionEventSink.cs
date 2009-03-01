using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.RunControl
{
	internal class AbstractActionEventSink : IActionEvents
	{
		public event EventHandler<NotificationEventArgs> Notification;
		public event EventHandler<HostEventArgs> HostSpawned;
		public event EventHandler<LogEventArgs> Log;
		public event EventHandler<ThreadEventArgs> ThreadStarted;
		public event EventHandler<ThreadEventArgs> ThreadFinished;
		public event EventHandler StatusChanged;

		private readonly IDispositionPolicy policy;
		private AbstractActionEventSink parent;

		protected AbstractActionEventSink(
			IDispositionPolicy policy 
			)
		{
			this.policy = policy;
			this.parent = null;
		}

		protected AbstractActionEventSink() : this( null )
		{
		}

		public void SetParent( AbstractActionEventSink parent )
		{
			this.parent = parent;
		}

		/*--------------------------------------------------------------
		 * IActionEvents.
		 */

		public IDispositionPolicy DispositionPolicy
		{
			get 
			{
				if ( this.parent != null )
				{
					return this.parent.DispositionPolicy;
				}
				else
				{
					return this.policy;
				}
			}
		}

		public void OnNotification( IResultItem item, int hr )
		{
			if ( Notification != null )
			{
				Notification( item, new NotificationEventArgs( hr ) );
			}

			if ( this.parent != null )
			{
				this.parent.OnNotification( item, hr );
			}
		}

		public void OnHostSpawned( uint processId )
		{
			if ( this.HostSpawned != null )
			{
				this.HostSpawned( this, new HostEventArgs( processId ) );
			}

			if ( this.parent != null )
			{
				this.parent.OnHostSpawned( processId );
			}
		}

		public void OnLog( IResultItem item, String message )
		{
			if ( Log != null )
			{
				Log( item, new LogEventArgs( message ) );
			}

			if ( this.parent != null )
			{
				this.parent.OnLog( item, message );
			}
		}

		public void OnThreadStarted( IResultItem item, uint threadId )
		{
			if ( ThreadStarted != null )
			{
				ThreadStarted( item, new ThreadEventArgs( threadId ) );
			}

			if ( this.parent != null )
			{
				this.parent.OnThreadStarted( item, threadId );
			}
		}

		public void OnThreadFinished( IResultItem item, uint threadId )
		{
			if ( ThreadFinished != null )
			{
				ThreadFinished( item, new ThreadEventArgs( threadId ) );
			}

			if ( this.parent != null )
			{
				this.parent.OnThreadFinished( item, threadId );
			}
		}

		public void OnStatusChanged( IResultItem item )
		{
			if ( StatusChanged != null )
			{
				StatusChanged( item, EventArgs.Empty );
			}

			if ( this.parent != null )
			{
				this.parent.OnStatusChanged( item );
			}
		}
	}
}