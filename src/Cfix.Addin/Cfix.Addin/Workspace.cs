/*----------------------------------------------------------------------
 * Purpose:
 *		Workspace.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

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
		private readonly string VSStd97CmdID = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";

		private readonly CfixStudio addin;
		private readonly Configuration config;
		private readonly ToolWindows toolWindows;
		private readonly IAgent searchAgent;
		private readonly AgentSet runAgents;
		private readonly ISession session;
		private readonly CommandEvents cmdEvents;

		private readonly object runLock = new object();

		private IRunnableTestItem lastItemRun;

		private LicenseInfo cachedLicenseInfo;

		private class DebugTerminator
		{
			private readonly CommandEvents cmdEvents;
			private readonly IRun run;

			private void cmdEvents_BeforeExecute(
				string guid,
				int id,
				object customIn,
				object customOut,
				ref bool cancelDefault )
			{
				Debug.Assert( id == ( int ) Dte.VSStd97CmdID.Stop );

				try
				{
					this.run.Terminate();
				}
				catch ( Exception x )
				{
					CfixStudio.HandleError( x );
				}
			}

			public DebugTerminator( CommandEvents cmdEvents, IRun run )
			{
				this.cmdEvents = cmdEvents;
				this.run = run;
			}

			public void Activate()
			{
				this.cmdEvents.BeforeExecute +=
					new _dispCommandEvents_BeforeExecuteEventHandler(
						cmdEvents_BeforeExecute );
			}

			public void Deactivate()
			{
				this.cmdEvents.BeforeExecute -= 
					new _dispCommandEvents_BeforeExecuteEventHandler( 
						cmdEvents_BeforeExecute );
			}

		}

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

		private IAgent CreateLocalAgent( Architecture arch )
		{
			Debug.Assert( this.config != null );

			IAgent agent = Agent.CreateLocalAgent(
				arch,
				false,
				this.config.HostCreationOptions );
			agent.SetTrialLicenseCookie( this.config.Cookie );

			//
			// Inherit own environment variables.
			//
			// N.B. It is absolutely crucial to inherit %SystemRoot% - 
			// otherwise, SXS will fail to load any SXS-based library,
			// including the CRT.
			//
			agent.DefaultEnvironment.MergeEnvironmentVariables(
				Environment.GetEnvironmentVariables() );

			return agent;
		}

		/*++
		 * Create AgentSet for all supported architectures.
		 --*/
		private AgentSet CreateRunAgent()
		{
			Debug.Assert( this.config != null );

			AgentSet target = new AgentSet();
			switch ( GetNativeArchitecture() )
			{
				case Architecture.Amd64:
					target.AddArchitecture(
						CreateLocalAgent(
							Architecture.Amd64 ) );
					target.AddArchitecture(
						CreateLocalAgent(
							Architecture.I386 ) );

					break;

				case Architecture.I386:
					target.AddArchitecture(
						CreateLocalAgent(
							Architecture.I386 ) );
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

		private IDispositionPolicy GetDispositionPolicy( bool debug )
		{
			if ( debug )
			{
				return new StandardDispositionPolicy(
				this.config.DefaultDebugUnhandledExceptionDisposition,
				this.config.DefaultDebugFailedAssertionDisposition );
			}
			else
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

		internal Workspace( CfixStudio addin )
		{
			this.addin = addin;

			this.config = Configuration.Load( addin );
			this.runAgents = CreateRunAgent();
			this.session = new Session();

			//
			// N.B. Search target is always i386 and inproc.
			//
			this.searchAgent = CreateLocalAgent( Architecture.I386 );
			
			//
			// N.B. We only need the Stop command for the Terminator.
			//
			Events2 events = ( Events2 ) addin.Events;
			this.cmdEvents = events.get_CommandEvents( 
				VSStd97CmdID, 
				( int ) Dte.VSStd97CmdID.Stop );

			//
			// N.B. Uses agents and config, therefore, create last.
			//
			this.toolWindows = new ToolWindows( addin, this );
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

		internal LicenseInfo QueryLicenseInfo()
		{
			if ( this.cachedLicenseInfo == null )
			{
				Native.CFIXCTL_LICENSE_INFO license = new Native.CFIXCTL_LICENSE_INFO();
				license.SizeOfStruct = ( uint ) 
					System.Runtime.InteropServices.Marshal.SizeOf( license );

				int hr = Native.CfixctlQueryLicenseInfo(
					true,
					this.config.Cookie,
					ref license );
				if ( hr != 0 )
				{
					throw new CfixException(
						this.searchAgent.ResolveMessage( hr ) );
				}
				else
				{
					this.cachedLicenseInfo = new LicenseInfo( license );
				}
			}

			return this.cachedLicenseInfo;
		}

		internal ToolWindows ToolWindows
		{
			get { return this.toolWindows; }
		}

		public Configuration Configuration
		{
			get { return this.config; }
		}

		public IAgent SearchAgent
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

		public ISession Session
		{
			get { return this.session; }
		}

		public bool RerunLastItemPossible
		{
			get { return this.lastItemRun != null; }
		}

		public void RunItem( IRunnableTestItem item, bool debug )
		{
			lock ( this.runLock )
			{
				if ( item != null )
				{
					this.lastItemRun = item;
				}
				else if ( this.lastItemRun != null )
				{
					item = this.lastItemRun;
				}
				else
				{
					return;
				}

				IRun currentRun = this.toolWindows.Run.UserControl.Run;
				if ( ( currentRun == null || currentRun.Status != TaskStatus.Running ) &&
				     this.runAgents.ActiveHostCount > 0 )
				{
					//
					// No run active, yet there are host active processes.
					//
					if ( CfixStudio.ShowQuestion(
						Strings.TerminateActiveHosts ) )
					{
						this.runAgents.TerminateActiveHosts();
					}
				}

				//
				// Make sure the run window is reset while the build
				// is active.
				//
				this.toolWindows.Run.UserControl.Run = null;

				if ( !BuildNodeIfRequired( item ) )
				{
					//
					// Bail out. Show error window.
					//
					this.addin.DTE.ToolWindows.ErrorList.Parent.Activate();
					return;
				}

				//
				// It is possible that the build has changed the item.
				// While this will cause an appropriate exception in 
				// case the item itself is gone, we have to ensure that
				// the children are up to date - therefore, refresh.
				//
				ITestItemCollection itemColl = item as ITestItemCollection;
				if ( itemColl != null )
				{
					itemColl.Refresh();

					//
					// Now the children are up to date.
					//
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
					GetDispositionPolicy( debug ),
					this.config.ExecutionOptions,
					this.config.ThreadingOptions,
					this.config.EnvironmentOptions,
					allowArchMixing );
				compiler.Add( item );

				IRun run = compiler.Compile();
				run.Log += new EventHandler<LogEventArgs>( run_Log );

				if ( debug )
				{
					//
					// N.B. As we late-attach the debugger, hitting 'Stop'
					// in the IDE will merely detach, but not kill the process.
					//
					// Therefore, the Stop command is trapped and the Terminator 
					// object is used to kill the host when the Stop command is
					// issued.
					//
					DebugTerminator terminator = new DebugTerminator( this.cmdEvents, run );

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

							terminator.Activate();
							process.Attach();

							//
							// N.B. By default, VS hides our tool windows when
							// going into debug mode.
							//
							this.toolWindows.Run.Activate();
						}
						catch ( Exception x )
						{
							CfixStudio.HandleError( x );
							run.Terminate();
						}
					};

					run.Finished += delegate( object sender, FinishedEventArgs e )
					{
						terminator.Deactivate();
					};
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
}
