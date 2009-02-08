using System;
using System.Runtime.InteropServices;


namespace Cfix.Addin.ShellBrowse
{
	public class ShellFunctions
	{
		public static IMalloc GetMalloc()
		{
			IntPtr ptrRet;
			ShellApi.SHGetMalloc( out ptrRet );

			Object obj = Marshal.GetTypedObjectForIUnknown( ptrRet, typeof( IMalloc ) );
			IMalloc imalloc = ( IMalloc ) obj;

			return imalloc;
		}

		public static IShellFolder GetDesktopFolder()
		{
			IntPtr ptrRet;
			ShellApi.SHGetDesktopFolder( out ptrRet );

			System.Type shellFolderType = typeof( IShellFolder );
			Object obj = Marshal.GetTypedObjectForIUnknown( ptrRet, shellFolderType );
			IShellFolder ishellFolder = ( IShellFolder ) obj;

			return ishellFolder;
		}

		public static IShellFolder GetShellFolder( IntPtr ptrShellFolder )
		{
			System.Type shellFolderType = typeof( IShellFolder );
			Object obj = Marshal.GetTypedObjectForIUnknown( ptrShellFolder, shellFolderType );
			return ( IShellFolder ) obj;
		}

	}

}