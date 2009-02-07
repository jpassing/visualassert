using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public interface IResultItemCollection : IResultItem, IEnumerable<IResultItem>
	{
		//
		// TestItemCollection this result corresponds to - may or may not be
		// still available.
		//
		ITestItemCollection ItemCollection { get; }

		uint ItemCount { get; }
		IResultItem GetItem( uint ordinal );

	}
}
