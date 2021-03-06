using System;
using Cfixctl;

namespace Cfix.Control
{
	public interface IAction : IDisposable
	{
		ITestItem Item { get; }
		IResultItem Result { get; }
		void Run( IHost host );
		void Stop();
		uint ItemCount { get; }
		Architecture Architecture { get; }
		ModuleType ModuleType { get; }
		IHost CreateHost( IAgent agent, HostEnvironment env );
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
		void OnFailureOccured( IResultItem item );
	}
}
