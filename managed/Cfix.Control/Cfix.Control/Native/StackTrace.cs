using System;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class StackTrace : IStackTrace
	{
		public static StackTrace Wrap( ICfixStackTrace trace )
		{
			// TODO
			return new StackTrace();
		}
	}
}
