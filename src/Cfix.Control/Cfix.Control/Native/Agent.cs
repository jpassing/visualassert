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

	/*++
	 * Class Description:
	 *		Wrapper for ICfixAgent.
	 * 
	 *		Threadsafe.
	 --*/
	public class Agent : IAgent
	{
		private ICfixAgent agent;
		private ICfixMessageResolver resolver;

		public const uint DefaultHostRegistrationTimeout = 3000;

		private readonly CfixTestModuleArch arch;
		private readonly Clsctx clsctx;

		private uint timeout;

		private readonly HostCreationOptions flags;
		private readonly EventDll eventDll;

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
			HostCreationOptions flags,
			uint hostRegistrationTimeout,
			EventDll eventDll	// optional
			)
		{
			Debug.Assert( agent != null );

			this.agent = agent;
			this.arch = arch;
			this.flags = flags;
			this.timeout = hostRegistrationTimeout;
			this.eventDll = eventDll;

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
		 * Protected.
		 */

		protected virtual ICfixHost CreateNativeHost(
			ICfixAgent agent,
			CfixTestModuleArch arch,
			uint clsctx,
			uint flags,
			uint timeout,
			string customHostPath,
			HostEnvironment env,
			string currentDir )
		{
			Debug.Assert( timeout > 0 );

			return agent.CreateHost(
				arch,
				( uint ) clsctx,
				( uint ) flags,
				timeout,
				customHostPath,
				env.NativeFormat,
				currentDir );
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

		public uint HostRegistrationTimeout
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

		public IHost CreateHost()
		{
			return CreateHost( null, null );
		}

		public IHost CreateHost(
			HostEnvironment env
			)
		{
			return CreateHost( null, env );
		}

		public IHost CreateHost(
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

			ICfixHost host = CreateNativeHost(
				this.agent,
				this.arch, 
				( uint ) this.clsctx,
				( uint ) this.flags,
				this.timeout,
				customHostPath,
				env,
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
					: this.agent.GetHostPath( this.arch ),
				this.eventDll );
		}

		public ITestItemCollection LoadModule(
			HostEnvironment env,
			ITestItemCollection parentCollection,
			string path,
			bool ignoreDuplicates
			)
		{
			if ( path.EndsWith( ".exe", StringComparison.OrdinalIgnoreCase ) )
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

		public String ResolveMessage( Exception x )
		{
			COMException comex = x as COMException;
			if ( comex == null )
			{
				return x.ToString();
			}
			else
			{
				return ResolveMessage( comex.ErrorCode );
			}
		}

		public CfixException WrapException( Exception x )
		{
			CfixException cfixEx = x as CfixException;
			if ( cfixEx != null )
			{
				return cfixEx;
			}

			COMException comEx = x as COMException;
			if ( comEx == null )
			{
				return new CfixException( x.Message, x );
			}
			else
			{
				return new CfixException( ResolveMessage( comEx.ErrorCode ), x );
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
			HostCreationOptions flags,
			uint hostRegistrationTimeout,
			EventDll eventDll
			)
		{
			return new Agent(
				new LocalAgentClass(),
				( CfixTestModuleArch ) arch,
				allowInproc,
				flags,
				hostRegistrationTimeout,
				eventDll );
		}

		public static Agent CreateLocalAgent(
			Architecture arch,
			bool allowInproc,
			uint hostRegistrationTimeout,
			EventDll eventDll
			)
		{
			return CreateLocalAgent(
				arch,
				allowInproc,
				HostCreationOptions.None,
				hostRegistrationTimeout,
				eventDll );
		}

		public static Agent CreateLocalAgent(
			Architecture arch,
			bool allowInproc,
			uint hostRegistrationTimeout
			)
		{
			return CreateLocalAgent(
				arch,
				allowInproc,
				HostCreationOptions.None,
				hostRegistrationTimeout,
				null );
		}
	}
}
