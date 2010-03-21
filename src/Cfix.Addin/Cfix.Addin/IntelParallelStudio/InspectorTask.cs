using System;
using Cfix.Control.RunControl;
using Cfix.Control;
using System.Diagnostics;
using Cfix.Control.Native;

namespace Cfix.Addin.IntelParallelStudio
{
	/*++
		Single-test case task.
	--*/
	internal class InspectorTask : Task
	{
		private readonly IResultItem rootResult;

		public InspectorTask( 
			IAgent agent,
			HostEnvironment hostEnv,
			InspectorLevel level,
			IAction action
			)
			: base( agent, new InspectorHostEnvironment( hostEnv, level ) )
		{
			Debug.Assert( !ReferenceEquals( action.Result.Item, action.Item ) );
			Debug.Assert( action.Result is IResultItemCollection );
			Debug.Assert( action.Item is TestCase );

			//
			// N.B.: single test case is run in separate process, the 
			// result belongs to the item's parent.
			//

			IResultItemCollection parentResult = 
				( IResultItemCollection ) action.Result;
			if ( action.Item.Ordinal > 0 && parentResult.ItemCount == 1 )
			{
				//
				// Single-test case run.
				//
				this.rootResult = parentResult.GetItem( 0 );
			}
			else
			{
				this.rootResult = parentResult.GetItem( action.Item.Ordinal );
			}
				
			this.Finished += new EventHandler<FinishedEventArgs>( 
				InspectorTask_Finished );
		}

		private void InspectorTask_Finished( object sender, FinishedEventArgs e )
		{
			IResultItem result = this.rootResult;
			this.rootResult.AddFailure( new FailedAssertionFailure( "test", null, null, 0, null, null, 0 ) );
			this.rootResult.Status = ExecutionStatus.PostprocessingFailed;
		}
	}
}
