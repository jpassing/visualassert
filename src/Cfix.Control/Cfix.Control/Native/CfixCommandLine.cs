using System;
using System.Text;

namespace Cfix.Control.Native
{
	public static class CfixCommandLine
	{
		public static string GetExecutableName( Architecture arch )
		{
			switch ( arch )
			{
				case Architecture.I386:
					return "cfix32.exe";

				case Architecture.Amd64:
					return "cfix64.exe";

				default:
					throw new ArgumentException();
			}
		}

		public static string CreateArguments(
			NativeTestItem item,
			ExecutionOptions executionOptions,
			IDispositionPolicy dispositionPolicy,
			bool summary,
			bool waitAtBegin,
			bool waitAtEnd
			)
		{
			StringBuilder cmdLine = new StringBuilder();

			TestModule module = item.Module;
			cmdLine.Append( ' ' );

			if ( dispositionPolicy.DefaultFailedAssertionDisposition == Disposition.BreakAlways )
			{
				cmdLine.Append( "-b " );
			}

			if ( dispositionPolicy.DefaultUnhandledExceptionDisposition == Disposition.Break )
			{
				cmdLine.Append( "-u " );
			}

			if ( ( executionOptions & ExecutionOptions.CaptureStackTraces ) == 0 )
			{
				cmdLine.Append( "-td " );
			}

			if ( ( executionOptions & ExecutionOptions.ShortCircuitRunOnFailure ) 
				== ExecutionOptions.ShortCircuitRunOnFailure )
			{
				cmdLine.Append( "-fsr " );
			}
			else if ( ( executionOptions & ExecutionOptions.ShortCircuitFixtureOnFailure ) 
				== ExecutionOptions.ShortCircuitFixtureOnFailure )
			{
				cmdLine.Append( "-fsf " );
			}
			else if ( ( executionOptions & ExecutionOptions.ShurtCircuitRunOnSetupFailure ) 
				== ExecutionOptions.ShurtCircuitRunOnSetupFailure )
			{
				cmdLine.Append( "-fss " );
			}

			if ( module.Type != ModuleType.UserEmbedded )
			{
				//
				// N.B. -y and -Y are not supported for exe modules.
				//

				if ( waitAtBegin )
				{
					cmdLine.Append( "-Y " );
				}

				if ( waitAtEnd )
				{
					cmdLine.Append( "-y " );
				}
			}

			if ( summary )
			{
				cmdLine.Append( "-z " );
			}

			switch ( module.Type )
			{
				case ModuleType.Kernel:
					cmdLine.Append( "-kern " );
					break;

				case ModuleType.UserEmbedded:
					cmdLine.Append( "-exe " );
					break;

				default: 
					break;
			}

			if ( item is TestModule )
			{
				//
				// No further args required.
				//
			}
			else if ( item is TestFixture )
			{
				cmdLine.Append( " -n " );
				cmdLine.Append( item.Name );
			}
			else if ( item is TestCase )
			{
				cmdLine.Append( " -n " );
				cmdLine.Append( item.Parent.Name );
				cmdLine.Append( '.' );
				cmdLine.Append( item.Name );
			}
			else
			{
				throw new ArgumentException( "Unsupported item type" );
			}

			cmdLine.Append( ' ' );

			cmdLine.Append( '\"' );
			cmdLine.Append( module.Path );
			cmdLine.Append( '\"' );

			return cmdLine.ToString();
		}
	}
}
