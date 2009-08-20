using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfixctl;


namespace Cfix.Control.Native
{
	public class TestCase : NativeTestItem
	{
		internal TestCase(
			NativeTestItemCollection parent,
			uint ordinal,
			ICfixTestItem item )
			: base( parent, ordinal, item )
		{
		}

	}
}
