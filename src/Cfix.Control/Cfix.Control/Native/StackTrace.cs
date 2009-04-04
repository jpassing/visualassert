using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class StackTraceFrame : IStackTraceFrame
	{
		private string module;
		private string function;
		private uint displacement;
		private string sourceFile;
		private uint sourceLine;

		public StackTraceFrame(
			ICfixStackTraceFrame frame
			)
		{
			this.module = frame.GetModuleName();
			this.function = frame.GetFunctionName();
			this.displacement = frame.GetDisplacement();
			this.sourceFile = frame.GetSourceFile();
			this.sourceLine = frame.GetSourceLine();

			//
			// Favor null over empty strings.
			//
			if ( String.IsNullOrEmpty( this.module ) )
			{
				this.module = null;
			}

			if ( String.IsNullOrEmpty( this.function ) )
			{
				this.function = null;
			}
			
			if ( String.IsNullOrEmpty( this.sourceFile ) )
			{
				this.sourceFile = null;
			}
		}

		public string Module
		{
			get { return this.module; }
		}

		public string Function
		{
			get { return this.function; }
		}

		public uint Dispacement
		{
			get { return this.displacement; }
		}

		public string SourceFile
		{
			get { return this.sourceFile; }
		}

		public uint SourceLine
		{
			get { return this.sourceLine; }
		}
	}

	internal class StackTrace : IStackTrace
	{
		private IStackTraceFrame[] frames;

		private StackTrace( IStackTraceFrame[] frames )
		{
			this.frames = frames;
		}

		public static StackTrace Wrap( ICfixStackTrace trace )
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
					frames[ i ] = new StackTraceFrame( ctlFrame );
				}
				finally
				{
					Marshal.ReleaseComObject( ctlFrame );
				}
			}

			Marshal.ReleaseComObject( trace );
			return new StackTrace( frames );
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
