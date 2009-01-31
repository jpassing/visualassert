using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Cfixctl;

namespace Cfix.Control.Native
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix" )]
	public class TestModule : TestItemCollection
	{
		private readonly String path;
		private readonly Target target;

		//
		// N.B. We have to use a separate parent (rather than relying
		// on base.parent) to allow arbitrary objects implementing 
		// ITestItemCollection to serve as parent.
		//
		private readonly ITestItemCollection parentCollection;

		private CfixTestModuleType type = 0;

		private TestModule(
			ITestItemCollection parentCollection,
			String path,
			Target target,
			ICfixTestModule ctlModule,
			bool ignoreDuplicates
			)
			: base( null, 0, ctlModule, ignoreDuplicates )
		{
			this.parentCollection = parentCollection;
			this.path = path;
			this.target = target;
		}

		private static ICfixTestModule Connect( Target target, String path )
		{
			try
			{
				ICfixHost host = target.CreateHost();
				try
				{
					return host.LoadModule( path );
				}
				finally
				{
					target.ReleaseObject( host );
				}
			}
			catch ( FileNotFoundException x )
			{
				throw new FileNotFoundException(
					String.Format( "Module {0} not found: {1} ",
						path,
						x.Message ) );
			}
			catch ( COMException x )
			{
				throw target.WrapException( x );
			}
		}

		private ICfixTestModule Connect()
		{
			return Connect( this.target, this.path );
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		public override ICfixTestItem NativeItem
		{
			get
			{
				return Connect();
			}
		}

		public override void Refresh()
		{
			Update();
		}

		public override ITestItemCollection Parent
		{
			get
			{
				return this.parentCollection;
			}
		}

		/*--------------------------------------------------------------
		 * Publics.
		 */

		internal override TestModule Module
		{
			get
			{
				return this;
			}
		}

		internal Target Target
		{
			get
			{
				return this.target;
			}
		}

		public void Update()
		{
			if ( this.target == null )
			{
				throw new ArgumentException();
			}

			ICfixTestModule ctlModule = Connect();
			try
			{
				Update( ctlModule );
			}
			finally
			{
				this.target.ReleaseObject( ctlModule );
			}
		}

		public CfixTestModuleType Type
		{
			get
			{
				return this.type;
			}
		}

		public String Path
		{
			get
			{
				return this.path;
			}
		}

		/*--------------------------------------------------------------
		 * Statics.
		 */

		public static TestModule LoadModule(
			Target target,
			String path,
			bool ignoreDuplicates
			)
		{
			return LoadModule( null, target, path, ignoreDuplicates );
		}

		public static TestModule LoadModule(
			ITestItemCollection parentCollection,
			Target target,
			String path,
			bool ignoreDuplicates
			)
		{
			if ( target == null || path == null )
			{
				throw new ArgumentException( "" );
			}

			ICfixTestModule ctlModule = Connect( target, path );
			try
			{
				TestModule mod = new TestModule(
					parentCollection,
					path,
					target,
					ctlModule,
					ignoreDuplicates );

				mod.Update( ctlModule );

				return mod;
			}
			finally
			{
				target.ReleaseObject( ctlModule );
			}
		}
	}
}
