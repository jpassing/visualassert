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

	[Flags]
	public enum HostCreationFlags
	{
		None = 0,
		UseJob = 1	// CFIXCTL_AGENT_FLAG_USE_JOB
	}

	public class Target : IDisposable
	{
		private const uint DefaultTimeout = 3000;

		private ICfixAgent agent;
		private CfixTestModuleArch arch;
		private Clsctx clsctx;

		private uint timeout = DefaultTimeout;

		private readonly HostCreationFlags flags;
		private readonly String currentDirectory;

		[Flags]
		private enum Clsctx
		{
			CLSCTX_INPROC_SERVER = 0x1,
			CLSCTX_LOCAL_SERVER = 0x4
		}

		protected Target(
			ICfixAgent agent,
			CfixTestModuleArch arch,
			bool allowInproc,
			HostCreationFlags flags,
			String currentDirectory
			)
		{
			Debug.Assert( agent != null );

			this.agent = agent;
			this.arch = arch;
			this.flags = flags;
			this.currentDirectory = currentDirectory;

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

		public uint Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		public static Target CreateLocalTarget(
			Architecture arch,
			bool allowInproc,
			HostCreationFlags flags,
			String currentDirectory
			)
		{
			return new Target(
				new LocalAgentClass(),
				( CfixTestModuleArch ) arch,
				allowInproc,
				flags,
				currentDirectory );
		}

		public static Target CreateLocalTarget(
			Architecture arch,
			bool allowInproc
			)
		{
			return CreateLocalTarget(
				arch,
				allowInproc,
				HostCreationFlags.None,
				null );
		}

		~Target()
		{
			Dispose( false );
		}

		public virtual ICfixHost CreateHost()
		{
			ICfixHost host = this.agent.CreateHost(
				this.arch, 
				( uint ) this.clsctx,
				( uint ) this.flags,
				this.timeout,
				currentDirectory );

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
