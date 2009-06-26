using System;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Control.Test
{
	[TestFixture]
	public class TestTestModule
	{
		private Agent target;
		private String testdataDir;

		[SetUp]
		public void Setup()
		{
			this.target = Agent.CreateLocalAgent(
				Architecture.I386,
				false );

			String binDir = new FileInfo( 
				Assembly.GetExecutingAssembly().FullName ).Directory.FullName;

			this.testdataDir =
				binDir +
				@"\..\..\..\src\Cfix.Control\Cfix.Control.Test\testdata\i386";
		}

		[TearDown]
		public void TearDown()
		{
			if ( this.target != null )
			{
				this.target.Dispose();
			}
		}

		[Test]
		public void LoadNonexistingModule()
		{
			try
			{
				using ( ITestItemCollection mod = this.target.CreateHost().LoadModule(
					null, "idonotexist.dll", true ) )
				{
					Assert.Fail( "Expected exception" );
				}
			}
			catch ( CfixException )
			{
			}
		}

		[Test]
		public void LoadNonTestlib()
		{
			using ( IHost host = this.target.CreateHost() )
			{
				TestModule mod = ( TestModule ) host.LoadModule(
					null, this.testdataDir + "\\notatestdll.dll", true );
				Assert.AreEqual( this.testdataDir + "\\notatestdll.dll", mod.Path );
				Assert.AreEqual( 0, mod.ItemCount );

				DefaultEventSink sink = new DefaultEventSink();
				IRunCompiler comp = new RunControl.SimpleRunCompiler(
					this.target,
					new StandardDispositionPolicy(
						Disposition.Break, Disposition.Break ),
					SchedulingOptions.None,
					ThreadingOptions.ComNeutralThreading,
					EnvironmentOptions.AutoAdjustCurrentDirectory );
				mod.Add(
					comp,
					sink,
					mod.CreateResultItem(
						null,
						sink,
						ExecutionStatus.Pending ) );
				IRun run = comp.Compile();
				AutoResetEvent done = new AutoResetEvent( false );
				run.Finished += delegate( object s, FinishedEventArgs e )
				{
					done.Set();
				};
				run.Start();
				done.WaitOne();

				Assert.AreEqual( 0, sink.Notifications );
				Assert.AreEqual( 1, sink.HostSpawns );
			}
		}

		[Test]
		public void LoadTestlibWithEmptyFixture()
		{
			using ( IHost host = this.target.CreateHost() )
			{
				TestModule mod = ( TestModule ) host.LoadModule(
					null, this.testdataDir + "\\simple.dll", true );

				Assert.AreEqual( 0, mod.Ordinal );
				Assert.AreEqual( this.testdataDir + "\\simple.dll", mod.Path );

				Assert.AreEqual( 1, mod.ItemCount );

				ITestItemCollection fixture = ( ITestItemCollection ) mod.GetItem( 0 );
				Assert.AreEqual( 0, fixture.Ordinal );
				Assert.AreEqual( "SampleFixture", fixture.Name );

				Assert.AreEqual( 0, fixture.ItemCount );
				Assert.AreEqual( 0, fixture.ItemCountRecursive );

				DefaultEventSink sink = new DefaultEventSink();
				IRunCompiler comp = new RunControl.SimpleRunCompiler(
					this.target,
					new StandardDispositionPolicy(
						Disposition.Break, Disposition.Break ),
					SchedulingOptions.None,
					ThreadingOptions.ComNeutralThreading,
					EnvironmentOptions.None );
				mod.Add(
					comp,
					sink,
					mod.CreateResultItem(
						null,
						sink,
						ExecutionStatus.Pending ) );
				IRun run = comp.Compile();
				AutoResetEvent done = new AutoResetEvent( false );
				run.Finished += delegate( object s, FinishedEventArgs e )
				{
					done.Set();
				};
				run.Start();
				done.WaitOne();

				Assert.AreEqual( 1, sink.HostSpawns );
				Assert.AreEqual( 0, sink.Notifications );
			}
		}

		[Test]
		public void LoadTestlibAndTerminate()
		{
			ThreadingOptions[] opts = { 
					ThreadingOptions.None, ThreadingOptions.ComNeutralThreading };

			foreach ( ThreadingOptions opt in opts )
			{
				using ( IHost host = this.target.CreateHost() )
				{
					Assert.AreNotEqual( 0, host.ProcessId );
					Assert.AreEqual( Architecture.I386, host.Architecture );

					TestModule mod = ( TestModule ) host.LoadModule(
						null, this.testdataDir + "\\simple.dll", true );
					Assert.AreEqual( Architecture.I386, mod.Architecture );
					Assert.AreEqual( ModuleType.User, mod.Type );

					Assert.AreEqual( 0, mod.Ordinal );
					Assert.AreEqual( this.testdataDir + "\\simple.dll", mod.Path );

					Assert.AreEqual( 1, mod.ItemCount );

					ITestItemCollection fixture = ( ITestItemCollection ) mod.GetItem( 0 );
					Assert.AreEqual( 0, fixture.Ordinal );
					Assert.AreEqual( "SampleFixture", fixture.Name );

					Assert.AreEqual( 0, fixture.ItemCount );
					Assert.AreEqual( 0, fixture.ItemCountRecursive );

					IRunCompiler comp = new RunControl.SimpleRunCompiler(
						this.target,
						new StandardDispositionPolicy(
							Disposition.Break, Disposition.Break ),
						SchedulingOptions.None,
						ThreadingOptions.ComNeutralThreading,
						EnvironmentOptions.None );
					comp.Add( mod );
					IRun run = comp.Compile();
					AutoResetEvent done = new AutoResetEvent( false );
					run.Finished += delegate( object s, FinishedEventArgs e )
					{
						done.Set();
					};
					run.HostSpawned += delegate( object s, HostEventArgs e )
					{
						run.Terminate();
					};

					run.Start();
					done.WaitOne();
				}
			}
		}

		[Test]
		[ExpectedException( typeof( CfixException ) )]
		public void LoadTestlibWithInvalidFixtureName()
		{
			using ( ITestItemCollection mod = this.target.CreateHost().LoadModule(			
				null, this.testdataDir + "\\toolong.dll", true ) )
			{}
		}

		[Test]
		[ExpectedException( typeof( BadImageFormatException ) )]
		public void LoadTestlibWithWrongArch()
		{
			using ( ITestItemCollection mod = this.target.CreateHost().LoadModule(
				null, this.testdataDir + "\\wrongarch.dll", true ) )
			{ }
		}

		[Test]
		[ExpectedException( typeof( CfixException ) )]
		public void LoadTestlibWithAmbiguousTestCaseNamesAndFail()
		{
			using ( ITestItemCollection mod = this.target.CreateHost().LoadModule(
				null, this.testdataDir + "\\dupfixturename.dll", false ) )
			{ }
		}

		[Test]
		public void LoadTestlibWithAmbiguousTestCaseNames()
		{
			using ( IHost host = this.target.CreateHost() )
			{
				TestModule mod = ( TestModule ) host.LoadModule(
					null, this.testdataDir + "\\dupfixturename.dll", true );

				Assert.AreEqual( 0, mod.Ordinal );
				Assert.AreEqual( this.testdataDir + "\\dupfixturename.dll", mod.Path );

				Assert.AreEqual( 1, mod.ItemCount );

				ITestItemCollection fixture = ( ITestItemCollection ) mod.GetItem( 0 );
				Assert.AreEqual( 0, fixture.Ordinal );
				Assert.AreEqual( "TestExecActionDummy", fixture.Name );

				Assert.AreEqual( 2, fixture.ItemCount );

				ITestItem tc = fixture.GetItem( 0 );
				Assert.AreEqual( 0, tc.Ordinal );
				Assert.AreEqual( "Log", tc.Name );

				tc = fixture.GetItem( 1 );
				Assert.IsNull( tc );

				mod.Refresh();

				Assert.AreEqual( 1, mod.ItemCount );

				fixture = ( ITestItemCollection ) mod.GetItem( 0 );
				Assert.AreEqual( 0, fixture.Ordinal );
				Assert.AreEqual( "TestExecActionDummy", fixture.Name );

				Assert.AreEqual( 2, fixture.ItemCount );

				tc = fixture.GetItem( 0 );
				Assert.AreEqual( 0, tc.Ordinal );
				Assert.AreEqual( "Log", tc.Name );

				tc = fixture.GetItem( 1 );
				Assert.IsNull( tc );
			}
		}
		
	}
}
