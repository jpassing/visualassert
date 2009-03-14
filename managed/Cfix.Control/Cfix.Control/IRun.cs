using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public class LogEventArgs : EventArgs
	{
		private readonly String message;

		public LogEventArgs( String message )
		{
			this.message = message;
		}

		public String Message
		{
			get { return message; }
		}
	}

	public class ThreadEventArgs : EventArgs
	{
		private readonly uint id;

		public ThreadEventArgs( uint id )
		{
			this.id = id;
		}

		public uint Id
		{
			get { return id; }
		}
	}

	public class NotificationEventArgs : EventArgs
	{
		private readonly int hr;

		public NotificationEventArgs( int hr )
		{
			this.hr = hr;
		}

		public int Hresult
		{
			get { return hr; }
		}
	}

	public class HostEventArgs : EventArgs
	{
		private readonly uint pid;

		public HostEventArgs( uint pid )
		{
			this.pid = pid;
		}

		public uint HostProcessId
		{
			get { return this.pid; }
		}
	}

	/*++
	 * Run. 
	 * 
	 * N.B. Do not Dispose() before the run has finished. 
	 * 
	 * You may NOT call Dispose() from within the Finished event.
	 --*/
	public interface IRun : ITask, IDisposable
	{
		//
		// N.B. Events may be raised on a worker thread.
		//
		event EventHandler<HostEventArgs> HostSpawned;
		
		event EventHandler<LogEventArgs> Log;
		event EventHandler<ThreadEventArgs> ThreadStarted;
		event EventHandler<ThreadEventArgs> ThreadFinished;
		event EventHandler StatusChanged;
		event EventHandler<NotificationEventArgs> Notification;

		IEnumerable<ITask> Tasks { get; }
		IResultItemCollection RootResult { get; }

		uint ItemsCompleted { get; }
	}
}
