/*----------------------------------------------------------------------
 * Purpose:
 *		Item with associated code that can be built.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Addin
{
	interface IBuildableTestItem
	{
		/*++
		 * Build the source code.
		 * 
		 * Return true if successful, false on build failure.
		 --*/
		bool Build();
	}
}
