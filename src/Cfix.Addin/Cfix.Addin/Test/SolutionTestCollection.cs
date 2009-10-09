/*----------------------------------------------------------------------
 * Purpose:
 *		Solution.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Addin.Test
{
	internal class SolutionTestCollection
		: ProjectCollectionBase, IBuildableTestItem
	{
		public event EventHandler Closed;

		private readonly SolutionEvents solutionEvents;
		private static SolutionTestCollection loadedSolution;

		private void LoadProjects( Projects projects )
		{
			foreach ( Project project in projects )
			{
				AddProject( project );
			}
		}

		public SolutionTestCollection(
			Solution2 solution,
			AgentSet agentSet,
			Configuration config
			)
			: base( 
				null, 

				//
				// N.B. When a new solution is being created, solution.FullName
				// is, for whatever reason, the empty string.
				//
				String.IsNullOrEmpty( solution.FullName ) 
					? Strings.CurrentSolution 
					: new FileInfo( solution.FullName ).Name,
				solution,
				agentSet,
				config )
		{
			this.solutionEvents = solution.DTE.Events.SolutionEvents;

			this.solutionEvents.ProjectAdded += new _dispSolutionEvents_ProjectAddedEventHandler( solutionEvents_ProjectAdded );
			this.solutionEvents.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler( solutionEvents_ProjectRemoved );
			this.solutionEvents.ProjectRenamed += new _dispSolutionEvents_ProjectRenamedEventHandler( solutionEvents_ProjectRenamed );
			this.solutionEvents.AfterClosing += new _dispSolutionEvents_AfterClosingEventHandler( solutionEvents_AfterClosing );

			LoadProjects( solution.Projects  );
		}

		/*----------------------------------------------------------------------
		 * Events.
		 */

		private void solutionEvents_AfterClosing()
		{
			if ( this.Closed != null )
			{
				this.Closed( this, EventArgs.Empty );
			}
		}

		private void solutionEvents_ProjectAdded( 
			Project project 
			)
		{
			AddProject( project );
		}

		private void solutionEvents_ProjectRenamed( 
			Project project, 
			string oldName 
			)
		{
			if ( project != null && ! CanProjectBeAdded( project ) )
			{
				return;
			}

			lock ( listLock )
			{
				//
				// Rebuild list.
				//
				List<ITestItem> oldList = new List<ITestItem>( this.list );
				this.list.Clear();

				foreach ( ITestItem oldItem in oldList )
				{
					if ( oldItem.Name == oldName )
					{
						OnItemRemoved( oldItem );
						oldItem.Dispose();

						if ( project != null )
						{
							AddProject( project );
						}
					}
					else
					{
						this.list.Add( oldItem );
					}
				}
				Debug.Assert( ItemCount <= oldList.Count );
				Debug.Assert( ItemCount >= oldList.Count - 1 );
			}
		}

		private void solutionEvents_ProjectRemoved( 
			Project project 
			)
		{
			solutionEvents_ProjectRenamed( null, project.Name );
		}

		/*----------------------------------------------------------------------
		 * Overrides.
		 */

		protected override void Dispose( bool disposing )
		{
			if ( disposing )
			{
				this.solutionEvents.ProjectAdded -= new _dispSolutionEvents_ProjectAddedEventHandler( solutionEvents_ProjectAdded );
				this.solutionEvents.ProjectRemoved -= new _dispSolutionEvents_ProjectRemovedEventHandler( solutionEvents_ProjectRemoved );
				this.solutionEvents.ProjectRenamed -= new _dispSolutionEvents_ProjectRenamedEventHandler( solutionEvents_ProjectRenamed );
				this.solutionEvents.AfterClosing -= new _dispSolutionEvents_AfterClosingEventHandler( solutionEvents_AfterClosing );
			}

			base.Dispose( disposing );
		}

		public override void Refresh()
		{
			loadedSolution = null;

			//
			// Assume DTE notifications are reliable, so just refresh
			// children.
			//
			base.Refresh();

			loadedSolution = this;
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public static SolutionTestCollection TryGet()
		{
			return loadedSolution;
		}

		/*----------------------------------------------------------------------
		 * IBuildableTestItem.
		 */

		public bool Build()
		{
			SolutionBuild build = this.solution.SolutionBuild;
			build.Build( true );
			
			if ( build.BuildState != vsBuildState.vsBuildStateDone )
			{
				Debug.Fail( "Unexpected build state" );
				return false;
			}

			return build.LastBuildInfo == 0;
		}
	}
}
