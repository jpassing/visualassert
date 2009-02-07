using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class TestItemResult 
		: AbstractResultItem, ICfixTestÌtemEventSink
	{
		private TestItemResult( 
			TestItemCollectionResult parent,
			ITestItem item,
			ExecutionStatus status
			)
			: base( parent, item, status )
		{
			Debug.Assert( ! ( item is ITestItemCollection ) );
		}

		internal override Run InternalRun
		{
			get { return this.parent.InternalRun; }
		}

		/*----------------------------------------------------------------------
		 * IResultItem.
		 */

		public override IRun Run
		{
			get { return this.Parent.Run; }
		}

		/*--------------------------------------------------------------
		 * ICfixTestÌtemEventSink.
		 */

		public void BeforeTestCaseStart()
		{
			Debug.Assert( this.Status == ExecutionStatus.Pending );
			this.Status = ExecutionStatus.Running;
		}

		public void AfterTestCaseFinish( int ranToCompletion )
		{
			Debug.Assert( this.Status == ExecutionStatus.Running );
			Debug.Assert( ( ranToCompletion == 1 ) == ( this.FailureCount == 0 ) );

			if ( this.IsInconclusive )
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

		/*--------------------------------------------------------------
		 * Factory.
		 */

		internal static TestItemResult CreateResult( 
			TestItemCollectionResult parent,
			ITestItem item,
			ExecutionStatus status
			)
		{
			Debug.Assert( ! ( item is ITestItemCollection ) );

			return new TestItemResult(
				parent,
				item,
				status );
		}

	}
}
