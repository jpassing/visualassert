using System;
using System.IO;
using System.Text;

namespace Cfix.Control.Ui.Result
{
	public class SourceReference : ISourceReference
	{
		private readonly String file;
		private readonly String fileShort;
		private readonly uint line;

		public SourceReference( string file, uint line )
		{
			this.file = file;
			this.line = line;

			if ( file != null )
			{
				this.fileShort = new FileInfo( file ).Name;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)" )]
		public String Location
		{
			get
			{
				if ( this.fileShort != null )
				{
					return String.Format( "{0}:{1}", this.fileShort, this.line );
				}
				else
				{
					return null;
				}
			}
		}

		/*----------------------------------------------------------------------
		 * ISourceReference.
		 */

		public string File
		{
			get { return this.file; }
		}

		public uint Line
		{
			get { return this.line; }
		}
	}
}
