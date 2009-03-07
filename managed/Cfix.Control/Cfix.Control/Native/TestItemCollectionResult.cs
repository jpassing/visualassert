using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class TestItemCollectionResult 
		: AbstractNativeResultItem, 
		  IResultItemCollection, 
		  ICfixTestÌtemContainerEventSink
	{
		private readonly IList<IResultItem> subItems;

		private volatile int subItemsFinished;
		private volatile bool subItemFailed;
		private volatile bool subItemInconclusive;

		internal TestItemCollectionResult( 
			IActionEvents events,
			IResultItemCollection parent,
			ITestItemCollection itemCollection,
			ExecutionStatus status
			)
			: base( events, parent, itemCollection, status )
		{
			Debug.Assert( events != null );
			Debug.Assert( itemCollection != null );

			//
			// Add children.
			//
			this.subItems = new List<IResultItem>(
				( int ) itemCollection.ItemCount );

			foreach ( ITestItem child in itemCollection )
			{
				IResultItemFactory fac = child as IResultItemFactory;
				if ( fac != null )
				{
					this.subItems.Add( fac.CreateResultItem(
						this,
						events,
						status ) );
				}
			}
		}

		internal void OnChildFinished( 
			ExecutionStatus status, 
			bool childIsLeaf,
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

			subItemsFinished++;

			//
			// N.B. For leaf children, we do need to track how many children
			// have finished as this object will get a AfterFixtureFinish
			// callback.
			//
			if ( ! childIsLeaf && subItemsFinished == this.subItems.Count )
			{
				OnFinished( ranToCompletion );
			}
		}

		private void OnFinished( bool ranToCompletion )
		{
#if DEBUG
			if ( ranToCompletion )
			{
				foreach ( AbstractNativeResultItem child in this.subItems )
				{
					Debug.Assert( child.Completed );
				}
			}
#endif

			if ( !ranToCompletion )
			{
				Debug.Assert( this.Status != ExecutionStatus.Succeeded );

				//
				// Adjust states of children that have been skipped.
				//
				foreach ( AbstractNativeResultItem child in this.subItems )
				{
					if ( child.Status == ExecutionStatus.Pending )
					{
						// TODO: recursive?
						child.Status = ExecutionStatus.Skipped;
					}
				}
			}

			//
			// Update status and notify parent.
			//
			this.Status = CalculateStatus( 
				this.subItemFailed, 
				this.subItemInconclusive );

			TestItemCollectionResult tp = this.parent as TestItemCollectionResult;
			if ( tp != null )
			{
				tp.OnChildFinished( this.Status, false, ranToCompletion );
			}
		}

		/*----------------------------------------------------------------------
		 * IResultItemCollection.
		 */

		public ITestItemCollection ItemCollection
		{
			get { return ( ITestItemCollection ) Item; }
		}

		public uint ItemCount 
		{
			get { return ( uint ) this.subItems.Count; }
		}

		public IResultItem GetItem( uint ordinal )
		{
			return this.subItems[ ( int ) ordinal ];
		}

		/*----------------------------------------------------------------------
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

		/*----------------------------------------------------------------------
		 * ICfixTestÌtemContainerEventSink.
		 * 
		 * Only applies if this object represents a fixture.
		 */

		public void BeforeFixtureStart()
		{
			Debug.Assert( this.Status == ExecutionStatus.Pending );
			this.Status = ExecutionStatus.Running;
		}

		public void AfterFixtureFinish( int ranToCompletionInt )
		{
			OnFinished( ranToCompletionInt != 0 );
		}

		public ICfixTestÌtemEventSink GetTestItemEventSink( 
			uint testCaseOrdinal, 
			uint threadId 
			)
		{
			IResultItem item = this.subItems[ ( int ) testCaseOrdinal ];
			Debug.Assert( item is TestItemResult );

			if ( item == null )
			{
				throw new CfixException( "Unknown item" );
			}
			else
			{
				return ( ICfixTestÌtemEventSink ) item;
			}
		}

		public override CFIXCTL_REPORT_DISPOSITION FailedAssertion(
			string expression,
			string routine,
			string file,
			string message,
			uint line,
			uint lastError,
			uint flags,
			uint reserved,
			ICfixStackTrace stackTrace
			)
		{
			CFIXCTL_REPORT_DISPOSITION disp = base.FailedAssertion(
				expression, routine, file, message,
				line, lastError, flags, reserved, stackTrace );

			if ( this.subItemsFinished == 0 )
			{
				//
				// Setup failure. There will not be any further callbacks, 
				// thus finish now.
				//
				OnFinished( false );
				Debug.Assert( Status == ExecutionStatus.Failed );
			}

			return disp;
		}

		public override CFIXCTL_REPORT_DISPOSITION FailedRelateAssertion(
			CFIXCTL_RELATE_OPERATOR op,
			object expectedValue,
			object actualValue,
			string routine,
			string file,
			string message,
			uint line,
			uint lastError,
			uint flags,
			uint reserved,
			ICfixStackTrace stackTrace )
		{
			CFIXCTL_REPORT_DISPOSITION disp = base.FailedRelateAssertion(
				op, expectedValue, actualValue, routine, file, message,
				line, lastError, flags, reserved, stackTrace );

			if ( this.subItemsFinished == 0 )
			{
				//
				// Setup failure. There will not be any further callbacks, 
				// thus finish now.
				//
				OnFinished( false );
				Debug.Assert( Status == ExecutionStatus.Failed );
			}

			return disp;
		}

		public override CFIXCTL_REPORT_DISPOSITION UnhandledException(
			uint exceptionCode,
			uint reserved,
			ICfixStackTrace stackTrace )
		{
			CFIXCTL_REPORT_DISPOSITION disp = base.UnhandledException(
				exceptionCode, reserved, stackTrace );

			if ( this.subItemsFinished == 0 )
			{
				//
				// Setup failure. There will not be any further callbacks, 
				// thus finish now.
				//
				OnFinished( false );
				Debug.Assert( Status == ExecutionStatus.Failed );
			}

			return disp;
		}

		public override void Inconclusive(
			string reason,
			uint reserved,
			ICfixStackTrace stackTrace
			)
		{
			base.Inconclusive( reason, reserved, stackTrace );

			if ( this.subItemsFinished == 0 )
			{
				//
				// Setup failure. There will not be any further callbacks, 
				// thus finish now.
				//
				OnFinished( false );
				Debug.Assert( Status == ExecutionStatus.Inconclusive );
			}
		}
	}
}
