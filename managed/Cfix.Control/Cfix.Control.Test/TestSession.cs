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
	public class TestSession
	{
		private Agent ooProcTarget;
		private Agent inProcTarget;
		private AgentSet multiTarget;
		private String binDir;

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

		private ITestItemCollection GetFixture( TestModule mod, string name )
		{
			foreach ( ITestItem item in mod )
			{
				if ( item.Name == name )
				{
					return ( ITestItemCollection ) item;
				}
			}

			Assert.Fail( "Fixture not available" );
			return null;
		}

		private IRun CreateRun( TestModule mod, String fixture )
		{
			IRunCompiler comp = new RunControl.SimpleRunCompiler(
				this.ooProcTarget,
				new StandardDispositionPolicy(
						Disposition.Continue, Disposition.Break ),
				SchedulingOptions.None,
				ThreadingOptions.ComNeutralThreading );
			comp.Add( GetFixture( mod, fixture ) );
			return comp.Compile();
		}

		[Test]
		public void TestBasicEvents()
		{
			using ( IHost host = this.ooProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					true ) )
			using ( IRun run = CreateRun( mod, "LogTwice" ) )
			{
				Assert.AreEqual( TaskStatus.Ready, run.Status );

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
			using ( IRun run = CreateRun( mod, "Inconclusive" ) )
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
		public void TestFailedAssertion()
		{
			using ( IHost host = this.ooProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					true ) )
			using ( IRun run = CreateRun( mod, "Fail" ) )
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
	}
}
