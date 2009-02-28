using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfix.Control;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal abstract class AbstractNativeResultItem
		: AbstractResultItem, ICfixReportEventSink
	{
		private readonly Object failuresLock = new Object();
		private ICollection<Failure> failures;
		private volatile bool inconclusive;

		protected AbstractNativeResultItem(
			IActionEvents events,
			IResultItemCollection parent,
			ITestItem item,
			ExecutionStatus status
			)
			: base( events, parent, item, status )
		{
		}

		private void AddFailure( Failure failure )
		{
			lock ( this.failuresLock )
			{
				if ( this.failures == null )
				{
					this.failures = new LinkedList<Failure>();
				}

				this.failures.Add( failure );
			}
		}
		
		public int FailureCount
		{
			get
			{
				if ( this.failures == null )
				{
					return 0;
				}
				else
				{
					return this.failures.Count;
				}
			}
		}

		public bool IsInconclusive
		{
			get { return this.inconclusive; }
		}

		public bool Completed
		{
			get 
			{
				switch ( this.Status )
				{
					case ExecutionStatus.Succeeded:
					case ExecutionStatus.SucceededWithInconclusiveParts:
					case ExecutionStatus.Failed:
					case ExecutionStatus.Inconclusive:
						return true;

					default:
						return false;
				}
			}
		}

		/*----------------------------------------------------------------------
		 * Overrides.
		 */

		public override ICollection<Failure> Failures
		{
			get { return this.failures; }
		}

		/*----------------------------------------------------------------------
		 * ICfixReportEventSink.
		 */

		public void BeforeChildThreadStart( uint threadId )
		{
			this.events.OnThreadStarted( this, threadId );
		}

		public void AfterChildThreadFinish( uint threadId )
		{
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
			this.inconclusive = true;
		}

		public void Log( string message, uint Reserved, ICfixStackTrace StackTrace )
		{
			this.events.OnLog( this, message );
		}

		public CFIXCTL_REPORT_DISPOSITION QueryDefaultFailedAssertionDisposition()
		{
			return ( CFIXCTL_REPORT_DISPOSITION )
				this.events.DispositionPolicy.DefaultFailedAssertionDisposition;
		}

		public CFIXCTL_REPORT_DISPOSITION QueryDefaultUnhandledExceptionDisposition()
		{
			return ( CFIXCTL_REPORT_DISPOSITION )
				this.events.DispositionPolicy.DefaultUnhandledExceptionDisposition;
		}

	}
}
