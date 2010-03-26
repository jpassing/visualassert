using System;
using Cfixctl;
using Cfix.Control.Native;
using Cfix.Control;
using System.Diagnostics;

namespace Cfix.Addin.IntelParallelStudio
{
	/*++
	 * Agent for Intel Inspector.
	--*/
	internal class InspectorAgent : Agent
	{
		private readonly Inspector inspector;

		/*--------------------------------------------------------------
		 * Ctor/Dtor.
		 */

		protected InspectorAgent(
			Inspector inspector,
			ICfixAgent agent,
			CfixTestModuleArch arch,
			bool allowInproc,
			HostCreationOptions flags,
			uint hostRegistrationTimeout
			)
			: base( 
				agent, 
				arch, 
				allowInproc, 
				flags,
				hostRegistrationTimeout )
		{
			this.inspector = inspector;
		}

		/*--------------------------------------------------------------
		 * Protected.
		 */

		protected override ICfixHost CreateNativeHost(
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

			InspectorHostEnvironment inspectorHostEnv =
				( InspectorHostEnvironment ) env;
			ResultLocation resultLocation = ResultLocation.Create( 
				inspectorHostEnv.Guid.ToString() );

			//
			// Inject inspector shim.
			//

			string hostPath;
			if ( customHostPath != null )
			{
				hostPath = customHostPath;
			}
			else
			{
				//
				// Use default host.
				//
				hostPath = agent.GetHostPath( arch );
			}

			string shimPath = this.inspector.ShimPath;
			string shimCommandLine = String.Format(
				"\"{0}\" -collect {1} -return-app-exitcode "+
				"{2} -- {3}",
				shimPath,
				inspectorHostEnv.InspectorLevel,
				String.Format( 
						"-result-dir \"{0}\"",
						resultLocation.ResultDirectory ),
				hostPath );

			Debug.Print( "Inspector command line: "+ shimCommandLine );

			return agent.CreateHostWithShim(
				arch,
				( uint ) clsctx,
				( uint ) flags,
				timeout,
				customHostPath,
				env.NativeFormat,
				currentDir,
				shimPath,
				shimCommandLine );
		}

		/*--------------------------------------------------------------
		 * Statics.
		 */

		public static InspectorAgent CreateLocalInspectorAgent(
			Inspector inspector,
			Architecture arch,
			bool allowInproc,
			HostCreationOptions flags,
			uint hostRegistrationTimeout
			)
		{
			return new InspectorAgent(
				inspector,
				new LocalAgentClass(),
				( CfixTestModuleArch ) arch,
				allowInproc,
				flags,
				hostRegistrationTimeout );
		}
	}
}
