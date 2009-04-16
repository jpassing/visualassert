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
	public class TestRun
	{
		private Agent ooProcTarget;
		private Agent inProcTarget;
		private AgentSet multiTarget;
		private String binDir;
		private String testdataDir;
		private String testdataDir2;

		[SetUp]
		public void Setup()
		{
			this.ooProcTarget = Agent.CreateLocalAgent(
				Architecture.I386,
				false );
			this.inProcTarget = Agent.CreateLocalAgent(
				Architecture.I386,
				true );

			this.multiTarget = new AgentSet();
			this.multiTarget.AddArchitecture( ooProcTarget );

			this.binDir = new FileInfo(
				Assembly.GetExecutingAssembly().FullName ).Directory.FullName;
			this.testdataDir =
				binDir +
				@"\..\..\..\src\Cfix.Control\Cfix.Control.Test\testdata\i386";
			this.testdataDir2 =
				binDir +
				@"\..\..\..\src\Cfix.Control\Cfix.Control.Test\testdata2\i386";
		}

		[TearDown]
		public void TearDown()
		{
			if ( this.ooProcTarget != null )
			{
				this.ooProcTarget.Dispose();
			}

			if ( this.inProcTarget != null )
			{
				this.inProcTarget.Dispose();
			}
		}

		private ITestItem GetItemByName( ITestItemCollection coll, string name )
		{
			foreach ( ITestItem item in coll )
			{
				if ( item.Name == name )
				{
					return item;
				}
			}

			Assert.Fail( "Item not available" );
			return null;
		}

		private IRun CreateRun( TestModule mod, ITestItem item )
		{
			IRunCompiler comp = new RunControl.SimpleRunCompiler(
				this.ooProcTarget,
				new StandardDispositionPolicy(
						Disposition.Continue, Disposition.Break ),
				SchedulingOptions.None,
				ThreadingOptions.ComNeutralThreading,
				ExecutionOptions.AutoAdjustCurrentDirectory );
			comp.Add( ( IRunnableTestItem ) item );
			return comp.Compile();
		}

		[Test]
		[ExpectedException( typeof( EmptyRunException ) )]
		public void TestEmptyRunRaisesException()
		{
			using ( IHost host = this.ooProcTarget.CreateHost() )
			using ( IRun run = new RunControl.SimpleRunCompiler(
				this.ooProcTarget,
				new StandardDispositionPolicy(
						Disposition.Continue, Disposition.Break ),
				SchedulingOptions.None,
				ThreadingOptions.ComNeutralThreading,
				ExecutionOptions.AutoAdjustCurrentDirectory ).Compile() )
			{
				AutoResetEvent done = new AutoResetEvent( false );
				run.Start();
			}
		}

		[Test]
		public void TestBasicEvents()
		{
			using ( IHost host = this.ooProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					true ) )
			using ( ITestItemCollection fixture = 
				( ITestItemCollection ) GetItemByName( mod, "LogTwice" ) )
			using ( IRun run = CreateRun( mod, fixture ) )
			{
				Assert.AreEqual( TaskStatus.Ready, run.Status );
				Assert.AreEqual( 1, run.ItemCount );

				int spawns = 0;
				run.HostSpawned += delegate( object sender, HostEventArgs e )
				{
					spawns++;
				};

				int starts = 0;
				run.Started += delegate( object sender, EventArgs e )
				{
					starts++;
				};

				int threadStarts = 0;
				run.ThreadStarted += delegate( object sender, ThreadEventArgs e )
				{
					threadStarts++;
				};

				int threadFinishs = 0;
				run.ThreadFinished += delegate( object sender, ThreadEventArgs e )
				{
					threadFinishs++;
				};

				AutoResetEvent done = new AutoResetEvent( false );

				int fails = 0;
				int successes = 0;
				run.Finished += delegate( object sender, FinishedEventArgs e )
				{
					switch ( run.Status )
					{
						case TaskStatus.Suceeded:
							successes++;
							break;
						case TaskStatus.Failed:
							fails++;
							break;
						default:
							Assert.Fail( "unexpected status" );
							break;
					}
					done.Set();
				};

				int logs = 0;
				run.Log += delegate( object sender, LogEventArgs e )
				{
					logs++;
					Assert.AreEqual( "test", e.Message );
				};

				run.Start();
				Assert.AreEqual( TaskStatus.Running, run.Status );

				int tasks = 0;
				foreach ( ITask t in run.Tasks )
				{
					tasks++;
				}

				Assert.AreEqual( 1, tasks );

				done.WaitOne();
				Assert.AreEqual( TaskStatus.Suceeded, run.Status );

				Assert.AreEqual( 1, successes );
				Assert.AreEqual( 2, logs );
				Assert.AreEqual( 1, threadStarts );
				Assert.AreEqual( 1, threadFinishs );
				Assert.AreEqual( 1, spawns );
				Assert.AreEqual( 1, starts );
				Assert.AreEqual( 0, fails );

				Assert.AreEqual( ExecutionStatus.Succeeded, run.RootResult.Status );
				Assert.AreEqual( 1, run.ItemsCompleted );

				run.Stop();
				run.Terminate();
			}
		}

		[Test]
		public void TestInconclusive()
		{
			using ( IHost host = this.ooProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					true ) )
			using ( ITestItemCollection fixture = 
				( ITestItemCollection ) GetItemByName( mod, "Inconclusive" ) )
			using ( IRun run = CreateRun( mod, fixture ) )
			{
				Assert.AreEqual( TaskStatus.Ready, run.Status );

				AutoResetEvent done = new AutoResetEvent( false );

				int fails = 0;
				int successes = 0;
				run.Finished += delegate( object sender, FinishedEventArgs e )
				{
					switch ( run.Status )
					{
						case TaskStatus.Suceeded:
							successes++;
							break;
						case TaskStatus.Failed:
							fails++;
							break;
						default:
							Assert.Fail( "unexpected status" );
							break;
					}
					done.Set();
				};

				run.Start();
				done.WaitOne();
				Assert.AreEqual( TaskStatus.Suceeded, run.Status );

				Assert.AreEqual( 1, successes );
				Assert.AreEqual( 0, fails );

				Assert.AreEqual(
					ExecutionStatus.SucceededWithInconclusiveParts,
					run.RootResult.Status );

				Assert.AreEqual(
					ExecutionStatus.Inconclusive,
					run.RootResult.GetItem( 0 ).Status );

				Assert.AreEqual( 1, run.RootResult.GetItem( 0 ).Failures.Count );
				foreach ( Failure f in run.RootResult.GetItem( 0 ).Failures )
				{
					Assert.IsInstanceOfType( typeof( Inconclusiveness ), f );
					Assert.AreEqual( "", f.Message );
				}
			}
		}

		[Test]
		public void TestSingleTestCase()
		{
			using ( IHost host = this.ooProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					true ) )
			using ( ITestItemCollection fixture =
				( ITestItemCollection ) GetItemByName( mod, "XSelectSingleTestCase" ) )
			using ( ITestItem tc = GetItemByName( fixture, "Nop02" ) )
			using ( IRun run = CreateRun( mod, tc ) )
			{
				Assert.AreEqual( TaskStatus.Ready, run.Status );

				AutoResetEvent done = new AutoResetEvent( false );

				run.Finished += delegate( object sender, FinishedEventArgs e )
				{
					done.Set();
				};

				run.Start();
				done.WaitOne();
				Assert.AreEqual( TaskStatus.Suceeded, run.Status );

				Assert.AreEqual(
					ExecutionStatus.Succeeded,
					run.RootResult.Status );

				Assert.AreEqual( 1, run.RootResult.ItemCount );
				Assert.IsFalse( run.RootResult.GetItem( 0 ) is IResultItemCollection );

				Assert.AreEqual(
					ExecutionStatus.Succeeded,
					run.RootResult.GetItem( 0 ).Status );
			}
		}

		[Test]
		public void TestFailedAssertion()
		{
			using ( IHost host = this.ooProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					true ) )
			using ( ITestItemCollection fixture =
				( ITestItemCollection ) GetItemByName( mod, "Fail" ) )
			using ( IRun run = CreateRun( mod, fixture ) )
			{
				Assert.AreEqual( TaskStatus.Ready, run.Status );

				AutoResetEvent done = new AutoResetEvent( false );

				int fails = 0;
				int successes = 0;
				run.Finished += delegate( object sender, FinishedEventArgs e )
				{
					switch ( run.Status )
					{
						case TaskStatus.Suceeded:
							successes++;
							break;
						case TaskStatus.Failed:
							fails++;
							break;
						default:
							Assert.Fail( "unexpected status" );
							break;
					}
					done.Set();
				};

				run.Start();
				done.WaitOne();
				Assert.AreEqual( TaskStatus.Suceeded, run.Status );

				Assert.AreEqual( 1, successes );
				Assert.AreEqual( 0, fails );

				Assert.AreEqual(
					ExecutionStatus.Failed,
					run.RootResult.Status );

				Assert.AreEqual( 2, run.RootResult.ItemCount );

				Assert.AreEqual(
					ExecutionStatus.Failed,
					run.RootResult.GetItem( 0 ).Status );

				Assert.AreEqual(
					ExecutionStatus.Failed,
					run.RootResult.GetItem( 1 ).Status );

				Assert.AreEqual( 1, run.RootResult.GetItem( 0 ).Failures.Count );
				foreach ( Failure f in run.RootResult.GetItem( 0 ).Failures )
				{
					Assert.IsInstanceOfType( typeof( FailedAssertionFailure ), f );
					FailedAssertionFailure fass = ( FailedAssertionFailure ) f;
					Assert.AreEqual( "FALSE", fass.Expression );
					Assert.IsNull( f.Message );
					Assert.AreEqual( 5, fass.LastError );
					Assert.AreEqual( "FailingAssertion", fass.Routine );

					Assert.IsNotNull( fass.StackTrace );
					Assert.Greater( fass.StackTrace.FrameCount, 2 );
					Assert.That( fass.StackTrace.ToString().Contains( "testmanaged!FailingAssertion\r\n" ) );
					Assert.IsTrue( fass.File.EndsWith( "basic.c" ) );
				}

				Assert.AreEqual( 1, run.RootResult.GetItem( 1 ).Failures.Count );
				foreach ( Failure f in run.RootResult.GetItem( 1 ).Failures )
				{
					Assert.IsInstanceOfType( typeof( UnhandledExceptionFailure ), f );
					UnhandledExceptionFailure ue = ( UnhandledExceptionFailure ) f;
					Assert.AreEqual( 0xCAFEBABE, ue.ExceptionCode );
				}

				run.Dispose();

				try
				{
					run.Start();
					Assert.Fail();
				}
				catch ( CfixException )
				{ }

				run.Stop();
				run.Stop();
			}
		}

		//[Test]
		//public void TestInvalidModule()
		//{
		//    using ( IHost host = this.inProcTarget.CreateHost() )
		//    using ( ITestItemCollection coll = host.SearchModules(
		//        new DirectoryInfo( this.testdataDir ),
		//        "toolong.dll",
		//        this.multiTarget,
		//        true ) )
		//    {
		//        coll.Refresh();

		//        Assert.IsInstanceOfType( typeof( InvalidModule ), coll.GetItem( 0 ) );
		//        InvalidModule inv = ( InvalidModule ) coll.GetItem( 0 );

		//        IRunCompiler comp = new RunControl.SimpleRunCompiler(
		//            this.ooProcTarget,
		//            new StandardDispositionPolicy(
		//                    Disposition.Continue, Disposition.Break ),
		//            SchedulingOptions.None,
		//            ThreadingOptions.None );
		//        comp.Add( inv );
		//        using ( IRun run = comp.Compile() )
		//        {
		//            Assert.IsNull( run.RootResult );
		//        }
		//    }
		//}

		[Test]
		public void TestModuleCollection()
		{
			using ( IHost host = this.inProcTarget.CreateHost() )
			using ( ITestItemCollection coll = host.SearchModules(
				new DirectoryInfo( this.testdataDir ),
				"*.dll",
				this.multiTarget,
				true ) )
			{
				coll.Refresh();

				Assert.AreEqual( 3, coll.ItemCount );

				IRunCompiler comp = new RunControl.SimpleRunCompiler(
					this.ooProcTarget,
					new StandardDispositionPolicy(
							Disposition.Continue, Disposition.Break ),
					SchedulingOptions.None,
					ThreadingOptions.None,
					ExecutionOptions.AutoAdjustCurrentDirectory );
				comp.Add( ( IRunnableTestItem ) coll );
				using ( IRun run = comp.Compile() )
				{
					//
					// Only simple.dll is a true test DLL and will
					// have a result item. All others are invalid/
					// do not apply.
					//
					Assert.AreSame( coll, run.RootResult.Item );
					Assert.AreEqual( 1, run.RootResult.ItemCount );

					IResultItemCollection simpleModRes =
						( IResultItemCollection ) run.RootResult.GetItem( 0 );
					Assert.AreEqual( "simple", simpleModRes.Name );

					AutoResetEvent done = new AutoResetEvent( false );

					run.Finished += delegate( object sender, FinishedEventArgs e )
					{
						Assert.AreEqual( TaskStatus.Suceeded, run.Status );
						done.Set();
					};

					run.Start();
					done.WaitOne();


					Assert.AreEqual( ExecutionStatus.Succeeded, run.RootResult.Status );
				}
			}
		}

		[Test]
		public void TestFixtureSkippingAfterFailure()
		{
			using ( IHost host = this.inProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					false ) )
			{
				//
				// Fail is the first fixture to be executed, remaining
				// must be skipped.
				//
				Assert.AreEqual( "Fail", mod.GetItem( 0 ).Name );

				IRunCompiler comp = new RunControl.SimpleRunCompiler(
					this.ooProcTarget,
					new StandardDispositionPolicy(
							Disposition.Continue, Disposition.Break ),
					SchedulingOptions.ShurtcutRunOnFailure,
					ThreadingOptions.None,
					ExecutionOptions.AutoAdjustCurrentDirectory );
				comp.Add( mod );
				using ( IRun run = comp.Compile() )
				{
					AutoResetEvent done = new AutoResetEvent( false );

					run.Finished += delegate( object sender, FinishedEventArgs e )
					{
						Assert.AreEqual( TaskStatus.Suceeded, run.Status );
						done.Set();
					};

					run.Start();
					done.WaitOne();

					Assert.AreEqual( ExecutionStatus.Failed, run.RootResult.Status );

					IResultItemCollection failResult = 
						( IResultItemCollection ) run.RootResult.GetItem( 0 );
					IResultItemCollection nextResult =
						( IResultItemCollection ) run.RootResult.GetItem( 1 );

					Assert.AreEqual( ExecutionStatus.Failed, failResult.Status );
					Util.Traverse(
						failResult,
						delegate( IResultItem item )
						{
							Assert.IsTrue( item.Status == ExecutionStatus.Failed ||
										   item.Status == ExecutionStatus.Skipped );
						} );

					Assert.AreEqual( ExecutionStatus.Skipped, nextResult.Status );
					Util.Traverse(
						nextResult,
						delegate( IResultItem item )
						{
							Assert.AreEqual( ExecutionStatus.Skipped, item.Status );
						} );
				}
			}
		}

		public void TestStopSinglethreaded( string fixtureName )
		{
			using ( IHost host = this.inProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					true ) )
			using ( ITestItemCollection fixture =
				( ITestItemCollection ) GetItemByName( mod, fixtureName ) )
			{
				//
				// Stop on first log message, check that the rest is skipped.
				//
				IRunCompiler comp = new RunControl.SimpleRunCompiler(
					this.ooProcTarget,
					new StandardDispositionPolicy(
							Disposition.Continue, Disposition.Break ),
					SchedulingOptions.ShurtcutRunOnFailure,
					ThreadingOptions.None,
					ExecutionOptions.AutoAdjustCurrentDirectory );
				comp.Add( ( IRunnableTestItem ) fixture );
				using ( IRun run = comp.Compile() )
				{
					AutoResetEvent done = new AutoResetEvent( false );

					run.Log += delegate( object sender, LogEventArgs e )
					{
						Assert.AreEqual( "Stop me now", e.Message );
						run.Stop();
					};

					run.Finished += delegate( object sender, FinishedEventArgs e )
					{
						//
						// Premature abort due to shortcutting.
						//
						Assert.AreEqual( TaskStatus.Stopped, run.Status );
						done.Set();
					};

					run.Start();
					done.WaitOne();

					Assert.AreEqual( ExecutionStatus.Stopped, run.RootResult.Status );

					IResultItem stopMeResult = run.RootResult.GetItem( 0 );
					IResultItem logResult = run.RootResult.GetItem( 1 );

					Assert.AreEqual( ExecutionStatus.Stopped, stopMeResult.Status );
					Assert.AreEqual( ExecutionStatus.Skipped, logResult.Status );
				}
			}
		}

		[Test]
		public void TestStopInTestCaseSinglethreaded()
		{
			TestStopSinglethreaded( "StopInTest" );
		}

		[Test]
		public void TestStopInAfterSinglethreaded()
		{
			TestStopSinglethreaded( "StopInAfter" );
		}

		[Test]
		public void TestStopInBeforeSinglethreaded()
		{
			TestStopSinglethreaded( "StopInBefore" );
		}

		[Test]
		public void TestStopInSetupSinglethreaded()
		{
			using ( IHost host = this.inProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					true ) )
			using ( ITestItemCollection fixture =
				( ITestItemCollection ) GetItemByName( mod, "StopInSetup" ) )
			{
				//
				// Stop on first log message, check that the rest is skipped.
				//
				IRunCompiler comp = new RunControl.SimpleRunCompiler(
					this.ooProcTarget,
					new StandardDispositionPolicy(
							Disposition.Continue, Disposition.Break ),
					SchedulingOptions.ShurtcutRunOnFailure,
					ThreadingOptions.None,
					ExecutionOptions.AutoAdjustCurrentDirectory );
				comp.Add( ( IRunnableTestItem ) fixture );
				using ( IRun run = comp.Compile() )
				{
					AutoResetEvent done = new AutoResetEvent( false );

					run.Log += delegate( object sender, LogEventArgs e )
					{
						Assert.AreEqual( "Stop me now", e.Message );
						run.Stop();
					};

					run.Finished += delegate( object sender, FinishedEventArgs e )
					{
						//
						// Premature abort due to shortcutting.
						//
						Assert.AreEqual( TaskStatus.Stopped, run.Status );
						done.Set();
					};

					run.Start();
					done.WaitOne();

					Assert.AreEqual( ExecutionStatus.Stopped, run.RootResult.Status );

					IResultItem stopMeResult = run.RootResult.GetItem( 0 );
					IResultItem logResult = run.RootResult.GetItem( 1 );

					Assert.AreEqual( ExecutionStatus.Skipped, stopMeResult.Status );
					Assert.AreEqual( ExecutionStatus.Skipped, logResult.Status );
				}
			}
		}

		[Test]
		public void TestStopInTeardownSinglethreaded()
		{
			using ( IHost host = this.inProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					true ) )
			using ( ITestItemCollection fixture =
				( ITestItemCollection ) GetItemByName( mod, "StopInTeardown" ) )
			{
				//
				// Stop on first log message, check that the rest is skipped.
				//
				IRunCompiler comp = new RunControl.SimpleRunCompiler(
					this.ooProcTarget,
					new StandardDispositionPolicy(
							Disposition.Continue, Disposition.Break ),
					SchedulingOptions.ShurtcutRunOnFailure,
					ThreadingOptions.None,
					ExecutionOptions.AutoAdjustCurrentDirectory );
				comp.Add( ( IRunnableTestItem ) fixture );
				using ( IRun run = comp.Compile() )
				{
					AutoResetEvent done = new AutoResetEvent( false );

					run.Log += delegate( object sender, LogEventArgs e )
					{
						Assert.AreEqual( "Stop me now", e.Message );
						run.Stop();
					};

					run.Finished += delegate( object sender, FinishedEventArgs e )
					{
						//
						// Premature abort due to shortcutting.
						//
						Assert.AreEqual( TaskStatus.Stopped, run.Status );
						done.Set();
					};

					run.Start();
					done.WaitOne();

					IResultItem stopMeResult = run.RootResult.GetItem( 0 );
					IResultItem logResult = run.RootResult.GetItem( 1 );

					Assert.AreEqual( ExecutionStatus.Succeeded, stopMeResult.Status );
					Assert.AreEqual( ExecutionStatus.Succeeded, logResult.Status );

					//
					// No special handling for Teardown, so the status 
					// should be Succeeded.
					//
					Assert.AreEqual( ExecutionStatus.Succeeded, run.RootResult.Status );
				}
			}
		}
		
		[Test]
		public void TestStopMultithreaded()
		{
			Assert.Fail( "NIY" );
		}

		[Test]
		public void TestTerminate()
		{
			using ( IHost host = this.inProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					true ) )
			using ( ITestItemCollection fixture =
				( ITestItemCollection ) GetItemByName( mod, "StopInTest" ) )
			{
				//
				// Stop on first log message, check that the rest is skipped.
				//
				IRunCompiler comp = new RunControl.SimpleRunCompiler(
					this.ooProcTarget,
					new StandardDispositionPolicy(
							Disposition.Continue, Disposition.Break ),
					SchedulingOptions.ShurtcutRunOnFailure,
					ThreadingOptions.None,
					ExecutionOptions.AutoAdjustCurrentDirectory );
				comp.Add( ( IRunnableTestItem ) fixture );
				using ( IRun run = comp.Compile() )
				{
					AutoResetEvent done = new AutoResetEvent( false );

					run.Log += delegate( object sender, LogEventArgs e )
					{
						Assert.AreEqual( "Stop me now", e.Message );
						run.Terminate();
					};

					bool started = false;
					run.Started += delegate( object sender, EventArgs e )
					{
						started = true;
					};

					run.Finished += delegate( object sender, FinishedEventArgs e )
					{
						//
						// Premature abort due to shortcutting.
						//
						Assert.AreEqual( TaskStatus.Terminated, run.Status );
						done.Set();
					};

					run.Start();
					done.WaitOne();
					Assert.IsTrue( started );

					Assert.AreEqual( ExecutionStatus.Skipped, run.RootResult.Status );

					IResultItem stopMeResult = run.RootResult.GetItem( 0 );
					IResultItem logResult = run.RootResult.GetItem( 1 );

					Assert.AreEqual( ExecutionStatus.Skipped, stopMeResult.Status );
					Assert.AreEqual( ExecutionStatus.Skipped, logResult.Status );
				}
			}
		}

		[Test]
		public void TestForceCompletionOfEmptyFixture()
		{
			using ( IHost host = this.inProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.testdataDir2 + "\\testslow.dll",
					false ) )
			{
				IRunCompiler comp = new RunControl.SimpleRunCompiler(
					this.ooProcTarget,
					new StandardDispositionPolicy(
							Disposition.Continue, Disposition.Break ),
					SchedulingOptions.ShurtcutRunOnFailure,
					ThreadingOptions.None,
					ExecutionOptions.AutoAdjustCurrentDirectory );
				comp.Add( mod );
				using ( IRun run = comp.Compile() )
				{
					AutoResetEvent done = new AutoResetEvent( false );

					run.Finished += delegate( object sender, FinishedEventArgs e )
					{
						//
						// Premature abort due to shortcutting.
						//
						Assert.AreEqual( TaskStatus.Terminated, run.Status );
						done.Set();
					};

					run.HostSpawned += delegate( object sender, HostEventArgs e )
					{
						run.Terminate();
					};

					run.Start();
					done.WaitOne();

					Assert.AreEqual( ExecutionStatus.Skipped, run.RootResult.Status );

					Util.Traverse(
						run.RootResult,
						delegate( IResultItem item )
						{
							Assert.AreEqual( ExecutionStatus.Skipped, item.Status );
						} );
				}
			}
		}

		[Test]
		public void TestDllDependencies()
		{
			//
			// importingdll imports from exportingdll - DLL load path
			// applies.
			//
			using ( IHost host = this.inProcTarget.CreateHost() )
			using ( ITestItemCollection col = host.SearchModules(
				new DirectoryInfo( this.testdataDir2 ),
				"ImportingDll.dll",
				this.multiTarget,
				true ) )
			{
				col.Refresh();
				Assert.AreEqual( 1, col.ItemCountRecursive );

				IRunCompiler comp = new RunControl.SimpleRunCompiler(
					this.ooProcTarget,
					new StandardDispositionPolicy(
							Disposition.Continue, Disposition.Break ),
					SchedulingOptions.ShurtcutRunOnFailure,
					ThreadingOptions.None,
					ExecutionOptions.AutoAdjustCurrentDirectory );
				comp.Add( ( IRunnableTestItem ) col );
				using ( IRun run = comp.Compile() )
				{
					AutoResetEvent done = new AutoResetEvent( false );

					run.Finished += delegate( object sender, FinishedEventArgs e )
					{
						done.Set();
					};

					run.Start();
					done.WaitOne();

					Assert.AreEqual( ExecutionStatus.Succeeded, run.RootResult.Status );
				}
			}
		}
	}
}
