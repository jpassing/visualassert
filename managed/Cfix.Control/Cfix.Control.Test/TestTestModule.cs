using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Control.Test
{
	[TestFixture]
	public class TestTestModule
	{
		private Target target;
		private String testdataDir;

		[SetUp]
		public void Setup()
		{
			this.target = Target.CreateLocalTarget(
				Architecture.I386,
				false );

			String binDir = new FileInfo( 
				Assembly.GetExecutingAssembly().FullName ).Directory.FullName;

			this.testdataDir =
				binDir +
				@"\..\..\..\managed\Cfix.Control\Cfix.Control.Test\testdata\i386";
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
		[ExpectedException( typeof( FileNotFoundException ) )]
		public void LoadNonexistingModule()
		{
			TestModule mod = TestModule.LoadModule(
				target, "idonotexist.dll", true );
		}

		[Test]
		public void LoadNonTestlib()
		{
			TestModule mod = TestModule.LoadModule(
				target, this.testdataDir + "\\notatestdll.dll", true );
			Assert.AreEqual( this.testdataDir + "\\notatestdll.dll", mod.Path );
			Assert.AreEqual( 0, mod.ItemCount );

			DefaultEventSink sink = new DefaultEventSink();
			mod.CreateAction( SchedulingOptions.None ).Run( sink );

			Assert.AreEqual( 0, sink.FixtureStarts );
		}

		[Test]
		public void LoadTestlibWithEmptyFixture()
		{
			TestModule mod = TestModule.LoadModule(
				target, this.testdataDir + "\\simple.dll", true );

			Assert.AreEqual( 0, mod.Ordinal );
			Assert.AreEqual( this.testdataDir + "\\simple.dll", mod.Path );

			Assert.AreEqual( 1, mod.ItemCount );

			ITestItemCollection fixture = ( ITestItemCollection ) mod.GetItem( 0 );
			Assert.AreEqual( 0, fixture.Ordinal );
			Assert.AreEqual( "SampleFixture", fixture.Name );

			Assert.AreEqual( 0, fixture.ItemCount );

			DefaultEventSink sink = new DefaultEventSink();
			IAction action = mod.CreateAction( SchedulingOptions.None );
			Assert.AreEqual( 0, action.TestCaseCount );
			
			action.Run( sink );

			Assert.AreEqual( 1, sink.FixtureStarts );
			Assert.AreEqual( 1, sink.FixtureFinishs );
			Assert.AreEqual( 0, sink.TestCaseStarts );
		}

		[Test]
		[ExpectedException( typeof(  CfixException ) )]
		public void LoadTestlibWithInvalidFixtureName()
		{
			TestModule mod = TestModule.LoadModule(
				target, this.testdataDir + "\\toolong.dll", true );
		}

		[Test]
		[ExpectedException( typeof( BadImageFormatException ) )]
		public void LoadTestlibWithWrongArch()
		{
			TestModule mod = TestModule.LoadModule(
				target, this.testdataDir + "\\wrongarch.dll", true );
		}

		[Test]
		[ExpectedException( typeof( CfixException ) )]
		public void LoadTestlibWithAmbiguousTestCaseNamesAndFail()
        {
            TestModule mod = TestModule.LoadModule(
				target, this.testdataDir + "\\dupfixturename.dll", false );
		}

		[Test]
		public void LoadTestlibWithAmbiguousTestCaseNames()
		{
			TestModule mod = TestModule.LoadModule(
				target, this.testdataDir + "\\dupfixturename.dll", true );

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
