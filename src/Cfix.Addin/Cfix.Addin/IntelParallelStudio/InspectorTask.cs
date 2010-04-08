using System;
using Cfix.Control.RunControl;
using Cfix.Control;
using System.Diagnostics;
using Cfix.Control.Native;
using System.IO;
using System.Threading;
using System.Collections.Generic;

#if INTELINSPECTOR
namespace Cfix.Addin.IntelParallelStudio
{
	/*++
		Single-test case task.
	--*/
	internal class InspectorTask : Task
	{
		private readonly IResultItem rootResult;
		private readonly bool filterCfixResults;
#if DEBUG
		private readonly Stopwatch watch = new Stopwatch();
#endif

		protected InspectorHostEnvironment InspectorHostEnvironment
		{
			get { return ( InspectorHostEnvironment ) this.Environment; }
		}

		protected ResultLocation ResultLocation
		{
			get
			{
				return ResultLocation.Create(
					InspectorHostEnvironment.Guid.ToString() );
			}
		}

		private InspectorResultFile LoadResults( 
			ResultLocation resultLocation 
			)
		{
			InspectorResultFile resultFile =
				InspectorResultFile.Load( resultLocation );

			if ( this.filterCfixResults )
			{
				//
				// Filter results before adding them to results window.
				//
				InspectorResultFilter filter = new InspectorResultFilter();
				resultFile = resultFile.Filter(
					( InspectorResultFile.FilterDelegate ) delegate( InspectorResult result )
					{
						return
							!filter.IsCfixResult( result ) &&
							!filter.EqualsLastResult( result );
					} );
			}

			return resultFile;
		}

		private uint GetProcessIdOfEnclosingPinProcess( ResultLocation loc )
		{
			//
			// The process if of the enclosing pin process is saved to a file
			// named <pid>.cpid.
			//
			// The actual file contents are irrelevant.
			//

			string dataDir = Path.Combine( loc.ResultDirectory, "data.0" );

			string[] cpidFiles = Directory.GetFiles( dataDir, "*.cpid" );
			string cpidFile = null;
			foreach ( string candidate in cpidFiles )
			{
				cpidFile = candidate;
				break;
			}

			if ( cpidFile == null )
			{
				throw new InspectorException( "No CPID file found" );
			}

			//
			// Extract basename.
			//
			string fileName = new FileInfo( cpidFile ).Name;
			fileName = fileName.Substring( 0, fileName.Length - 5 );

			return UInt32.Parse( fileName );
		}

		public InspectorTask( 
			IAgent agent,
			HostEnvironment hostEnv,
			InspectorLevel level,
			IAction action,
			bool filterCfixResults
			)
			: base( agent, new InspectorHostEnvironment( hostEnv, level ) )
		{
			Debug.Assert( !ReferenceEquals( action.Result.Item, action.Item ) );
			Debug.Assert( action.Result is IResultItemCollection );
			Debug.Assert( action.Item is TestCase );

			this.filterCfixResults = filterCfixResults;

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
			// N.B. It is crucial to wait until the pin process has exited -- 
			// otherwise, the result file may not have been written yet.
			//
			this.Finished += delegate( object sender, FinishedEventArgs e )
			{
				Debug.Assert( this.ProcessId != 0 );

				uint pid = GetProcessIdOfEnclosingPinProcess( ResultLocation );

				Process pin = Process.GetProcessById( ( int ) pid );
				pin.EnableRaisingEvents = true;
				pin.Exited += new EventHandler( pin_Exited );
			};

#if DEBUG
			this.watch.Start();
#endif
		}

		private void pin_Exited( object sender, EventArgs e )
		{
			if ( this.Status != TaskStatus.Suceeded )
			{
				return;
			}

			//
			// N.B. Result file is now available, but not finalized yet
			// (i.e. no symbols).
			//

			//
			// Parse result file (1st pass) to get number of failures.
			//
			try
			{
				InspectorResultFile resultFile = LoadResults( ResultLocation );

				if ( resultFile.Results.Count > 0 )
				{
					//
					// Before doing a second pass, incp-cl must have exited so
					// that the result file is guaranteed to have been finalized.
					//
					Process inspcl;
					try
					{
						inspcl = Process.GetProcessById( ( int ) this.ProcessId );
					}
					catch ( ArgumentException )
					{
						//
						// Process gone already.
						//
						inspcl = null;
					}

					this.rootResult.Object = ResultLocation;
					this.rootResult.Status = ExecutionStatus.PostprocessingFailed;
					this.rootResult.AddFailure( new ExpandoFailure(
						Strings.IssuesFoundByInspector,
						null,
						( ExpandoFailure.ExpandFailures ) delegate
						{
							if ( inspcl != null )
							{
								inspcl.WaitForExit();
							}

							//
							// Do second pass.
							//
							try
							{
								return SecondPass();
							}
							catch ( Exception x )
							{
								VisualAssert.HandleError( x );

								return new List<Failure>();
							}
						} ) );
				}
			}
			catch ( Exception x )
			{
				this.rootResult.AddFailure(
					new GenericError( x.Message, null ) );
			}

#if DEBUG
			this.watch.Stop();

			Debug.Print( "InspectorTask: " + this.watch.ElapsedMilliseconds + " ms elapsed" );
#endif
		}

		private IEnumerable<Failure> SecondPass()
		{
			InspectorResultFile resultFile2ndPass =
				LoadResults( ResultLocation );

			foreach ( InspectorResult result in resultFile2ndPass.Results )
			{
				if ( result.SourceFile == null && result.StackTrace == null )
				{
					// 
					// Pointless to show this node -- ignore.
					//
					continue;
				}

				switch ( result.Severity )
				{
					case InspectorResult.ResultSeverity.Information:
						yield return new GenericCodeInformation(
							result.Description ?? result.Type.ToString(),
							result.SourceFile,
							result.SourceLine,
							result.Function,
							result.StackTrace );
						break;

					case InspectorResult.ResultSeverity.Warning:
						yield return new GenericCodeWarning(
							result.Description ?? result.Type.ToString(),
							result.SourceFile,
							result.SourceLine,
							result.Function,
							result.StackTrace );
						break;

					case InspectorResult.ResultSeverity.Error:
						yield return new GenericCodeError(
							result.Description ?? result.Type.ToString(),
							result.SourceFile,
							result.SourceLine,
							result.Function,
							result.StackTrace );
						break;

					default:
						continue;
				}
			}
		}
	}
}
#endif