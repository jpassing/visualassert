using System;
using Cfixctl;

namespace Cfix.Control
{
	public interface IAction : IDisposable
	{
		IHost Host { get; }
		ITestItem Item { get; }
		IResultItem Result { get; }
		void Run();
		void Stop();
	}

	public interface IActionEvents
	{
		IDispositionPolicy DispositionPolicy { get; }
		void OnNotification( IResultItem item, int hr );
		void OnHostSpawned( uint pid );
		void OnLog( IResultItem item, String message );
		void OnThreadStarted( IResultItem item, uint threadId );
		void OnThreadFinished( IResultItem item, uint threadId );
		void OnStatusChanged( IResultItem item );
	}
}
