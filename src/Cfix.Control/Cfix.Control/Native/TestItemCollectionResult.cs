using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control.Native
{
	/*++
	 * Class Description:
	 *		Represents the outcome of a composite ICfixTestItem 
	 *		(e.g. fixture or module).
	 * 
	 *		Threadsafe.
	 --*/
	internal class TestItemCollectionResult
		: GenericResultCollection,
		  IResultItemCollection, 
		  ICfixTest�temContainerEventSink
	{
		internal TestItemCollectionResult( 
			IActionEvents events,
			IResultItemCollection parent,
			ITestItemCollection itemCollection,
			ExecutionStatus status
			)
			: base( events, parent, itemCollection, status )
		{
		}

		internal TestItemCollectionResult(
			IActionEvents events,
			IResultItemCollection parent,
			ITestItemCollection itemCollection,
			IEnumerable<ITestItem> children,
			ExecutionStatus status
			)
			: base( events, parent, itemCollection, children, status )
		{
		}

		/*----------------------------------------------------------------------
		 * ICfixTest�temContainerEventSink.
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

		public ICfixTest�temEventSink GetTestItemEventSink( 
			uint testCaseOrdinal, 
			uint threadId 
			)
		{
			IResultItem item;
			if ( testCaseOrdinal < this.ItemCount )
			{
				item = this.GetItem( testCaseOrdinal );
			}
			else if ( this.ItemCount == 1 &&
				this.ItemCollection.ItemCount > 1 )
			{
				//
				// Mismatch between result item count and item count -- 
				// must be due to this being a single-testcase run.
				//
				item = this.GetItem( 0 );
			}
			else
			{
				throw new CfixException( "Inconsistent result" );
			}

			Debug.Assert( item is TestItemResult );

			if ( item == null )
			{
				throw new CfixException( "Unknown item" );
			}
			else
			{
				return ( ICfixTest�temEventSink ) item;
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

		public CFIXCTL_REPORT_DISPOSITION FailedAssertion(
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
			CFIXCTL_REPORT_DISPOSITION disp = ( CFIXCTL_REPORT_DISPOSITION )
				this.events.DispositionPolicy.FailedAssertion( ass );

			return disp;
		}

		public CFIXCTL_REPORT_DISPOSITION FailedRelateAssertion(
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
			CFIXCTL_REPORT_DISPOSITION disp = ( CFIXCTL_REPORT_DISPOSITION )
				this.events.DispositionPolicy.FailedAssertion( fr );

			return disp;
		}

		public CFIXCTL_REPORT_DISPOSITION UnhandledException(
			uint exceptionCode,
			uint reserved,
			ICfixStackTrace stackTrace )
		{
			UnhandledExceptionFailure u = new UnhandledExceptionFailure(
				exceptionCode,
				StackTrace.Wrap( stackTrace ) );

			AddFailure( u );

			Debug.Assert( this.events != null );
			CFIXCTL_REPORT_DISPOSITION disp = ( CFIXCTL_REPORT_DISPOSITION )
				this.events.DispositionPolicy.UnhandledException( u );

			return disp;
		}

		public void Inconclusive(
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