using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control.Native
{
	/*++
	 * Class that may represent a module or a fixture.
	 --*/
	internal class TestItemCollectionResult 
		: AbstractNativeResultItem, 
		  IResultItemCollection, 
		  ICfixTestÌtemContainerEventSink
	{
		private IList<IResultItem> subItems;

		private volatile int subItemsFinished;
		private volatile bool subItemFailed;
		private volatile bool subItemInconclusive;

		internal TestItemCollectionResult( 
			IActionEvents events,
			IResultItemCollection parent,
			ITestItem item,
			ExecutionStatus status
			)
			: base( events, parent, item, status )
		{
			Debug.Assert( item is ITestItemCollection );
			Debug.Assert( events != null );
		}

		internal void SetItems( IList<IResultItem> items )
		{
			Debug.Assert( this.subItems == null );
			this.subItems = items;
		}

		internal void OnChildStarted()
		{
			if ( this.Status == ExecutionStatus.Pending )
			{
				this.Status = ExecutionStatus.Running;

				TestItemCollectionResult tp = this.parent as TestItemCollectionResult;
				if ( tp != null )
				{
					tp.OnChildStarted();
				}
			}
		}

		internal void OnChildFinished( ExecutionStatus status, bool childIsLeaf )
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
				OnFinished( true );
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
						child.Status = ExecutionStatus.Skipped;
					}
				}
			}

			//
			// Update status and notify parent.
			//
			if ( this.subItemFailed )
			{
				Status = ExecutionStatus.Failed;
			}
			else if ( this.IsInconclusive )
			{
				Debug.Assert( this.FailureCount > 0 );
				this.Status = ExecutionStatus.Inconclusive;
			}
			else if ( this.FailureCount > 0 )
			{
				//
				// Setup/Teardown failed.
				//
				Status = ExecutionStatus.Failed;
			}
			else if ( this.subItemInconclusive )
			{
				//
				// Largely successful, but some inconclusive.
				//
				this.Status = ExecutionStatus.SucceededWithInconclusiveParts;
			}
			else
			{
				this.Status = ExecutionStatus.Succeeded;
			}

			TestItemCollectionResult tp = this.parent as TestItemCollectionResult;
			if ( tp != null )
			{
				tp.OnChildFinished( this.Status, false );
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

		/*--------------------------------------------------------------
		 * Factory.
		 */

		//private static TestItemCollectionResult CreateResult(
		//    Run run,
		//    TestItemCollectionResult parent,
		//    ITestItemCollection itemColl,
		//    ExecutionStatus status
		//    )
		//{
		//    Debug.Assert( run != null );

		//    TestItemCollectionResult result;
		//    if ( parent == null )
		//    {
		//        result = new TestItemCollectionResult(
		//            run,
		//            itemColl,
		//            status );
		//    }
		//    else
		//    {
		//        result = new TestItemCollectionResult(
		//            run,
		//            parent,
		//            itemColl,
		//            status );
		//    }

		//    IList<IResultItem> children = new List<IResultItem>( 
		//        ( int ) itemColl.ItemCount );

		//    foreach ( ITestItem item in itemColl )
		//    {
		//        ITestItemCollection childColl = item as ITestItemCollection;
		//        if ( childColl != null )
		//        {
		//            children.Add( 
		//                CreateResult( run, result, childColl, status ) );
		//        }
		//        else
		//        {
		//            TestItemResult itemResult = TestItemResult.CreateResult(
		//                run,
		//                result, 
		//                item, 
		//                status );
		//            children.Add( itemResult );
		//        }
		//    }

		//    result.SetItems( children );
		//    return result;
		//}

		//internal static TestItemCollectionResult CreateResult(
		//    Run run,
		//    ITestItemCollection itemColl,
		//    ExecutionStatus status
		//    )
		//{
		//    return CreateResult( run, null, itemColl, status );
		//}
	}
}
