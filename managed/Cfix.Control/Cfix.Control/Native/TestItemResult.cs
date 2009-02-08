using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class TestItemResult 
		: AbstractResultItem, ICfixTest�temEventSink
	{
		private volatile int executions;

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
		 * ICfixTest�temEventSink.
		 */

		public void BeforeTestCaseStart()
		{
			//
			// N.B. This method may be called twice in case of ambiguous
			// test case names.
			//
			Debug.Assert( ( executions == 0 ) == ( this.Status == ExecutionStatus.Pending ) );
			this.Status = ExecutionStatus.Running;

			this.executions++;
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

			this.parent.OnChildFinished( this.Status, true );
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