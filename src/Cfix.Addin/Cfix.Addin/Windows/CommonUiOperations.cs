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
using Cfix.Addin.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.VCCodeModel;
using Cfix.Addin.IntelParallelStudio;

namespace Cfix.Addin.Windows
{
	public enum RunMode
	{
		Normal,
		Debug
	}

	internal static class CommonUiOperations
	{
		private delegate void RunItemDelegate();

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

		public static void RunItemOnCommandLine( 
			Workspace ws, 
			ITestItem item,
			RunMode mode
			)
		{
			NativeTestItem nativeItem = item as NativeTestItem;

			if ( nativeItem == null )
			{
				VisualAssert.ShowInfo( Strings.EmptyRun );
			}
			else
			{
				ws.RunItemOnCommandLine( nativeItem, mode == RunMode.Debug );
			}
		}

#if INTELINSPECTOR
		public static void RunItemInIntelInspector(
			Workspace ws,
			ITestItem item,
			InspectorLevel level
			)
		{
			RunItem(
				ws,
				delegate()
				{
					IRunnableTestItem runItem;
					if ( item == null )
					{
						//
						// Rerun last.
						//
						runItem = null;
					}
					else
					{
						runItem = item as IRunnableTestItem;
					}

					ws.RunItemInIntelInspector( runItem, level );
				} );
		}
#endif
		
		public static void RunItem( 
			Workspace ws, 
			ITestItem item, 
			RunMode mode )
		{
			RunItem(
				ws,
				delegate() 
				{
					IRunnableTestItem runItem;
					if ( item == null )
					{
						//
						// Rerun last.
						//
						runItem = null;
					}
					else
					{
						runItem = item as IRunnableTestItem;
					}

					ws.RunItem( runItem, mode == RunMode.Debug );
				} );
		}

		private static void RunItem(
			Workspace ws,
			RunItemDelegate dlg )
		{
			try
			{
				dlg();
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

#if VS100
                CodeElements matches = codeModel.CodeElementFromFullName2( functionName );
#else
				CodeElements matches = codeModel.CodeElementFromFullName( functionName );
#endif
				if ( matches == null || matches.Count == 0 )
				{
					return false;
				}

                for (int i = 1; i <= matches.Count; i++)
                {
                    VCCodeFunction func = (VCCodeFunction)matches.Item(1);

                    TextPoint point = func.GetStartPoint(vsCMPart.vsCMPartHeader);

                    if (GoToSource(dte, func.File, (uint)point.Line))
                    {
                        return true;
                    }
                }

                //
                // None of the matches worked.
                //
                return false;
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
			RunMode mode
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
				else
				{
					Project prj = GetProjectFromCurrentHierarchyItem( dte );
					if ( prj != null )
					{
						testItem = VCProjectTestCollection.TryGetByName(
							prj.Name );
					}
				}

				if ( testItem != null )
				{
					RunItem( ws, testItem, mode );
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
			OpenBrowser( "http://www.visualassert.com/" );
		}

		public static void OpenBrowser( string url )
		{
			try
			{
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo.FileName = url;
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
            Array slnHierArray = ( Array ) slnHier.SelectedItems;
            if ( slnHierArray.GetLength( 0 ) == 0 )
            {
                return null;
            }

			UIHierarchyItem selected = ( UIHierarchyItem ) slnHierArray.GetValue( 0 );

			Project currentProject = selected.Object as Project;
			if ( currentProject != null )
			{
				return currentProject;
			}

			//
			// May be a folder or sub-project, then.
			//
			ProjectItem item = selected.Object as ProjectItem;
			if ( item == null )
			{
				return null;
			}

			currentProject = item.Object as Project;
			if ( currentProject != null )
			{
				//
				// Sub-project.
				//
				return currentProject;
			}

			//
			// Folder.
			//
			return item.ContainingProject;
		}

		public static void LaunchAddFixtureWizardForCurrentHierarchyItem( DTE2 dte )
		{
			try
			{
				Project currentProject = GetProjectFromCurrentHierarchyItem( dte );
				if ( currentProject != null && ProjectKinds.IsCppProjectKind( currentProject.Kind ) )
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
					 ProjectKinds.IsCppProjectKind( currentProject.Kind );
			}
			catch ( Exception )
			{
				return false;
			}
		}

		public static bool IsCurrentHierarchyItemSolution( DTE2 dte )
		{
			try
			{
				UIHierarchy slnHier = dte.ToolWindows.SolutionExplorer;
                Array slnHierArray = ( Array ) slnHier.SelectedItems;
                if ( slnHierArray.GetLength( 0 ) == 0 )
                {
                    return false;
                }

                UIHierarchyItem selected = ( UIHierarchyItem ) slnHierArray.GetValue( 0 );

				return selected.Object is Solution;
			}
			catch ( Exception )
			{
				return false;
			}
		}

		public static void ActivatePropertyWindow( DTE2 dte )
		{
			dte.ExecuteCommand( "View.PropertiesWindow", "" );
		}

        public static void SetActiveSelectionItem( Window window, object item )
        {
            //
            // Update property window. Unsuppotred on VS100 due to casting problems.
            //
#if !VS100
			try
			{
				object[] propObjects = new object[] { item };
				window.SetSelectionContainer( ref propObjects );
			}
			catch ( Exception )
			{
				//
				// Spurious E_FAIL exceptions.
				//
			}
#endif
        }
	}
}
