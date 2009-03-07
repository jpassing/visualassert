using System;
using System.Diagnostics;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Addin
{
	public class Workspace : IDisposable
	{
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
		private static AgentSet CreateRunAgents()
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
		internal Workspace()
		{
			//
			// Search target is always i386 and inproc.
			//
			this.searchAgent = Agent.CreateLocalAgent( Architecture.I386, true );
			this.runAgents = CreateRunAgents();
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
			get { return searchAgent; }
		}

		public AgentSet RunAgents
		{
			get { return runAgents; }
		} 
		
		public ISession Session
		{
			get { return session; }
		}

		public IRun CurrentRun
		{
			get { return this.currentRun; }
		}

		//public bool IsRunActive
		//{
		//    get
		//    {
		//        lock ( this.runLock )
		//        {
		//            return this.currentRun != null && 
		//               this.currentRun.IsStarted &&
		//               ! this.currentRun.IsFinished;
		//        }
		//    }
		//}

		//public void CreateRun()
		//{
		//    lock ( this.runLock )
		//    {
		//        if ( IsRunActive )
		//        {
		//            //
		//            // Run still active.
		//            //
		//            throw new CfixException( Strings.RunActive );
		//        }

		//        Debug.Assert( this.currentRun == null || this.currentRun.IsFinished );

		//        SchedulingOptions schOpts =
		//            this.config.UseComNeutralThread
		//                ? SchedulingOptions.ComNeutralThreading
		//                : SchedulingOptions.None;

		//        this.currentRun = this.session.CreateRun(
		//            this.DispositionPolicy,
		//            schOpts,
		//            CompositionOptions.NonComposite );
		//    }
		//}

	}
}
