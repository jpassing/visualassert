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

	}
}
