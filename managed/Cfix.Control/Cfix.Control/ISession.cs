using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public interface ISession : IDisposable
	{
		ITestItemCollection Tests { get; }
	}
}
