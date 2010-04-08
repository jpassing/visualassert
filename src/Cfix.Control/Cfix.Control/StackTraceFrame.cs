using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	/*++
		Generic implementation of IStackTraceFrame.
	--*/
	public class StackTraceFrame : IStackTraceFrame
	{
		private string module;
		private string function;
		private uint displacement;
		private string sourceFile;
		private uint sourceLine;

		public StackTraceFrame(
			string module,
			string function,
			uint displacement,
			string sourceFile,
			uint sourceLine
			)
		{
			this.module = module;
			this.function = function;
			this.displacement = displacement;
			this.sourceFile = sourceFile;
			this.sourceLine = sourceLine;
		}

		public override string ToString()
		{
			return String.Format( "{0}!{1}+0x{2:x}",
				( this.module ?? "[unknown]" ),
				( this.function ?? "[unknown]" ),
				this.displacement );
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

}
