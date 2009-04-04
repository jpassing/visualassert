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
