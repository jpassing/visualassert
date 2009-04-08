/*----------------------------------------------------------------------
 * Purpose:
 *		Item that serves as a reference for relative paths.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Addin
{
	public interface IRelativePathReferenceItem
	{
		string GetFullPath( string relativePath );
	}
}
