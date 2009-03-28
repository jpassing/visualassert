using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public interface IStackTraceFrame
	{
		string Module { get; }
		string Function { get; }
		uint Dispacement { get; }
		string SourceFile { get; }
		uint SourceLine { get; }
	}

	public interface IStackTrace : IEnumerable< IStackTraceFrame >
	{
		uint FrameCount { get; }
	}
}
