using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class TestItemCollectionResult 
		: AbstractResultItem, IResultItemCollection, ICfixTestÌtemContainerEventSink
	{
		private readonly IRun run;

		private IList<TestItemResult> subItems;
		private volatile bool subItemFailed;

		private TestItemCollectionResult( 
			TestItemCollectionResult parent,
			IList<TestItemResult> subItems,
			ITestItem item,
			ExecutionStatus status
			)
			: base( parent, item, status )
		{
			Debug.Assert( parent != null );
			Debug.Assert( item is ITestItemCollection );
			this.subItems = subItems;
			this.run = parent.Run;
		}

		private TestItemCollectionResult(
			IRun run,
			IList<TestItemResult> subItems,
			ITestItem item,
			ExecutionStatus status
			)
			: base( null, item, status )
		{
			Debug.Assert( item is ITestItemCollection );
			this.subItems = subItems;
			this.run = run;
		}

		internal void OnItemFailed( AbstractResultItem item )
		{
			this.subItemFailed = true;
		}

		/*----------------------------------------------------------------------
		 * IResultItem.
		 */

		public override IRun Run
		{
			get { return this.Run; }
		}

		/*----------------------------------------------------------------------
		 * IResultItemCollection.
		 */

		public ITestItemCollection ItemCollection
		{
			get { return ( ITestItemCollection ) Item; }
		}

		/*----------------------------------------------------------------------
		 * IEnumerable.
		 */

		public IEnumerator<IResultItem> GetEnumerator()
		{
			return ( IEnumerator<IResultItem> ) this.subItems.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.subItems.GetEnumerator();
		}

		/*----------------------------------------------------------------------
		 * ICfixTestÌtemContainerEventSink.
		 */

		public void BeforeFixtureStart()
		{
			Debug.Assert( this.Status == ExecutionStatus.Pending );
			this.Status = ExecutionStatus.Running;
		}

		public void AfterFixtureFinish( int ranToCompletionInt )
		{
			bool ranToCompletion = ( ranToCompletionInt != 0 );
			
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
				this.Status = ExecutionStatus.Failed;
			}
			else
			{
				this.Status = ExecutionStatus.Succeeded;
			}
		}

		public ICfixTestÌtemEventSink GetTestItemEventSink( 
			uint testCaseOrdinal, 
			uint threadId 
			)
		{
			return this.subItems[ ( int ) testCaseOrdinal ];
		}

	}
}
