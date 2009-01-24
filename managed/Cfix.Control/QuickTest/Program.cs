using System;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.Native;

namespace QuickTest
{
	class Program
	{
		[STAThread]
		static void Main( string[] args )
		{
			Target target = Target.CreateLocalTarget( 
				Architecture.I386, 
				false );

			TestModule mod = TestModule.LoadModule(
				target,
				@"D:\dev\wdev\cfixplus\trunk\bin\chk\i386\testslow.dll",
				true );

			ExplorerForm f = new ExplorerForm();
			f.Explorer.Session = new GenericSession( mod );
			Application.Run( f );
		}
	}
}
