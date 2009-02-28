using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public interface IAgent : IDisposable
	{
		IHost CreateHost();
		Architecture Architecture { get; }
		String ResolveMessage( int code );
	}
}
