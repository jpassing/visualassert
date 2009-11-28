using System;
using System.Drawing;
using System.Collections.Generic;

namespace Cfix.Control.Ui.Result
{
	[Flags]
	public enum ResultNodeFilter
	{
		//
		// Nodes that indicate failure.
		//
		FailureNodes = 1,

		//
		// Nodes that indicate success or a pending/intermediate status.
		//
		NonFailureNodes = 2,

		//
		// All nodes, regardless of status.
		//
		All = 3
	}

	public interface IResultNode
	{
		IResultNode Parent { get; }
		
		bool IsLeaf { get; }
		IEnumerable<IResultNode> GetChildren( ResultNodeFilter filter );

		Color? TextColor { get; }
		Image Icon { get; }
		string Status { get; }
		string Name { get; }
		string Expression { get; }
		string Message { get; }
		string Location { get; }
		string Routine { get; }
		string LastError { get; }
		string Duration { get; }
	}
}
