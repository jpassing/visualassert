using System;
using Cfix.Control;
using EnvDTE;
using EnvDTE80;

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

		public static bool GoToSource(
			DTE2 dte,
			string file,
			uint line
			)
		{
			try
			{
				Window wnd = dte.ItemOperations.OpenFile( file, Constants.vsViewKindCode );
				if ( wnd != null )
				{
					TextSelection sel = dte.ActiveDocument.Selection as TextSelection;
					if ( sel != null )
					{
						sel.GotoLine( ( int ) line, false );
						return true;
					}
				}

				return false;
			}
			catch ( Exception )
			{
				//
				// File not found, ignore. 
				//
				return false;
			}
		}
	}
}
