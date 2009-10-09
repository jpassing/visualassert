using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;
using Cfix.Control.Native;


namespace Cfix.Addin.Test
{
	internal abstract class ProjectCollectionBase : GenericTestItemCollection
	{
		protected readonly Solution2 solution;
		protected readonly AgentSet agentSet;
		protected readonly Configuration config;

		protected ProjectCollectionBase(
			ITestItemCollection parent,
			string name,
			Solution2 solution,
			AgentSet agentSet,
			Configuration config
			)
			: base( parent, name )
		{
			this.solution = solution;
			this.agentSet = agentSet;
			this.config = config;
		}

		protected void AddProject( Project prj )
		{
			switch ( prj.Kind )
			{
				case ProjectKinds.VcProject:
					Add( new VCProjectTestCollection(
						this,
						this.solution,
						prj,
						this.agentSet,
						this.config ) );
					break;

				case ProjectKinds.SolutionFolder:
					Add( new SolutionFolderTestCollection(
						this,
						prj,
						this.solution,
						this.agentSet,
						this.config ) );
					break;

				default:
					//
					// Ignore.
					//
					break;
			}
		}

		protected bool CanProjectBeAdded( Project prj )
		{
			switch ( prj.Kind )
			{
				case ProjectKinds.VcProject:
					return true;

				case ProjectKinds.SolutionFolder:
				   return true;

				default:
					return false;
			}
		}

		public void RefreshProject( string name )
		{
			//
			// N.B. Assume project collection is small.
			//
			// N.B. name may refer to a project located in a subfolder.
			//
			lock ( listLock )
			{
				foreach ( ITestItem item in this.list )
				{
					VCProjectTestCollection vcPrj =
						item as VCProjectTestCollection;
					if ( vcPrj != null && vcPrj.UniqueName == name )
					{
						vcPrj.Refresh();
						break;
					}

					ProjectCollectionBase folder =
						item as ProjectCollectionBase;
					if ( folder != null )
					{
						//
						// Recurse.
						//
						folder.RefreshProject( name );
					}
				}
			}
		}
	}
}
