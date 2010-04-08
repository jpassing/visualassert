using System;
using System.Collections.Generic;
using System.Text;
using Cfix.Control;

namespace Cfix.Addin.IntelParallelStudio
{
	public class InspectorResult
	{
		public enum ResultSeverity
		{
			Information,
			Warning,
			Error
		}

		private readonly ResultSeverity severity;
		private readonly uint type;
		private readonly string description;
		private readonly uint threadId;
		private readonly IStackTrace stackTrace;

		public InspectorResult(
			uint type,
			ResultSeverity severity,
			string description,
			uint threadId,
			IStackTrace stackTrace 
			)
		{
			this.type = type;
			this.severity = severity;
			this.description = description;
			this.threadId = threadId;
			this.stackTrace = stackTrace;
		}

		public uint Type
		{
			get { return this.type; }
		}

		public ResultSeverity Severity
		{
			get { return this.severity; }
		}

		public string Description
		{
			get { return this.description; }
		}

		public uint ThreadId
		{
			get { return this.threadId; }
		}

		public IStackTrace StackTrace
		{
			get { return this.stackTrace; }
		}

		public string SourceFile
		{
			get
			{
				if ( this.stackTrace == null )
				{
					return null;
				}
				else
				{
					foreach ( IStackTraceFrame frame in this.stackTrace )
					{
						return frame.SourceFile;
					}

					return null;
				}
			}
		}

		public string Function
		{
			get
			{
				if ( this.stackTrace == null )
				{
					return null;
				}
				else
				{
					foreach ( IStackTraceFrame frame in this.stackTrace )
					{
						return frame.Function;
					}

					return null;
				}
			}
		}

		public uint SourceLine
		{
			get
			{
				if ( this.stackTrace == null )
				{
					return 0;
				}
				else
				{
					foreach ( IStackTraceFrame frame in this.stackTrace )
					{
						return frame.SourceLine;
					}

					return 0;
				}
			}
		}

		public static String GetDescriptionFromCode( string descriptionCode )
		{
			string originalDescriptionCode = descriptionCode;

			while ( true )
			{
				string descr = ( string )
					ProblemDescriptions.ResourceManager.GetObject( descriptionCode );
				if ( descr != null )
				{
					return descr;
				}

				//
				// No direct match -- strip last component.
				//
				if ( descriptionCode.Contains( "_" ) )
				{
					descriptionCode = descriptionCode.Substring( 
						0, descriptionCode.LastIndexOf( '_' ) );
				}
				else
				{
					return originalDescriptionCode;
				}
			}
		}

		/*--------------------------------------------------------------
		 * Equality.
		 */

		public override bool Equals( Object obj )
		{
			return obj is InspectorResult && this == ( InspectorResult ) obj;
		}

		public override int GetHashCode()
		{
			return this.description.GetHashCode() ^ ( int ) this.threadId;
		}

		public static bool operator ==( InspectorResult x, InspectorResult y )
		{
			if ( ReferenceEquals( x, null ) && ReferenceEquals( y, null ) ) return true;
			if ( ReferenceEquals( x, null ) != ReferenceEquals( y, null ) ) return false;
			if ( x.description != y.description ) return false;
			if ( ( x.stackTrace == null ) != ( y.stackTrace == null ) ) return false;

			if ( x.stackTrace != null )
			{
				if ( x.stackTrace.FrameCount != x.stackTrace.FrameCount ) return false;
				if ( x.stackTrace.ToString() != y.stackTrace.ToString() ) return false;
			}

			return true;
		}

		public static bool operator !=( InspectorResult x, InspectorResult y )
		{
			return !( x == y );
		}

	}
}
