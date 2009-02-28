using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Control.Test
{
	[TestFixture]
	public class TestTestModuleCollection
	{
		private Agent ooProcTarget;
		private Agent inProcTarget;
		private MultiTarget multiTarget;
		private String testdataDir;

		[SetUp]
		public void Setup()
		{
			this.ooProcTarget = Agent.CreateLocalTarget(
				Architecture.I386,
				false );
			this.inProcTarget = Agent.CreateLocalTarget(
				Architecture.I386,
				true );

			this.multiTarget = new MultiTarget();
			this.multiTarget.AddArchitecture( ooProcTarget );

			String binDir = new FileInfo(
				Assembly.GetExecutingAssembly().FullName ).Directory.FullName;

			this.testdataDir =
				binDir +
				@"\..\..\..\managed\Cfix.Control\Cfix.Control.Test\testdata\i386";
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

		[Test]
		public void SearchWithNoTargets()
		{
			MultiTarget tgt = new MultiTarget();
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir ),
				"*",
				inProcTarget,
				tgt,
				true,
				true );
			try
			{
				coll.Refresh();
				Assert.Fail( "Expected exception" );
			}
			catch ( ArgumentException )
			{ }
		}

		[Test]
		public void SearchSingleFile()
		{
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir ),
				"simple.dll",
				inProcTarget,
				this.multiTarget,
				true,
				true );
			coll.Refresh();

			Assert.AreEqual( 1, coll.ItemCount );

			ITestItem item = coll.GetItem( 0 );
			Assert.IsInstanceOfType( typeof( TestModule ), item );
			Assert.IsNotNull( item );
			Assert.AreEqual( "simple", item.Name );
			Assert.AreSame( item.Parent, coll );
		}

		[Test]
		public void SearchTwoFiles()
		{
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir ),
				"*.dll",
				inProcTarget,
				this.multiTarget,
				true,
				true );
			coll.Refresh();

			Assert.AreEqual( "i386", coll.Name );
			Assert.AreEqual( 3, coll.ItemCount );

			Assert.IsNotNull( coll.GetItem( 0 ) );
			Assert.IsNotNull( coll.GetItem( 1 ) );

			int invalids = 0;
			foreach ( ITestItem item in coll )
			{
				if ( item is InvalidModule )
				{
					invalids++;
				}
				else
				{
					Assert.IsInstanceOfType( typeof( TestModule ), item );
				}
			}

			Assert.AreEqual( 1, invalids );
		}

		[Test]
		public void SearchEmptyDir()
		{
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir + "\\dummy" ),
				"*",
				inProcTarget,
				this.multiTarget,
				true,
				true );
			coll.Refresh();

			Assert.AreEqual( 0, coll.ItemCount );
		}

		[Test]
		public void SearchDir()
		{
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir ),
				"*",
				inProcTarget,
				this.multiTarget,
				true,
				true );
			coll.Refresh();

			Assert.AreEqual( "i386", coll.Name );
			Assert.AreEqual( 3, coll.ItemCount );

			Assert.IsNotNull( coll.GetItem( 0 ) );
			Assert.IsNotNull( coll.GetItem( 1 ) );

			int invalids = 0;
			foreach ( ITestItem item in coll )
			{
				if ( item is InvalidModule )
				{
					invalids++;
				}
				else
				{
					Assert.IsInstanceOfType( typeof( TestModule ), item );
				}
			}

			Assert.AreEqual( 1, invalids );
		}

		[Test]
		public void SearchDirRecursive()
		{
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir + "\\.." ),
				"*",
				inProcTarget,
				this.multiTarget,
				true,
				true );
			coll.Refresh();

			for ( int i = 0; i < 3; i++ )
			{
				Assert.AreEqual( "testdata", coll.Name );
				Assert.AreEqual( 1, coll.ItemCount );
				ITestItem item = coll.GetItem( 0 );

				Assert.IsInstanceOfType( typeof( TestModuleCollection ), item );
				Assert.AreEqual( "i386", item.Name );

				TestModuleCollection subColl = ( TestModuleCollection ) coll.GetItem( 0 );
				Assert.AreSame( subColl.Parent, coll );

				Assert.AreEqual( 3, subColl.ItemCount );

				int invalids = 0;
				foreach ( ITestItem subitem in subColl )
				{
					if ( subitem is InvalidModule )
					{
						invalids++;
					}
					else
					{
						Assert.IsInstanceOfType( typeof( TestModule ), subitem );
					}
				}

				Assert.AreEqual( 1, invalids );

				Assert.IsNotNull( subColl.GetItem( 0 ) );
				Assert.IsNotNull( subColl.GetItem( 1 ) );
				Assert.AreSame( subColl.GetItem( 0 ).Parent.Parent, coll );

				if ( i == 0 )
				{
					coll.Refresh();
				}
				else
				{
					subColl.Refresh();
				}
			}
		}
	}
}
