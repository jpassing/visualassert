using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public interface ISession : IDisposable
	{
		event EventHandler BeforeSetTests;
		event EventHandler AfterSetTests;

		ITestItemCollection Tests { get; set; }
	}
}
