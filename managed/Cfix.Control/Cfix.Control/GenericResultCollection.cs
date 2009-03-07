using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Cfix.Control
{
	public class GenericResultCollection :
		AbstractResultItem, IResultItemCollection
	{
		private readonly IList<IResultItem> subItems;
		private readonly ITestItemCollection item;

		public GenericResultCollection(
			IActionEvents events,
			IResultItemCollection parent,
			ITestItemCollection itemCollection,
			ExecutionStatus status
			) 
			: base( events, parent, itemCollection, status )
		{
			Debug.Assert( events != null );
			Debug.Assert( itemCollection != null );
			
			this.item = itemCollection;

			//
			// Add children.
			//
			this.subItems = new List<IResultItem>(
				( int ) itemCollection.ItemCount );

			foreach ( ITestItem child in itemCollection )
			{
				IResultItemFactory fac = child as IResultItemFactory;
				if ( fac != null )
				{
					this.subItems.Add( fac.CreateResultItem(
						this,
						events,
						status ) );
				}
			}
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		public override ICollection<Failure> Failures
		{
			get { return null; }
		}

		/*--------------------------------------------------------------
		 * IResultItemCollection.
		 */

		public ITestItemCollection ItemCollection
		{
			get { return this.item; }
		}

		public uint ItemCount
		{
			get { return ( uint ) this.subItems.Count; }
		}

		public IResultItem GetItem( uint ordinal )
		{
			return this.subItems[ ( int ) ordinal ];
		}

		/*--------------------------------------------------------------
		 * IEnumerable.
		 */

		public IEnumerator<IResultItem> GetEnumerator()
		{
			return this.subItems.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.subItems.GetEnumerator();
		}
	}
}
