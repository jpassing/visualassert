using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cfix.Addin.IntelParallelStudio
{
	public struct ResultLocation
	{
		public string ResultDirectory;
		public string ResultFile;

		private ResultLocation( string dir, string file )
		{
			this.ResultDirectory = dir;
			this.ResultFile = file;
		}

		/*++
			Deterministically creates a result location (path) based
			on a name.
		--*/
		public static ResultLocation Create( string name )
		{
			string resultsBaseDir = Path.Combine(
				Path.GetTempPath(),
				"va-insp" );

			if ( !Directory.Exists( resultsBaseDir ) )
			{
				Directory.CreateDirectory( resultsBaseDir );
			}

			string tempDir = Path.Combine( resultsBaseDir, Guid.NewGuid().ToString() );

			if ( !Directory.Exists( tempDir ) )
			{
				Directory.CreateDirectory( tempDir );
			}

			string resultDir = Path.Combine( tempDir, name );

			return new ResultLocation(
				resultDir,
				Path.Combine( resultDir, name + ".insp" ) );
		}

		
	}
}
