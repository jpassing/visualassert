using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Cfix.Addin.ShellBrowse;
using Cfix.Control.Native;

namespace Cfix.Addin.Windows
{
	public partial class ExplorerWindow : UserControl
	{
		public static readonly Guid Guid = new Guid( "e89c09c9-4e89-4ae2-b328-79dcbdfd852c" );
		private const bool UserModulesOnly = true;
		private readonly String[] SupportedExtensions = new String[] { "DLL" };

		private readonly int SolutionIconIndex;
		private readonly int SolutionIconSelectedIndex;
		private readonly int ProjectIconIndex;
		private readonly int ProjectIconSelectedIndex;

		private Workspace workspace;

		public ExplorerWindow()
		{
			InitializeComponent();

			//
			// Add icons for VS object nodes.
			//
			this.SolutionIconIndex = this.explorer.ImageList.Images.Add(
				Icons.VSObject_Solution, Color.Magenta );
			this.ProjectIconIndex = this.explorer.ImageList.Images.Add(
				Icons.VSObject_VCProject, Color.Magenta );

			this.SolutionIconSelectedIndex = this.SolutionIconIndex;
			this.ProjectIconSelectedIndex = this.ProjectIconIndex;

			//
			// Switch NodeFactory s.t. we can use our own nodes.
			//
			this.explorer.NodeFactory = new VSNodeFactory();

			this.workspace = null;

			this.statusText.GotFocus += new EventHandler( statusText_GotFocus );
		}

		public void SetWorkspace( Workspace ws )
		{
			Debug.Assert( this.workspace == null );
			this.workspace = ws;

			this.explorer.SetSession( ws.Session, false );
			this.explorer.ExceptionRaised += new EventHandler<Cfix.Control.Ui.Explorer.ExceptionEventArgs>( explorer_ExceptionRaised );
			this.explorer.RefreshStarted += new EventHandler( explorer_RefreshStarted );
			this.explorer.RefreshFinished += new EventHandler( explorer_RefreshFinished );
		}

		/*----------------------------------------------------------------------
		 * Various events.
		 */

		private void explorer_RefreshFinished( object sender, EventArgs e )
		{
			this.throbberPic.Visible = false;
			this.statusText.Text = "";

			this.refreshButton.Enabled = true;
		}

		private void explorer_RefreshStarted( object sender, EventArgs e )
		{
			this.throbberPic.Visible = true;
			this.statusText.Text = Strings.Searching;

			this.refreshButton.Enabled = false;
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

		private void Explore( String path )
		{
			Debug.Assert( this.workspace != null );

			this.refreshButton.Enabled = false;

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
				Explore( dialog.FullName );
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

		private void refreshButton_Click( object sender, EventArgs e )
		{
			this.explorer.RefreshSession( true );
		}


		/*----------------------------------------------------------------------
		 * Solution mode events.
		 */

		private void selectSlnModeButton_Click( object sender, EventArgs e )
		{
			this.refreshButton.Enabled = false;
		}

	}
}
