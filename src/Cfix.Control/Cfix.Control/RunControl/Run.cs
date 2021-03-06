using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Cfix.Control.RunControl
{
	public class Run : AbstractActionEventSink, IRun
	{
		public event EventHandler Started;
		public event EventHandler<FinishedEventArgs> Finished;
		public event EventHandler<HostEventArgs> BeforeTerminate;

		private readonly List<ITask> tasks = new List<ITask>();
		
		private volatile TaskStatus status = TaskStatus.Ready;
		private volatile uint tasksFinished;

		private IResultItemCollection rootResult;
		
		private readonly object actionLock = new object();

		private bool involvedPostprocessing;

		public Run(
			IDispositionPolicy policy
			)
			: base( policy )
		{
		}

		~Run()
		{
			Dispose( false );
		}

		public bool InvolvesPostprocessing
		{
			get { return this.involvedPostprocessing; }
			set { this.involvedPostprocessing = value; }
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
			if ( this.status != TaskStatus.Ready )
			{
				throw new InvalidOperationException();
			}

			this.tasks.Add( task );
			task.Finished += new EventHandler<FinishedEventArgs>( task_Finished );
		}

		private void task_Finished( object sender, FinishedEventArgs e )
		{
			this.status = ( TaskStatus ) Math.Max(
				( int ) ( ( ITask ) sender ).Status,
				( int ) this.status );
			this.tasksFinished++;

			if ( this.tasksFinished == this.tasks.Count &&
				 this.Finished != null )
			{
				//
				// XXX: e may be less severe than the previous 
				// FinishedEventArgs.
				//
				this.Finished( this, e );
			}
		}

		/*--------------------------------------------------------------
		 * IRun.
		 */

		public IResultItemCollection RootResult
		{
			get { return this.rootResult; }
			internal set 
			{
				Debug.Assert( this.rootResult == null );
				this.rootResult = value; 
			}
		}

		public IEnumerable<ITask> Tasks
		{
			get { return this.tasks; }
		}

		public uint TaskCount
		{
			get { return ( uint ) this.tasks.Count; }
		}

		public uint ItemCount
		{
			get 
			{
				uint count = 0;
				foreach ( ITask task in this.tasks )
				{
					count += task.ItemCount;
				}

				return count;
			}
		}

		public TaskStatus Status
		{
			get { return this.status; }
		}
		
		public void Start()
		{
			Debug.Assert( ( this.rootResult == null ) == ( this.tasks.Count == 0 ) );

			lock ( this.actionLock )
			{
				this.status = TaskStatus.Running;
				int tasksStarted = 0;
				foreach ( ITask task in this.tasks )
				{
					task.Start();
					tasksStarted++;
				}

				if ( tasksStarted > 0 )
				{
					if ( this.Started != null )
					{
						this.Started( this, EventArgs.Empty );
					}
				}
				else
				{
					this.status = TaskStatus.Suceeded;
					if ( this.Finished != null )
					{
						this.Finished( this, FinishedEventArgs.Empty );
					}
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
					IProcessTask procTask = task as IProcessTask;
					if ( this.BeforeTerminate != null && procTask != null )
					{
						this.BeforeTerminate(
							this,
							new HostEventArgs( procTask.ProcessId ) );
					}

					task.Terminate();
				}

				this.status = TaskStatus.Terminated;
			}
		}

		
	}
}
