using System;
using Cfix.Control.Native;
using Cfix.Control;

#if INTELINSPECTOR
namespace Cfix.Addin.IntelParallelStudio
{
	internal class InspectorAgentFactory : AgentFactory
	{
		private readonly Inspector inspector;

		/*--------------------------------------------------------------
		 * Ctor.
		 */

		public InspectorAgentFactory( 
			Inspector inspector
			)
		{
			this.inspector = inspector;
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		protected override Agent CreateLocalAgent(
			Architecture arch,
			bool allowInproc,
			HostCreationOptions flags,
			uint hostRegistrationTimeout,
			EventDll eventDll
			)
		{
			Agent agent = InspectorAgent.CreateLocalInspectorAgent(
				this.inspector,
				arch,
				allowInproc,
				flags,
				hostRegistrationTimeout,
				eventDll );

			return agent;
		}

		protected override uint GetHostRegistrationTimeout(
			Configuration config
			)
		{
			return config.InstrumentedHostRegistrationTimeout;
		}
	}
}
#endif