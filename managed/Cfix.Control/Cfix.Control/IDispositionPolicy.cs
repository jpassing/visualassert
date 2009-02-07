using System;
using Cfixctl;

namespace Cfix.Control
{
	public enum Disposition
	{
		Continue = CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionContinue,
		Break = CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionBreak,
		BreakAlways = CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionBreakAlways,
		Abort = CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionAbort,
	}

	public interface IDispositionPolicy
	{
		Disposition FailedAssertion( Failure f );
		Disposition UnhandledException( Failure f );
		Disposition DefaultFailedAssertionDisposition { get; }
		Disposition DefaultUnhandledExceptionDisposition { get; }
	}
}
