using System;
using System.IO;
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
			Target ooTarget = Target.CreateLocalTarget( 
				Architecture.I386, 
				false );

			Target inTarget = Target.CreateLocalTarget(
				Architecture.I386,
				true );

			//TestModule mod = TestModule.LoadModule(
			//    target,
			//    @"D:\dev\wdev\cfixplus\trunk\bin\chk\i386\testslow.dll",
			//    true );

			//GenericTestItemCollection cont = new GenericTestItemCollection( "Cont" );
			//cont.Add( mod );

			using ( MultiTarget multiTarget = new MultiTarget() )
			{
				multiTarget.AddArchitecture( ooTarget );

				TestModuleCollection cont = TestModuleCollection.Search(
					new DirectoryInfo( @"D:\dev\wdev\cfix-cfixctl" ),
					"*",
					inTarget,
					multiTarget,
					false,
					true );

				using ( ExplorerForm f = new ExplorerForm() )
				{
					f.Explorer.SetSession( new GenericSession(), true );
					f.Explorer.Session.Tests = cont;
					Application.Run( f );
				}
			}
		}
	}
}
