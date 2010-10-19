using System;
using System.Collections.Generic;
using System.Text;

#if INTELINSPECTOR
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

		public static InspectorLevel FromString( string name )
		{
			switch ( name )
			{
				case "ti1":
				case "ti2":
				case "ti3":
				case "ti4":
				case "ad1":
				case "mi1":
				case "mi2":
				case "mi3":
				case "mi4":
					return new InspectorLevel( name );

				default:
					throw new ArgumentException( "Invalid level" );
			}
		}

		/*--------------------------------------------------------------
		 * Equality.
		 */

		public override bool Equals( Object obj )
		{
			return obj is InspectorLevel && this == ( InspectorLevel ) obj;
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode();
		}

		public static bool operator ==( InspectorLevel x, InspectorLevel y )
		{
			if ( ReferenceEquals( x, null ) && ReferenceEquals( y, null ) ) return true;
			if ( ReferenceEquals( x, null ) != ReferenceEquals( y, null ) ) return false;
			return x.name == y.name;
		}

		public static bool operator !=( InspectorLevel x, InspectorLevel y )
		{
			return !( x == y );
		}
	}
}
#endif