using System;
using Cfix.Control;
using System.Diagnostics;
using Cfix.Control.Native;

namespace Cfix.Addin
{
	internal class AgentFactory
	{
		/*--------------------------------------------------------------
		 * Virtuals.
		 */

		protected virtual Agent CreateLocalAgent(
			Architecture arch,
			bool allowInproc,
			HostCreationOptions flags,
			uint hostRegistrationTimeout,
			EventDll eventDll
			)
		{
			return Agent.CreateLocalAgent(
				arch,
				allowInproc,
				flags,
				hostRegistrationTimeout,
				eventDll );
		}

		protected virtual uint GetHostRegistrationTimeout(
			Configuration config
			)
		{
			return config.HostRegistrationTimeout;
		}

		/*--------------------------------------------------------------
		 * Statics.
		 */

		public IAgent CreateOutOfProcessLocalAgent( 
			Configuration config,
			Architecture arch 
			)
		{
			Debug.Assert( config != null );

			IAgent agent = CreateLocalAgent(
				arch,
				false,
				config.HostCreationOptions,
				GetHostRegistrationTimeout( config ),
				config.GetEventDll( arch ) );
			agent.SetTrialLicenseCookie( config.Cookie );

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

		public IAgent CreateInProcessLocalAgent(
			Configuration config,
			Architecture arch 
			)
		{
			Debug.Assert( config != null );

			IAgent agent = CreateLocalAgent(
				arch,
				true,
				config.HostCreationOptions,
				GetHostRegistrationTimeout( config ),
				config.GetEventDll( arch ) );
			agent.SetTrialLicenseCookie( config.Cookie );

			//
			// N.B. No need to specify environment or search path.
			// (would not work either)
			//

			return agent;
		}

		/*++
		 * Create AgentSet for all supported architectures.
		 --*/
		public AgentSet CreateRunAgent( Configuration config )
		{
			Debug.Assert( config != null );

			AgentSet target = new AgentSet();
			switch ( ArchitectureUtil.NativeArchitecture )
			{
				case Architecture.Amd64:
					target.AddArchitecture(
						CreateOutOfProcessLocalAgent(
							config,
							Architecture.Amd64 ) );
					target.AddArchitecture(
						CreateOutOfProcessLocalAgent(
							config,
							Architecture.I386 ) );

					break;

				case Architecture.I386:
					target.AddArchitecture(
						CreateOutOfProcessLocalAgent(
							config,
							Architecture.I386 ) );
					break;

				default:
					throw new CfixAddinException(
						Strings.UnsupportedArchitecture );
			}

			return target;
		}
	}
}
