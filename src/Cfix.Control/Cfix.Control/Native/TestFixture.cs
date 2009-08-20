using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfixctl;


namespace Cfix.Control.Native
{
	public class TestFixture : NativeTestItemCollection
	{
		internal TestFixture(
			NativeTestItemCollection parent,
			uint ordinal,
			ICfixTestItem item
			)
			: base( parent, ordinal, item )
		{
		}
	}
}
