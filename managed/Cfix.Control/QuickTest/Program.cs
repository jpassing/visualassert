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
			Agent ooTarget = Agent.CreateLocalAgent( 
				Architecture.I386, 
				false );

			Agent inTarget = Agent.CreateLocalAgent(
				Architecture.I386,
				true );

			using ( IHost host = inTarget.CreateHost() )
			using ( ITestItemCollection mod = host.LoadModule(
				null,
				@"D:\dev\wdev\cfixplus\trunk\bin\chk\i386\testslow.dll",
				false ) )
			using ( ExplorerForm f = new ExplorerForm() )
			{
				IRunCompiler comp = new SimpleRunCompiler(
					ooTarget,
					new StandardDispositionPolicy(
							Disposition.Continue, Disposition.Break ),
					SchedulingOptions.ShurtcutRunOnFailure,
					ThreadingOptions.None );
				comp.Add( ( IRunnableTestItem ) mod );
				using ( IRun run = comp.Compile() )
				{
					f.Explorer.SetSession( new Session(), true );
					f.Explorer.Session.Tests = mod;

					f.Results.Run = run;

					Application.Run( f );
				}
			}
		}
	}
}
