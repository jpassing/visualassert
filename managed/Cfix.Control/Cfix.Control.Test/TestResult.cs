using System;
using System.IO;
using System.Reflection;
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

		private delegate void VisitHandler( IResultItem item );

		private void Traverse( 
			IResultItemCollection coll,
			VisitHandler handler
			)
		{
			handler( coll );

			foreach ( IResultItem item in coll )
			{
				IResultItemCollection subColl = item as IResultItemCollection;
				if ( subColl != null )
				{
					Traverse( subColl, handler );
				}
				else
				{
					handler( item );
				}
			}
		}

		[Test]
		public void testDispose()
		{
			MockModule module = new MockModule( "foo.dll" );
			MockContainer fixture = new MockContainer( "fixture" );
			MockItem test = new MockItem( "test" );
			fixture.Children.Add( test );
			module.Children.Add( fixture );

			Target target = new MockTarget( module );
			TestModule mod = TestModule.LoadModule(
				target, module.GetPath(), false );

			IRun run = new Run( this.policy, mod );

			Assert.AreSame( run.RootResult.Run, run );

			Traverse(
				run.RootResult,
				delegate( IResultItem item )
				{
					Assert.AreEqual( ExecutionStatus.Pending, item.Status );
				} );

			Traverse(
				run.RootResult,
				delegate( IResultItem item )
				{
					Assert.IsNotNull( item.Item );
				} );

			mod.Dispose();

			Traverse(
				run.RootResult,
				delegate( IResultItem item )
				{
					Assert.IsNull( item.Item );
				} );
		}

		[Test]
		public void testFixtureAllSuccess()
		{
			MockModule module = new MockModule( "foo.dll" );
			MockContainer testFixture = new MockContainer( "fixture" );
			MockItem test1 = new MockItem( "test1" );
			MockItem test2 = new MockItem( "test2" );
			testFixture.Children.Add( test1 );
			testFixture.Children.Add( test2 );
			module.Children.Add( testFixture );

			Target target = new MockTarget( module );
			TestModule mod = TestModule.LoadModule(
				target, module.GetPath(), false );

			IRun run = new Run( this.policy, mod );

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
		}

		[Test]
		public void testFixtureSucFirstFailSecondSkipThird()
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

			Target target = new MockTarget( module );
			TestModule mod = TestModule.LoadModule(
				target, module.GetPath(), false );

			IRun run = new Run( this.policy, mod );

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
			Assert.AreEqual( 1, statusChanges );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );

			//
			// 1st test succeeds.
			//
			Cfixctl.ICfixTestÌtemEventSink tcSink =
				( Cfixctl.ICfixTestÌtemEventSink ) fixture.GetItem( 0 );
			tcSink.BeforeTestCaseStart();
			Assert.AreEqual( 2, statusChanges );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.GetItem( 0 ).Status );
			
			tcSink.AfterTestCaseFinish( 1 );
			Assert.AreEqual( 3, statusChanges );

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
			Assert.AreEqual( 4, statusChanges );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );

			tcSink.UnhandledException( 1, 0, null );
			tcSink.AfterTestCaseFinish( 0 );
			Assert.AreEqual( 5, statusChanges );
			Assert.AreEqual(
				ExecutionStatus.Running,
				fixture.Status );

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
			Assert.AreEqual( 7, statusChanges );

			Assert.AreEqual(
				ExecutionStatus.Skipped,
				fixture.GetItem( 2 ).Status );
			Assert.AreEqual(
				ExecutionStatus.Failed,
				fixture.Status );
		}

	}
}
