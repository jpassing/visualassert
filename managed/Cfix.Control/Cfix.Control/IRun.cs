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

	public interface IRun
	{
		event EventHandler<LogEventArgs> Log;
		event EventHandler<ThreadEventArgs> ThreadStarted;
		event EventHandler<ThreadEventArgs> ThreadFinished;
		event EventHandler StatusChanged;

		IResultItemCollection Root { get; }
	}
}
