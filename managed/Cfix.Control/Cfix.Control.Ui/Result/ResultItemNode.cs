using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cfix.Control.Ui.Result
{
	internal class ResultItemNode : IResultNode
	{
		private readonly IResultItem result;
		private readonly ImageList iconsList;

		public ResultItemNode( IResultItem result, ImageList iconsList )
		{
			this.result = result;
			this.iconsList = iconsList;
		}

		/*----------------------------------------------------------------------
		 * IResultNode.
		 */

		public bool IsLeaf 
		{
			get
			{
				return !( this.result is IResultItemCollection );
			}
		}

		public System.Collections.IEnumerable GetChildren()
		{
			IResultItemCollection coll = this.result as IResultItemCollection;
			if ( coll == null )
			{
				yield break;
			}

			foreach ( IResultItem child in coll )
			{
				yield return new ResultItemNode( child, this.iconsList );
			}
		}

		public Image Icon 
		{
			get 
			{ 
				return this.iconsList.Images[ ( int ) this.result.Status ];
			}
		}

		public string Name
		{
			get { return this.result.Name; }
		}

		public string Expression
		{
			get { return ""; }
		}

		public string Message
		{
			get { return ""; }
		}

		public string Location
		{
			get { return ""; }
		}

		public string Routine
		{
			get { return ""; }
		}

		public uint LastError
		{
			get { return 0; }
		}

		public int Failures 
		{
			get 
			{ 
				ICollection<Failure> failures = this.result.Failures;
				if ( failures == null )
				{
					return 0;
				}
				else
				{
					return failures.Count;
				}
			}
		}

		public string Status
		{
			get { return this.result.Status.ToString(); }
		}
	}
}
