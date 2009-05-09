using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cfix.Control
{
	public class GenericResultItem : IResultItem
	{
		private readonly ITestItem item;

		protected internal readonly IActionEvents events;
		
		private volatile ExecutionStatus status;
		private volatile bool inconclusive;

		private readonly IResultItemCollection parent;

		private readonly Object failuresLock = new Object();
		private ICollection<Failure> failures;

		private DateTime? startTime;
		private DateTime? finishTime;

		protected GenericResultItem(
			IActionEvents events,
			IResultItemCollection parent,
			ITestItem item,
			ExecutionStatus status
			)
		{
			Debug.Assert( events != null );
			Debug.Assert( item != null );

			this.events = events;
			this.parent = parent;
			this.status = status;

			this.item = item;
		}

		/*----------------------------------------------------------------------
		 * Protected.
		 */

		protected void AddFailure( Failure failure )
		{
			lock ( this.failuresLock )
			{
				if ( this.failures == null )
				{
					this.failures = new LinkedList<Failure>();
				}

				this.failures.Add( failure );

				this.events.OnFailureOccured( this );
			}
		}

		protected bool IsInconclusive
		{
			get { return this.inconclusive; }
			set { this.inconclusive = value; }
		}

		protected ExecutionStatus CalculateStatus(
			bool subItemFailed,
			bool subItemInconclusive,
			bool subItemSkipped,
			bool subItemStopped,
			bool allSubItemsSkipped
			)
		{
			if ( subItemFailed )
			{
				return ExecutionStatus.Failed;
			}
			else if ( subItemStopped )
			{
				return ExecutionStatus.Stopped;
			}
			else if ( this.IsInconclusive )
			{
				Debug.Assert( this.FailureCount > 0 );
				return ExecutionStatus.Inconclusive;
			}
			else if ( this.FailureCount > 0 )
			{
				//
				// Item itself failed.
				// 
				// N.B. In case of collections, this may occur if 
				// Setup/Teardown has failed.
				//
				return ExecutionStatus.Failed;
			}
			else if ( subItemInconclusive )
			{
				//
				// Largely successful, but some inconclusive.
				//
				return ExecutionStatus.SucceededWithInconclusiveParts;
			}
			else if ( allSubItemsSkipped )
			{
				return ExecutionStatus.Skipped;
			}
			else if ( subItemSkipped )
			{
				//
				// Largely successful, but some skipped.
				//
				return ExecutionStatus.SucceededWithSkippedParts;
			}
			else 
			{
				return ExecutionStatus.Succeeded;
			}
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public int FailureCount
		{
			get
			{
				if ( this.failures == null )
				{
					return 0;
				}
				else
				{
					return this.failures.Count;
				}
			}
		}

		public bool Completed
		{
			get
			{
				switch ( this.Status )
				{
					case ExecutionStatus.Succeeded:
					case ExecutionStatus.SucceededWithInconclusiveParts:
					case ExecutionStatus.SucceededWithSkippedParts:
					case ExecutionStatus.Failed:
					case ExecutionStatus.Inconclusive:
					case ExecutionStatus.Skipped:
					case ExecutionStatus.Stopped:
						return true;

					case ExecutionStatus.Pending:
					case ExecutionStatus.Running:
						return false;

					default:
						Debug.Fail( "Invalid case" );
						return false;
				}
			}
		}

		/*----------------------------------------------------------------------
		 * IResultItem.
		 */

		public IResultItemCollection Parent
		{
			get
			{
#if DEBUG
				if ( this.parent != null )
				{
					bool foundInParent = false;
					foreach ( IResultItem child in this.parent )
					{
						if ( ReferenceEquals( child, this ) )
						{
							foundInParent = true;
							break;
						}
					}

					Debug.Assert( foundInParent );
				}
#endif
				return this.parent;
			}
		}

		public ITestItem Item
		{
			get { return this.item; }
		}

		public string Name
		{
			get { return this.item.Name; }
		}

		public ExecutionStatus Status
		{
			get { return this.status; }
			set
			{
				if ( value != this.status )
				{
					Debug.Print( "Status of " + Item.Name + " is " + value );
					Debug.Assert( this.events != null );
					Debug.Assert( this.item != null );

					if ( value == ExecutionStatus.Running )
					{
						this.startTime = DateTime.Now;
					}
					else if ( this.startTime != null )
					{
						this.finishTime = DateTime.Now;
					}

					this.status = value;
					this.events.OnStatusChanged( this );

					if ( this.parent != null && value == ExecutionStatus.Running )
					{
						this.parent.Status = value;
					}
				}
			}
		}

		public TimeSpan Duration
		{
			get 
			{
				if ( this.startTime == null || this.finishTime == null )
				{
					return new TimeSpan( 0 );
				}
				else
				{
					return ( TimeSpan ) ( this.finishTime - this.startTime );
				}
			}
		}

		public ICollection<Failure> Failures
		{
			get { return this.failures; }
		}

		public virtual void ForceCompletion( bool propagateToParent )
		{
			if ( !Completed )
			{
				this.Status = ExecutionStatus.Skipped;

				GenericResultCollection tp = this.Parent as GenericResultCollection;
				if ( tp != null )
				{
					tp.OnChildFinished( 
						this.Status,
						!propagateToParent,
						false );
				}
			}
		}
	}
}
