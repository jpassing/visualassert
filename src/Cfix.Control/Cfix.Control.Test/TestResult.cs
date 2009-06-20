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
	public class TestResult
	{
		private readonly IDispositionPolicy policy =
			new StandardDispositionPolicy(
				Disposition.Break,
				Disposition.Break );

		[Test]
		public void TestBasics()
		{
			MockModule module = new MockModule( "foo.dll" );
			MockContainer fixture = new MockContainer( "fixture" );
			MockItem test = new MockItem( "test" );
			fixture.Children.Add( test );
			module.Children.Add( fixture );

			Agent target = new MockTarget( module );
			TestModule mod = ( TestModule ) target.CreateHost().LoadModule(
				null, module.GetPath(), false );

			IRunCompiler comp = new RunControl.SimpleRunCompiler(
				target,
				this.policy, 
				SchedulingOptions.None,
				ThreadingOptions.ComNeutralThreading,
				ExecutionOptions.AutoAdjustCurrentDirectory );
			comp.Add( mod );
			IRun run = comp.Compile();

			Util.Traverse(
				run.RootResult,
				delegate( IResultItem item )
				{
					Assert.AreEqual( new TimeSpan( 0 ), item.Duration );
					Assert.AreEqual( ExecutionStatus.Pending, item.Status );
				} );

			Util.Traverse(
				run.RootResult,
				delegate( IResultItem item )
				{
					Assert.IsNotNull( item.Item );
					Assert.AreEqual( item.Name, item.Item.Name );
				} );

			mod.Dispose();

			Util.Traverse(
				run.RootResult,
				delegate( IResultItem item )
				{
					Assert.IsNotNull( item.Item );
				} );
		}

		[Test]
		public void TestFixtureAllSuccess()
		{
			MockModule module = new MockModule( "foo.dll" );
			MockContainer testFixture = new MockContainer( "fixture" );
			MockItem test1 = new MockItem( "test1" );
			MockItem test2 = new MockItem( "test2" );
			testFixture.Children.Add( test1 );
			testFixture.Children.Add( test2 );
			module.Children.Add( testFixture );

			Agent target = new MockTarget( module );
			TestModule mod = ( TestModule ) target.CreateHost().LoadModule(
				null, module.GetPath(), false );

			IRunCompiler comp = new RunControl.SimpleRunCompiler(
				target,
				this.policy,
				SchedulingOptions.None,
				ThreadingOptions.ComNeutralThreading,
				ExecutionOptions.AutoAdjustCurrentDirectory );
			comp.Add( mod );
			IRun run = comp.Compile();

			Assert.AreEqual( 1, run.RootResult.ItemCount );
			IResultItemCollection fixture =
				( IResultItemCollection ) run.RootResult.GetItem( 0 );
			Assert.AreEqual( 2, fixture.ItemCount );

			Cfixctl.ICfixTestÌtemContainerEventSink fixSink =
				( Cfixctl.ICfixTestÌtemContainerEventSink ) fixture;

			fixSink.BeforeFixtureStart();

			Cfixctl.ICfixTestÌtemEventSink tcSink =
				( Cfixctl.ICfixTestÌtemEventSink ) fixture.GetItem( 0 );
			tcSink.BeforeTestCaseStart();
			Thread.Sleep( 10 );
			tcSink.AfterTestCaseFinish( 1 );
			Assert.AreEqual( 
				ExecutionStatus.Succeeded,
				fixture.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Pending,
				fixture.GetItem( 1 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );

			tcSink =
				( Cfixctl.ICfixTestÌtemEventSink ) fixture.GetItem( 1 );
			tcSink.BeforeTestCaseStart();
			tcSink.AfterTestCaseFinish( 1 );

			Assert.AreEqual(
				ExecutionStatus.Succeeded,
				fixture.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Succeeded,
				fixture.GetItem( 1 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );

			fixSink.AfterFixtureFinish( 1 );

			Assert.AreEqual(
				ExecutionStatus.Succeeded,
				fixture.Status );
			Assert.AreEqual(
				ExecutionStatus.Succeeded,
				run.RootResult.Status );

			Assert.Greater( fixture.GetItem( 0 ).Duration, new TimeSpan( 0 ) );
		}

		[Test]
		public void TestFixtureFailSetupSkipTestcases()
		{
			MockModule module = new MockModule( "foo.dll" );
			MockContainer testFixture = new MockContainer( "fixture" );
			MockItem test1 = new MockItem( "test1" );
			MockItem test2 = new MockItem( "test2" );
			MockItem test3 = new MockItem( "test3" );
			testFixture.Children.Add( test1 );
			testFixture.Children.Add( test2 );
			testFixture.Children.Add( test3 );
			module.Children.Add( testFixture );

			Agent target = new MockTarget( module );
			TestModule mod = ( TestModule ) target.CreateHost().LoadModule(
				null, module.GetPath(), false );

			IRunCompiler comp = new RunControl.SimpleRunCompiler(
				target,
				this.policy,
				SchedulingOptions.None,
				ThreadingOptions.ComNeutralThreading,
				ExecutionOptions.AutoAdjustCurrentDirectory );
			comp.Add( mod );
			IRun run = comp.Compile();

			int statusChanges = 0;

			run.StatusChanged += delegate( object sender, EventArgs e )
			{
				statusChanges++;
			};

			Assert.AreEqual( 1, run.RootResult.ItemCount );
			IResultItemCollection fixture =
				( IResultItemCollection ) run.RootResult.GetItem( 0 );
			Assert.AreEqual( 3, fixture.ItemCount );

			Cfixctl.ICfixTestÌtemContainerEventSink fixSink =
				( Cfixctl.ICfixTestÌtemContainerEventSink ) fixture;

			//
			// Start fixture.
			//
			fixSink.BeforeFixtureStart();
			Assert.AreEqual( 2, statusChanges );

			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Parent.Status );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );

			fixSink.FailedAssertion( "test", null, null, null, 0, 0, 0, 0, null );
			fixSink.AfterFixtureFinish( 0 );

			//
			// Skip all test cases.
			//
			Assert.AreEqual(
				ExecutionStatus.Skipped,
				fixture.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Skipped,
				fixture.GetItem( 1 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Skipped,
				fixture.GetItem( 2 ).Status );

			Assert.AreEqual( 7, statusChanges );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				fixture.Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.Status );

			Assert.AreEqual( 1, fixture.Failures.Count );
		}

		[Test]
		public void TestFixtureSucFirstFailSecondSkipThird()
		{
			MockModule module = new MockModule( "foo.dll" );
			MockContainer testFixture = new MockContainer( "fixture" );
			MockItem test1 = new MockItem( "test1" );
			MockItem test2 = new MockItem( "test2" );
			MockItem test3 = new MockItem( "test3" );
			testFixture.Children.Add( test1 );
			testFixture.Children.Add( test2 );
			testFixture.Children.Add( test3 );
			module.Children.Add( testFixture );

			Agent target = new MockTarget( module );
			TestModule mod = ( TestModule ) target.CreateHost().LoadModule(
				null, module.GetPath(), false );

			IRunCompiler comp = new RunControl.SimpleRunCompiler(
				target,
				this.policy,
				SchedulingOptions.None,
				ThreadingOptions.ComNeutralThreading,
				ExecutionOptions.AutoAdjustCurrentDirectory );
			comp.Add( mod );
			IRun run = comp.Compile();

			int statusChanges = 0;

			run.StatusChanged += delegate( object sender, EventArgs e )
			{
				statusChanges++;
			};

			Assert.AreEqual( 1, run.RootResult.ItemCount );
			IResultItemCollection fixture =
				( IResultItemCollection ) run.RootResult.GetItem( 0 );
			Assert.AreEqual( 3, fixture.ItemCount );

			Cfixctl.ICfixTestÌtemContainerEventSink fixSink =
				( Cfixctl.ICfixTestÌtemContainerEventSink ) fixture;

			//
			// Start fixture.
			//
			fixSink.BeforeFixtureStart();
			Assert.AreEqual( 2, statusChanges );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Parent.Status );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );

			//
			// 1st test succeeds.
			//
			Cfixctl.ICfixTestÌtemEventSink tcSink =
				( Cfixctl.ICfixTestÌtemEventSink ) fixture.GetItem( 0 );
			tcSink.BeforeTestCaseStart();
			Assert.AreEqual( 3, statusChanges );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.GetItem( 0 ).Status );

			Assert.IsNull( fixture.Failures );
			Assert.IsNull( fixture.GetItem( 0 ).Failures );
			
			tcSink.AfterTestCaseFinish( 1 );
			Assert.AreEqual( 4, statusChanges );

			Assert.AreEqual(
				ExecutionStatus.Succeeded,
				fixture.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Pending,
				fixture.GetItem( 1 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Pending,
				fixture.GetItem( 2 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );

			//
			// 2nd test fails.
			//
			tcSink =
				( Cfixctl.ICfixTestÌtemEventSink ) fixture.GetItem( 1 );
			tcSink.BeforeTestCaseStart();
			Assert.AreEqual( 5, statusChanges );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );

			tcSink.UnhandledException( 1, 0, null );
			tcSink.AfterTestCaseFinish( 0 );
			Assert.AreEqual( 6, statusChanges );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );
			Assert.IsNull( fixture.GetItem( 0 ).Failures );
			Assert.AreEqual( 1, fixture.GetItem( 1 ).Failures.Count );
			Assert.IsNull( fixture.GetItem( 2 ).Failures );

			Assert.AreEqual(
				ExecutionStatus.Succeeded,
				fixture.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				fixture.GetItem( 1 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Pending,
				fixture.GetItem( 2 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );

			//
			// 3rd test is skipped.
			//

			fixSink.AfterFixtureFinish( 0 );
			Assert.AreEqual( 9, statusChanges );

			Assert.AreEqual(
				ExecutionStatus.Skipped,
				fixture.GetItem( 2 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				fixture.Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.Status );

			Assert.IsNull( fixture.Failures );
		}

		[Test]
		public void TestFixtureWithInconclusiveTest()
		{
			MockModule module = new MockModule( "foo.dll" );
			MockContainer testFixture = new MockContainer( "fixture" );
			MockItem test1 = new MockItem( "test1" );
			testFixture.Children.Add( test1 );
			module.Children.Add( testFixture );

			Agent target = new MockTarget( module );
			TestModule mod = ( TestModule ) target.CreateHost().LoadModule(
				null, module.GetPath(), false );

			IRunCompiler comp = new RunControl.SimpleRunCompiler(
				target,
				this.policy,
				SchedulingOptions.None,
				ThreadingOptions.ComNeutralThreading,
				ExecutionOptions.AutoAdjustCurrentDirectory );
			comp.Add( mod );
			IRun run = comp.Compile();

			int statusChanges = 0;

			run.StatusChanged += delegate( object sender, EventArgs e )
			{
				statusChanges++;
			};

			Assert.AreEqual( 1, run.RootResult.ItemCount );
			IResultItemCollection fixture =
				( IResultItemCollection ) run.RootResult.GetItem( 0 );
			Assert.AreEqual( 1, fixture.ItemCount );

			Cfixctl.ICfixTestÌtemContainerEventSink fixSink =
				( Cfixctl.ICfixTestÌtemContainerEventSink ) fixture;

			//
			// Start fixture.
			//
			fixSink.BeforeFixtureStart();
			Assert.AreEqual( 2, statusChanges );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Parent.Status );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );

			//
			// Test inconclusive.
			//
			Cfixctl.ICfixTestÌtemEventSink tcSink =
				( Cfixctl.ICfixTestÌtemEventSink ) fixture.GetItem( 0 );
			tcSink.BeforeTestCaseStart();
			tcSink.Inconclusive( "test", 0, null );
			tcSink.AfterTestCaseFinish( 0 );

			Assert.AreEqual( 4, statusChanges );
			Assert.AreEqual(
				ExecutionStatus.Inconclusive,
				fixture.GetItem( 0 ).Status );

			Assert.IsNull( fixture.Failures );
			Assert.AreEqual( 1, fixture.GetItem( 0 ).Failures.Count );

			fixSink.AfterFixtureFinish( 1 );
			Assert.AreEqual( 6, statusChanges );

			Assert.AreEqual(
				ExecutionStatus.Inconclusive,
				fixture.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.SucceededWithInconclusiveParts,
				fixture.Status );
			Assert.AreEqual(
				ExecutionStatus.SucceededWithInconclusiveParts,
				run.RootResult.Status );


			Assert.IsNull( fixture.Failures );
		}

		private IRun RunTest(
			ExecutionStatus fixSetup,
			ExecutionStatus[] testCases,
			int testCasesToExecute,
			ExecutionStatus fixTeardown
			)
		{
			MockModule module = new MockModule( "foo.dll" );
			MockContainer testFixture = new MockContainer( "fixture" );
			for ( int i = 0; i < testCases.Length; i++ )
			{
				testFixture.Children.Add( new MockItem( "test" + i ) );
			}
			module.Children.Add( testFixture );

			Agent target = new MockTarget( module );
			TestModule mod = ( TestModule ) target.CreateHost().LoadModule(
				null, module.GetPath(), false );

			IRunCompiler comp = new RunControl.SimpleRunCompiler(
				target,
				this.policy,
				SchedulingOptions.None,
				ThreadingOptions.ComNeutralThreading,
				ExecutionOptions.AutoAdjustCurrentDirectory );
			comp.Add( mod );
			IRun run = comp.Compile();

			int spawns = 0;
			run.HostSpawned += delegate( object sender, HostEventArgs e )
			{
				Assert.AreEqual( 1234, e.HostProcessId );
				spawns++;
			};

			Cfixctl.ICfixTestÌtemContainerEventSink fixSink =
				( Cfixctl.ICfixTestÌtemContainerEventSink ) 
					run.RootResult.GetItem( 0 );

			IResultItemCollection fixture =
				( IResultItemCollection ) run.RootResult.GetItem( 0 );

			fixSink.BeforeFixtureStart();
			if ( fixSetup == ExecutionStatus.Inconclusive )
			{
				fixSink.Inconclusive( null, 0, null );
			}
			else if ( fixSetup == ExecutionStatus.Failed )
			{
				fixSink.UnhandledException( 0, 0, null );
			}
			else
			{
				for ( uint i = 0; i < testCasesToExecute; i++ )
				{
					Cfixctl.ICfixTestÌtemEventSink tcSink =
						( Cfixctl.ICfixTestÌtemEventSink ) fixture.GetItem( i );
					tcSink.BeforeTestCaseStart();

					if ( testCases[ i ] == ExecutionStatus.Inconclusive )
					{
						tcSink.Inconclusive( null, 0, null );
						tcSink.AfterTestCaseFinish( 0 );
					}
					else if ( testCases[ i ] == ExecutionStatus.Failed )
					{
						tcSink.UnhandledException( 0, 0, null );
						tcSink.AfterTestCaseFinish( 0 );
					}
					else
					{
						tcSink.Log( null, 0, null );
						tcSink.AfterTestCaseFinish( 1 );
					}
				}

				if ( fixTeardown == ExecutionStatus.Inconclusive )
				{
					fixSink.Inconclusive( null, 0, null );
				}
				else if ( fixTeardown == ExecutionStatus.Failed )
				{
					fixSink.UnhandledException( 0, 0, null );
				}
			}

			fixSink.AfterFixtureFinish(
				testCases.Length == testCasesToExecute ? 1 : 0 );

			Assert.AreEqual( 0, spawns );

			return run;
		}

		[Test]
		public void SucceedEmptyFixture()
		{
			IRun run = RunTest(
				ExecutionStatus.Succeeded,
				new ExecutionStatus[ 0 ],
				0,
				ExecutionStatus.Succeeded );

			Assert.AreEqual( 
				ExecutionStatus.Succeeded, 
				run.RootResult.Status );
			Assert.AreEqual( 
				ExecutionStatus.Succeeded, 
				run.RootResult.GetItem( 0 ).Status );
			Assert.AreSame(
				run.RootResult,
				run.RootResult.GetItem( 0 ).Parent );

		}

		[Test]
		public void FailingSetup()
		{
			IRun run = RunTest(
				ExecutionStatus.Failed,
				new ExecutionStatus[] { ExecutionStatus.Succeeded },
				0,
				ExecutionStatus.Succeeded );

			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Skipped,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 0 ).Status );
		}

		[Test]
		public void InconcSetup()
		{
			IRun run = RunTest(
				ExecutionStatus.Inconclusive,
				new ExecutionStatus[] { ExecutionStatus.Succeeded },
				0,
				ExecutionStatus.Succeeded );

			Assert.AreEqual(
				ExecutionStatus.SucceededWithInconclusiveParts,
				run.RootResult.Status );
			Assert.AreEqual(
				ExecutionStatus.Inconclusive,
				run.RootResult.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Skipped,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 0 ).Status );
		}

		[Test]
		public void FailingTeardown()
		{
			IRun run = RunTest(
				ExecutionStatus.Succeeded,
				new ExecutionStatus[] { ExecutionStatus.Succeeded },
				1,
				ExecutionStatus.Failed );

			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Succeeded,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 0 ).Status );
		}

		[Test]
		public void InconcTeardown()
		{
			IRun run = RunTest(
				ExecutionStatus.Succeeded,
				new ExecutionStatus[] { ExecutionStatus.Succeeded },
				1,
				ExecutionStatus.Inconclusive );

			Assert.AreEqual(
				ExecutionStatus.SucceededWithInconclusiveParts,
				run.RootResult.Status );
			Assert.AreEqual(
				ExecutionStatus.Inconclusive,
				run.RootResult.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Succeeded,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 0 ).Status );
		}

		[Test]
		public void ShortCircuitAfterFirstFailure()
		{
			IRun run = RunTest(
				ExecutionStatus.Succeeded,
				new ExecutionStatus[] { ExecutionStatus.Failed, ExecutionStatus.Succeeded },
				1,
				ExecutionStatus.Succeeded );

			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Skipped,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 1 ).Status );
		}

		[Test]
		public void ShortCircuitAfterFirstFailureWithInconcTeardown()
		{
			IRun run = RunTest(
				ExecutionStatus.Succeeded,
				new ExecutionStatus[] { ExecutionStatus.Failed, ExecutionStatus.Succeeded },
				1,
				ExecutionStatus.Inconclusive );

			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Skipped,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 1 ).Status );
		}

		[Test]
		public void ContinueAfterFirstFailure()
		{
			IRun run = RunTest(
				ExecutionStatus.Succeeded,
				new ExecutionStatus[] { ExecutionStatus.Failed, ExecutionStatus.Succeeded },
				2,
				ExecutionStatus.Inconclusive );

			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				run.RootResult.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Succeeded,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 1 ).Status );
		}

		[Test]
		public void ShortCircuitHalfway()
		{
			IRun run = RunTest(
				ExecutionStatus.Succeeded,
				new ExecutionStatus[] { ExecutionStatus.Succeeded, ExecutionStatus.Succeeded },
				1,
				ExecutionStatus.Succeeded );

			Assert.AreEqual(
				ExecutionStatus.SucceededWithSkippedParts,
				run.RootResult.Status );
			Assert.AreEqual(
				ExecutionStatus.SucceededWithSkippedParts,
				run.RootResult.GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Succeeded,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 0 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Skipped,
				( ( IResultItemCollection ) run.RootResult.GetItem( 0 ) ).GetItem( 1 ).Status );
		}
	}
}
