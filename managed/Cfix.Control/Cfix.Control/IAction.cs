using System;
using Cfixctl;

namespace Cfix.Control
{
	public interface IAction : IDisposable
	{
		uint TestCaseCount { get; }
		void Run( ICfixEventSink sink );
		void Stop();
	}

	/*++
	 *	Non-composite action referring to fixtures/testcases
	 *	of a single module, and thus, of a single architecture.
	--*/
	public interface IComponentAction : IAction
	{
		Architecture Architecture { get; }
	}

	/*++
	 *	Composite containing one or more IComponentAction that may
	 *	differ in their architecture.
	--*/
	public interface ICompositeAction : IAction
	{
		void Add( IComponentAction action );
	}
}
