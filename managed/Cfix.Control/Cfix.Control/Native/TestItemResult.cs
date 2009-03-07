using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class TestItemResult 
		: AbstractNativeResultItem, ICfixTestÌtemEventSink
	{
		private volatile int executions;

		internal TestItemResult(
			IActionEvents events,
			IResultItemCollection parent,
			ITestItem item,
			ExecutionStatus status
			)
			: base( events, parent, item, status )
		{
			Debug.Assert( ! ( item is ITestItemCollection ) );
		}

		/*--------------------------------------------------------------
		 * ICfixTestÌtemEventSink.
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

			this.Status = CalculateStatus( false, false );
			
			TestItemCollectionResult tp = this.parent as TestItemCollectionResult;
			if ( tp != null )
			{
				tp.OnChildFinished( this.Status, true, ranToCompletion > 0 );
			}
		}

	}
}
