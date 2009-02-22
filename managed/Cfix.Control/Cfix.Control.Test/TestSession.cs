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
		private Target ooProcTarget;
		private Target inProcTarget;
		private MultiTarget multiTarget;
		private String binDir;

		[SetUp]
		public void Setup()
		{
			this.ooProcTarget = Target.CreateLocalTarget(
				Architecture.I386,
				false );
			this.inProcTarget = Target.CreateLocalTarget(
				Architecture.I386,
				true );

			this.multiTarget = new MultiTarget();
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

		private ITestItemCollection GetFixture( string name )
		{
			TestModule mod = TestModule.LoadModule(
				this.ooProcTarget,
				this.binDir + "\\testmanaged.dll",
				true );

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

		[Test]
		public void TestBasicEvents()
		{
			ISession session = new Session();
			session.Tests = GetFixture( "LogTwice" );

			IRun run = session.CreateRun(
				new StandardDispositionPolicy(
					Disposition.Continue,
					Disposition.Break ),
				SchedulingOptions.ComNeutralThreading,
				CompositionOptions.NonComposite );

			Assert.IsFalse( run.IsFinished );
			Assert.IsFalse( run.IsStarted );

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
			run.Failed += delegate( object sender, FailEventArgs e )
			{
				fails++;
				done.Set();
			};

			int successes = 0;
			run.Succeeded += delegate( object sender, EventArgs e )
			{
				successes++;
				done.Set();
			};

			int logs = 0;
			run.Log += delegate( object sender, LogEventArgs e )
			{
				logs++;
				Assert.AreEqual( "test", e.Message );
			};

			run.Start();
			Assert.IsTrue( run.IsStarted );

			done.WaitOne();
			Assert.IsTrue( run.IsStarted );
			Assert.IsTrue( run.IsFinished );

			Assert.AreEqual( 1, successes );
			Assert.AreEqual( 2, logs );
			Assert.AreEqual( 1, threadStarts );
			Assert.AreEqual( 1, threadFinishs );
			Assert.AreEqual( 1, spawns );
			Assert.AreEqual( 1, starts );
			Assert.AreEqual( 0, fails );

			Assert.AreEqual( ExecutionStatus.Succeeded, run.RootResult.Status );
		}

		[Test]
		public void TestInconclusive()
		{
			ISession session = new Session();
			session.Tests = GetFixture( "Inconclusive" );

			IRun run = session.CreateRun(
				new StandardDispositionPolicy(
					Disposition.Continue,
					Disposition.Break ),
				SchedulingOptions.ComNeutralThreading,
				CompositionOptions.NonComposite );

			Assert.IsFalse( run.IsFinished );
			Assert.IsFalse( run.IsStarted );

			AutoResetEvent done = new AutoResetEvent( false );

			int fails = 0;
			run.Failed += delegate( object sender, FailEventArgs e )
			{
				fails++;
				done.Set();
			};

			int successes = 0;
			run.Succeeded += delegate( object sender, EventArgs e )
			{
				successes++;
				done.Set();
			};

			run.Start();
			done.WaitOne();
			Assert.IsTrue( run.IsFinished );

			Assert.AreEqual( 1, successes );
			Assert.AreEqual( 0, fails );

			Assert.AreEqual( 
				ExecutionStatus.SucceededWithInconclusiveParts, 
				run.RootResult.Status );

			IResultItemCollection fix = ( IResultItemCollection ) run.RootResult.GetItem( 0 );
			Assert.AreEqual(
				ExecutionStatus.SucceededWithInconclusiveParts,
				fix.Status );
			Assert.AreEqual(
				ExecutionStatus.Inconclusive,
				fix.GetItem( 0 ).Status );
		}
	}
}
