using System;
using System.Drawing;
using System.Windows.Forms;
using stdole;

namespace Cfix.Addin.Dte
{
	internal static class IconUtil
	{
		private class IconConverter : AxHost
		{
			public IconConverter()
				: base( "52D64AAC-29C1-4EC8-BB3A-336F0D3D77CB" )
			{
			}

			public IPictureDisp GetIPictureDispFromImageWrapper( Image image )
			{
				return ( IPictureDisp ) AxHost.GetIPictureDispFromPicture( image );
			}
		}

		public static IPictureDisp GetIPictureDispFromImage( Image image )
		{
			return new IconConverter().GetIPictureDispFromImageWrapper( image );
		}
	}
}
