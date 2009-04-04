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
		private AgentSet multiTarget;
		private String testdataDir;

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
			AgentSet tgt = new AgentSet();
			ITestItemCollection coll = this.inProcTarget.CreateHost().SearchModules(
				new DirectoryInfo( this.testdataDir ),
				"*",
				tgt,
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
			ITestItemCollection coll = this.inProcTarget.CreateHost().SearchModules(
				new DirectoryInfo( this.testdataDir ),
				"simple.dll",
				this.multiTarget,
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
			ITestItemCollection coll = this.inProcTarget.CreateHost().SearchModules(
				new DirectoryInfo( this.testdataDir ),
				"*.dll",
				this.multiTarget,
				true );
			coll.Refresh();

			Assert.AreEqual( "i386", coll.Name );
			Assert.AreEqual( 3, coll.ItemCount );

			Assert.IsNotNull( coll.GetItem( 0 ) );
			Assert.IsNotNull( coll.GetItem( 1 ) );

			int invalids = 0;
			foreach ( ITestItem item in coll )
			{
				InvalidModule inv = item as InvalidModule;
				if ( inv != null )
				{
					Assert.IsNotNull( inv.InvalidityCause );
					Assert.IsTrue( inv.Name == "toolong.dll" ||
								   inv.Name == "dupfixturename.dll" );

					invalids++;
				}
				else
				{
					Assert.IsInstanceOfType( typeof( TestModule ), item );
				}
			}

			Assert.AreEqual( 2, invalids );
		}

		[Test]
		public void SearchEmptyDir()
		{
			ITestItemCollection coll = this.inProcTarget.CreateHost().SearchModules(
				new DirectoryInfo( this.testdataDir + "\\dummy" ),
				"*",
				this.multiTarget,
				true );
			coll.Refresh();

			Assert.AreEqual( 0, coll.ItemCount );
		}

		[Test]
		public void SearchDir()
		{
			ITestItemCollection coll = this.inProcTarget.CreateHost().SearchModules(
				new DirectoryInfo( this.testdataDir ),
				"*",
				this.multiTarget,
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

			Assert.AreEqual( 2, invalids );
		}

		[Test]
		public void SearchDirRecursive()
		{
			ITestItemCollection coll = this.inProcTarget.CreateHost().SearchModules(
				new DirectoryInfo( this.testdataDir + "\\.." ),
				"*",
				this.multiTarget,
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

				Assert.AreEqual( 2, invalids );

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
