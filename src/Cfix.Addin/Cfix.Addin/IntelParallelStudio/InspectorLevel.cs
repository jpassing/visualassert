using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Addin.IntelParallelStudio
{
	public class InspectorLevel
	{
		private readonly string name;

		public static readonly InspectorLevel CheckDeadlocks = new InspectorLevel( "ti1" );
		public static readonly InspectorLevel CheckDeadlocksAndRaces = new InspectorLevel( "ti2" );
		public static readonly InspectorLevel LocateDeadlocksAndRaces = new InspectorLevel( "ti3" );
		public static readonly InspectorLevel Complete = new InspectorLevel( "ti4" );

		private InspectorLevel( string name )
		{
			this.name = name;
		}

		public override String ToString()
		{
			return this.name;
		}
	}
}
