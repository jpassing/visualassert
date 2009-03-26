using System;
using Cfix.Control;

namespace Cfix.Addin.Windows
{
	internal static class CommonUiOperations
	{
		public static void RunItem( Workspace ws, ITestItem item, bool debug )
		{
			try
			{
				IRunnableTestItem runItem = item as IRunnableTestItem;
				if ( item != null )
				{
					ws.RunItem( runItem, debug );
				}
			}
			catch ( ConcurrentRunException )
			{
				//
				// Ok, error dialog should have been provided.
				//
			}
			catch ( ArchitectureMismatchException )
			{
				CfixPlus.ShowInfo( Strings.ArchitectureMismatch );
			}
			catch ( EmptyRunException )
			{
				CfixPlus.ShowInfo( Strings.EmptyRun );
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
			}
		}

		
	}
}
