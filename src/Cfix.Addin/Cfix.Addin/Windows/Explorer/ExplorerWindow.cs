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
using Cfix.Addin.ShellBrowse;
using Cfix.Control;
using Cfix.Control.Ui.Explorer;
using Cfix.Control.Native;

namespace Cfix.Addin.Windows.Explorer
{
	public partial class ExplorerWindow : UserControl
	{
		public static readonly Guid Guid = new Guid( "e89c09c9-4e89-4ae2-b328-79dcbdfd852c" );

		private Workspace workspace;
		private DTE2 dte;
		private SolutionEvents solutionEvents;
		private BuildEvents buildEvents;

		private AbstractExplorerNode contextMenuReferenceNode;

		public ExplorerWindow()
		{
			InitializeComponent();

			//
			// Switch NodeFactory s.t. we can use our own nodes.
			//
			this.explorer.NodeFactory = new VSNodeFactory( this.explorer );

			this.workspace = null;

			this.statusText.GotFocus += new EventHandler( statusText_GotFocus );
			this.selectModeButton.DropDownOpening += new EventHandler( selectModeButton_DropDownOpening );
			
			this.explorer.NodeContextMenu = this.ctxMenu;
			this.ctxMenuRefreshButton.Click += new EventHandler( ctxMenuRefreshButton_Click );

			this.Disposed += new EventHandler( ExplorerWindow_Disposed );
		}

		public void Initialize( 
			Workspace ws, 
			DTE2 dte )
		{
			Debug.Assert( this.workspace == null );
			Debug.Assert( this.dte == null );
			
			this.workspace = ws;
			this.dte = dte;
			this.solutionEvents = dte.Events.SolutionEvents;
			this.buildEvents = dte.Events.BuildEvents;

			this.explorer.SetSession( ws.Session, false );
			this.explorer.ExceptionRaised += new EventHandler<ExceptionEventArgs>( explorer_ExceptionRaised );
			this.explorer.RefreshStarted += new EventHandler( explorer_RefreshStarted );
			this.explorer.RefreshFinished += new EventHandler( explorer_RefreshFinished );
			this.explorer.AfterSelected += new EventHandler<ExplorerNodeEventArgs>( explorer_AfterSelected );
			this.explorer.BeforeContextMenuPopup += new EventHandler<ExplorerNodeEventArgs>( explorer_BeforeContextMenuPopup );
			this.explorer.TreeKeyDown +=new KeyEventHandler( explorer_TreeKeyDown );
			this.solutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler( solutionEvents_Opened );
			this.buildEvents.OnBuildProjConfigDone += new _dispBuildEvents_OnBuildProjConfigDoneEventHandler( buildEvents_OnBuildProjConfigDone );
			this.buildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler( buildEvents_OnBuildDone );

			Configuration config = this.workspace.Configuration;
			this.autoRefreshButton.Checked = config.AutoRefreshAfterBuild;
			
			SchedulingOptions schedOpts = config.SchedulingOptions;
			this.shurtcutFixtureOnFailureButton.Checked =
				( schedOpts == SchedulingOptions.ShortcutFixtureOnFailure );
			this.shurtcutRunOnFailureButton.Checked =
				( schedOpts == SchedulingOptions.ShurtcutRunOnFailure );
		}

		/*----------------------------------------------------------------------
		 * Context Menu.
		 */

		private void explorer_BeforeContextMenuPopup( 
			object sender, 
			ExplorerNodeEventArgs e )
		{
			bool runnable = e.Item is IRunnableTestItem;
			bool debugPossible = this.workspace.IsDebuggingPossible;

			this.ctxMenuDebugButton.Enabled = runnable && debugPossible;
			this.ctxMenuRunButton.Enabled = runnable;
			
			this.ctxMenuRefreshButton.Enabled = e.Item is ITestItemCollection;

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
				this.explorer.RefreshSession(
					true,
					this.contextMenuReferenceNode.Item );
			}
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
			this.abortRefreshButton.Enabled = true;
		}

		private void explorer_AfterSelected( object sender, ExplorerNodeEventArgs e )
		{
			bool runnable = e.Item is IRunnableTestItem;
			bool debugPossible = this.workspace.IsDebuggingPossible;

			this.debugButton.Enabled = runnable && debugPossible;
			this.runButton.Enabled = runnable;

			UpdateRefreshButtonStatus();
		}

