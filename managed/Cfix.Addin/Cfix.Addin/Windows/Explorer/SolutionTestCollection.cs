using System;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;

namespace Cfix.Addin.Windows.Explorer
{
	internal class SolutionTestCollection : GenericTestItemCollection
	{
		private readonly Solution2 solution;
		private readonly SolutionEvents solutionEvents;

		private void LoadProjects()
		{
			foreach ( Project project in this.solution.Projects )
			{
				//TODO: filter VCprojs
				Add( new VCProjectTestCollection( project ) );
			}
		}

		public SolutionTestCollection(
			Solution2 solution
			)
			: base( null, solution.FileName )
		{
			this.solution = solution;
			this.solutionEvents = solution.DTE.Events.SolutionEvents;

			this.solutionEvents.ProjectAdded += new _dispSolutionEvents_ProjectAddedEventHandler( solutionEvents_ProjectAdded );
			this.solutionEvents.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler( solutionEvents_ProjectRemoved );
			this.solutionEvents.ProjectRenamed += new _dispSolutionEvents_ProjectRenamedEventHandler( solutionEvents_ProjectRenamed );

			LoadProjects();
		}

		/*----------------------------------------------------------------------
		 * Events.
		 */

		private void solutionEvents_ProjectAdded( Project Project )
		{
			Refresh();
		}

		private void solutionEvents_ProjectRenamed( Project Project, string OldName )
		{
			Refresh();
		}

		private void solutionEvents_ProjectRemoved( Project Project )
		{
			Refresh();
		}

		/*----------------------------------------------------------------------
		 * Overrides.
		 */

		protected override void Dispose( bool disposing )
		{
			this.solutionEvents.ProjectAdded -= new _dispSolutionEvents_ProjectAddedEventHandler( solutionEvents_ProjectAdded );
			this.solutionEvents.ProjectRemoved -= new _dispSolutionEvents_ProjectRemovedEventHandler( solutionEvents_ProjectRemoved );
			this.solutionEvents.ProjectRenamed -= new _dispSolutionEvents_ProjectRenamedEventHandler( solutionEvents_ProjectRenamed );

			base.Dispose( disposing );
		}

		public override void Refresh()
		{
			Clear();
			LoadProjects();
		}


	}
}
