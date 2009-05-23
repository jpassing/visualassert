using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfix.Control.Diag;
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
		  ICfixTestÌtemContainerEventSink
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
			try
			{
				OnFinished( ranToCompletionInt != 0 );
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}

		public ICfixTestÌtemEventSink GetTestItemEventSink( 
			uint testCaseOrdinal, 
			uint threadId 
			)
		{
			try
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
					return ( ICfixTestÌtemEventSink ) item;
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
				Debug.Assert( this.events != null );
				this.events.OnThreadStarted( this, threadId );
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
				Debug.Assert( this.events != null );
				this.events.OnThreadFinished( this, threadId );
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
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
			try
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
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
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
					StackTrace.Wrap( stackTrace ),
					lastError );

				AddFailure( fr );

				Debug.Assert( this.events != null );
				CFIXCTL_REPORT_DISPOSITION disp = ( CFIXCTL_REPORT_DISPOSITION )
					this.events.DispositionPolicy.FailedAssertion( fr );

				return disp;
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}

		public CFIXCTL_REPORT_DISPOSITION UnhandledException(
			uint exceptionCode,
			uint reserved,
			ICfixStackTrace stackTrace )
		{
			try
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
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}

		public void Inconclusive(
			string reason,
			uint reserved,
			ICfixStackTrace stackTrace
			)
		{
			try
			{
				AddFailure( new Inconclusiveness(
					reason,
					StackTrace.Wrap( stackTrace ) ) );
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
				Debug.Assert( this.events != null );
				this.events.OnLog( this, message );
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
				Debug.Assert( this.events != null );
				return ( CFIXCTL_REPORT_DISPOSITION )
					this.events.DispositionPolicy.DefaultFailedAssertionDisposition;
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
				Debug.Assert( this.events != null );
				return ( CFIXCTL_REPORT_DISPOSITION )
					this.events.DispositionPolicy.DefaultUnhandledExceptionDisposition;
			}
			catch ( Exception x )
			{
				Logger.LogError( "Sink", x );
				throw;
			}
		}
	}
}
