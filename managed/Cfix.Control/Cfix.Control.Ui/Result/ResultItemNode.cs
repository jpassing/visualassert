using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cfix.Control.Ui.Result
{
	internal class ResultItemNode : IResultNode
	{
		private readonly ResultItemNode parent;
		private readonly IResultItem result;
		private readonly ImageList iconsList;

		public ResultItemNode( IResultItem result, ImageList iconsList, ResultItemNode parent )
		{
			this.parent = parent;
			this.result = result;
			this.iconsList = iconsList;
		}

		public IResultItem ResultItem
		{
			get { return this.result; }
		}

		internal ResultItemNode Parent
		{
			get { return this.parent; }
		}

		internal ICollection<Failure> Failures
		{
			get { return this.result.Failures; }
		}

		internal bool HasFailures
		{
			get { return this.result.Failures != null && this.result.Failures.Count > 0; }
		}

		/*----------------------------------------------------------------------
		 * IResultNode.
		 */

		public bool IsLeaf 
		{
			get
			{
				return ! ( this.result is IResultItemCollection ) && ! HasFailures;
			}
		}

		public IEnumerable<IResultNode> GetChildren()
		{
			IResultItemCollection coll = this.result as IResultItemCollection;
			if ( coll != null )
			{
				foreach ( IResultItem child in coll )
				{
					yield return new ResultItemNode( child, this.iconsList, this );
				}
			}

			if ( this.result.Failures != null )
			{
				foreach ( Failure f in this.result.Failures )
				{
					yield return FailureNode.Create( f, this.iconsList );
				}
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
			get { return null; }
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

		public string Status
		{
			get { return this.result.Status.ToString(); }
		}
	}
}
