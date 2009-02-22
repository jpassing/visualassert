using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal abstract class AbstractResultItem : IResultItem, ICfixReportEventSink
	{
		//
		// TestItem this result corresponds to - may or may not be
		// still available.
		//
		private ITestItem item;
		private volatile ExecutionStatus status;

		private readonly String itemName;
		protected readonly TestItemCollectionResult parent;

		private readonly Object failuresLock = new Object();
		private ICollection<Failure> failures;

		private volatile bool inconclusive;

		protected AbstractResultItem( 
			TestItemCollectionResult parent,
			ITestItem item,
			ExecutionStatus status
			)
		{
			this.parent = parent;
			this.status = status;
			this.itemName = item.Name;
			
			this.item = item;
			this.item.Disposed += new EventHandler( item_Disposed );
		}

		//private AbstractResultItem(
		//    TestItemCollectionResult parent,
		//    String itemName,
		//    ExecutionStatus status
		//    )
		//{
		//    this.parent = parent;
		//    this.status = status;
		//    this.itemName = itemName;
		//}

		private void item_Disposed( object sender, EventArgs e )
		{
			//
			// Remove own reference.
			//
			this.item = null;
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
				switch ( this.status )
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

		internal abstract Run InternalRun { get; }

		protected bool IsNative
		{
			get { return this.item is TestItem; }
		}

		/*----------------------------------------------------------------------
		 * IResultItem.
		 */

		public IResultItemCollection Parent
		{
			get { return this.parent; }
		}

		public ITestItem Item
		{
			get { return this.item; }
		}

		public ICollection<Failure> Failures
		{
			get { return this.failures; }
		}

		public string Name
		{
			get { return this.itemName; }
		}

		public ExecutionStatus Status
		{
			get { return this.status; }
			set 
			{
				if ( value != this.status )
				{
					this.status = value;
					this.InternalRun.OnStatusChanged( this );
				}
			}
		}

		public abstract IRun Run { get; }

		/*----------------------------------------------------------------------
		 * ICfixReportEventSink.
		 */

		public void BeforeChildThreadStart( uint threadId )
		{
			InternalRun.OnThreadStarted( this, threadId );
		}

		public void AfterChildThreadFinish( uint threadId )
		{
			InternalRun.OnThreadFinished( this, threadId );
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
				InternalRun.DispositionPolicy.FailedAssertion( ass );
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
				InternalRun.DispositionPolicy.FailedAssertion( fr );
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
				InternalRun.DispositionPolicy.UnhandledException( u );
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
			InternalRun.OnLog( this, message );
		}

		public CFIXCTL_REPORT_DISPOSITION QueryDefaultFailedAssertionDisposition()
		{
			return ( CFIXCTL_REPORT_DISPOSITION )
				InternalRun.DispositionPolicy.DefaultFailedAssertionDisposition;
		}

		public CFIXCTL_REPORT_DISPOSITION QueryDefaultUnhandledExceptionDisposition()
		{
			return ( CFIXCTL_REPORT_DISPOSITION )
				InternalRun.DispositionPolicy.DefaultUnhandledExceptionDisposition;
		}

	}
}
