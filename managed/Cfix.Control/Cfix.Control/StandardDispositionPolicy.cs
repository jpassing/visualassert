using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public class StandardDispositionPolicy : IDispositionPolicy
	{
		private readonly Disposition defaultUnhandledExceptionDisp;
		private readonly Disposition defaultFailedAssertionDisp;

		public StandardDispositionPolicy(
			Disposition defaultUnhandledExceptionDisp,
			Disposition defaultFailedAssertionDisp
			)
		{
			this.defaultUnhandledExceptionDisp = defaultUnhandledExceptionDisp;
			this.defaultFailedAssertionDisp = defaultFailedAssertionDisp;
		}

		public Disposition FailedAssertion( Failure f )
		{
			return DefaultFailedAssertionDisposition;
		}

		public Disposition UnhandledException( Failure f )
		{
			return DefaultUnhandledExceptionDisposition;
		}

		public Disposition DefaultFailedAssertionDisposition
		{
			get { return this.defaultFailedAssertionDisp; }
		}

		public Disposition DefaultUnhandledExceptionDisposition
		{
			get { return this.defaultUnhandledExceptionDisp; }
		}
	}
}
