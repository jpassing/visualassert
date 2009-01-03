using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Cfix.Control;

namespace Cfix.Control.Test
{
	[TestFixture]
	public class TestTestModule
	{
		private Target target;
		private String binDir;

		[SetUp]
		public void Setup()
		{
			this.target = Target.CreateLocalTarget(
				Architecture.I386,
				true );

			this.binDir = @"D:\dev\wdev\cfixplus\trunk\bin\chk\i386";
		}

		[Test]
		[ExpectedException( typeof( FileNotFoundException ) )]
		public void LoadNonexistingModule()
		{
			TestModule mod = TestModule.LoadModule(
				target, "idonotexist.dll" );
		}

		[Test]
		public void LoadTestlib10()
        {
            
            TestModule mod = TestModule.LoadModule(
                target, this.binDir + "\\testlib10.dll" );

            Assert.AreEqual( 0, mod.Ordinal );
            Assert.AreEqual( this.binDir + "\\testlib10.dll", mod.Path );

			Assert.AreEqual( 1, mod.ItemCount );

			ITestItemContainer fixture = ( ITestItemContainer ) mod.GetItem( 0 );
			Assert.AreEqual( 0, fixture.Ordinal );
			Assert.AreEqual( "TestExecActionDummy", fixture.Name );

			Assert.AreEqual( 2, fixture.ItemCount );

			ITestItem tc = fixture.GetItem( 0 );
			Assert.AreEqual( 0, tc.Ordinal );
			Assert.AreEqual( "Log", tc.Name );

			tc = fixture.GetItem( 1 );
			Assert.AreEqual( 1, tc.Ordinal );
			Assert.AreEqual( "Log", tc.Name );
		}
	}
}
