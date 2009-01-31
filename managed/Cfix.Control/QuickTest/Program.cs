using System;
using System.IO;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.Native;

namespace QuickTest
{
	class Program
	{
		private class Listener : TestModuleCollection.ISearchListener
		{
			public uint Invalids;

			public void InvalidModule( string path, string reason )
			{
				this.Invalids++;
			}
		}

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
					true,
					new Listener() );

				using ( ExplorerForm f = new ExplorerForm() )
				{
					f.Explorer.SetSession( new GenericSession( cont ), true );
					Application.Run( f );
				}
			}
		}
	}
}
