/*----------------------------------------------------------------------
 * Purpose:
 *		Test Explorer.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Cfix.Addin.Dte;
using Cfix.Addin.ShellBrowse;
using Cfix.Addin.Test;
using Cfix.Addin.Windows;
using Cfix.Control;
using Cfix.Control.Ui.Explorer;
using Cfix.Control.Native;
using Cfix.Addin.IntelParallelStudio;
using Cfix.Control.RunControl;

namespace Cfix.Addin.Windows
{
	public partial class ExplorerWindow : UserControl, IDteToolWindowControl
	{
		public static readonly Guid Guid = new Guid( "e89c09c9-4e89-4ae2-b328-79dcbdfd852c" );

		private Workspace workspace;
		private DTE2 dte;
		private Window window;

		private SolutionEvents solutionEvents;
		private BuildEvents buildEvents;

		private AbstractExplorerNode contextMenuReferenceNode;

		public ExplorerWindow()
		{
			InitializeComponent();

#if VS100
            this.BackColor = Chrome.WindowBackColor;
            this.toolbar.BackColor = Chrome.WindowBackColor;
#endif

			//
			// Switch NodeFactory s.t. we can use our own nodes.
			//
			this.explorer.NodeFactory = new VSNodeFactory( this.explorer );

			this.workspace = null;

			this.selectModeButton.DropDownOpening += new EventHandler( selectModeButton_DropDownOpening );
			
			this.explorer.NodeContextMenu = this.ctxMenu;
			this.ctxMenuRefreshButton.Click += new EventHandler( ctxMenuRefreshButton_Click );

			this.Disposed += new EventHandler( ExplorerWindow_Disposed );

#if !BETA
			this.lameButton.Visible = false;
#endif
		}

		public void Initialize( 
			Workspace ws, 
			DTE2 dte,
			Window window )
		{
			Debug.Assert( this.workspace == null );
			Debug.Assert( this.dte == null );
			
			this.workspace = ws;
			this.dte = dte;
			this.window = window;

			this.solutionEvents = dte.Events.SolutionEvents;
			this.buildEvents = dte.Events.BuildEvents;

			this.explorer.SetSession( ws.Session, false );
			this.explorer.ExceptionRaised += new EventHandler<ExceptionEventArgs>( explorer_ExceptionRaised );
			this.explorer.RefreshStarted += new EventHandler( explorer_RefreshStarted );
			this.explorer.RefreshFinished += new EventHandler( explorer_RefreshFinished );
			this.explorer.AfterSelected += new EventHandler<ExplorerNodeEventArgs>( explorer_AfterSelected );
			this.explorer.BeforeContextMenuPopup += new EventHandler<ExplorerNodeEventArgs>( explorer_BeforeContextMenuPopup );
			this.explorer.TreeKeyDown +=new KeyEventHandler( explorer_TreeKeyDown );
			this.explorer.TreeDoubleClick += new TreeNodeMouseClickEventHandler( explorer_TreeDoubleClick );
			this.solutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler( solutionEvents_Opened );
			this.solutionEvents.BeforeClosing += new _dispSolutionEvents_BeforeClosingEventHandler( solutionEvents_BeforeClosing );
			this.buildEvents.OnBuildProjConfigDone += new _dispBuildEvents_OnBuildProjConfigDoneEventHandler( buildEvents_OnBuildProjConfigDone );
			this.buildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler( buildEvents_OnBuildDone );

			Configuration config = this.workspace.Configuration;
			this.refreshAfterBuildToolStripMenuItem.Checked = config.AutoRefreshAfterBuild;
			
			ExecutionOptions executionOpts = config.ExecutionOptions;

			//
			// N.B. Complex checks due to inclusive bitmaps.
			//
			this.shortCircuitFixtureOnFailureMenuItem.Checked =
				( executionOpts & ( ExecutionOptions.ShortCircuitFixtureOnFailure | ExecutionOptions.ShortCircuitRunOnFailure ) )
					== ExecutionOptions.ShortCircuitFixtureOnFailure;
			this.shortCircuitRunOnFailureMenuItem.Checked =
				( executionOpts & ( ExecutionOptions.ShortCircuitFixtureOnFailure | ExecutionOptions.ShortCircuitRunOnFailure ) )
					== ExecutionOptions.ShortCircuitRunOnFailure;
			
			this.captureStackTracesMenuItem.Checked =
				( executionOpts & ExecutionOptions.CaptureStackTraces ) != 0;
			this.breakOnFailedAssertionsWhenDebuggingMenuItem.Checked =
				( config.DefaultDebugFailedAssertionDisposition == Disposition.Break );
			this.breakOnUnhandledExceptionsWhenDebuggingMenuItem.Checked =
				( config.DefaultDebugUnhandledExceptionDisposition == Disposition.Break );

#if ! DEBUG
			this.runEachTestInSeparateProcessMenuItem.Visible = false;
#else
			this.runEachTestInSeparateProcessMenuItem.Visible = true;
			this.runEachTestInSeparateProcessMenuItem.Checked =
				this.workspace.Configuration.RunCompilerType == RunCompilerType.ProcessPerTest;
#endif

#if INTELINSPECTOR
			SetInspectorMiSelection( config.MostRecentlyUsedInspectorMemoryAnalysisLevel );
			SetInspectorTiSelection( config.MostRecentlyUsedInspectorThreadingAnalysisLevel );
			
#else
			this.intelInspectorSeparator.Visible = false;
			this.intelInspectorMemoryErrorsAnalysisLevelToolStripMenuItem.Visible = false;
			this.intelInspectorThreadingErrorAnalysisLevelToolStripMenuItem.Visible = false;
#endif

			SetInfoPanel();
		}

#if INTELINSPECTOR
		private void SetInspectorMiSelection( InspectorLevel level )
		{
			this.intelInspectorMi1MenuItem.Checked = level == InspectorLevel.CheckMemoryLeaks;
			this.intelInspectorMi2MenuItem.Checked = level == InspectorLevel.CheckMemoryAccessIssues;
			this.intelInspectorMi3MenuItem.Checked = level == InspectorLevel.LocateMemoryAccessIssues;
			this.intelInspectorMi4MenuItem.Checked = level == InspectorLevel.AllMemoryIssues;
		}

		private void SetInspectorTiSelection( InspectorLevel level )
		{
			this.intelInspectorTi1MenuItem.Checked = level == InspectorLevel.CheckDeadlocks;
			this.intelInspectorTi2MenuItem.Checked = level == InspectorLevel.CheckDeadlocksAndRaces;
			this.intelInspectorTi3MenuItem.Checked = level == InspectorLevel.LocateDeadlocksAndRaces;
			this.intelInspectorTi4MenuItem.Checked = level == InspectorLevel.AllThreadingIssues;
		}
#endif		

		private void UpdateExecutionOptions()
		{
			ExecutionOptions options = ExecutionOptions.None;
			if ( this.shortCircuitFixtureOnFailureMenuItem.Checked )
			{
				options |= ExecutionOptions.ShortCircuitFixtureOnFailure;
			}

			if ( this.shortCircuitRunOnFailureMenuItem.Checked )
			{
				options |= ExecutionOptions.ShortCircuitRunOnFailure;
			}

			if ( this.captureStackTracesMenuItem.Checked )
			{
				options |= ExecutionOptions.CaptureStackTraces;
			}

			this.workspace.Configuration.ExecutionOptions = options;
		}

		/*----------------------------------------------------------------------
		 * Status/Info panel.
		 */

		private void SetInfoPanel( string text, Image icon )
		{
			this.infoIcon.Image = icon;
			this.infoLabel.Text = text;

			this.infoBar.Invalidate();
			this.infoLabel.Invalidate();
			this.infoIcon.Invalidate();
		}

		private void SetInfoPanel( string text )
		{
			SetInfoPanel( text, Icons.Information );
		}

		private void SetInfoPanel()
		{
			if ( this.explorer.Session.Tests != null )
			{
				IRunnableTestItemCollection runnableColl =
					this.explorer.Session.Tests as IRunnableTestItemCollection;

				uint testCount;
				if ( runnableColl != null && ( testCount = runnableColl.RunnableItemCountRecursive ) > 0 )
				{
					SetInfoPanel( String.Format(
						Strings.DefaultInfoText,
						testCount ) );
				}
				else
				{
					SetInfoPanel( Strings.NoTestsAvailable );
				}
			}
			else
			{
				SetInfoPanel( Strings.NoSlnLoaded );
			}
		}

		/*----------------------------------------------------------------------
		 * Context Menu.
		 */

		private static bool IsItemPartOfIcProject( ITestItem item )
		{
			while ( item != null )
			{
				ICProjectTestCollection icProject = item as ICProjectTestCollection;
				if ( icProject != null )
				{
					return true;
				}

				item = item.Parent;
			}

			return false;
		}

		private void explorer_BeforeContextMenuPopup( 
			object sender, 
			ExplorerNodeEventArgs e )
		{
			bool runnable = e.Item is IRunnableTestItem;

			this.ctxMenuDebugButton.Enabled = runnable;
			this.ctxMenuRunButton.Enabled = runnable;

			this.ctxMenuRefreshButton.Visible = e.Item is ITestItemCollection;
			this.ctxMenuViewCodeButton.Visible = e.Item is ITestCodeElement;
			this.ctxMenuRunInConsole.Visible = e.Item is NativeTestItem;
			this.ctxMenuDebugWithWindbg.Visible = e.Item is NativeTestItem;

#if INTELINSPECTOR
			this.ctxMenuRunWithInspectorMi.Enabled = this.workspace.IntelInspector != null;
			this.ctxMenuRunWithInspectorTi.Enabled = this.workspace.IntelInspector != null;
#else
			this.ctxMenuRunWithInspectorMi.Visible = false;
			this.ctxMenuRunWithInspectorTi.Visible = false;
#endif

			bool showAddFixture = Wizards.CanAddFixture( e.Item );
			this.ctxMenuAddFixtureButton.Visible = showAddFixture;
			this.ctxMenuSeparator1.Visible = showAddFixture;

			//
			// Remember node to associate menu item clicks with
			// this node.
			//
			this.contextMenuReferenceNode = e.Node;
		}

		private void ctxMenuRefreshButton_Click( object sender, EventArgs e )
		{
			if ( this.contextMenuReferenceNode != null )
			{
				try
				{
					this.explorer.RefreshSession(
						true,
						this.contextMenuReferenceNode.Item );
				}
				catch ( Exception x )
				{
					VisualAssert.HandleError( x );
				}
			}
		}

		
		private void ctxMenuAddFixtureButton_Click( object sender, EventArgs e )
		{
			try
			{
				Wizards.LaunchAddFixtureWizard(
					this.dte,
					this.contextMenuReferenceNode.Item );
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void ctxMenuViewCodeButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.GoToTestItem( this.dte, this.contextMenuReferenceNode.Item );			
		}


		private void ctxMenuViewProperties_Click( object sender, EventArgs e )
		{
            CommonUiOperations.SetActiveSelectionItem( 
                this.window, 
                this.contextMenuReferenceNode.Item);
			CommonUiOperations.ActivatePropertyWindow( this.dte );
		}

		/*----------------------------------------------------------------------
		 * Refreshing.
		 */

		private void UpdateRefreshButtonStatus()
		{
			ITestItem selItem = this.explorer.SelectedItem;
			bool enable = ( selItem == null || selItem is ITestItemCollection );
			this.refreshButton.Enabled = enable;
		}

		private void DisableRefresh()
		{
			this.refreshButton.Enabled = false;
			this.abortRefreshButton.Enabled = false;
		}

		private void explorer_AfterSelected( object sender, ExplorerNodeEventArgs e )
		{
			bool runnable = e.Item is IRunnableTestItem;

			this.debugButton.Enabled = runnable;
			this.runButton.Enabled = runnable;

			UpdateRefreshButtonStatus();

			CommonUiOperations.SetActiveSelectionItem( this.window, e.Item );
		}

		private void explorer_RefreshFinished( object sender, EventArgs e )
		{
			this.selectModeButton.Enabled = true;

			SetInfoPanel();

			UpdateRefreshButtonStatus();
			this.abortRefreshButton.Enabled = false;
		}

		private void explorer_RefreshStarted( object sender, EventArgs e )
		{
			this.selectModeButton.Enabled = false;

			SetInfoPanel( Strings.Searching, Icons.Throb );

			DisableRefresh();
			this.abortRefreshButton.Enabled = true;
		}

		private void refreshButton_Click( object sender, EventArgs e )
		{
			try
			{
				this.explorer.RefreshSession( true, this.explorer.SelectedItem );
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void abortRefreshButton_Click( object sender, EventArgs e )
		{
			try
			{
				this.explorer.AbortRefreshSession();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void buildEvents_OnBuildProjConfigDone( 
			string project, 
			string projectConfig, 
			string platform, 
			string solutionConfig, 
			bool success )
		{
			SolutionTestCollection slnColl =
				this.explorer.Session.Tests as SolutionTestCollection;
			if ( !success || 
				 !this.refreshAfterBuildToolStripMenuItem.Checked ||
				 slnColl == null )
			{
				//
				// N.B. We do not refresh in case we are in directory mode
				// as this would lead to redundant refreshed in case the
				// SLN has more than one project that is being built.
				//
				return;
			}

			//
			// Project build done, refresh it.
			//
			try
			{
				slnColl.RefreshProject( project );

				SetInfoPanel();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void buildEvents_OnBuildDone( vsBuildScope Scope, vsBuildAction Action )
		{
			ITestItemCollection coll = this.explorer.Session.Tests;
			SolutionTestCollection slnColl = coll as SolutionTestCollection;
			if ( !this.refreshAfterBuildToolStripMenuItem.Checked ||
				 coll == null ||
				 slnColl != null )
			{
				return;
			}

			try
			{
				this.explorer.RefreshSession( true );
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void refreshAfterBuildToolStripMenuItem_Click( object sender, EventArgs e )
		{
			try
			{
				this.workspace.Configuration.AutoRefreshAfterBuild = this.refreshAfterBuildToolStripMenuItem.Checked;
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		/*----------------------------------------------------------------------
		 * Various events.
		 */

		private void solutionEvents_Opened()
		{
			try
			{
				if ( this.workspace.Session.Tests == null )
				{
					ExploreSolution();
				}
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void solutionEvents_BeforeClosing()
		{
			DisableRefresh();

			this.debugButton.Enabled = false;
			this.runButton.Enabled = false;
		}

		private void ExplorerWindow_Disposed( object sender, EventArgs e )
		{
			this.solutionEvents.Opened -= new _dispSolutionEvents_OpenedEventHandler( solutionEvents_Opened );
			this.solutionEvents.BeforeClosing -= new _dispSolutionEvents_BeforeClosingEventHandler( solutionEvents_BeforeClosing );
			
			this.buildEvents.OnBuildProjConfigDone -= new _dispBuildEvents_OnBuildProjConfigDoneEventHandler( buildEvents_OnBuildProjConfigDone );
			this.buildEvents.OnBuildDone -= new _dispBuildEvents_OnBuildDoneEventHandler( buildEvents_OnBuildDone );
		
		}

		private void selectModeButton_DropDownOpening( object sender, EventArgs e )
		{
			try
			{
				//
				// Enable button only when a solution is open.
				//
				this.selectSlnModeButton.Enabled =
					this.workspace.IsSolutionOpened;
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void statusText_GotFocus( object sender, EventArgs e )
		{
			this.explorer.Focus();
		}
		
		private void explorer_ExceptionRaised( 
			object sender, 
			Cfix.Control.Ui.Explorer.ExceptionEventArgs e 
			)
		{
			VisualAssert.HandleError( e.Exception );
		}

		/*----------------------------------------------------------------------
		 * Session updating.
		 */

		private void ExploreDirectoryOrFile( String path )
		{
			Debug.Assert( this.workspace != null );

			DisableRefresh();
			this.refreshAfterBuildToolStripMenuItem.Enabled = true;

			DirectoryInfo dir;
			String filter;

			if ( Directory.Exists( path ) )
			{
				dir = new DirectoryInfo( path );
				filter = "*";
			}
			else
			{
				FileInfo info = new FileInfo( path );
				dir = info.Directory;
				filter = info.Name;
			}

			Debug.Print( "Search for " + filter + " in " + dir );

			using ( IHost host = this.workspace.SearchAgent.CreateHost() )
			{
				this.workspace.Session.Tests = host.SearchModules(
					dir,
					filter,
					this.workspace.RunAgent,
					! this.workspace.Configuration.KernelModeFeaturesEnabled );
			}
		}

		public void ExploreSolution()
		{
			Debug.Assert( this.workspace != null );
			if ( !this.workspace.ToolWindows.Explorer.Visible )
			{
				//
				// Continueing would not only be futile, it would also
				// cause problems as the controls may not be available
				// yet.
				//
				return;
			}

			DisableRefresh();
			this.refreshAfterBuildToolStripMenuItem.Enabled = true;

			Solution curSolution = this.dte.Solution;
			if ( curSolution != null && curSolution.Projects.Count > 0 )
			{
				SolutionTestCollection slnCollection = new SolutionTestCollection(
					( Solution2 ) curSolution,
					this.workspace.RunAgent,
					this.workspace.Configuration );
				slnCollection.Closed += new EventHandler( slnCollection_Closed );
				this.workspace.Session.Tests = slnCollection;
			}
		}

		private void slnCollection_Closed( object sender, EventArgs e )
		{
			this.workspace.Session.Tests = null;
			SetInfoPanel();
		}

		/*----------------------------------------------------------------------
		 * File/Folder mode events.
		 */

		private void selectDirModeButton_Click( object sender, EventArgs e )
		{
			try
			{
				ShellBrowseForFolderDialog dialog = new ShellBrowseForFolderDialog();
				dialog.hwndOwner = this.Handle;

				dialog.Filter = new FilterByExtension( 
					this.workspace.Configuration.SupportedExtensions );
				dialog.DetailsFlags = 
					ShellBrowseForFolderDialog.BrowseInfoFlag.BIF_NONEWFOLDERBUTTON |
					ShellBrowseForFolderDialog.BrowseInfoFlag.BIF_BROWSEINCLUDEFILES |
					ShellBrowseForFolderDialog.BrowseInfoFlag.BIF_NEWDIALOGSTYLE |
					ShellBrowseForFolderDialog.BrowseInfoFlag.BIF_STATUSTEXT |
					ShellBrowseForFolderDialog.BrowseInfoFlag.BIF_USENEWUI |
					ShellBrowseForFolderDialog.BrowseInfoFlag.BIF_VALIDATE;
				dialog.OnSelChanged +=
					new ShellBrowseForFolderDialog.SelChangedHandler(dialog_OnSelChanged);
				dialog.OnInitialized += new ShellBrowseForFolderDialog.InitializedHandler( dialog_OnInitialized );

				dialog.Title = Strings.SelectFileOrFolder;
				dialog.ShowDialog();
				
				if ( ! dialog.Canceled )
				{
					string directoryPath = dialog.FullName;
					this.workspace.Configuration.MostRecentlyUsedDirectory = directoryPath;
					ExploreDirectoryOrFile( directoryPath );
				}
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void dialog_OnInitialized( ShellBrowseForFolderDialog sender, ShellBrowseForFolderDialog.InitializedEventArgs args )
		{
			string mruDirectory = this.workspace.Configuration.MostRecentlyUsedDirectory;
			if ( mruDirectory != null )
			{
				sender.SetSelection( args.hwnd, mruDirectory );
			}
		}

		private void dialog_OnSelChanged(
			ShellBrowseForFolderDialog sender, 
			ShellBrowseForFolderDialog.SelChangedEventArgs args
			)
		{
			try
			{
				//
				// Filter stuff that the built-in filtering ignores.
				//
				string path;
				IShellFolder isf = ShellFunctions.GetDesktopFolder();
				ShellApi.STRRET ptrDisplayName;
				isf.GetDisplayNameOf( 
					args.pidl, 
					( uint ) ShellApi.SHGNO.SHGDN_NORMAL | ( uint ) ShellApi.SHGNO.SHGDN_FORPARSING, 
					out ptrDisplayName );
				ShellApi.StrRetToBSTR( ref ptrDisplayName, ( IntPtr ) 0, out path );

				if ( Directory.Exists( path ) )
				{
					//
					// Ok.
					//
					sender.EnableOk( args.hwnd, true );
				}
				else
				{
					//
					// Re-check extension.
					//
					sender.EnableOk(
						args.hwnd,
						this.workspace.Configuration.IsSupportedTestModulePath( path ) );
				}
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		/*----------------------------------------------------------------------
		 * Solution mode events.
		 */

		private void selectSlnModeButton_Click( object sender, EventArgs e )
		{
			try
			{
				ExploreSolution();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		/*----------------------------------------------------------------------
		 * Run/Debug.
		 */

		private void debugButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.RunItem( 
				this.workspace,
				this.explorer.SelectedItem, 
				RunMode.Debug );
		}

		private void runButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.RunItem(
				this.workspace,
				this.explorer.SelectedItem,
				RunMode.Normal );
		}

		private void ctxMenuDebugButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.RunItem(
				this.workspace,
				this.contextMenuReferenceNode.Item,
				RunMode.Debug );
		}

		private void ctxMenuRunButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.RunItem(
				this.workspace,
				this.contextMenuReferenceNode.Item,
				RunMode.Normal );
		}


		private void ctxMenuRunOnConsole_Click( object sender, EventArgs e )
		{
			CommonUiOperations.RunItemOnCommandLine(
				this.workspace,
				this.contextMenuReferenceNode.Item,
				RunMode.Normal );
		}

		private void ctxMenuDebugWithWindbg_Click( object sender, EventArgs e )
		{
			CommonUiOperations.RunItemOnCommandLine(
				this.workspace,
				this.contextMenuReferenceNode.Item,
				RunMode.Debug );
		}

		private void explorer_TreeKeyDown( object sender, KeyEventArgs e )
		{
			if ( this.explorer.SelectedItem == null )
			{
				return;
			}

			if ( e.KeyCode == Keys.Enter && e.Control )
			{
				IRunnableTestItem item = this.explorer.SelectedItem as IRunnableTestItem;
				if ( item == null )
				{
					return;
				}

				CommonUiOperations.RunItem(
					this.workspace,
					item,
					e.Shift ? RunMode.Normal : RunMode.Debug );

				e.Handled = true;
			}
			else if ( e.KeyCode == Keys.Enter )
			{
				CommonUiOperations.GoToTestItem( 
					this.dte,
					this.explorer.SelectedItem );

				e.Handled = true;
			}
		}

		private void explorer_TreeDoubleClick( object sender, TreeNodeMouseClickEventArgs e )
		{
			AbstractExplorerNode explNode = e.Node as AbstractExplorerNode;
			if ( explNode == null )
			{
				return;
			}

			CommonUiOperations.GoToTestItem( this.dte, explNode.Item );					
		}

		private void shurtcutFixtureOnFailureButton_Click( object sender, EventArgs e )
		{
			if ( this.shortCircuitFixtureOnFailureMenuItem.Checked )
			{
				this.shortCircuitRunOnFailureMenuItem.Checked = false;
			}

			try
			{
				UpdateExecutionOptions();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void shurtcutRunOnFailureButton_Click( object sender, EventArgs e )
		{
			if ( this.shortCircuitRunOnFailureMenuItem.Checked )
			{
				this.shortCircuitFixtureOnFailureMenuItem.Checked = false;
			}

			try
			{
				UpdateExecutionOptions();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}


		private void captureStackTracesMenuItem_Click( object sender, EventArgs e )
		{
			try
			{
				UpdateExecutionOptions();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}


		private void breakOnFailedAssertionsWhenDebuggingMenuItem_Click( object sender, EventArgs e )
		{
			try
			{
				this.workspace.Configuration.DefaultDebugFailedAssertionDisposition =
					breakOnFailedAssertionsWhenDebuggingMenuItem.Checked
						? Disposition.Break
						: Disposition.Continue;
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void breakOnUnhandledExceptionsWhenDebuggingMenuItem_Click( object sender, EventArgs e )
		{
			try
			{
				this.workspace.Configuration.DefaultDebugUnhandledExceptionDisposition =
					breakOnUnhandledExceptionsWhenDebuggingMenuItem.Checked
						? Disposition.Break
						: Disposition.Continue;
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		/*----------------------------------------------------------------------
		 * IDteToolWindowControl.
		 */

		public void OnActivate()
		{
			try
			{
				ExploreSolution();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void lameButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.OpenLameWebpage( this.dte, "Explorer" );
		}

		private void runEachTestInSeparateProcessMenuItem_Click( object sender, EventArgs e )
		{
			if ( runEachTestInSeparateProcessMenuItem.Checked )
			{
				this.workspace.Configuration.RunCompilerType = RunCompilerType.ProcessPerTest;
			}
			else
			{
				this.workspace.Configuration.RunCompilerType = RunCompilerType.Simple;
			}
		}

		private void intelInspectorMiXxxMenuItem_Click( object sender, EventArgs e )
		{
#if INTELINSPECTOR
			InspectorLevel level;
			if ( sender == this.intelInspectorMi1MenuItem )
			{
				level = InspectorLevel.CheckMemoryLeaks;
			}
			else if ( sender == this.intelInspectorMi2MenuItem )
			{
				level = InspectorLevel.CheckMemoryAccessIssues;
			}
			else if ( sender == this.intelInspectorMi3MenuItem )
			{
				level = InspectorLevel.LocateMemoryAccessIssues;
			}
			else if ( sender == this.intelInspectorMi4MenuItem )
			{
				level = InspectorLevel.AllMemoryIssues;
			}
			else
			{
				return;
			}

			SetInspectorMiSelection( level );
			this.workspace.Configuration.MostRecentlyUsedInspectorMemoryAnalysisLevel = level;
#endif
		}

		private void intelInspectorTiXxxMenuItem_Click( object sender, EventArgs e )
		{
#if INTELINSPECTOR
			InspectorLevel level;
			if ( sender == this.intelInspectorTi1MenuItem )
			{
				level = InspectorLevel.CheckDeadlocks;
			}
			else if ( sender == this.intelInspectorTi2MenuItem )
			{
				level = InspectorLevel.CheckDeadlocksAndRaces;
			}
			else if ( sender == this.intelInspectorTi3MenuItem )
			{
				level = InspectorLevel.LocateDeadlocksAndRaces;
			}
			else if ( sender == this.intelInspectorTi4MenuItem )
			{
				level = InspectorLevel.AllThreadingIssues;
			}
			else
			{
				return;
			}

			SetInspectorTiSelection( level );
			this.workspace.Configuration.MostRecentlyUsedInspectorThreadingAnalysisLevel = level;
#endif
		}

		private void ctxMenuRunWithInspectorMi_Click( object sender, EventArgs e )
		{
#if INTELINSPECTOR
			CommonUiOperations.RunItemInIntelInspector(
				this.workspace,
				this.contextMenuReferenceNode.Item,
				this.workspace.Configuration.MostRecentlyUsedInspectorMemoryAnalysisLevel );
#endif
		}

		private void ctxMenuRunWithInspectorTi_Click( object sender, EventArgs e )
		{
#if INTELINSPECTOR
			CommonUiOperations.RunItemInIntelInspector(
				this.workspace,
				this.contextMenuReferenceNode.Item,
				this.workspace.Configuration.MostRecentlyUsedInspectorThreadingAnalysisLevel );
#endif
		}
	}
}
