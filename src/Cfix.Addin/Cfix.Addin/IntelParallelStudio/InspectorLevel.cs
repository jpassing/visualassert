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
		public static readonly InspectorLevel AllThreadingIssues = new InspectorLevel( "ti4" );

		public static readonly InspectorLevel LocateDataSharingIssues = new InspectorLevel( "ad1" );

		public static readonly InspectorLevel CheckMemoryLeaks = new InspectorLevel( "mi1" );
		public static readonly InspectorLevel CheckMemoryAccessIssues = new InspectorLevel( "mi2" );
		public static readonly InspectorLevel LocateMemoryAccessIssues = new InspectorLevel( "mi3" );
		public static readonly InspectorLevel AllMemoryIssues = new InspectorLevel( "mi4" );

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
