using System;
using System.Collections.Generic;
using System.Text;
using Cfix.Control;
using System.IO;

#if INTELINSPECTOR
namespace Cfix.Addin.IntelParallelStudio
{
	public class InspectorResultFilter
	{
		private readonly Dictionary<string, object> binaries
			= new Dictionary<string, object>();

		private InspectorResult lastResult;

		public InspectorResultFilter()
		{
			//
			// Obtain list of binaries s.t. we can dynamically
			// filter out runtime files.
			//

			foreach ( string file in Directories.GetBinaries( Architecture.I386 ) )
			{
				string basename = new FileInfo( file ).Name;
				this.binaries.Add( basename.ToLower(), null );
			}
		}

		public bool IsCfixResult( InspectorResult result )
		{
			IStackTrace trace = result.StackTrace;
			if ( trace != null )
			{
				IEnumerator<IStackTraceFrame> en = trace.GetEnumerator();
				en.MoveNext();
				if ( this.binaries.ContainsKey( en.Current.Module.ToLower() ) )
				{
					//
					// Binary.
					//
					return true;
				}

				if ( en.Current.Function.StartsWith( "InvokeBefore<" ) )
				{
					//
					// Known false positive.
					//
					return true;
				}
			}
			
			return false;
		}

		public bool EqualsLastResult( InspectorResult result )
		{
			bool equal;
			if ( this.lastResult == null )
			{
				equal = false;
			}
			else
			{
				equal = this.lastResult == result;
			}

			this.lastResult = result;

			return equal;
		}
	}
}
#endif