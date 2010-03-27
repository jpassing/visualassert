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

		protected InspectorHostEnvironment InspectorHostEnvironment
		{
			get { return ( InspectorHostEnvironment ) this.Environment; }
		}

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

			//
			// N.B. It is crucial to wait until the process has exited -- 
			// otherwise, the result file may not have been finalized yet.
			//
			this.Finished += delegate( object sender, FinishedEventArgs e )
			{
				Debug.Assert( this.ProcessId != 0 );

				Process process = Process.GetProcessById( ( int ) this.ProcessId );
				process.EnableRaisingEvents = true;
				process.Exited += new EventHandler( process_Exited );
			};
		}

		private void process_Exited( object sender, EventArgs e )
		{
			if ( this.Status != TaskStatus.Suceeded )
			{
				return;
			}

			//
			// Load inspector result file.
			//
			ResultLocation resultLocation = ResultLocation.Create(
				InspectorHostEnvironment.Guid.ToString() );

			try
			{
				InspectorResultFile resultFile =
					InspectorResultFile.Load( resultLocation );

				foreach ( InspectorResult result in resultFile.Results )
				{
					if ( result.SourceFile == null && result.StackTrace == null )
					{
						// 
						// Pointless to show this node -- ignore.
						//
						continue;
					}
					
					CodeFailure failure;
					switch ( result.Severity )
					{
						case InspectorResult.ResultSeverity.Information:
							failure = new GenericCodeInformation(
								result.Description ?? result.Type.ToString(),
								result.SourceFile,
								result.SourceLine,
								result.Function,
								result.StackTrace );
							break;

						case InspectorResult.ResultSeverity.Warning:
							failure = new GenericCodeWarning(
								result.Description ?? result.Type.ToString(),
								result.SourceFile,
								result.SourceLine,
								result.Function,
								result.StackTrace );
							break;

						case InspectorResult.ResultSeverity.Error:
							failure = new GenericCodeError(
								result.Description ?? result.Type.ToString(),
								result.SourceFile,
								result.SourceLine,
								result.Function,
								result.StackTrace );
							break;

						default:
							continue;
					}

					this.rootResult.Object = resultLocation;
					this.rootResult.AddFailure( failure );
				}
			}
			catch ( Exception x )
			{
				this.rootResult.AddFailure(
					new GenericError( x.Message, null ) );
			}
			
			this.rootResult.Status = ExecutionStatus.PostprocessingFailed;
		}
	}
}
