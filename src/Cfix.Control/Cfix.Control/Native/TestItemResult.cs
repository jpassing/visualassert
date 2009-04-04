using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control.Native
{
	/*++
	 * Class Description:
	 *		Represents the outcome of a leaf ICfixTestItem 
	 *		(e.g. test case).
	 * 
	 *		Threadsafe.
	 --*/
	internal class TestItemResult 
		: GenericResultItem, 
		  ICfixTestÌtemEventSink
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

			//
			// N.B. It is possible that the run has been stopped, yet there
			// already has been an error reported. The status will be Failed
			// rather than Stopped in this case -- this is considered ok.
			//
			bool stopped = ( ranToCompletion == 0 ) && this.FailureCount == 0;

			this.Status = CalculateStatus( 
				false, 
				false,
				false,
				stopped, 
				false );

			GenericResultCollection tp = this.Parent as GenericResultCollection;
			if ( tp != null )
			{
				tp.OnChildFinished( this.Status, true, ranToCompletion > 0 );
			}
		}

		/*----------------------------------------------------------------------
		 * ICfixReportEventSink.
		 */

		public void BeforeChildThreadStart( uint threadId )
		{
			Debug.Assert( this.events != null );
			this.events.OnThreadStarted( this, threadId );
		}

		public void AfterChildThreadFinish( uint threadId )
		{
			Debug.Assert( this.events != null );
			this.events.OnThreadFinished( this, threadId );
		}

		public virtual CFIXCTL_REPORT_DISPOSITION FailedAssertion(
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
			FailedAssertionFailure ass = new FailedAssertionFailure(
				expression,
				message,
				file,
				line,
				routine,
				StackTrace.Wrap( stackTrace ),
				lastError );

			AddFailure( ass );

			Debug.Assert( this.events != null );
			return ( CFIXCTL_REPORT_DISPOSITION )
				this.events.DispositionPolicy.FailedAssertion( ass );
		}

		public virtual CFIXCTL_REPORT_DISPOSITION FailedRelateAssertion(
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
			FailedRelateExpressionFailure fr = new FailedRelateExpressionFailure(
				( RelateOperator ) op,
				expectedValue,
				actualValue,
				message,
				file,
				line,
				routine,
				StackTrace.Wrap( stackTrace ),
				lastError );

			AddFailure( fr );

			Debug.Assert( this.events != null );
			return ( CFIXCTL_REPORT_DISPOSITION )
				this.events.DispositionPolicy.FailedAssertion( fr );
		}

		public virtual CFIXCTL_REPORT_DISPOSITION UnhandledException(
			uint exceptionCode,
			uint reserved,
			ICfixStackTrace stackTrace )
		{
			UnhandledExceptionFailure u = new UnhandledExceptionFailure(
				exceptionCode,
				StackTrace.Wrap( stackTrace ) );

			AddFailure( u );

			Debug.Assert( this.events != null );
			return ( CFIXCTL_REPORT_DISPOSITION )
				this.events.DispositionPolicy.UnhandledException( u );
		}

		public virtual void Inconclusive(
			string reason,
			uint reserved,
			ICfixStackTrace stackTrace
			)
		{
			AddFailure( new Inconclusiveness(
				reason,
				StackTrace.Wrap( stackTrace ) ) );
			IsInconclusive = true;
		}

		public void Log( string message, uint Reserved, ICfixStackTrace StackTrace )
		{
			Debug.Assert( this.events != null );
			this.events.OnLog( this, message );
		}

		public CFIXCTL_REPORT_DISPOSITION QueryDefaultFailedAssertionDisposition()
		{
			Debug.Assert( this.events != null );
			return ( CFIXCTL_REPORT_DISPOSITION )
				this.events.DispositionPolicy.DefaultFailedAssertionDisposition;
		}

		public CFIXCTL_REPORT_DISPOSITION QueryDefaultUnhandledExceptionDisposition()
		{
			Debug.Assert( this.events != null );
			return ( CFIXCTL_REPORT_DISPOSITION )
				this.events.DispositionPolicy.DefaultUnhandledExceptionDisposition;
		}
	}
}
