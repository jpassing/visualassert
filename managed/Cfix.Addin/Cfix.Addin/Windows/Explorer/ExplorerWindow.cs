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
		private const bool UserModulesOnly = true;
		private readonly String[] SupportedExtensions = new String[] { "DLL" };

		private Workspace workspace;
		private DTE2 dte;

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
		}

		public void Initialize( Workspace ws, DTE2 dte )
		{
			Debug.Assert( this.workspace == null );
			Debug.Assert( this.dte == null );
			
			this.workspace = ws;
			this.dte = dte;

			this.explorer.SetSession( ws.Session, false );
			this.explorer.ExceptionRaised += new EventHandler<ExceptionEventArgs>( explorer_ExceptionRaised );
			this.explorer.RefreshStarted += new EventHandler( explorer_RefreshStarted );
			this.explorer.RefreshFinished += new EventHandler( explorer_RefreshFinished );
			this.explorer.AfterSelected += new EventHandler<ExplorerNodeEventArgs>( explorer_AfterSelected );
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
		}


		private void explorer_AfterSelected( object sender, ExplorerNodeEventArgs e )
		{
			UpdateRefreshButtonStatus();
		}

		private void explorer_RefreshFinished( object sender, EventArgs e )
		{
			this.throbberPic.Visible = false;
			this.statusText.Text = "";

			UpdateRefreshButtonStatus();
		}

		private void explorer_RefreshStarted( object sender, EventArgs e )
		{
			this.throbberPic.Visible = true;
			this.statusText.Text = Strings.Searching;

			DisableRefresh();
		}

		private void refreshButton_Click( object sender, EventArgs e )
		{
			this.explorer.RefreshSession( true, true );
		}

		/*----------------------------------------------------------------------
		 * Various events.
		 */

		private void selectModeButton_DropDownOpening( object sender, EventArgs e )
		{
			//
			// Enable button only when a solution is open.
			//
			Solution curSolution = this.dte.Solution;
			this.selectSlnModeButton.Enabled = curSolution.FileName.Length > 0;
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

			this.workspace.Session.Tests =
				TestModuleCollection.Search(
					dir,
					filter,
					this.workspace.SearchTarget,
					this.workspace.RunTarget,
					UserModulesOnly,
					true,
					null );
		}

		private void ExploreSolution()
		{
			Debug.Assert( this.workspace != null );

			DisableRefresh();

			Solution curSolution = this.dte.Solution;
			this.workspace.Session.Tests = new SolutionTestCollection(
				( Solution2 ) curSolution );
		}

		/*----------------------------------------------------------------------
		 * File/Folder mode events.
		 */

		private void selectDirModeButton_Click( object sender, EventArgs e )
		{
			ShellBrowseForFolderDialog dialog = new ShellBrowseForFolderDialog();
			dialog.hwndOwner = this.Handle;

			dialog.Filter = new FilterByExtension( SupportedExtensions );
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

		private void dialog_OnSelChanged(
			ShellBrowseForFolderDialog sender, 
			ShellBrowseForFolderDialog.SelChangedEventArgs args
			)
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
				foreach ( String ext in this.SupportedExtensions )
				{
					if ( path.ToUpper().EndsWith( ext ) )
					{
						sender.EnableOk( args.hwnd, true );
						return;
					}
				}

				sender.EnableOk( args.hwnd, false );
			}
		}

		
		/*----------------------------------------------------------------------
		 * Solution mode events.
		 */

		private void selectSlnModeButton_Click( object sender, EventArgs e )
		{
			ExploreSolution();
		}

	}
}
