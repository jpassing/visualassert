using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class NativeStackTraceFrame : StackTraceFrame
	{
		private static string NullifyIfEmpty( string s )
		{
			if ( String.IsNullOrEmpty( s ) )
			{
				return null;
			}
			else
			{
				return s;
			}
		}

		public NativeStackTraceFrame(
			ICfixStackTraceFrame frame
			)
			: base (
				NullifyIfEmpty( frame.GetModuleName() ),
				NullifyIfEmpty( frame.GetFunctionName() ),
				frame.GetDisplacement(),
				NullifyIfEmpty( frame.GetSourceFile() ),
				frame.GetSourceLine() )
		{
		}
	}

	internal class NativeStackTrace : IStackTrace
	{
		private IStackTraceFrame[] frames;

		private NativeStackTrace( IStackTraceFrame[] frames )
		{
			this.frames = frames;
		}

		public static NativeStackTrace Wrap( ICfixStackTrace trace )
		{
			if ( trace == null )
			{
				return null;
			}

			IStackTraceFrame[] frames = new IStackTraceFrame[ trace.GetFrameCount() ];
			for ( uint i = 0; i < frames.Length; i++ )
			{
				ICfixStackTraceFrame ctlFrame = trace.GetFrame( i );
				try
				{
					frames[ i ] = new NativeStackTraceFrame( ctlFrame );
				}
				finally
				{
					Marshal.ReleaseComObject( ctlFrame );
				}
			}

			Marshal.ReleaseComObject( trace );
			return new NativeStackTrace( frames );
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			foreach ( IStackTraceFrame frame in this.frames )
			{
				buf.Append( frame.ToString() );
				buf.Append( "\r\n" );
			}

			return buf.ToString();
		}

		/*--------------------------------------------------------------
		 * IStackTrace.
		 */

		public uint FrameCount
		{
			get { return ( uint ) this.frames.Length; }
		}

		/*--------------------------------------------------------------
		 * IEnumerable.
		 */

		public IEnumerator<IStackTraceFrame> GetEnumerator()
		{
			return ( ( IList<IStackTraceFrame> ) this.frames ).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.frames.GetEnumerator();
		}

	}
}
