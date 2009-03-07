using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cfix.Control
{
	public class GenericResultItem : IResultItem
	{
		//
		// TestItem this result corresponds to - may or may not be
		// still available.
		//
		private ITestItem item;
		private readonly String itemName;

		protected IActionEvents events;
		
		private volatile ExecutionStatus status;
		private volatile bool inconclusive;

		private readonly IResultItemCollection parent;

		private readonly Object failuresLock = new Object();
		private ICollection<Failure> failures;

		protected GenericResultItem(
			IActionEvents events,
			IResultItemCollection parent,
			ITestItem item,
			ExecutionStatus status
			)
		{
			this.events = events;
			this.parent = parent;
			this.status = status;
			this.itemName = item.Name;

			this.item = item;
			this.item.Disposed += new EventHandler( item_Disposed );
		}

		private void item_Disposed( object sender, EventArgs e )
		{
			//
			// Remove own reference.
			//
			this.item = null;
			this.events = null;
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
			bool allSubItemsSkipped 
			)
		{
			if ( subItemFailed )
			{
				return ExecutionStatus.Failed;
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
					case ExecutionStatus.Failed:
					case ExecutionStatus.Inconclusive:
					case ExecutionStatus.Skipped:
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
			get { return this.itemName; }
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

					this.status = value;
					this.events.OnStatusChanged( this );
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
