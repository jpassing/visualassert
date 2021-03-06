using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cfix.Control
{
	public class GenericResultCollection :
		GenericResultItem, IResultItemCollection
	{
		private readonly IList<IResultItem> subItems;
		private readonly ITestItemCollection item;

		private volatile int subItemsFinished;
		private volatile bool subItemFailed;
		private volatile bool subItemInconclusive;
		private volatile bool subItemStopped;
		private volatile int subItemsSkipped;
		private volatile int subItemsPartlySkipped;

		public GenericResultCollection(
			IActionEvents events,
			IResultItemCollection parent,
			ITestItemCollection itemCollection,
			ExecutionStatus status
			)
			: this( events, parent, itemCollection, itemCollection, status )
		{
		}

		/*++
		 * N.B. children can be explicitly provided to allow using
		 * aubset of the ITestItemCollection's children.
		 --*/
		public GenericResultCollection(
			IActionEvents events,
			IResultItemCollection parent,
			ITestItemCollection itemCollection,
			IEnumerable< ITestItem > children,
			ExecutionStatus status
			) 
			: base( events, parent, itemCollection, status )
		{
			Debug.Assert( events != null );
			Debug.Assert( itemCollection != null );
			
			this.item = itemCollection;

			//
			// Add children.
			//
			this.subItems = new List<IResultItem>(
				( int ) itemCollection.ItemCount );

			foreach ( ITestItem child in children )
			{
				IRunnableTestItem fac = child as IRunnableTestItem;
				IRunnableTestItemCollection facColl = child as IRunnableTestItemCollection;

				//
				// N.B. Avoid runnable containers without children.
				//
				bool resultRequired;
				if ( facColl != null )
				{
					//
					// If the collection contains non-runnables only,
					// omit it.
					//
					resultRequired = facColl.IsRunnable;
				}
				else
				{
					resultRequired = ( fac != null );
				}

				if ( resultRequired )
				{
					this.subItems.Add( fac.CreateResultItem(
						this,
						events,
						status ) );
				}
			}
		}

		/*--------------------------------------------------------------
		 * Protected.
		 */

		protected int SubItemsFinished
		{
			get { return this.subItemsFinished; }
		}

		protected void OnFinished( bool ranToCompletion )
		{
			//
			// N.B. Normally, when receiving this call and ranToCompletion
			// is set, all children have completed.
			//
			// When splitting a test run over multiple processes though,
			// ranToCompletion=1 does not necessarily imply that all
			// children have been run.
			//
			// Therefore, play it safe and check the status of all children
			// before considering this node finished.
			//

			if ( ranToCompletion )
			{
				foreach ( GenericResultItem child in this.subItems )
				{
					if ( !child.Completed )
					{
						//
						// Ignore this call - we will receive another call
						// when all children have completed.
						//
						return;
					}
				}
			}

			bool stoppedInSetup =
				!ranToCompletion &&
				this.subItemsFinished == 0 &&
				this.FailureCount == 0 &&
				this.ItemCount > 0;

			if ( !ranToCompletion )
			{
				Debug.Assert( this.Status != ExecutionStatus.Succeeded );

				//
				// Adjust states of children that have been skipped.
				//
				foreach ( GenericResultItem child in this.subItems )
				{
					if ( child.Status == ExecutionStatus.Pending )
					{
						child.ForceCompletion( false );
					}
				}
			}

			//
			// Update status and notify parent.
			//
			this.Status = CalculateStatus(
				this.subItemFailed,
				this.subItemInconclusive,
				this.subItemsSkipped > 0 || this.subItemsPartlySkipped > 0,
				this.subItemStopped || stoppedInSetup,
				this.subItemsSkipped > 0 && 
					this.subItemsSkipped == ItemCount &&
					this.subItemsPartlySkipped == 0 );

			GenericResultCollection tp = this.Parent as GenericResultCollection;
			if ( tp != null )
			{
				tp.OnChildFinished( this.Status, false, ranToCompletion );
			}
		}

		/*--------------------------------------------------------------
		 * Override.
		 */

		public override void ForceCompletion( bool propagateToParent )
		{
			if ( this.subItems.Count > 0 )
			{
				//
				// Force completion of childre, which will lead to own
				// status being set properly (skipped/failed/etc).
				//
				foreach ( IResultItem child in this.subItems )
				{
					child.ForceCompletion( propagateToParent );
				}
			}
			else
			{
				//
				// Must do it myself.
				//
				base.ForceCompletion( propagateToParent );
			}
		}

		/*--------------------------------------------------------------
		 * Internal.
		 */

		internal void OnChildFinished(
			ExecutionStatus status,
			bool noPropagation,
			bool ranToCompletion
			)
		{
			if ( status == ExecutionStatus.Failed )
			{
				this.subItemFailed = true;
			}
			else if ( status == ExecutionStatus.Inconclusive ||
				status == ExecutionStatus.SucceededWithInconclusiveParts )
			{
				this.subItemInconclusive = true;
			}
			else if ( status == ExecutionStatus.SucceededWithSkippedParts )
			{
				this.subItemsPartlySkipped++;
			}
			else if ( status == ExecutionStatus.Skipped )
			{
				this.subItemsSkipped++;
			}
			else if ( status == ExecutionStatus.Stopped )
			{
				this.subItemStopped = true;
			}

			subItemsFinished++;

			Debug.Assert( subItemsFinished <= this.subItems.Count );

			//
			// N.B. For leaf children, we do need to track how many children
			// have finished as this object will get a AfterFixtureFinish
			// callback.
			//
			if ( !noPropagation && subItemsFinished == this.subItems.Count )
			{
				OnFinished( ranToCompletion );
			}
		}

		/*--------------------------------------------------------------
		 * IResultItemCollection.
		 */

		[Browsable( false )]
		public ITestItemCollection ItemCollection
		{
			get { return this.item; }
		}

		public uint ItemCount
		{
			get { return ( uint ) this.subItems.Count; }
		}

		public IResultItem GetItem( uint ordinal )
		{
			return this.subItems[ ( int ) ordinal ];
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#" )]
		public IResultItem GetItem( ITestItem testItem )
		{
			foreach ( IResultItem child in this.subItems )
			{
				if ( ReferenceEquals( testItem, child.Item ) )
				{
					return child;
				}
			}

			return null;
		}

		/*--------------------------------------------------------------
		 * IEnumerable.
		 */

		public IEnumerator<IResultItem> GetEnumerator()
		{
			return this.subItems.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.subItems.GetEnumerator();
		}
	}
}
