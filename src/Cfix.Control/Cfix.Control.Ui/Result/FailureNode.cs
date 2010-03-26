using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Cfix.Control.Ui.Result
{
	public class FailureNode : SourceReference, IResultNode
	{
		private readonly String name;
		private readonly String message;
		private readonly IStackTrace stackTrace;
		private readonly String routine;
		private readonly String expression;
		private readonly int lastError;

		private readonly ImageList iconsList;
		private readonly int iconIndex;

		private readonly ResultItemNode parent;

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
			int iconIndex,
			ResultItemNode parent
			)
			: base( file, line, parent )
		{
			this.name = name;
			this.message = message;
			this.stackTrace = stackTrace;
			this.routine = routine;
			this.expression = expression;
			this.lastError = lastError;
			this.iconsList = iconsList;
			this.iconIndex = iconIndex;
			this.parent = parent;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)" )]
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)" ), System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1800:DoNotCastUnnecessarily" )]
		public static FailureNode Create( 
			Failure f, 
			ImageList iconsList, 
			ResultItemNode parent )
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
					ResultExplorer.InconclusiveneIconIndex,
					parent );
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
					ResultExplorer.UnhandledExceptionIconIndex,
					parent );
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
					ResultExplorer.FailedAssertionIconIndex,
					parent );
			}
			else if ( f is GenericCodeInformation )
			{
				GenericCodeInformation a = ( GenericCodeInformation ) f;
				return new FailureNode(
					a.Message,
					null,
					null,
					a.File,
					a.Line,
					a.Routine,
					-1,
					a.StackTrace,
					iconsList,
					ResultExplorer.GenericInformationIconIndex,
					parent );
			}
			else if ( f is GenericCodeWarning )
			{
				GenericCodeWarning a = ( GenericCodeWarning ) f;
				return new FailureNode(
					a.Message,
					null,
					null,
					a.File,
					a.Line,
					a.Routine,
				 	-1,
					a.StackTrace,
					iconsList,
					ResultExplorer.GenericWarningIconIndex,
					parent );
			}
			else if ( f is GenericCodeError )
			{
				GenericCodeError a = ( GenericCodeError ) f;
				return new FailureNode(
					a.Message,
					null,
					null,
					a.File,
					a.Line,
					a.Routine,
					-1,
					a.StackTrace,
					iconsList,
					ResultExplorer.GenericErrorIconIndex,
					parent );
			}
			else if ( f is GenericError )
			{
				GenericError a = ( GenericError ) f;
				return new FailureNode(
					a.Message,
					null,
					null,
					null,
					0,
					null,
					-1,
					a.StackTrace,
					iconsList,
					ResultExplorer.GenericErrorIconIndex,
					parent );
			}
			else
			{
				throw new ArgumentException( "Unrecognized failure type" );
			}
		}

		public IStackTrace StackTrace
		{
			get { return this.stackTrace; }
		}

		/*----------------------------------------------------------------------
		 * IResultNode.
		 */

		public IResultNode Parent
		{
			get { return this.parent; }
		}

		public bool IsLeaf
		{
			get
			{
				return this.stackTrace == null;
			}
		}

		public IEnumerable<IResultNode> GetChildren( ResultNodeFilter filter )
		{
			return StackFrameNode.EnumerateFrames( 
				this.stackTrace,
				this.iconsList,
				this.parent );
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

		public Color? TextColor
		{
			get { return null; }
		}
	}
}
