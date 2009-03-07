using System;
using System.Collections;

namespace Cfix.Control.Ui.Result
{
	internal interface IResultNode
	{
		bool IsLeaf { get; }
		IEnumerable GetChildren();

		string Name { get; }
		string Location { get; }
		int Failures { get; }
		string Status { get; }
	}
}
