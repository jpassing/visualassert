/*----------------------------------------------------------------------
 * Purpose:
 *		Button with elevation shield.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace Cfix.LicAdmin
{
	public class ElevatedButton : Button
	{
		[DllImport( "user32.dll" )]
		private static extern IntPtr
			SendMessage( HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam );

		private const uint BCM_SETSHIELD = 0x0000160C;

		private bool IsElevated()
		{
			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			WindowsPrincipal principal = new WindowsPrincipal( identity );
			return principal.IsInRole( WindowsBuiltInRole.Administrator );
		}

		private void ShowShield()
		{
			IntPtr wParam = new IntPtr( 0 );
			IntPtr lParam = new IntPtr( 1 );
			SendMessage( new HandleRef( this, Handle ), BCM_SETSHIELD, wParam, lParam );
		}

		public ElevatedButton()
		{
			FlatStyle = FlatStyle.System;

			if ( !IsElevated() ) ShowShield();
		}
	}
}
