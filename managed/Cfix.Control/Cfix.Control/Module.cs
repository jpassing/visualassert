using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Cfixctl;

namespace Cfix.Control
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix" )]
	public class TestModule : TestItemCollection
	{
		private readonly String path;
		private readonly Target target;

		private CfixTestModuleType type = 0;

		private TestModule(
			String path,
			Target target,
			ICfixTestModule ctlModule
			)
			: base( null, 0, ctlModule )
		{
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
		}

		private ICfixTestModule Connect()
		{
			return Connect( this.target, this.path );
		}

		/*--------------------------------------------------------------
		 * ITestItem.
		 */

		public override ICfixTestItem NativeItem
		{
			get
			{
				return Connect();
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

		public static TestModule LoadModule(
			Target target,
			String path
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
					path,
					target,
					ctlModule );

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
