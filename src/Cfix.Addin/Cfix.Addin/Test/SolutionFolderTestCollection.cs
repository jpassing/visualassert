using System;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Addin.Test
{
	internal class SolutionFolderTestCollection : ProjectCollectionBase
	{
		private void LoadSubProjects( ProjectItems projectItems )
		{
			foreach ( ProjectItem item in projectItems )
			{
				AddProject( ( Project ) item.Object );
			}
		}

		public SolutionFolderTestCollection(
			ITestItemCollection parent,
			Project slnFolder,
			Solution2 solution,
			AgentSet agentSet,
			Configuration config
			)
			: base(
				parent,
				slnFolder.Name,
				solution,
				agentSet,
				config )
		{
			Debug.Assert( slnFolder.Kind == ProjectKinds.SolutionFolder );
			
			LoadSubProjects( slnFolder.ProjectItems );
		}

	}
}
