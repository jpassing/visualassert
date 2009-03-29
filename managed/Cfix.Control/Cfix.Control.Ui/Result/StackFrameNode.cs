using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Cfix.Control;

namespace Cfix.Control.Ui.Result
{
	public class StackFrameNode : IResultNode, ISourceReference
	{
		private readonly IStackTraceFrame frame;
		private readonly string name;
		private readonly ImageList iconsList;
		
		private bool IsProperlyUnwound
		{
			get { return this.frame.Function != null; }
		}

		private bool IsCfixFrame
		{
			get
			{
				return this.frame.Module.Equals(
						"cfix",
						StringComparison.OrdinalIgnoreCase );
			}
		}

		public StackFrameNode( 
			IStackTraceFrame frame, 
			string name,
			ImageList iconsList )
		{
			this.frame = frame;
			this.name = name;
			this.iconsList = iconsList;
		}

		public static IEnumerable<IResultNode> EnumerateFrames(
			IStackTrace stackTrace,
			ImageList iconsList 
			)
		{
			if ( stackTrace == null )
			{
				yield break;
			}
			else
			{
				bool inCapturingCode = true;
				bool warningShown = false;
				int i = 0;
				foreach ( IStackTraceFrame frame in stackTrace )
				{
					StackFrameNode node = new StackFrameNode( 
						frame, 
						i.ToString(),
						iconsList );
					if ( inCapturingCode && node.IsCfixFrame )
					{
						//
						// Ignore CfixpCatureStackTrace etc.
						//
						continue;
					}
					else
					{
						inCapturingCode = false;
					}

					if ( ! warningShown && ! node.IsProperlyUnwound )
					{
						warningShown = true;
						yield return new UnwindWarningNode();
					}

					yield return node;
					i++;
				}
			}
		}

		/*----------------------------------------------------------------------
		 * ISourceReference.
		 */

		public string File
		{
			get { return this.frame.SourceFile; }
		}

		public uint Line
		{
			get { return this.frame.SourceLine; }
		}

		/*----------------------------------------------------------------------
		 * IResultNode.
		 */

		public bool IsLeaf
		{
			get
			{
				return true;
			}
		}

		public IEnumerable<IResultNode> GetChildren()
		{
			yield break;
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
			get { return null; }
		}

		public String Expression
		{
			get 
			{
				if ( this.frame.Function == null )
				{
					return "[unknown]";
				}
				else
				{
					return String.Format(
						"{0}!{1}", this.frame.Module, this.frame.Function );
				}
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)" )]
		public String Location
		{
			get
			{
				if ( this.frame.SourceFile != null )
				{
					return String.Format( 
						"{0}:{1}", 
						this.frame.SourceFile, 
						this.frame.SourceLine );
				}
				else
				{
					return null;
				}
			}
		}

		public String Routine
		{
			get { return this.frame.Function; }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)" )]
		public String LastError
		{
			get
			{
				return null;
			}
		}

		public string Duration
		{
			get { return String.Empty; }
		}

		public Image Icon
		{
			get { return this.iconsList.Images[ ResultExplorer.StackFrameIconIndex ]; }
		}
	}

	public class UnwindWarningNode : IResultNode
	{
		public bool IsLeaf
		{
			get { return true; }
		}

		public IEnumerable<IResultNode> GetChildren()
		{
			yield break;
		}

		public Image Icon
		{
			get { return null; }
		}

		public string Status
		{
			get { return null; }
		}

		public string Name
		{
			get { return null; }
		}

		public string Expression
		{
			get { return Strings.MissingUnwindInfo; }
		}

		public string Message
		{
			get { return null; }
		}

		public string Location
		{
			get { return null; }
		}

		public string Routine
		{
			get { return null; }
		}

		public string LastError
		{
			get { return null; }
		}

		public string Duration
		{
			get { return null; }
		}
	}
}
