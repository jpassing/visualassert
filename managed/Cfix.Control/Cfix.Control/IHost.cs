using System;
using System.IO;

namespace Cfix.Control
{
	public interface IHost : IDisposable
	{
		uint ProcessId { get; }
		Architecture Architecture { get; }

		/*++
		 * Create a collection of test items for the given directory.
		 * The caller has to invoke Refresh() to actually load the
		 * children.
		 --*/
		ITestItemCollection SearchModules(
			DirectoryInfo dir,
			String filter,
			AgentSet runAgents,
			bool userOnly,
			bool ignoreDuplicates
			);

		ITestItemCollection LoadModule(
			ITestItemCollection parentCollection,
			string path,
			bool ignoreDuplicates
			);

		void Terminate();
	}
}
