using System;
using System.Collections.Generic;

namespace Cfix.Control.RunControl
{
	internal class Run : AbstractActionEventSink, IRun
	{
		public event EventHandler Started;
		public event EventHandler Succeeded;
		public event EventHandler<FailEventArgs> Failed;

		private readonly List<ITask> tasks = new List<ITask>();
		private readonly IResultItemCollection rootResult;
		
		private volatile TaskStatus status = TaskStatus.Ready;

		private readonly object actionLock = new object();

		public Run(
			IDispositionPolicy policy,
			IResultItemCollection rootResult
			)
			: base( policy )
		{
			this.rootResult = rootResult;
		}

		~Run()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			foreach ( ITask task in this.tasks )
			{
				task.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		internal void AddTask( ITask task )
		{
			this.tasks.Add( task );
		}

		/*--------------------------------------------------------------
		 * IRun.
		 */

		public IResultItemCollection RootResult
		{
			get { return this.rootResult; }
		}

		public IEnumerable<ITask> Tasks
		{
			get { return this.tasks; }
		}

		public TaskStatus Status
		{
			get { return this.status; }
		}
		
		public void Start()
		{
			lock ( this.actionLock )
			{
				this.status = TaskStatus.Running;
				foreach ( ITask task in this.tasks )
				{
					task.Start();
				}
			}
		}

		public void Stop()
		{
			lock ( this.actionLock )
			{
				foreach ( ITask task in this.tasks )
				{
					task.Stop();
				}

				this.status = TaskStatus.Stopped;
			}
		}

		public void Terminate()
		{
			lock ( this.actionLock )
			{
				foreach ( ITask task in this.tasks )
				{
					task.Terminate();
				}

				this.status = TaskStatus.Terminated;
			}
		}

		
	}
}
