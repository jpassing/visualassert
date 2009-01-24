using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	/// <summary>
	/// Generic container for test items.
	/// 
	/// Threadsafe.
	/// </summary>
	public class GenericTestItemContainer : ITestItemCollection
	{
		private readonly String name;
		private readonly Object listLock = new Object();
		private readonly List<ITestItem> list = new List<ITestItem>();

		public GenericTestItemContainer( String name )
		{
			this.name = name;
		}

		/*--------------------------------------------------------------
		 * Protected.
		 */

		protected void OnItemAdded( ITestItem item )
		{
			if ( this.ItemAdded != null )
			{
				this.ItemAdded( this, item );
			}
		}

		protected void OnItemRemoved( ITestItem item )
		{
			if ( this.ItemRemoved != null )
			{
				this.ItemRemoved( this, item );
			}
		}

		/*--------------------------------------------------------------
		 * Publics.
		 */

		public void Add( ITestItem item )
		{
			lock ( this.listLock )
			{
				this.list.Add( item );
				OnItemAdded( item );
			}
		}

		public bool Remove( ITestItem item )
		{
			lock ( this.listLock )
			{
				bool removed = this.list.Remove( item );

				if ( removed )
				{
					OnItemAdded( item );
				}

				return removed;
			}
		}

		/*--------------------------------------------------------------
		 * IEnumerable.
		 */

		public IEnumerator<ITestItem> GetEnumerator()
		{
			lock ( this.listLock )
			{
				return new List<ITestItem>( this.list ).GetEnumerator();
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			lock ( this.listLock )
			{
				return new List<ITestItem>( this.list ).GetEnumerator();
			}
		}

		/*--------------------------------------------------------------
		 * ITestItem.
		 */

		public String Name 
		{
			get
			{
				return this.name;
			}
		}

		public uint Ordinal
		{
			get
			{
				return 0;
			}
		}

		public IAction CreateAction( SchedulingOptions flags )
		{
			IList<IAction> actions = new List<IAction>();
			lock ( this.listLock )
			{
				foreach ( ITestItem item in this.list )
				{
					actions.Add( item.CreateAction( flags ) );
				}
			}

			return new SequenceAction( this, actions );
		}

		/*--------------------------------------------------------------
		 * ITestItemCollection.
		 */

		public event TestItemChangedEventHandler ItemAdded;
		public event TestItemChangedEventHandler ItemRemoved;


		public ITestItem GetItem( uint ordinal )
		{
			lock ( this.listLock )
			{
				return this.list[ ( int ) ordinal ];
			}
		}

		public uint ItemCount
		{
			get
			{
				lock ( this.listLock )
				{
					return ( uint ) this.list.Count;
				}
			}
		}

		public virtual void Refresh()
		{
			lock ( this.listLock )
			{
				foreach ( ITestItem item in this.list )
				{
					ITestItemCollection coll = item as ITestItemCollection;
					if ( coll != null )
					{
						coll.Refresh();
					}
				}
			}
		}

		public void Dispose()
		{
			lock ( this.listLock )
			{
				foreach ( ITestItem item in this.list )
				{
					item.Dispose();
				}
			}
		}
	}
}
