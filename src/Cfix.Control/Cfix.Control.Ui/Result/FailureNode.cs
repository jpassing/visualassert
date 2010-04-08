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

		private readonly IResultNode parentNode;
		private readonly ResultItemNode parentItemNode;

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
			IResultNode parentNode,			// "real" parent.
			ResultItemNode parentItemNode	// next ResultItemNode, may be grandparent.
			)
			: base( file, line, parentItemNode )
		{
			this.name = name;
			this.message = message;
			this.stackTrace = stackTrace;
			this.routine = routine;
			this.expression = expression;
			this.lastError = lastError;
			this.iconsList = iconsList;
			this.iconIndex = iconIndex;

			this.parentNode = parentNode;
			this.parentItemNode = parentItemNode;
		}

		public static FailureNode Create(
			Failure f,
			ImageList iconsList,
			ResultItemNode parent
			)
		{
			return Create( f, iconsList, parent, parent );
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)" )]
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)" ), System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1800:DoNotCastUnnecessarily" )]
		private static FailureNode Create( 
			Failure f, 
			ImageList iconsList,
			IResultNode parentNode,
			ResultItemNode parentItemNode
			)
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
					parentNode,
					parentItemNode );
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
					parentNode,
					parentItemNode );
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
					parentNode,
					parentItemNode );
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
					parentNode,
					parentItemNode );
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
					parentNode,
					parentItemNode );
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
					parentNode,
					parentItemNode );
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
					parentNode,
					parentItemNode );
			}
			else if ( f is ExpandoFailure )
			{
				return new ExpandoFailureNode(
					( ExpandoFailure ) f,
					iconsList,
					ResultExplorer.GenericErrorIconIndex,
					parentItemNode );
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
			get { return this.parentNode; }
		}

		public virtual bool IsLeaf
		{
			get
			{
				return this.stackTrace == null;
			}
		}

		public virtual IEnumerable<IResultNode> GetChildren( ResultNodeFilter filter )
		{
			return StackFrameNode.EnumerateFrames( 
				this.stackTrace,
				this.iconsList,
				this.parentItemNode );
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

		/*----------------------------------------------------------------------
		 * Expando Failure.
		 */

		private class ExpandoFailureNode : FailureNode
		{
			private readonly ExpandoFailure failure;
			private readonly ResultItemNode parentItemNode;

			public ExpandoFailureNode(
				ExpandoFailure failure,
				ImageList iconsList,
				int iconIndex,
				ResultItemNode parent 
				)
				: base(
					failure.Message,
					null,
					null,
					null,
					0,
					null,
					-1,
					null,
					iconsList,
					iconIndex,
					parent,
					parent )
			{
				this.failure = failure;
				this.parentItemNode = parent;
			}

			public override IEnumerable<IResultNode> GetChildren( 
				ResultNodeFilter filter 
				)
			{
				foreach ( Failure subFailure in this.failure.GetFailures() )
				{
					yield return Create( 
						subFailure, 
						this.iconsList, 
						this,
						this.parentItemNode );
				}
			}

			public override bool IsLeaf
			{
				get
				{
					return false;
				}
			}
		}
	}
}
