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
using Cfix.Addin.Windows;
using EnvDTE;
using EnvDTE80;
using System.IO;
using Microsoft.VisualStudio.VCProjectEngine;

namespace Cfix.Addin
{
	public class Workspace : IDisposable
	{
		private readonly string VSStd97CmdID = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";

		private readonly DTE2 dte;
		private readonly Configuration config;
		private readonly Cfix.Addin.Windows.ToolWindows toolWindows;
		private readonly IAgent searchAgent;
		private readonly AgentSet runAgents;
		private readonly ISession session;
		private readonly CommandEvents cmdEvents;

		private readonly object runLock = new object();

		private IRunnableTestItem lastItemRun;

		private LicenseInfo cachedLicenseInfo;

		private static bool vcDirectoriesRegistered;

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
					VisualAssert.HandleError( x );
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

		private IAgent CreateOutOfProcessLocalAgent( Architecture arch )
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

			//
			// Add own library path to PATH s.t. custom hosts can
			// find cfixctl.dll, cfix.dll, etc.
			//
			// Prioritize these directories s.t. an EXE modules does
			// not accidently load an old DLL from another VA/cfix
			// installation.
			//
			agent.DefaultEnvironment.AddSearchPath(
				Directories.GetBinDirectory( arch ),
				true );

			return agent;
		}

		private IAgent CreateInProcessLocalAgent( Architecture arch )
		{
			Debug.Assert( this.config != null );

			IAgent agent = Agent.CreateLocalAgent(
				arch,
				true,
				this.config.HostCreationOptions );
			agent.SetTrialLicenseCookie( this.config.Cookie );

			//
			// N.B. No need to specify environment or search path.
			// (would not work either)
			//

			return agent;
		}

		/*++
		 * Create AgentSet for all supported architectures.
		 --*/
		private AgentSet CreateRunAgent()
		{
			Debug.Assert( this.config != null );

			AgentSet target = new AgentSet();
			switch ( ArchitectureUtil.NativeArchitecture )
			{
				case Architecture.Amd64:
					target.AddArchitecture(
						CreateOutOfProcessLocalAgent(
							Architecture.Amd64 ) );
					target.AddArchitecture(
						CreateOutOfProcessLocalAgent(
							Architecture.I386 ) );

					break;

				case Architecture.I386:
					target.AddArchitecture(
						CreateOutOfProcessLocalAgent(
							Architecture.I386 ) );
					break;

				default:
					throw new CfixAddinException(
						Strings.UnsupportedArchitecture );
			}

			return target;
		}

		/*----------------------------------------------------------------------
		 * Private - Misc.
		 */

		private string ResolveMessage( int code )
		{
			return this.searchAgent.ResolveMessage( code );
		}

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
				return this.dte.Windows.Item(
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

		private static bool IsArchitectureSupported( VCPlatform plaf )
		{
			string plafName = plaf.Name;
			switch ( plafName )
			{
				case "Win32":
					return true;

				case "x64":
					return true;

				default:
					return false;
			}
		}

		/*++
		 * Register cfix directories s.t. the compiler finds them. 
		 --*/
		private static void RegisterVcDirectories( DTE2 dte )
		{
			Projects projects = ( Projects ) dte.GetObject( "VCProjects" );
			VCProjectEngine engine = ( VCProjectEngine )
				projects.Properties.Item( "VCProjectEngine" ).Object;

			if ( vcDirectoriesRegistered || engine == null )
			{
				return;
			}

			IVCCollection platforms = ( IVCCollection ) engine.Platforms;
			bool changePerformed = false;

			foreach ( VCPlatform platform in platforms )
			{
				if ( !IsArchitectureSupported( platform ) )
				{
					continue;
				}

				Architecture arch = GetArchitecture( platform );
				if ( !platform.IncludeDirectories.Contains( Directories.IncludeDirectory ) )
				{
					platform.IncludeDirectories +=
						";" + Directories.IncludeDirectory;

					changePerformed = true;
				}

				if ( !platform.LibraryDirectories.Contains( Directories.GetLibDirectory( arch ) ) )
				{
					platform.LibraryDirectories +=
						";" + Directories.GetLibDirectory( arch );

					changePerformed = true;
				}

#if !VS100
				//
				// Write to disk.
				//
				 platform.CommitChanges();
#endif
			}

			vcDirectoriesRegistered = true;

			if ( changePerformed )
			{
				VisualAssert.ShowInfo( Strings.VcDirectoriesUpdated );
			}
		}
		
		/*----------------------------------------------------------------------
		 * ctor/dtor.
		 */

		internal Workspace( DTE2 dte, AddIn addin )
		{
			this.dte = dte;

			this.config = Configuration.Load( dte );
			this.runAgents = CreateRunAgent();
			this.session = new Session();

			//
			// If not happened yet, register VC directories.
			//
			RegisterVcDirectories( dte );

			//
			// N.B. Search target is always i386 merely because this works on
			// all machines.
			//
			// N.B. Searching does not involve loading test modules, so it 
			// is safe to do in-process and there is no need to set up a 
			// special environment and search path.
			//
			if ( config.SearchOutOfProcess )
			{
				this.searchAgent = CreateOutOfProcessLocalAgent( Architecture.I386 );
			}
			else
			{
				this.searchAgent = CreateInProcessLocalAgent( Architecture.I386 );
			}
			
			//
			// N.B. We only need the Stop command for the Terminator.
			//
			this.cmdEvents = dte.Events.get_CommandEvents( 
				VSStd97CmdID, 
				( int ) Dte.VSStd97CmdID.Stop );

			//
			// N.B. Uses agents and config, therefore, create last.
			//
			this.toolWindows = new Cfix.Addin.Windows.ToolWindows( dte, addin, this );
		}

		~Workspace()
		{
			Dispose( false );
		}

		protected void Dispose( bool disposing )
		{
			if ( this.toolWindows != null )
			{
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

			this.dte.Debugger.DetachAll();
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
					throw new CfixException( ResolveMessage( hr ) );
				}
				else
				{
					this.cachedLicenseInfo = new LicenseInfo( license );
				}
			}

			return this.cachedLicenseInfo;
		}

