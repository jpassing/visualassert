using System;
using System.IO;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.RunControl;
using Cfix.Control.Native;

namespace QuickTest
{
	class Program
	{
		[STAThread]
		static void Main( string[] args )
		{
			ExplorerForm f = new ExplorerForm();


			Application.Run( f );
		}
	}
}
