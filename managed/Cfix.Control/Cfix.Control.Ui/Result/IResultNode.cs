using System;
using System.Drawing;
using System.Collections;

namespace Cfix.Control.Ui.Result
{
	internal interface IResultNode
	{
		bool IsLeaf { get; }
		IEnumerable GetChildren();

		Image Icon { get; }
		string Status { get; }
		string Name { get; }
		string Expression { get; }
		string Message { get; }
		string Location { get; }
		string Routine { get; }
		uint LastError { get; }
	}
}
