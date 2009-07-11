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
		IHost CreateHost(
			string customHostPath,
			HostEnvironment env
			);
		ITestItemCollection LoadModule(
			HostEnvironment env,
			ITestItemCollection parentCollection,
			string customHostPath,
			string path,
			bool ignoreDuplicates
			);
		Architecture Architecture { get; }
		String ResolveMessage( int code );
		HostEnvironment DefaultEnvironment { get; }
		void SetTrialLicenseCookie( uint cookie );

		uint ActiveHostCount { get; }
		void TerminateActiveHosts();
	}
}
