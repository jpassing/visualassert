using System;
using System.Collections.Generic;
using System.Text;
using Cfix.Control.RunControl;
using Cfix.Control;

namespace Cfix.Addin.IntelParallelStudio
{
	internal class InspectorRunCompiler : ProcessPerTestRunCompiler
	{
		private readonly InspectorLevel level;
		private readonly bool filterCfixResults;

		public InspectorRunCompiler(
			AgentSet agentSet,
			IDispositionPolicy policy,
			ExecutionOptions executionOptions,
			EnvironmentOptions environmentOptions,
			InspectorLevel level,
			bool filterCfixResults
			)
			: base(
				agentSet,
				policy,
				executionOptions,
				environmentOptions )
		{
			this.level = level;
			this.filterCfixResults = filterCfixResults;
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		protected override Task CreateTask( IAction action )
		{
			return new InspectorTask(
				this.agentSet.GetAgent( action.Architecture ),
				this.Environment,
				this.level,
				action,
				this.filterCfixResults );
		}

		public override IRun Compile()
		{
			IRun run = base.Compile();
			run.InvolvesPostprocessing = true;
			return run;
		}
	}
}
