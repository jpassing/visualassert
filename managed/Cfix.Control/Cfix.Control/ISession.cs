using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public interface ISession : IDisposable
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1713:EventsShouldNotHaveBeforeOrAfterPrefix" )]
		event EventHandler BeforeSetTests;
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1713:EventsShouldNotHaveBeforeOrAfterPrefix" )]
		event EventHandler AfterSetTests;

		ITestItemCollection Tests { get; set; }

		IRun CreateRun(
			IDispositionPolicy policy,
			SchedulingOptions schedulingOptions
			);
	}
}
