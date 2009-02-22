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

		private static NativeConnection Connect( Target target, String path )
		{
			try
			{
				ICfixHost host = target.CreateHost();
				try
				{
					ICfixTestModule mod = host.LoadModule( path );

					return new NativeConnection(
						host,
						mod );
				}
				catch ( COMException x )
				{
					target.ReleaseObject( host );
					throw target.WrapException( x );
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

		private NativeConnection Connect()
		{
			return Connect( this.target, this.path );
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		internal override NativeConnection NativeConnection
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

		public Architecture Architecture
		{
			get { return this.target.Architecture; }
		}

		public void Update()
		{
			if ( this.target == null )
			{
				throw new ArgumentException();
			}

			NativeConnection connection = Connect();
			try
			{
				Update( ( ICfixTestModule ) connection.Item );
			}
			finally
			{
				this.target.ReleaseObject( connection.Item );
				this.target.ReleaseObject( connection.Host );
			}
		}

		public CfixTestModuleType Type
		{
			get { return this.type; }
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

			NativeConnection connection = Connect( target, path );
			try
			{
				ICfixTestModule ctlModule = ( ICfixTestModule ) connection.Item;

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
				target.ReleaseObject( connection.Item );
				target.ReleaseObject( connection.Host );
			}
		}
	}
}
