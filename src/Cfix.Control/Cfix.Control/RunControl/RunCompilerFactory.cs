using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.RunControl
{
	public enum RunCompilerType
	{
		Simple = 0,
		ProcessPerTest = 1
	}
	
	public static class RunCompilerFactory
	{
		public static IRunCompiler CreateCompiler(
			RunCompilerType compilerType,
			AgentSet agentSet,
			IDispositionPolicy policy,
			ExecutionOptions executionOptions,
			EnvironmentOptions environmentOptions,
			bool allowArchMixing
			)
		{
			switch ( compilerType )
			{
				case RunCompilerType.Simple:
					return new SimpleRunCompiler(
						agentSet,
						policy,
						executionOptions,
						environmentOptions,
						allowArchMixing );

				case RunCompilerType.ProcessPerTest:
					if ( !allowArchMixing )
					{
						throw new ArgumentException();
					}

					return new ProcessPerTestRunCompiler(
					   agentSet,
						policy,
						executionOptions,
						environmentOptions );

				default:
					throw new ArgumentException();
			}
		}
	}
}
