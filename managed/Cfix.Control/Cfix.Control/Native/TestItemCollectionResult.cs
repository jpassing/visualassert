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
		: AbstractResultItem, 
		  IResultItemCollection, 
		  ICfixTestÌtemContainerEventSink
	{
		private readonly Run run;

		private IList<IResultItem> subItems;

		private volatile int subItemsFinished;
		private volatile bool subItemFailed;
		private volatile bool subItemInconclusive;

		private TestItemCollectionResult( 
			TestItemCollectionResult parent,
			ITestItem item,
			ExecutionStatus status
			)
			: base( parent, item, status )
		{
			Debug.Assert( parent != null );
			Debug.Assert( item is ITestItemCollection );
			Debug.Assert( parent.InternalRun != null );

			this.run = parent.InternalRun;
		}

		private TestItemCollectionResult(
			Run run,
			ITestItem item,
			ExecutionStatus status
			)
			: base( null, item, status )
		{
			Debug.Assert( item is ITestItemCollection );
			Debug.Assert( run != null );

			this.run = run;
		}

		private void SetItems( IList<IResultItem> items )
		{
			this.subItems = items;
		}

		internal void OnChildStarted()
		{
			if ( this.Status == ExecutionStatus.Pending )
			{
				this.Status = ExecutionStatus.Running;
				this.parent.OnChildStarted();
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
				foreach ( AbstractResultItem child in this.subItems )
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
				foreach ( AbstractResultItem child in this.subItems )
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

			if ( this.parent != null )
			{
				this.parent.OnChildFinished( this.Status, false );
			}
		}

		internal override Run InternalRun
		{
			get { return this.run; }
		}

		/*----------------------------------------------------------------------
		 * IResultItem.
		 */

		public override IRun Run
		{
			get { return this.run; }
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

			return ( ICfixTestÌtemEventSink ) item;
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

		private static TestItemCollectionResult CreateResult(
			Run run,
			TestItemCollectionResult parent,
			ITestItemCollection itemColl,
			ExecutionStatus status
			)
		{
			Debug.Assert( run != null );

			TestItemCollectionResult result;
			if ( parent == null )
			{
				result = new TestItemCollectionResult(
					run,
					itemColl,
					status );
			}
			else
			{
				result = new TestItemCollectionResult(
					parent,
					itemColl,
					status );
			}

			run.OnItemAdded( result );

			IList<IResultItem> children = new List<IResultItem>( 
				( int ) itemColl.ItemCount );

			foreach ( ITestItem item in itemColl )
			{
				ITestItemCollection childColl = item as ITestItemCollection;
				if ( childColl != null )
				{
					children.Add( 
						CreateResult( run, result, childColl, status ) );
				}
				else
				{
					TestItemResult itemResult = TestItemResult.CreateResult(
						result, item, status );
					run.OnItemAdded( itemResult );
					children.Add( itemResult );
				}
			}

			result.SetItems( children );
			return result;
		}

		internal static TestItemCollectionResult CreateResult(
			Run run,
			ITestItemCollection itemColl,
			ExecutionStatus status
			)
		{
			TestItemCollectionResult result = CreateResult( run, null, itemColl, status );

			TestItemCollection nativeColl = itemColl as TestItemCollection;
			TestModule nativeMod = itemColl as TestModule;
			if ( itemColl != null && nativeMod == null )
			{
				//
				// This is a fixture. We must add the module in order
				// to yield a fully workable result, which, in particular,
				// can be used as cfixctl event sink.
				//

				TestItemCollectionResult moduleResult = new TestItemCollectionResult(
					run,
					nativeColl.Module,
					status );
				moduleResult.SetItems( new IResultItem[] { result } );
				run.OnItemAdded( moduleResult );

				return moduleResult;
			}
			else
			{
				return result;
			}
		}
	}
}
