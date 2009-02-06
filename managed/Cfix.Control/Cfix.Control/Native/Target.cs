using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	[Flags]
	public enum HostCreationOptions
	{
		None = 0,
		UseJob = 1	// CFIXCTL_AGENT_FLAG_USE_JOB
	}

	public class Target : IDisposable
	{
		private const uint DefaultTimeout = 3000;

		private ICfixAgent agent;
		private ICfixMessageResolver resolver;

		private CfixTestModuleArch arch;
		private Clsctx clsctx;

		private uint timeout = DefaultTimeout;

		private readonly HostCreationOptions flags;
		private readonly String currentDirectory;

		[Flags]
		private enum Clsctx
		{
			CLSCTX_INPROC_SERVER = 0x1,
			CLSCTX_LOCAL_SERVER = 0x4
		}

		/*--------------------------------------------------------------
		 * Ctor/Dtor.
		 */

		protected Target(
			ICfixAgent agent,
			CfixTestModuleArch arch,
			bool allowInproc,
			HostCreationOptions flags,
			String currentDirectory
			)
		{
			Debug.Assert( agent != null );

			if ( agent == null )
			{
				throw new ArgumentException( "null", "agent" );
			}

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

			try
			{
				this.resolver = agent.CreateMessageResolver();
			}
			catch ( COMException x )
			{
				throw WrapException( x );
			}
		}

		~Target()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( this.agent != null )
			{
				ReleaseObject( this.agent );
				this.agent = null;
			}

			if ( this.resolver != null )
			{
				ReleaseObject( this.resolver );
				this.resolver = null;
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/*--------------------------------------------------------------
		 * Internal.
		 */

		internal CfixException WrapException( COMException x )
		{
			String message = ResolveMessage( x.ErrorCode );
			return new CfixException( message, x );
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

		public Architecture Architecture
		{
			get
			{
				return ( Architecture ) this.arch;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)" )]
		public String ResolveMessage( int code )
		{
			try
			{
				return this.resolver.ResolveMessage( ( uint ) code, 0 );
			}
			catch ( COMException )
			{
				return String.Format( "[0x{0:X}]", code );
			}
		}

		public virtual void ReleaseObject( Object obj )
		{
			Marshal.ReleaseComObject( obj );
		}

		/*--------------------------------------------------------------
		 * Statics.
		 */

		public static Target CreateLocalTarget(
			Architecture arch,
			bool allowInproc,
			HostCreationOptions flags,
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
				HostCreationOptions.None,
				null );
		}

	}
}
