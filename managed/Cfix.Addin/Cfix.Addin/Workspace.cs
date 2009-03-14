using System;
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
		private readonly Agent searchAgent;
		private readonly AgentSet runAgents;
		private readonly ISession session;

		private readonly object runLock = new object();
		private IRun currentRun;

		/*----------------------------------------------------------------------
		 * Private.
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

		private IDispositionPolicy DispositionPolicy
		{
			get
			{
				return new StandardDispositionPolicy(
					this.config.DefaultUnhandledExceptionDisposition,
					this.config.DefaultFailedAssertionDisposition );
			}
		}
		
		/*----------------------------------------------------------------------
		 * ctor/dtor.
		 */
		internal Workspace( CfixPlus addin )
		{
			//
			// Search target is always i386 and inproc.
			//
			this.addin = addin;
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
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

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
				Solution curSolution = this.addin.DTE.Solution;
				if ( curSolution.Projects.Count == 0 )
				{
					return false;
				}

				return false;
			}
		}

		public ISession Session
		{
			get { return this.session; }
		}

		public void DebugItem( ITestItem item )
		{
			//
			// Without a project, debugging does not work properly.
			// Moreover, we need a project to ontain the architecture
			// to be used for debugging.
			//
			
			//
			// N.B. When debugging, it is crucial to use a single host
			// process only. Debug runs are thus limited to a single
			// architecture only.
			//

			throw new NotImplementedException();
		}

		public void RunItem( IRunnableTestItem item )
		{
			SimpleRunCompiler compiler = new SimpleRunCompiler(
				this.runAgents,
				this.DispositionPolicy,
				this.config.SchedulingOptions,
				this.config.ThreadingOptions );
			compiler.Add( item );
			//IRun run = compiler.Compile();
		}


	}
}
