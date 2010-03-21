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

		public InspectorRunCompiler(
			AgentSet agentSet,
			IDispositionPolicy policy,
			ExecutionOptions executionOptions,
			EnvironmentOptions environmentOptions,
			InspectorLevel level
			)
			: base(
				agentSet,
				policy,
				executionOptions,
				environmentOptions )
		{
			this.level = level;
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
				action );
		}

		public override IRun Compile()
		{
			IRun run = base.Compile();
			run.InvolvesPostprocessing = true;
			return run;
		}
	}
}
