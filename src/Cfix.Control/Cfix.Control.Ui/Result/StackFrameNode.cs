using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Cfix.Control;

namespace Cfix.Control.Ui.Result
{
	public class StackFrameNode : SourceReference, IResultNode
	{
		private enum FrameType { CapturingCode, UserCode, FrameworkCode }

		private readonly IStackTraceFrame frame;
		private readonly string name;
		private readonly ImageList iconsList;
		private readonly FrameType type;

		private readonly ResultItemNode parent;

		private bool IsProperlyUnwound
		{
			get { return this.frame.Function != null; }
		}

		private static bool IsCfixFrame( string module )
		{
			return module != null && module.Equals(
					"cfix",
					StringComparison.OrdinalIgnoreCase );
		}

		private StackFrameNode( 
			IStackTraceFrame frame, 
			string name,
			FrameType type,
			ImageList iconsList,
			ResultItemNode parent
			)
			: base( frame.SourceFile, frame.SourceLine, parent )
		{
			this.frame = frame;
			this.name = name;
			this.iconsList = iconsList;
			this.type = type;
			this.parent = parent;
		}

		public static IEnumerable<IResultNode> EnumerateFrames(
			IStackTrace stackTrace,
			ImageList iconsList,
			ResultItemNode parent
			)
		{
			if ( stackTrace == null )
			{
				yield break;
			}
			else
			{
				FrameType frameType = FrameType.CapturingCode;
				bool warningShown = false;
				int i = 0;
				foreach ( IStackTraceFrame frame in stackTrace )
				{
					if ( frameType == FrameType.CapturingCode )
					{
						if ( IsCfixFrame( frame.Module ) )
						{
							//
							// Ignore CfixpCatureStackTrace etc.
							//
							continue;
						}
						else
						{
							frameType = FrameType.UserCode;
						}
					}
					else if ( frameType == FrameType.UserCode && 
						      IsCfixFrame( frame.Module ) )
					{
						frameType = FrameType.FrameworkCode;
					}

					StackFrameNode node = new StackFrameNode(
						frame,
						i.ToString(),
						frameType,
						iconsList,
						parent );
					
					if ( !warningShown && !node.IsProperlyUnwound )
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

		public Color? TextColor
		{
			get
			{
				if ( this.type != FrameType.UserCode )
				{
					return Color.Gray;
				}
				else
				{
					return null;
				}
			}
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

		public Color? TextColor
		{
			get { return null; }
		}
	}
}
