using System;
using Cfixctl;

namespace Cfix.Control.Test
{
	internal class DefaultEventSink : 
		ICfixEventSink, 
		ICfixProcessEventSink,
		ICfixTestÌtemContainerEventSink,
		ICfixTestÌtemEventSink
	{
		public uint Notifications;

		public uint TestCaseStarts;
		public uint TestCaseFinishs;
		public uint FixtureStarts;
		public uint FixtureFinishs;
		public uint ChildThreadStarts;
		public uint ChildThreadFinishs;

		public uint Logs;
		public uint Assertions;
		public uint Inconclusives;
		public uint Exceptions;
		public uint FailedRelates;

		public virtual ICfixProcessEventSink GetProcessEventSink( uint ProcessId )
		{
			return this;
		}

		public virtual ICfixTestÌtemContainerEventSink GetTestÌtemContainerEventSink(
			ICfixTestModule Module,
			uint FixtureOrdinal
			)
		{
			return this;
		}

		public virtual void Notification( int Hr )
		{
			this.Notifications++;
		}

		public virtual void AfterChildThreadFinish( uint ThreadId )
		{
			this.ChildThreadFinishs++;
		}

		public virtual void AfterFixtureFinish( int RanToCompletion )
		{
			this.FixtureFinishs++;
		}

		public virtual void BeforeChildThreadStart( uint ThreadId )
		{
			this.ChildThreadStarts++;
		}

		public virtual void BeforeFixtureStart()
		{
			this.FixtureStarts++;
		}

		public virtual void AfterTestCaseFinish( int RanToCompletion )
		{
			this.TestCaseFinishs++;
		}

		public virtual void BeforeTestCaseStart()
		{
			this.TestCaseStarts++;
		}

		public virtual CFIXCTL_REPORT_DISPOSITION FailedAssertion( string Expression, string Routine, string File, string Message, uint Line, uint LastError, uint Flags, uint Reserved, ICfixStackTrace StackTrace )
		{
			this.Assertions++;
			return CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionContinue;
		}

		public virtual CFIXCTL_REPORT_DISPOSITION FailedRelateAssertion( CFIXCTL_RELATE_OPERATOR Operator, object ExpectedValue, object ActualValue, string Routine, string File, string Message, uint Line, uint LastError, uint Flags, uint Reserved, ICfixStackTrace StackTrace )
		{
			this.FailedRelates++;
			return CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionContinue;
		}

		public virtual ICfixTestÌtemEventSink GetTestItemEventSink( uint TestCaseOrdinal, uint ThreadId )
		{
			return this;
		}

		public virtual void Inconclusive( string Message, uint Reserved, ICfixStackTrace StackTrace )
		{
			this.Inconclusives++;
		}

		public virtual void Log( string Message, uint Reserved, ICfixStackTrace StackTrace )
		{
			this.Logs++;
		}

		public virtual CFIXCTL_REPORT_DISPOSITION QueryDefaultFailedAssertionDisposition()
		{
			return CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionContinue;
		}

		public virtual CFIXCTL_REPORT_DISPOSITION QueryDefaultUnhandledExceptionDisposition()
		{
			return CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionContinue;
		}

		public virtual CFIXCTL_REPORT_DISPOSITION UnhandledException( uint ExceptionCode, uint Reserved, ICfixStackTrace StackTrace )
		{
			this.Exceptions++;
			return CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionContinue;
		}
	}
}
