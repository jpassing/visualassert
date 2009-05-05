/*----------------------------------------------------------------------
 * Purpose:
 *		Helpers.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Diagnostics;
using System.IO;
using Cfix.Control;
using Cfix.Control.Native;
using Cfix.Addin.Test;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.VCCodeModel;

namespace Cfix.Addin.Windows
{
	internal static class CommonUiOperations
	{
		private static Project GetProject( ITestItem item )
		{
			ITestItem projectItem = item;
			while ( item != null )
			{
				VCProjectTestCollection projectColl = 
					item as VCProjectTestCollection;
				if ( projectColl != null )
				{
					return projectColl.Project;
				}
				else
				{
					item = item.Parent;
				}
			}
			
			return null;
		}

		public static void RunItem( Workspace ws, ITestItem item, bool debug )
		{
			try
			{
				if ( item == null )
				{
					//
					// Rerun last.
					//
					ws.RunItem( null, debug );
				}
				else
				{
					IRunnableTestItem runItem = item as IRunnableTestItem;
					if ( item != null )
					{
						ws.RunItem( runItem, debug );
					}
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
				//
				// N.B. VS cannot handle paths like foo\.\bar.c - therefore,
				// normalize first.
				//
				file = new FileInfo( file ).FullName;

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

		public static bool GoToTestItem(
			DTE2 dte,
			ITestItem item
			)
		{
			Debug.Assert( item != null );

			Project project = GetProject( item );
			if ( project == null )
			{
				//
				// No project, no code model.
				//
				return false;
			}

			string functionName = item.FullName;
			if ( functionName == null )
			{
				return false;
			}

			bool success = GoToFunction( dte, project, functionName );

			if ( !success && !( item is ITestItemCollection ) )
			{
				//
				// Jumping to tests does not work for C++/WinUnit,
				// try jumping to fixture.
				//
				return GoToTestItem( dte, item.Parent );
			}
			else
			{
				return success;
			}
		}

		public static bool GoToFunction(
			DTE2 dte,
			Project project,
			string functionName
			)
		{
			try
			{
				VCCodeModel codeModel = ( VCCodeModel ) project.CodeModel;
				CodeElements matches = codeModel.CodeElementFromFullName( functionName );
				if ( matches == null || matches.Count == 0 )
				{
					return false;
				}

				VCCodeFunction func = ( VCCodeFunction ) matches.Item( 1 );

				//TextPoint point = func.get_StartPointOf( 
				//    vsCMPart.vsCMPartWhole, 
				//    vsCMWhere.vsCMWhereDefinition );
				TextPoint point = func.GetStartPoint(vsCMPart.vsCMPartHeader);

				GoToSource( dte, func.File, ( uint ) point.Line );
				return true;
			}
			catch ( Exception )
			{
				//
				// Not found, ignore. 
				//
				return false;
			}
		}

		public static void RunSelectedUiHierarchyItem(
			DTE2 dte,
			Workspace ws,
			bool debug
			)
		{
			try
			{
				UIHierarchy slnHier = dte.ToolWindows.SolutionExplorer;
				UIHierarchyItem selected = ( UIHierarchyItem )
					( ( System.Array ) slnHier.SelectedItems ).GetValue( 0 );

				ITestItem testItem = null;

				if ( selected.Object is Solution )
				{
					//
					// N.B. There can be only one solution at a time.
					//
					testItem = SolutionTestCollection.TryGet();
				}
				else if ( selected.Object is Project )
				{
					testItem = VCProjectTestCollection.TryGetByName(
						selected.Name );
				}

				if ( testItem != null )
				{
					RunItem( ws, testItem, debug );
				}
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
			}
		}

		public static void OpenLameWebpage()
		{
			try
			{
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo.FileName = "http://www.cfix-studio.com/go/lame";
				proc.Start();
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
			}
		}
	}
}
