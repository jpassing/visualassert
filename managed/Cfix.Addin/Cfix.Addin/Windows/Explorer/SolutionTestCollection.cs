using System;
using System.Diagnostics;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Addin.Windows.Explorer
{
	internal class SolutionTestCollection : GenericTestItemCollection
	{
		public event EventHandler Closed;

		private readonly Solution2 solution;
		private readonly SolutionEvents solutionEvents;
		private readonly MultiTarget target;

		private bool IsVcProject( Project prj )
		{
			return prj.Kind == ProjectKinds.VcProject;
		}

		private void AddProject( Project prj )
		{
			if ( IsVcProject( prj ) )
			{
				Add( new VCProjectTestCollection( prj, this.target ) );
			}
		}

		private void LoadProjects()
		{
			foreach ( Project project in this.solution.Projects )
			{
				AddProject( project );
			}
		}

		public SolutionTestCollection(
			Solution2 solution,
			MultiTarget target
			)
			: base( null, solution.FileName )
		{
			this.solution = solution;
			this.solutionEvents = solution.DTE.Events.SolutionEvents;
			this.target = target;

			this.solutionEvents.ProjectAdded += new _dispSolutionEvents_ProjectAddedEventHandler( solutionEvents_ProjectAdded );
			this.solutionEvents.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler( solutionEvents_ProjectRemoved );
			this.solutionEvents.ProjectRenamed += new _dispSolutionEvents_ProjectRenamedEventHandler( solutionEvents_ProjectRenamed );
			this.solutionEvents.AfterClosing += new _dispSolutionEvents_AfterClosingEventHandler( solutionEvents_AfterClosing );

			LoadProjects();
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
			if ( project != null && ! IsVcProject( project ) )
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
			//
			// Assume DTE notifications are reliable, so just refresh
			// children.
			//
			base.Refresh();
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public void RefreshProject( string name )
		{
			//
			// N.B. Assume project collection is small.
			//
			lock ( listLock )
			{
				foreach ( ITestItem item in this.list )
				{
					VCProjectTestCollection vcPrj =
						( VCProjectTestCollection ) item;
					if ( vcPrj.UniqueName == name )
					{
						vcPrj.Refresh();
						break;
					}
				}
			}
		}
	}
}
