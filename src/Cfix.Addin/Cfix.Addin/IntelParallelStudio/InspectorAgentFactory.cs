using System;
using Cfix.Control.Native;
using Cfix.Control;

namespace Cfix.Addin.IntelParallelStudio
{
	internal class InspectorAgentFactory : AgentFactory
	{
		private readonly Inspector inspector;

		public InspectorAgentFactory( Inspector inspector )
		{
			this.inspector = inspector;
		}

		public override Agent CreateLocalAgent(
			Architecture arch,
			bool allowInproc,
			HostCreationOptions flags
			)
		{
			return InspectorAgent.CreateLocalInspectorAgent(
				this.inspector,
				arch,
				allowInproc,
				flags );
		}
	}
}
