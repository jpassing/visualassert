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

		internal void OnItemFailed()
		{
			this.subItemFailed = true;
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
			Run run,
			TestItemCollectionResult parent,
			ITestItemCollection itemColl,
			ExecutionStatus status
			)
		{
			Debug.Assert( ( run == null ) != ( parent == null ) );

			TestItemCollectionResult result;
			if ( run != null )
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
			Run run,
			ITestItemCollection itemColl,
			ExecutionStatus status
			)
		{
			return CreateResult( run, null, itemColl, status );
		}

	}
}
