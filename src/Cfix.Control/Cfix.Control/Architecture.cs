using System;
using Cfixctl;

namespace Cfix.Control
{
	[Flags]
	public enum Architecture
	{
		//
		// N.B. Define Max first i.o. to have ToString() ignore it.
		//
		Max = CfixTestModuleArch.CfixTestModuleArchMax,
		
		I386 = CfixTestModuleArch.CfixTestModuleArchI386,
		Amd64 = CfixTestModuleArch.CfixTestModuleArchAmd64,
	}

	public enum ModuleType
	{
		User = CfixTestModuleType.CfixTestModuleTypeUser,
		UserEmbedded = CfixTestModuleType.CfixTestModuleTypeUserEmbedded,
		Kernel = CfixTestModuleType.CfixTestModuleTypeKernel
	}
}
