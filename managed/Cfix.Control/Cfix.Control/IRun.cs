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

	public class FailEventArgs : EventArgs
	{
		private readonly Exception exception;

		public FailEventArgs( Exception x )
		{
			this.exception = x;
		}

		public Exception Exception
		{
			get { return this.exception; }
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


	public interface IRun : IDisposable
	{
		//
		// N.B. Eents may be raised on a worker thread.
		//

		event EventHandler Started;
		event EventHandler Succeeded;
		event EventHandler<FailEventArgs> Failed;
		event EventHandler<HostEventArgs> HostSpawned;
		
		event EventHandler<LogEventArgs> Log;
		event EventHandler<ThreadEventArgs> ThreadStarted;
		event EventHandler<ThreadEventArgs> ThreadFinished;
		event EventHandler StatusChanged;
		event EventHandler<NotificationEventArgs> Notification;

		IResultItemCollection RootResult { get; }

		bool IsStarted { get; }
		bool IsFinished { get; }

		void Start();
		void Stop();
		void Terminate();
	}
}
