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
			try
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
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}

		/*----------------------------------------------------------------------
		 * ICfixReportEventSink.
		 */

		public void BeforeChildThreadStart( uint threadId )
		{
			try
			{
				Debug.Assert( this.Events != null );
				this.Events.OnThreadStarted( this, threadId );
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}

		public void AfterChildThreadFinish( uint threadId )
		{
			try
			{
				Debug.Assert( this.Events != null );
				this.Events.OnThreadFinished( this, threadId );
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
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
			try
			{
				FailedAssertionFailure ass = new FailedAssertionFailure(
					expression,
					message,
					file,
					line,
					routine,
					NativeStackTrace.Wrap( stackTrace ),
					lastError );

				AddFailure( ass );

				Debug.Assert( this.Events != null );
				return ( CFIXCTL_REPORT_DISPOSITION )
					this.Events.DispositionPolicy.FailedAssertion( ass );
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
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
			try
			{
				FailedRelateExpressionFailure fr = new FailedRelateExpressionFailure(
					( RelateOperator ) op,
					expectedValue,
					actualValue,
					message,
					file,
					line,
					routine,
					NativeStackTrace.Wrap( stackTrace ),
					lastError );

				AddFailure( fr );

				Debug.Assert( this.Events != null );
				return ( CFIXCTL_REPORT_DISPOSITION )
					this.Events.DispositionPolicy.FailedAssertion( fr );
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}

		public virtual CFIXCTL_REPORT_DISPOSITION UnhandledException(
			uint exceptionCode,
			uint reserved,
			ICfixStackTrace stackTrace )
		{
			try
			{
				UnhandledExceptionFailure u = new UnhandledExceptionFailure(
					exceptionCode,
					NativeStackTrace.Wrap( stackTrace ) );

				AddFailure( u );

				Debug.Assert( this.Events != null );
				return ( CFIXCTL_REPORT_DISPOSITION )
					this.Events.DispositionPolicy.UnhandledException( u );
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}

		public virtual void Inconclusive(
			string reason,
			uint reserved,
			ICfixStackTrace stackTrace
			)
		{
			try
			{
				AddFailure( new Inconclusiveness(
					reason,
					NativeStackTrace.Wrap( stackTrace ) ) );
				IsInconclusive = true;
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}

		public void Log( string message, uint Reserved, ICfixStackTrace StackTrace )
		{
			try
			{
				Debug.Assert( this.Events != null );
				this.Events.OnLog( this, message );
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}

		public CFIXCTL_REPORT_DISPOSITION QueryDefaultFailedAssertionDisposition()
		{
			try
			{
				Debug.Assert( this.Events != null );
				return ( CFIXCTL_REPORT_DISPOSITION )
					this.Events.DispositionPolicy.DefaultFailedAssertionDisposition;
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}

		public CFIXCTL_REPORT_DISPOSITION QueryDefaultUnhandledExceptionDisposition()
		{
			try
			{
				Debug.Assert( this.Events != null );
				return ( CFIXCTL_REPORT_DISPOSITION )
					this.Events.DispositionPolicy.DefaultUnhandledExceptionDisposition;
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}
	}
}
