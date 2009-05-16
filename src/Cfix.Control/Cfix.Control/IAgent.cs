using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public interface IAgent : IDisposable
	{
		IHost CreateHost();
		IHost CreateHost(
			HostEnvironment env
			);
		Architecture Architecture { get; }
		String ResolveMessage( int code );
		HostEnvironment DefaultEnvironment { get; }
		void SetTrialLicenseCookie( uint cookie );

		uint ActiveHostCount { get; }
		void TerminateActiveHosts();
	}
}