		private void explorer_RefreshFinished( object sender, EventArgs e )
		{
			this.throbberPic.Visible = false;
			this.selectModeButton.Enabled = true;
			this.statusText.Text = "";

			UpdateRefreshButtonStatus();
			this.abortRefreshButton.Enabled = false;
		}

		private void explorer_RefreshStarted( object sender, EventArgs e )
		{
			this.throbberPic.Visible = true;
			this.selectModeButton.Enabled = false;
			this.statusText.Text = Strings.Searching;

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
				CfixPlus.HandleError( x );
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
				CfixPlus.HandleError( x );
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
				 !this.autoRefreshButton.Checked ||
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
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
			}
		}

		private void buildEvents_OnBuildDone( vsBuildScope Scope, vsBuildAction Action )
		{
			ITestItemCollection coll = this.explorer.Session.Tests;
			SolutionTestCollection slnColl = coll as SolutionTestCollection;
			if ( !this.autoRefreshButton.Checked ||
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
				CfixPlus.HandleError( x );
			}
		}

		private void autoRefreshButton_Click( object sender, EventArgs e )
		{
			try
			{
				this.workspace.Configuration.AutoRefreshAfterBuild = this.autoRefreshButton.Checked;
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
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
				CfixPlus.HandleError( x );
			}
		}

		private void ExplorerWindow_Disposed( object sender, EventArgs e )
		{
			this.solutionEvents.Opened -= new _dispSolutionEvents_OpenedEventHandler( solutionEvents_Opened );
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
				CfixPlus.HandleError( x );
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
			CfixPlus.HandleError( e.Exception );
		}

		/*----------------------------------------------------------------------
		 * Session updating.
		 */

		private void ExploreDirectoryOrFile( String path )
		{
			Debug.Assert( this.workspace != null );

			DisableRefresh();
			this.autoRefreshButton.Enabled = true;

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

		private void ExploreSolution()
		{
			Debug.Assert( this.workspace != null );

			DisableRefresh();
			this.autoRefreshButton.Enabled = true;

			Solution curSolution = this.dte.Solution;
			SolutionTestCollection slnCollection = new SolutionTestCollection(
				( Solution2 ) curSolution,
				this.workspace.RunAgent,
				this.workspace.Configuration );
			slnCollection.Closed += new EventHandler( slnCollection_Closed );
			this.workspace.Session.Tests = slnCollection;
		}

		private void slnCollection_Closed( object sender, EventArgs e )
		{
			this.workspace.Session.Tests = null;
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

				dialog.Title = Strings.SelectFileOrFolder;
				dialog.ShowDialog();

				if ( ! dialog.Canceled )
				{
					ExploreDirectoryOrFile( dialog.FullName );
				}
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
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
				CfixPlus.HandleError( x );
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
				CfixPlus.HandleError( x );
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
				true );
		}

		private void runButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.RunItem(
				this.workspace,
				this.explorer.SelectedItem,
				false );
		}

		private void ctxMenuDebugButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.RunItem(
				this.workspace,
				this.contextMenuReferenceNode.Item, 
				true );
		}

		private void ctxMenuRunButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.RunItem(
				this.workspace,
				this.contextMenuReferenceNode.Item, 
				false );
		}

		private void explorer_TreeKeyDown( object sender, KeyEventArgs e )
		{
			IRunnableTestItem item = this.explorer.SelectedItem as IRunnableTestItem;
			if ( item != null && e.KeyCode == Keys.Enter && e.Control )
			{
				CommonUiOperations.RunItem(
					this.workspace,
					item, 
					! e.Shift );

				e.Handled = true;
			}
		}

		private void shurtcutFixtureOnFailureButton_Click( object sender, EventArgs e )
		{
			if ( this.shurtcutFixtureOnFailureButton.Checked )
			{
				this.shurtcutRunOnFailureButton.Checked = false;

				this.workspace.Configuration.SchedulingOptions =
					SchedulingOptions.ShortcutFixtureOnFailure;
			}
			else
			{
				this.workspace.Configuration.SchedulingOptions =
					SchedulingOptions.None;
			}
		}

		private void shurtcutRunOnFailureButton_Click( object sender, EventArgs e )
		{
			if ( this.shurtcutRunOnFailureButton.Checked )
			{
				this.shurtcutFixtureOnFailureButton.Checked = false;

				this.workspace.Configuration.SchedulingOptions =
					SchedulingOptions.ShurtcutRunOnFailure;
			}
			else
			{
				this.workspace.Configuration.SchedulingOptions =
					SchedulingOptions.None;
			}
		}

		
		
	}
}
