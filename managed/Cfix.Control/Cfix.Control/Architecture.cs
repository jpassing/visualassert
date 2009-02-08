using System;
using Cfixctl;

namespace Cfix.Control
{
	[Flags]
	public enum Architecture
	{
		I386 = CfixTestModuleArch.CfixTestModuleArchI386,
		Amd64 = CfixTestModuleArch.CfixTestModuleArchAmd64,
		Max = CfixTestModuleArch.CfixTestModuleArchMax
	}
}