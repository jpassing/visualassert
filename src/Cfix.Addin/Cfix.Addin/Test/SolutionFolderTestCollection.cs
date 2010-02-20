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
				//
				// N.B. Under strange circumstances, Object can be 
				// null (#355).
				//
				if ( item.Object != null )
				{
					AddProject( ( Project ) item.Object );
				}
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
			Debug.Assert( ProjectKinds.IsSolutionFolderKind( slnFolder.Kind ) );

			if ( slnFolder.ProjectItems == null )
			{
				throw new ArgumentNullException();
			}

			LoadSubProjects( slnFolder.ProjectItems );
		}

	}
}
