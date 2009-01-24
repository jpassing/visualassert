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
		private Target target;
		private MultiTarget multiTarget;
		private String testdataDir;

		private class Listener : TestModuleCollection.SearchListener
		{
			public uint Invalids;

			public void InvalidModule( string path, string reason )
			{
				this.Invalids++;
			}
		}

		[SetUp]
		public void Setup()
		{
			this.target = Target.CreateLocalTarget(
				Architecture.I386,
				false );

			this.multiTarget = new MultiTarget();
			this.multiTarget.AddArchitecture( target );

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
		[ExpectedException( typeof( UnsupportedArchitectureException ) )]
		public void SearchWithNoTargets()
		{
			MultiTarget tgt = new MultiTarget();
			Listener lst = new Listener();
			TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir ),
				"*",
				tgt,
				true,
				true,
				lst );
			Assert.AreEqual( 0, lst.Invalids );
		}

		[Test]
		public void SearchSingleFile()
		{
			Listener lst = new Listener();
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir ),
				"simple.dll",
				this.multiTarget,
				true,
				true,
				lst );

			Assert.AreEqual( 0, lst.Invalids );
			Assert.AreEqual( 1, coll.ItemCount );

			ITestItem item = coll.GetItem( 0 );
			Assert.IsNotNull( item );
			Assert.AreEqual( "simple", item.Name );
		}

		[Test]
		public void SearchTwoFiles()
		{
			Listener lst = new Listener();
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir ),
				"*.dll",
				this.multiTarget,
				true,
				true,
				lst );

			Assert.AreEqual( 1, lst.Invalids );

			Assert.AreEqual( "i386", coll.Name );
			Assert.AreEqual( 1, coll.ItemCount );
			Assert.IsInstanceOfType( typeof( TestModuleCollection ), coll );

			coll = ( TestModuleCollection ) coll.GetItem( 0 );

			Assert.AreEqual( 2, coll.ItemCount );

			Assert.IsNotNull( coll.GetItem( 0 ) );
			Assert.IsNotNull( coll.GetItem( 1 ) );

			Assert.IsInstanceOfType( typeof( TestModule ), coll.GetItem( 0 ) );
			Assert.IsInstanceOfType( typeof( TestModule ), coll.GetItem( 1 ) );
		}

		[Test]
		public void SearchEmptyDir()
		{
			Listener lst = new Listener();
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir + "\\dummy" ),
				"*",
				this.multiTarget,
				true,
				true,
				lst );

			Assert.AreEqual( 0, lst.Invalids );
			Assert.AreEqual( 0, coll.ItemCount );
		}

		[Test]
		public void SearchDir()
		{
			Listener lst = new Listener();
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir ),
				"*",
				this.multiTarget,
				true,
				true,
				lst );

			Assert.AreEqual( 1, lst.Invalids );

			Assert.AreEqual( "i386", coll.Name );
			Assert.AreEqual( 1, coll.ItemCount );
			Assert.IsInstanceOfType( typeof( TestModuleCollection ), coll );

			coll = ( TestModuleCollection ) coll.GetItem( 0 );

			Assert.AreEqual( 2, coll.ItemCount );

			Assert.IsNotNull( coll.GetItem( 0 ) );
			Assert.IsNotNull( coll.GetItem( 1 ) );

			Assert.IsInstanceOfType( typeof( TestModule ), coll.GetItem( 0 ) );
			Assert.IsInstanceOfType( typeof( TestModule ), coll.GetItem( 1 ) );
		}

		[Test]
		public void SearchDirRecursive()
		{
			Listener lst = new Listener();
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir + "\\.." ),
				"*",
				this.multiTarget,
				true,
				true,
				lst );

			Assert.AreEqual( 1, lst.Invalids );
			Assert.AreEqual( 1, coll.ItemCount );
			ITestItem item = coll.GetItem( 0 );

			Assert.IsInstanceOfType( typeof( TestModuleCollection ), item );
			Assert.AreEqual( "i386", item.Name );

			coll = ( TestModuleCollection ) coll.GetItem( 0 );
			Assert.AreEqual( 2, coll.ItemCount );

			Assert.IsNotNull( coll.GetItem( 0 ) );
			Assert.IsNotNull( coll.GetItem( 1 ) );

			Assert.IsInstanceOfType( typeof( TestModule ), coll.GetItem( 0 ) );
			Assert.IsInstanceOfType( typeof( TestModule ), coll.GetItem( 1 ) );
		}
	}
}
