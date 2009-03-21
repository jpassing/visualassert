using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using Cfix.Control;
using Cfix.Control.Native;
using Cfix.Control.RunControl;
using EnvDTE;
using EnvDTE80;

namespace Cfix.Addin
{
	public class Workspace : IDisposable
	{
		private readonly CfixPlus addin;
		private readonly Configuration config;
		private readonly ToolWindows toolWindows;
		private readonly Agent searchAgent;
		private readonly AgentSet runAgents;
		private readonly ISession session;

		private readonly object runLock = new object();

		/*----------------------------------------------------------------------
		 * Private - Agent creation.
		 */

		private static Architecture GetNativeArchitecture()
		{
			//
			// N.B. Use CfixklGetNativeSystemInfo for downlevel compat.
			//
			Native.SYSTEM_INFO info = new Native.SYSTEM_INFO();
			Native.CfixklGetNativeSystemInfo( ref info );

			switch ( info.processorArchitecture )
			{
				case Native.PROCESSOR_ARCHITECTURE_AMD64:
					return Architecture.Amd64;

				case Native.PROCESSOR_ARCHITECTURE_INTEL:
					return Architecture.I386;

				default:
					throw new CfixAddinException(
						Strings.UnsupportedArchitecture );
			}
		}

		/*++
		 * Create AgentSet for all supported architectures.
		 --*/
		private static AgentSet CreateRunAgent()
		{
			AgentSet target = new AgentSet();
			switch ( GetNativeArchitecture() )
			{
				case Architecture.Amd64:
					target.AddArchitecture(
						Agent.CreateLocalAgent(
							Architecture.Amd64,
							false ) );
					target.AddArchitecture(
						Agent.CreateLocalAgent(
							Architecture.I386,
							false ) );

					break;

				case Architecture.I386:
					target.AddArchitecture(
						Agent.CreateLocalAgent(
							Architecture.I386,
							false ) );
					break;

				default:
					throw new CfixAddinException(
						Strings.UnsupportedArchitecture );
			}

			return target;
		}

		/*----------------------------------------------------------------------
		 * Private - Run.
		 */

		/*----------------------------------------------------------------------
		 * Private - Run events.
		 * 
		 * N.B. Execute on non-GUI thread.
		 */

		private void run_Log( object sender, LogEventArgs e )
		{
			
		}

		/*----------------------------------------------------------------------
		 * Private - Misc.
		 */

		private IDispositionPolicy DispositionPolicy
		{
			get
			{
				return new StandardDispositionPolicy(
					this.config.DefaultUnhandledExceptionDisposition,
					this.config.DefaultFailedAssertionDisposition );
			}
		}

		private Window OutputWindow
		{
			get
			{
				return this.addin.DTE.Windows.Item(
					"{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}" );
			}
		}

		private bool BuildNodeIfRequired( ITestItem item )
		{
			//
			// Walk ancestry and build if required.
			//
			while ( item != null )
			{
				IBuildableTestItem buildItem = item as IBuildableTestItem;
				if ( buildItem != null )
				{
					//
					// Build. 
					//
					OutputWindow.Activate();

					return buildItem.Build();
				}

				item = item.Parent;
			}

			//
			// No need to build.
			//
			return true;
		}

		private Process2 FindProcess( Debugger2 debugger, uint pid )
		{
			foreach( EnvDTE.Process proc in debugger.LocalProcesses )
			{
				if ( proc.ProcessID == pid )
				{
					return ( Process2 ) proc;
				}
			}

			return null;
		}

		/*----------------------------------------------------------------------
		 * ctor/dtor.
		 */

		internal Workspace( CfixPlus addin )
		{
			this.addin = addin;
			this.toolWindows = new ToolWindows( addin );

			//
			// N.B. Search target is always i386 and inproc.
			//
			this.searchAgent = Agent.CreateLocalAgent( Architecture.I386, true );
			
			this.runAgents = CreateRunAgent();
			this.config = Configuration.Load();
			this.session = new Session();
		}

