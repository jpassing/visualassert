using System;
using System.IO;
using System.Diagnostics;

namespace Cfix.Control
{
	public class EventDll
	{
		private readonly string path;
		private readonly string options;

		public EventDll( string path, string options )
		{
			this.path = path;
			this.options = options;
		}

		public string Path { get { return this.path; } }
		public string Options { get { return this.options; } }
	}

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
			bool userOnly
			);

		ITestItemCollection LoadModule(
			ITestItemCollection parentCollection,
			string path,
			bool ignoreDuplicates
			);

		void Terminate();
		string Path { get; }
		EventDll EventDll { get; }
	}
}
