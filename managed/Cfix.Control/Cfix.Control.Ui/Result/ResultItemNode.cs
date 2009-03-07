using System;
using System.Collections.Generic;

namespace Cfix.Control.Ui.Result
{
	internal class ResultItemNode : IResultNode
	{
		private readonly IResultItem result;

		public ResultItemNode( IResultItem result )
		{
			this.result = result;
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
				yield return new ResultItemNode( child );
			}
		}

		public string Name
		{
			get { return this.result.Name; }
		}

		public string Location
		{
			get { return ""; }
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