		~Workspace()
		{
			Dispose( false );
		}

		protected void Dispose( bool disposing )
		{
			if ( this.toolWindows != null )
			{
				this.toolWindows.SaveWindowState();
				this.toolWindows.CloseAll();
				this.toolWindows.Dispose();
			}

			if ( this.searchAgent != null )
			{
				this.searchAgent.Dispose();
			}

			if ( this.runAgents != null )
			{
				this.runAgents.Dispose();
			}

			if ( this.config != null )
			{
				this.config.Dispose();
			}

			this.addin.DTE.Debugger.DetachAll();
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		internal ToolWindows ToolWindows
		{
			get { return this.toolWindows; }
		}

		public Configuration Configuration
		{
			get { return this.config; }
		}

		public Agent SearchAgent
		{
			get { return this.searchAgent; }
		}

		public AgentSet RunAgent
		{
			get { return this.runAgents; }
		}

		public bool IsSolutionOpened
		{
			get
			{
				Solution curSolution = this.addin.DTE.Solution;
				return curSolution.Projects.Count > 0;
			}
		}

		public bool IsDebuggingPossible
		{
			get
			{
				//
				// Without a project, debugging does not work properly.
				// Moreover, we need a project to ontain the architecture
				// to be used for debugging.
				//
				Solution curSolution = this.addin.DTE.Solution;
				return ( curSolution.Projects.Count > 0 );
			}
		}

		public ISession Session
		{
			get { return this.session; }
		}

		public void RunItem( IRunnableTestItem item, bool debug )
		{
			Debug.Assert( IsDebuggingPossible );

			if ( !BuildNodeIfRequired( item ) )
			{
				//
				// Bail out. Errors should have been provided by VS.
				//
				return;
			}

			bool allowArchMixing;
			if ( debug )
			{
				//
				// N.B. When debugging, it is crucial to use a single host
				// process only. Debug runs are thus limited to a single
				// architecture only.
				//
				allowArchMixing = false;
			}
			else
			{
				allowArchMixing = true;
			}

			SimpleRunCompiler compiler = new SimpleRunCompiler(
				this.runAgents,
				this.DispositionPolicy,
				this.config.SchedulingOptions,
				this.config.ThreadingOptions,
				allowArchMixing );
			compiler.Add( item );

			IRun run = compiler.Compile();
			run.Log +=new EventHandler<LogEventArgs>( run_Log );

			if ( debug )
			{
				run.HostSpawned += delegate( object sender, HostEventArgs e )
				{
					try
					{
						Debugger2 debugger = ( Debugger2 ) this.addin.DTE.Debugger;
						Process2 process = FindProcess( debugger, e.HostProcessId );
						if ( process == null )
						{
							//
							// Weird, must be gone already. Nop.
							//
							return;
						}

						process.Attach();

						//
						// N.B. By default, VS hides our tool windows when
						// going into debug mode.
						//
						this.toolWindows.Run.Activate();
					}
					catch ( Exception x )
					{
						CfixPlus.HandleError( x );
						run.Terminate();
					}
				};

				//run.BeforeTerminate += delegate( object sender, HostEventArgs e )
				//{
				//    //
				//    // To allow terminating via our own UI while being
				//    // broken in, terminate via DTE API.
				//    //

				//    Debugger2 debugger = ( Debugger2 ) this.addin.DTE.Debugger;
				//    Process2 process = FindProcess( debugger, e.HostProcessId );
				//    if ( process == null )
				//    {
				//        //
				//        // Weird, must be gone already. Nop.
				//        //
				//        return;
				//    }

				//    process.Terminate( false );
				//};
			}

			try
			{
				this.toolWindows.Run.UserControl.Run = run;
				this.toolWindows.Run.Activate();
				this.toolWindows.Run.UserControl.StartRun();
			}
			catch ( ConcurrentRunException )
			{
				//
				// Another run is still active. Dispose run as it will
				// not ever be started.
				//
				run.Dispose();
				throw;
			}
		}


	}
}
