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
			catch ( IncompatibleModulesException )
			{
				VisualAssert.ShowInfo( Strings.IncompatibleModule );
			}
			catch ( EmptyRunException )
			{
				VisualAssert.ShowInfo( Strings.EmptyRun );
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
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

			ITestCodeElement codeElem = item as ITestCodeElement;
			if ( codeElem == null )
			{
				return false;
			}

			return GoToFunction( dte, project, codeElem.CodeElementName );
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
				if ( !ws.ToolWindows.IsExplorerLoaded )
				{
					//
					// Explorer not loaded -- this means two things:
					// (1) The session probably has not been loaded
					// yet and (2) we cannot be sure that we are in
					// Solution mode.
					//
					// Escape by activating the Explorer.
					//
					ws.ToolWindows.Explorer.Activate();
					return;
				}

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
				VisualAssert.HandleError( x );
			}
		}

		public static void OpenLameWebpage( DTE2 dte, string windowName )
		{
			try
			{
				string url = string.Format(
					"https://cfix.fogbugz.com/default.asp?pg=pgPublicEdit&ixArea=16" +
					"&sEvent=%0a%0aOS%20Version:%20{0}%0aVS%20Version:%20{1}%0a"+
					"VS%20Locale:%20%20{4}%0aWindow:" +
					"%20%20%20%20%20{2}&sVersion={3}",
					Environment.OSVersion,
					dte.Version,
					windowName,
					VisualAssert.Version,
					dte.LocaleID );

				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo.FileName = url;
				proc.Start();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		public static void OpenHomepage()
		{
			try
			{
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo.FileName = "http://www.visualassert.com/";
				proc.Start();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		public static void OpenDocumentation()
		{
			try
			{
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo.FileName = Directories.DocDirectory + "\\VisualAssert.chm";
				proc.Start();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		public static void OpenQuickStartPage( DTE2 dte )
		{
			try
			{
				dte.ItemOperations.Navigate(
					Directories.QuickStartPage,
					vsNavigateOptions.vsNavigateOptionsDefault );
			}
			catch
			{ }
		}

		private static Project GetProjectFromCurrentHierarchyItem( DTE2 dte )
		{
			UIHierarchy slnHier = dte.ToolWindows.SolutionExplorer;
			UIHierarchyItem selected = ( UIHierarchyItem )
				( ( System.Array ) slnHier.SelectedItems ).GetValue( 0 );

			Project currentProject = selected.Object as Project;
			if ( currentProject == null )
			{
				//
				// Should be a folder, then.
				//
				ProjectItem item = selected.Object as ProjectItem;
				if ( item == null )
				{
					return null;
				}

				currentProject = item.ContainingProject;
			}

			return currentProject;
		}

		public static void LaunchAddFixtureWizardForCurrentHierarchyItem( DTE2 dte )
		{
			try
			{
				Project currentProject = GetProjectFromCurrentHierarchyItem( dte );
				if ( currentProject != null && currentProject.Kind == ProjectKinds.VcProject )
				{
					Wizards.LaunchAddFixtureWizard( dte, currentProject );
				}
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		public static bool IsCurrentHierarchyItemPartOfVcProject( DTE2 dte )
		{
			try
			{
				Project currentProject = GetProjectFromCurrentHierarchyItem( dte );
				return currentProject != null &&
					currentProject.Kind == ProjectKinds.VcProject;
			}
			catch ( Exception )
			{
				return false;
			}
		}
	}
}
