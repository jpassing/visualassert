using System;
using System.Collections.Generic;
using Cfix.Control;
using Cfixctl;

namespace Cfix.Control.Native
{
	public class Run : AbstractRun
	{
		private readonly IResultItemCollection rootResult;

		public Run( 
			IDispositionPolicy policy,
			SchedulingOptions schedulingOptions,
			CompositionOptions compositionOptions,
			ITestItemCollection rootItem 
			)
			: base( 
				policy,
				schedulingOptions,
				compositionOptions,
				rootItem )
		{
			this.rootResult = ( IResultItemCollection ) rootItem.CreateResultItem(
					null, this, ExecutionStatus.Pending );
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		protected override void RunAction( IAction action )
		{
			action.Run();
		}

		public override IResultItemCollection RootResult
		{
			get { return this.rootResult; }
		}

	}
}
