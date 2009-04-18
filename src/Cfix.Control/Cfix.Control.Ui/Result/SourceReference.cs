using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Cfix.Control.Ui.Result
{
	public abstract class SourceReference : ISourceReference
	{
		private readonly String file;
		private readonly String fileShort;
		private readonly uint line;

		private readonly ResultItemNode parent;

		public SourceReference( string file, uint line, ResultItemNode parent )
		{
			this.file = file;
			this.line = line;
			this.parent = parent;

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
			get 
			{
				//
				// Sometimes (most commonly in x64), the compiler generates
				// file names in the form of .\foo.c -- resolve these paths.
				//
				if ( !Path.IsPathRooted( this.file ) )
				{
					ITestItem item = parent.ResultItem.Item;
					do
					{
						IRelativePathReferenceItem refItem =
							item as IRelativePathReferenceItem;
						if ( refItem != null )
						{
							//
							// This item is the reference the path is relative
							// to.
							//
							string absPath = refItem.GetFullPath( this.file );
							Debug.Assert( Path.IsPathRooted( absPath ) );

							return absPath;
						}

						item = item.Parent;
					}
					while ( item != null );
				}

				return this.file; 
			}
		}

		public uint Line
		{
			get { return this.line; }
		}
	}
}
