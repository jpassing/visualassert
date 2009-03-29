using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Cfix.Control.Ui.Result
{
	public class FailureNode : IResultNode, ISourceReference
	{
		private readonly String name;
		private readonly String message;
		private readonly IStackTrace stackTrace;
		private readonly String file;
		private readonly uint line;
		private readonly String routine;
		private readonly String expression;
		private readonly int lastError;

		private readonly ImageList iconsList;
		private readonly int iconIndex;

		private static string Resolve( int errorCode )
		{
			return new Win32Exception( errorCode ).Message;
		}

		private FailureNode( 
			String name,
			String message,
			String expression,
			String file,
			uint line,
			String routine,
			int lastError,
			IStackTrace stackTrace,
			ImageList iconsList,
			int iconIndex 
			)
		{
			this.name = name;
			this.message = message;
			this.stackTrace = stackTrace;
			this.file = file;
			this.line = line;
			this.routine = routine;
			this.expression = expression;
			this.lastError = lastError;
			this.iconsList = iconsList;
			this.iconIndex = iconIndex;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)" ), System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1800:DoNotCastUnnecessarily" )]
		public static FailureNode Create( Failure f, ImageList iconsList )
		{
			if ( f is Inconclusiveness )
			{
				Inconclusiveness i = ( Inconclusiveness ) f;
				return new FailureNode(
					Strings.Inconclusiveness,
					i.Message,
					null,
					null,
					0,
					null,
					-1,
					i.StackTrace,
					iconsList,
					ResultExplorer.InconclusiveneIconIndex );
			}
			else if ( f is UnhandledExceptionFailure )
			{
				UnhandledExceptionFailure u = ( UnhandledExceptionFailure ) f;
				return new FailureNode(
					Strings.UnhandledException,
					String.Format( 
						"0x{0:X} ({1})", 
						u.ExceptionCode,
						Resolve( ( int ) u.ExceptionCode ) ),
					null,
					null,
					0,
					null,
					-1,
					u.StackTrace,
					iconsList,
					ResultExplorer.UnhandledExceptionIconIndex );
			}
			else if ( f is FailedAssertionFailure )
			{
				FailedAssertionFailure a = ( FailedAssertionFailure ) f;
				return new FailureNode(
					Strings.FailedAssertion,
					a.Message,
					a.Expression,
					a.File,
					a.Line,
					a.Routine,
					( int ) a.LastError,
					a.StackTrace,
					iconsList,
					ResultExplorer.FailedAssertionIconIndex );
			}
			else
			{
				throw new ArgumentException( "Unrecognized failure type" );
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

		/*----------------------------------------------------------------------
		 * IResultNode.
		 */

		public bool IsLeaf
		{
			get
			{
				return this.stackTrace == null;
			}
		}

		public IEnumerable<IResultNode> GetChildren()
		{
			return StackFrameNode.EnumerateFrames( 
				this.stackTrace,
				this.iconsList );
		}

		public String Status
		{
			get { return null; }
		}

		public String Name
		{
			get { return this.name; }
		}

		public String Message
		{
			get { return this.message; }
		}

		public String Expression
		{
			get { return this.expression; }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)" )]
		public String Location
		{
			get 
			{ 
				if ( this.file != null )
				{
					return String.Format( "{0}:{1}", this.file, this.line ); 
				}
				else
				{
					return null;
				}
			}
		}

		public String Routine
		{
			get { return this.routine; }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)" )]
		public String LastError
		{
			get 
			{
				if ( this.lastError == -1 )
				{
					return null;
				}
				else
				{
					return String.Format( 
						"{0} ({1})", 
						this.lastError, 
						Resolve( this.lastError ) ); 
				}
			}
		}

		public string Duration
		{
			get { return String.Empty; }
		}

		public Image Icon
		{
			get { return this.iconsList.Images[ this.iconIndex ]; }
		}
	}
}
