using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.Ui.Result
{
	public interface ISourceReference
	{
		string File { get; }
		uint Line { get; }
	}
}
