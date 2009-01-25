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
		private Target ooProcTarget;
		private Target inProcTarget;
		private MultiTarget multiTarget;
		private String testdataDir;

		private class Listener : TestModuleCollection.ISearchListener
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
			this.ooProcTarget = Target.CreateLocalTarget(
				Architecture.I386,
				false );
			this.inProcTarget = Target.CreateLocalTarget(
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
			Listener lst = new Listener();
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir ),
				"*",
				inProcTarget,
				tgt,
				true,
				true,
				lst );
			Assert.AreEqual( 0, lst.Invalids );
			Assert.AreEqual( 0, coll.ItemCount );
		}

		[Test]
		public void SearchSingleFile()
		{
			Listener lst = new Listener();
			TestModuleCollection coll = TestModuleCollection.Search(
				new DirectoryInfo( this.testdataDir ),
				"simple.dll",
				inProcTarget,
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
				inProcTarget,
				this.multiTarget,
				true,
				true,
				lst );

			Assert.AreEqual( 1, lst.Invalids );

			Assert.AreEqual( "i386", coll.Name );
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
				inProcTarget,
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
				inProcTarget,
				this.multiTarget,
				true,
				true,
				lst );

			Assert.AreEqual( 1, lst.Invalids );

			Assert.AreEqual( "i386", coll.Name );
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
				inProcTarget,
				this.multiTarget,
				true,
				true,
				lst );

			Assert.AreEqual( 1, lst.Invalids );

			Assert.AreEqual( "testdata", coll.Name );
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
