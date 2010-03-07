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
			HostCreationOptions flags
			)
			: base( agent, arch, allowInproc, flags )
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
				this.inspector.InspectorLevel.ToString(),
				this.inspector.ResultLocation.ResultDirectory != null
					? String.Format( 
						"-result-dir \"{0}\"",
						this.inspector.ResultLocation.ResultDirectory )
					: "",
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
			HostCreationOptions flags
			)
		{
			return new InspectorAgent(
				inspector,
				new LocalAgentClass(),
				( CfixTestModuleArch ) arch,
				allowInproc,
				flags );
		}
	}
}
