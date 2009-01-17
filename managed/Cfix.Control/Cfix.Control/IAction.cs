using System;
using Cfixctl;

namespace Cfix.Control
{
	public enum SchedulingOptions
	{
		None = 0
	}

	public interface IAction : IDisposable
	{
		ITestItem TestItem { get; }
		void Run( ICfixEventSink sink );
		void Stop();
	}
}
