using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfix.Control.Diag;
using Cfixctl;

namespace Cfix.Control.Native
{
	[Flags]
	public enum HostCreationOptions
	{
		None = 0,
		UseJob = 1	// CFIXCTL_AGENT_FLAG_USE_JOB
	}

	/*++
	 * Class Description:
	 *		Wrapper for ICfixAgent.
	 * 
	 *		Threadsafe.
	 --*/
	public class Agent : IAgent
	{
#if DEBUG
		private const uint DefaultTimeout = 30000;
#else
		private const uint DefaultTimeout = 3000;
#endif

		private ICfixAgent agent;
		private ICfixMessageResolver resolver;

		private readonly CfixTestModuleArch arch;
		private readonly Clsctx clsctx;

		private uint timeout = DefaultTimeout;

		private readonly HostCreationOptions flags;

		private readonly LocalProcessWatcher processWatcher = new LocalProcessWatcher();

		//
		// Default environment; merged with specific environment.
		//
		private readonly HostEnvironment defaultHostEnv = new HostEnvironment();

		[Flags]
		private enum Clsctx
		{
			CLSCTX_INPROC_SERVER = 0x1,
			CLSCTX_LOCAL_SERVER = 0x4
		}

		/*--------------------------------------------------------------
		 * Ctor/Dtor.
		 */

		protected Agent(
			ICfixAgent agent,
			CfixTestModuleArch arch,
			bool allowInproc,
			HostCreationOptions flags
			)
		{
			Debug.Assert( agent != null );

			this.agent = agent;
			this.arch = arch;
			this.flags = flags;

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
				Debug.Assert( this.resolver != null );
			}
			catch ( COMException x )
			{
				Logger.LogError( "Agent", "Failed to create resolver", x );
				throw WrapException( x );
			}
		}

		~Agent()
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

			this.processWatcher.Dispose();
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

		public virtual void ReleaseObject( Object obj )
		{
			Marshal.ReleaseComObject( obj );
		}

		public void SetTrialLicenseCookie( uint cookie )
		{
			this.agent.SetTrialLicenseCookie( cookie );
		}

		/*--------------------------------------------------------------
		 * IAgent.
		 */

		public virtual IHost CreateHost()
		{
			return CreateHost( null, null );
		}

		public virtual IHost CreateHost(
			HostEnvironment env
			)
		{
			return CreateHost( null, env );
		}

		public virtual IHost CreateHost(
			string customHostPath,
			HostEnvironment env
			)
		{
			string currentDir = null;

			if ( env != null )
			{
				//
				// Bring in the default settings.
				//
				env = env.Merge( this.defaultHostEnv );

				currentDir = env.CurrentDirectory;
			}
			else
			{
				env = this.defaultHostEnv;
			}

			ICfixHost host = this.agent.CreateHost(
				this.arch, 
				( uint ) this.clsctx,
				( uint ) this.flags,
				this.timeout,
				customHostPath,
				env.NativeFormat,
				currentDir );

			Debug.Assert( host != null );

			//
			// Watch this process (local agent only!)
			//
			this.processWatcher.Watch( ( int ) host.GetHostProcessId() );

			return new Host( 
				this, 
				host,
				customHostPath != null,
				customHostPath != null 
					? customHostPath
					: this.agent.GetHostPath( this.arch ) );
		}

		public ITestItemCollection LoadModule(
			HostEnvironment env,
			ITestItemCollection parentCollection,
			string path,
			bool ignoreDuplicates
			)
		{
			if ( path.ToLower().EndsWith( ".exe" ) )
			{
				using ( IHost host = CreateHost( path, env ) )
				{
					return host.LoadModule(
						parentCollection,
						null,
						ignoreDuplicates );
				}
			}
			else
			{
				using ( IHost host = CreateHost( env ) )
				{
					return host.LoadModule(
						parentCollection,
						path,
						ignoreDuplicates );
				}
			}
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
			catch ( COMException x )
			{
				Logger.LogError( "Agent", "Failed to resolve", x );
				return String.Format( "[0x{0:X}]", code );
			}
		}

		public HostEnvironment DefaultEnvironment
		{
			get { return this.defaultHostEnv; }
		}

		public uint ActiveHostCount 
		{ 
			get { return this.processWatcher.ProcessCount; }
		}

		public void TerminateActiveHosts()
		{
			this.processWatcher.TerminateAll();
		}

		/*--------------------------------------------------------------
		 * Statics.
		 */

		public static Agent CreateLocalAgent(
			Architecture arch,
			bool allowInproc,
			HostCreationOptions flags
			)
		{
			return new Agent(
				new LocalAgentClass(),
				( CfixTestModuleArch ) arch,
				allowInproc,
				flags );
		}

		public static Agent CreateLocalAgent(
			Architecture arch,
			bool allowInproc
			)
		{
			return CreateLocalAgent(
				arch,
				allowInproc,
				HostCreationOptions.None );
		}

	}
}
