using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public enum TaskStatus
	{
		Ready,
		Running,
		Stopped,
		Terminated,
		Finished
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

	public interface ITask : IDisposable
	{
		event EventHandler Started;
		event EventHandler Succeeded;
		event EventHandler<FailEventArgs> Failed;
		
		TaskStatus Status { get; }

		void Start();
		void Stop();
		void Terminate();
	}
}
