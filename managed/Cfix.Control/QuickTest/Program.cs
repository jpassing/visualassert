using System;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.Native;

namespace QuickTest
{
	class Program
	{
		static void Main( string[] args )
		{
			Target target = Target.CreateLocalTarget( 
				Architecture.I386, 
				true );

			TestModule mod = TestModule.LoadModule(
				target,
				@"D:\dev\wdev\cfix-cfixctl\bin\chk\i386\testlib6.dll" );

			Application.Run( new ExplorerForm() );
		}
	}
}