		internal Cfix.Addin.Windows.ToolWindows ToolWindows
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
				Solution curSolution = this.dte.Solution;
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
					if ( VisualAssert.ShowQuestion(
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
					this.dte.ToolWindows.ErrorList.Parent.Activate();
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
					this.config.EnvironmentOptions,
					allowArchMixing );
				compiler.Add( item );

				IRun run = compiler.Compile();

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
							Debugger2 debugger = ( Debugger2 ) this.dte.Debugger;
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
							VisualAssert.HandleError( x );
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

		public void RunItemOnCommandLine( 
			NativeTestItem item,
			bool debug
			)
		{
			NativeTestItem nativeItem = item as NativeTestItem;
			Debug.Assert( nativeItem != null );

			if ( !BuildNodeIfRequired( item ) )
			{
				//
				// Bail out. Show error window.
				//
				this.dte.ToolWindows.ErrorList.Parent.Activate();
				return;
			}

			Architecture arch = nativeItem.Module.Architecture;

			string cfixPath = Path.Combine(
					Directories.GetBinDirectory( arch ),
					CfixCommandLine.GetExecutableName( arch ) );
			string cfixArguments = CfixCommandLine.CreateArguments(
					nativeItem,
					this.Configuration.ExecutionOptions,
					GetDispositionPolicy( false ),
					true,
					false,
					false );

			string cfixCmdLine = "\"" + cfixPath + "\" " + cfixArguments;

			if ( debug )
			{
				string windbgPath = this.Configuration.GetWinDbgInstallFolder( arch );
				if ( String.IsNullOrEmpty( windbgPath ) )
				{
					VisualAssert.ShowInfo( Strings.WindbgNotConfigured );
					return;
				}
				else
				{
					windbgPath = Path.Combine( windbgPath, "windbg.exe" );
				}

				SpawnProcess(
					windbgPath,
					this.Configuration.WinDbgAdditionalOptions + " " + cfixCmdLine,
					nativeItem.Module.Path,
					arch );
			}
			else
			{
				SpawnProcess(
					Path.Combine( Environment.SystemDirectory, "cmd.exe" ),
					"/c \"" + cfixCmdLine + "\" & pause",
					nativeItem.Module.Path,
					arch );
			}
		}

		private void SpawnProcess( 
			string executable, 
			string arguments, 
			string workingDir,
			Architecture arch )
		{
			if ( !File.Exists( executable ) )
			{
				VisualAssert.ShowError( 
					String.Format( Strings.ExecutableNotFound, executable ) );
				return;
			}

			ProcessStartInfo procInfo = new ProcessStartInfo();

			if ( ( this.config.EnvironmentOptions & EnvironmentOptions.AutoAdjustCurrentDirectory ) != 0 )
			{
				procInfo.WorkingDirectory = new FileInfo(
					workingDir ).Directory.FullName;
			}

			try
			{
				procInfo.FileName = executable;
				procInfo.Arguments = arguments;

				procInfo.UseShellExecute = false;

				//
				// Add own library path to PATH s.t. custom hosts can
				// find cfix.dll etc..
				//
				string pathEnvVar = procInfo.EnvironmentVariables[ "PATH" ];
				procInfo.EnvironmentVariables[ "PATH" ] = 
					( String.IsNullOrEmpty( pathEnvVar ) ? "" : pathEnvVar + ";" ) +
					Directories.GetBinDirectory( arch );
			
				System.Diagnostics.Process.Start( procInfo );
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( 
					String.Format( Strings.ExecutableCouldNotBeLaunched, executable ), 
					x );
			}
		}

		public static Architecture GetArchitecture( VCPlatform plaf )
		{
			string plafName = plaf.Name;
			switch ( plafName )
			{
				case "Win32":
					return Architecture.I386;

				case "x64":
					return Architecture.Amd64;

				default:
					throw new CfixAddinException(
						String.Format( Strings.UnrecognizedPlatform, plafName ) );
			}
		}

		public static Architecture GetArchitecture( VCConfiguration config )
		{
			return GetArchitecture( ( VCPlatform ) config.Platform );
		}
	}
}
