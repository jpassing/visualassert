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
				false );

			this.binDir = @"D:\dev\wdev\cfixplus\trunk\bin\chk\i386";
		}

		[TearDown]
		public void TearDown()
		{
			this.target.Dispose();
		}

		[Test]
		[ExpectedException( typeof( FileNotFoundException ) )]
		public void LoadNonexistingModule()
		{
			TestModule mod = TestModule.LoadModule(
				target, "idonotexist.dll" );
		}

		[Test]
		[ExpectedException( typeof( CfixException ) )]
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

		[Test]
		public void UpdateWithNoChanges()
		{
			MockModule module = new MockModule( "foo.dll" );
			module.Children.Add( new MockItem( "foo" ) );
			
			Target target = new MockTarget( module );
			TestModule mod = TestModule.LoadModule(
				target, module.GetPath() );

			Assert.AreEqual( 1, mod.ItemCount );

			ITestItem item = mod.GetItem( 0 );
			Assert.AreEqual( 0, item.Ordinal );
			Assert.AreEqual( "foo", item.Name );

			mod.ItemRemoved += delegate( ITestItemContainer sender, ITestItem it )
			{
				Assert.Fail( "Should not be called " );
			};

			mod.ItemAdded += delegate( ITestItemContainer sender, ITestItem it )
			{
				Assert.Fail( "Should not be called " );
			};

			mod.Update();
			mod.Update();

			Assert.AreEqual( 1, mod.ItemCount );
			Assert.AreEqual( 0, mod.GetItem( 0 ).Ordinal );
			Assert.AreEqual( "foo", mod.GetItem( 0 ).Name );
		}

		[Test]
		public void UpdateAddAndRemove()
		{
			MockModule module = new MockModule( "foo.dll" );
			module.Children.Add( new MockItem( "foo" ) );

			Target target = new MockTarget( module );
			TestModule mod = TestModule.LoadModule(
				target, module.GetPath() );

			Assert.AreEqual( 1, mod.ItemCount );

			ITestItem item0 = mod.GetItem( 0 );
			Assert.AreEqual( 0, item0.Ordinal );
			Assert.AreEqual( "foo", item0.Name );

			int deletions = 0;
			int additions = 0;

			mod.ItemRemoved += delegate( ITestItemContainer sender, ITestItem it )
			{
				deletions++;
			};

			mod.ItemAdded += delegate( ITestItemContainer sender, ITestItem it )
			{
				additions++;
			};

			//
			// Append one.
			//
			module.Children.Add( new MockItem( "bar" ) );

			mod.Update();

			Assert.AreEqual( 1, additions );
			Assert.AreEqual( 0, deletions );

			additions = 0;

			Assert.AreEqual( 2, mod.ItemCount );
			Assert.AreSame( item0, mod.GetItem( 0 ) );
			Assert.AreEqual( 0, mod.GetItem( 0 ).Ordinal );
			Assert.AreEqual( "foo", mod.GetItem( 0 ).Name );
			Assert.AreEqual( 1, mod.GetItem( 1 ).Ordinal );
			Assert.AreEqual( "bar", mod.GetItem( 1 ).Name );

			//
			// Delete last.
			//
			module.Children.RemoveAt( 1 );

			mod.Update();

			Assert.AreEqual( 0, additions );
			Assert.AreEqual( 1, deletions );

			Assert.AreEqual( 1, mod.ItemCount );
			Assert.AreEqual( 0, mod.GetItem( 0 ).Ordinal );
			Assert.AreEqual( "foo", mod.GetItem( 0 ).Name );
		}

		[Test]
		public void UpdateAddNameCollision()
		{
			MockModule module = new MockModule( "foo.dll" );
			module.Children.Add( new MockItem( "foo" ) );

			Target target = new MockTarget( module );
			TestModule mod = TestModule.LoadModule(
				target, module.GetPath() );

			Assert.AreEqual( 1, mod.ItemCount );

			ITestItem item = mod.GetItem( 0 );
			Assert.AreEqual( 0, item.Ordinal );
			Assert.AreEqual( "foo", item.Name );

			int deletions = 0;
			int additions = 0;

			mod.ItemRemoved += delegate( ITestItemContainer sender, ITestItem it )
			{
				deletions++;
			};

			mod.ItemAdded += delegate( ITestItemContainer sender, ITestItem it )
			{
				additions++;
			};

			//
			// Append one.
			//
			module.Children.Add( new MockItem( "foo" ) );

			try
			{
				mod.Update();
				Assert.Fail();
			}
			catch ( CfixException )
			{ }

			Assert.AreEqual( 1, deletions );
			Assert.AreEqual( 0, mod.ItemCount );
		}

		[Test]
		public void UpdateAddMovePosition()
		{
			MockModule module = new MockModule( "foo.dll" );
			module.Children.Add( new MockItem( "foo" ) );

			Target target = new MockTarget( module );
			TestModule mod = TestModule.LoadModule(
				target, module.GetPath() );

			Assert.AreEqual( 1, mod.ItemCount );

			ITestItem item = mod.GetItem( 0 );
			Assert.AreEqual( 0, item.Ordinal );
			Assert.AreEqual( "foo", item.Name );

			int deletions = 0;
			int additions = 0;

			mod.ItemRemoved += delegate( ITestItemContainer sender, ITestItem it )
			{
				deletions++;
			};

			mod.ItemAdded += delegate( ITestItemContainer sender, ITestItem it )
			{
				additions++;
			};

			//
			// Append one.
			//
			module.Children.Add( new MockItem( "bar" ) );

			mod.Update();

			Assert.AreEqual( 1, additions );
			Assert.AreEqual( 0, deletions );

			additions = 0;

			Assert.AreEqual( 2, mod.ItemCount );
			Assert.AreEqual( 0, mod.GetItem( 0 ).Ordinal );
			Assert.AreEqual( "foo", mod.GetItem( 0 ).Name );
			Assert.AreEqual( 1, mod.GetItem( 1 ).Ordinal );
			Assert.AreEqual( "bar", mod.GetItem( 1 ).Name );

			//
			// Move #2 to #1.
			//
			module.Children.Clear();
			module.Children.Add( new MockItem( "bar" ) );

			mod.Update();

			Assert.AreEqual( 1, additions );
			Assert.AreEqual( 2, deletions );

			Assert.AreEqual( 1, mod.ItemCount );
			Assert.AreEqual( 0, mod.GetItem( 0 ).Ordinal );
			Assert.AreEqual( "bar", mod.GetItem( 0 ).Name );
		}

		[Test]
		public void UpdateReplace()
		{
			MockModule module = new MockModule( "foo.dll" );
			module.Children.Add( new MockItem( "foo" ) );

			Target target = new MockTarget( module );
			TestModule mod = TestModule.LoadModule(
				target, module.GetPath() );

			Assert.AreEqual( 1, mod.ItemCount );

			ITestItem item = mod.GetItem( 0 );
			Assert.AreEqual( 0, item.Ordinal );
			Assert.AreEqual( "foo", item.Name );

			int deletions = 0;
			int additions = 0;

			mod.ItemRemoved += delegate( ITestItemContainer sender, ITestItem it )
			{
				deletions++;
			};

			mod.ItemAdded += delegate( ITestItemContainer sender, ITestItem it )
			{
				additions++;
			};

			//
			// Replace #1.
			//
			module.Children.Clear();
			module.Children.Add( new MockItem( "bar" ) );

			mod.Update();

			Assert.AreEqual( 1, additions );
			Assert.AreEqual( 1, deletions );

			Assert.AreEqual( 1, mod.ItemCount );
			Assert.AreEqual( 0, mod.GetItem( 0 ).Ordinal );
			Assert.AreEqual( "bar", mod.GetItem( 0 ).Name );
		}
	}
}
