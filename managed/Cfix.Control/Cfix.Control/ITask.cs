using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	//
	// N.B. Sorted in ascending severity.
	//
	public enum TaskStatus
	{
		Ready,
		Running,
		Suceeded,
		Failed,
		Stopped,
		Terminated,
	}

	public class FinishedEventArgs : EventArgs
	{
		private readonly Exception exception;

		public static readonly FinishedEventArgs Empty =
			new FinishedEventArgs( null );

		public FinishedEventArgs( Exception x )
		{
			this.exception = x;
		}

		public Exception Exception
		{
			get { return this.exception; }
		}
	}

	public interface ITask : IDisposable
	{
		event EventHandler Started;
		event EventHandler<FinishedEventArgs> Finished;
		
		TaskStatus Status { get; }

		void Start();
		void Stop();
		void Terminate();
	}
}