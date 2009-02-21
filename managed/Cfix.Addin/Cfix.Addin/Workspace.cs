using System;
using System.Diagnostics;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Addin
{
	public class Workspace : IDisposable
	{
		private readonly Configuration config;
		private readonly Target searchTarget;
		private readonly MultiTarget target;
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
		 * Create MultiTarget for all supported architectures.
		 --*/
		private static MultiTarget CreateMultiTarget()
		{
			MultiTarget target = new MultiTarget();
			switch ( GetNativeArchitecture() )
			{
				case Architecture.Amd64:
					target.AddArchitecture(
						Target.CreateLocalTarget(
							Architecture.Amd64,
							false ) );
					target.AddArchitecture(
						Target.CreateLocalTarget(
							Architecture.I386,
							false ) );

					break;

				case Architecture.I386:
					target.AddArchitecture(
						Target.CreateLocalTarget(
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
			this.searchTarget = Target.CreateLocalTarget( Architecture.I386, true );
			this.target = CreateMultiTarget();
			this.config = Configuration.Load();
			this.session = new Session();
		}

		~Workspace()
		{
			Dispose( false );
		}

		protected void Dispose( bool disposing )
		{
			if ( this.searchTarget != null )
			{
				this.searchTarget.Dispose();
			}

			if ( this.target != null )
			{
				this.target.Dispose();
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

		public Target SearchTarget
		{
			get { return searchTarget; }
		}

		public MultiTarget RunTarget
		{
			get { return target; }
		} 
		
		public ISession Session
		{
			get { return session; }
		}

		public IRun CurrentRun
		{
			get { return this.currentRun; }
		}

		public bool IsRunActive
		{
			get
			{
				lock ( this.runLock )
				{
					return this.currentRun != null && 
					   this.currentRun.IsStarted &&
					   ! this.currentRun.IsFinished;
				}
			}
		}

		public void CreateRun()
		{
			lock ( this.runLock )
			{
				if ( IsRunActive )
				{
					//
					// Run still active.
					//
					throw new CfixException( Strings.RunActive );
				}

				Debug.Assert( this.currentRun == null || this.currentRun.IsFinished );

				SchedulingOptions schOpts =
					this.config.UseComNeutralThread
						? SchedulingOptions.ComNeutralThreading
						: SchedulingOptions.None;

				this.currentRun = this.session.CreateRun(
					this.DispositionPolicy,
					schOpts,
					CompositionOptions.NonComposite );
			}
		}

	}
}
