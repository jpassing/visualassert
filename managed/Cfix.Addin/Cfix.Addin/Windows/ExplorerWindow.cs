using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Cfix.Addin.ShellBrowse;

namespace Cfix.Addin.Windows
{
	public partial class ExplorerWindow : UserControl
	{
		public static readonly Guid Guid = new Guid( "e89c09c9-4e89-4ae2-b328-79dcbdfd852c" );
		private readonly String[] SupportedExtensions = new String[] { "DLL" };

		public ExplorerWindow()
		{
			InitializeComponent();
		}

		private void Explore( String path )
		{
			MessageBox.Show( path );


		}

		/*----------------------------------------------------------------------
		 * File/Folder selection.
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

	}
}
