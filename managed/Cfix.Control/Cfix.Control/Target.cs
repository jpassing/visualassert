using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control
{
	public enum Architecture
	{
		I386 = CfixTestModuleArch.CfixTestModuleArchI386,
		Amd64 = CfixTestModuleArch.CfixTestModuleArchAmd64
	}

	public class Target : IDisposable
	{
		private ICfixAgent agent;
		private CfixTestModuleArch arch;
		private Clsctx clsctx;

		[Flags]
		private enum Clsctx
		{
			CLSCTX_INPROC_SERVER = 0x1,
			CLSCTX_LOCAL_SERVER = 0x4
		}

		protected Target(
			ICfixAgent agent,
			CfixTestModuleArch arch,
			bool allowInproc
			)
		{
			Debug.Assert( agent != null );

			this.agent = agent;
			this.arch = arch;

			if ( allowInproc )
			{
				this.clsctx = Clsctx.CLSCTX_INPROC_SERVER | Clsctx.CLSCTX_LOCAL_SERVER;
			}
			else
			{
				this.clsctx = Clsctx.CLSCTX_LOCAL_SERVER;
			}
		}

		/*--------------------------------------------------------------
		 * Publics.
		 */

		public static Target CreateLocalTarget(
			Architecture arch,
			bool allowInproc
			)
		{
			return new Target(
				new LocalAgentClass(),
				( CfixTestModuleArch ) arch,
				allowInproc );
		}

		~Target()
		{
			Dispose( false );
		}

		public virtual ICfixHost CreateHost()
		{
			ICfixHost host = this.agent.CreateHost( this.arch, ( uint ) this.clsctx );

			Debug.Assert( host != null );
			return host;
		}

		public CfixTestModuleArch Architecture
		{
			get
			{
				return this.arch;
			}
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( this.agent != null )
			{
				ReleaseObject( this.agent );
				this.agent = null;
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		public virtual void ReleaseObject( Object obj )
		{
			Marshal.ReleaseComObject( obj );
		}
	}
}
