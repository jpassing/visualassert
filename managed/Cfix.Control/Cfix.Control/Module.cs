using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Cfixctl;

namespace Cfix.Control
{
	public class TestModule : TestItemContainer
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
					Marshal.ReleaseComObject( host );
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

		private void Update( ICfixTestModule ctlModule )
		{
			Update( ( ICfixTestContainer ) ctlModule );
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

		public void Update()
		{
			if ( target == null )
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
				Marshal.ReleaseComObject( ctlModule );
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
				Marshal.ReleaseComObject( ctlModule );
			}
		}
	}
}
