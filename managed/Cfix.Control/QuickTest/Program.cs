using System;
using Cfix.Control;

namespace QuickTest
{
	class Program
	{
		static void Main( string[] args )
		{
			Target target = Target.CreateLocalTarget( Architecture.I386, true );

			TestModule mod = TestModule.LoadModule(
				target,
				@"D:\dev\wdev\cfixplus\trunk\bin\chk\i386\testlib10.dll" );

			new Cfix.Control.Test.TestTestModule();
		}
	}
}
