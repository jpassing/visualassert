using System;
using System.Drawing;
using System.Collections.Generic;

namespace Cfix.Control.Ui.Result
{
	public interface IResultNode
	{
		IResultNode Parent { get; }
		
		bool IsLeaf { get; }
		IEnumerable<IResultNode> GetChildren();

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
