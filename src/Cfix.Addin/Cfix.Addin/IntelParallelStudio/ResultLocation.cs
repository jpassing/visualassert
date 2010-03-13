using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Addin.IntelParallelStudio
{
	public struct ResultLocation
	{
		public string ResultDirectory;
		public string ResultFile;

		public ResultLocation( string dir, string file )
		{
			this.ResultDirectory = dir;
			this.ResultFile = file;
		}
	}
}
