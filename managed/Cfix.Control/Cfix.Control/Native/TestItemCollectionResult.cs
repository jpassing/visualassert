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
		private readonly IRun run;

		private IList<IResultItem> subItems;
		private volatile bool subItemFailed;

		private TestItemCollectionResult( 
			TestItemCollectionResult parent,
			ITestItem item,
			ExecutionStatus status
			)
			: base( parent, item, status )
		{
			Debug.Assert( parent != null );
			Debug.Assert( item is ITestItemCollection );
			this.run = parent.Run;
		}

		private TestItemCollectionResult(
			IRun run,
			ITestItem item,
			ExecutionStatus status
			)
			: base( null, item, status )
		{
			Debug.Assert( item is ITestItemCollection );
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
			get { return this.run; }
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
			IResultItem item = this.subItems[ ( int ) testCaseOrdinal ];
			Debug.Assert( item is TestItemResult );

			return ( ICfixTestÌtemEventSink ) item;
		}

		/*--------------------------------------------------------------
		 * Factory.
		 */

		private static TestItemCollectionResult CreateResult(
			IRun run,
			TestItemCollectionResult parent,
			ITestItemCollection itemColl,
			ExecutionStatus status
			)
		{
			Debug.Assert( ( run == null ) != ( parent == null ) );

			TestItemCollectionResult result = new TestItemCollectionResult(
				run,
				itemColl,
				status );
			IList<IResultItem> children = new List<IResultItem>( 
				( int ) itemColl.ItemCount );

			foreach ( ITestItem item in itemColl )
			{
				ITestItemCollection childColl = item as ITestItemCollection;
				if ( childColl != null )
				{
					children.Add( 
						CreateResult( null, result, childColl, status ) );
				}
				else
				{
					children.Add( TestItemResult.CreateResult(
						result, item, status ) );
				}
			}

			result.subItems = children;
			return result;
		}

		internal static TestItemCollectionResult CreateResult(
			IRun run,
			ITestItemCollection itemColl,
			ExecutionStatus status
			)
		{
			return CreateResult( run, null, itemColl, status );
		}

	}
}
