using System;
using System.Runtime.InteropServices;

namespace Cfix.Addin.ShellBrowse
{
	[ComVisible(true)]
	[Guid("3766C955-DA6F-4fbc-AD36-311E342EF180")]
	public class FilterByExtension : IFolderFilter
	{
		// Allows a client to specify which individual items should be enumerated.
		// Note: The host calls this method for each item in the folder. Return S_OK, to have the item enumerated. 
		// Return S_FALSE to prevent the item from being enumerated.
		public Int32 ShouldShow(
			Object psf,				// A pointer to the folder's IShellFolder interface.
			IntPtr pidlFolder,		// The folder's PIDL.
			IntPtr pidlItem)		// The item's PIDL.
		{
			// check extension, and if not ok return 1 (S_FALSE)
			
			// get display name of item
			Cfix.Addin.ShellBrowse.IShellFolder isf = (Cfix.Addin.ShellBrowse.IShellFolder)psf;

			Cfix.Addin.ShellBrowse.ShellApi.STRRET ptrDisplayName;
			isf.GetDisplayNameOf(pidlItem,(uint)Cfix.Addin.ShellBrowse.ShellApi.SHGNO.SHGDN_NORMAL | (uint)Cfix.Addin.ShellBrowse.ShellApi.SHGNO.SHGDN_FORPARSING,out ptrDisplayName);
			
			String sDisplay;
			Cfix.Addin.ShellBrowse.ShellApi.StrRetToBSTR(ref ptrDisplayName,(IntPtr)0,out sDisplay);
			
			// check if item is file or folder
			IntPtr[] aPidl = new IntPtr[1];
			aPidl[0] = pidlItem;
			uint Attrib;
			Attrib = (uint)Cfix.Addin.ShellBrowse.ShellApi.SFGAO.SFGAO_FOLDER;

			int temp;
			temp = isf.GetAttributesOf(1,aPidl,ref Attrib);

			// if item is a folder, accept
			if ((Attrib & (uint)Cfix.Addin.ShellBrowse.ShellApi.SFGAO.SFGAO_FOLDER) == (uint)Cfix.Addin.ShellBrowse.ShellApi.SFGAO.SFGAO_FOLDER)
				return 0;

			// if item is file, check if it has a valid extension
			for (int i=0 ; i<this.validExtension.Length ; i++)
			{
				if (sDisplay.ToUpper().EndsWith("." + validExtension[i].ToUpper()))
					return 0;
			}

			return 1;
		}

		// Allows a client to specify which classes of objects in a Shell folder should be enumerated.
		public Int32 GetEnumFlags( 
			Object psf,				// A pointer to the folder's IShellFolder interface.
			IntPtr pidlFolder,		// The folder's PIDL.
			IntPtr phwnd,			// A pointer to the host's window handle.
			out UInt32 pgrfFlags)	// One or more SHCONTF values that specify which classes of objects to enumerate.
		{
			pgrfFlags = (uint)ShellApi.SHCONTF.SHCONTF_FOLDERS | (uint)ShellApi.SHCONTF.SHCONTF_NONFOLDERS;
			return 0;
		}

		private string[] validExtension;

		public FilterByExtension( string[] extensions )
		{
			this.validExtension = extensions;
		}
	}

}