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
