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
	public class GenericTestItemCollection : ITestItemCollection
	{
		private readonly ITestItemCollection parent;
		private readonly String name;
		protected readonly Object listLock = new Object();
		protected readonly List<ITestItem> list = new List<ITestItem>();

		private bool disposed;

		public event EventHandler Disposed;

		public GenericTestItemCollection( ITestItemCollection parent, String name )
		{
			this.parent = parent;
			this.name = name;
		}

		~GenericTestItemCollection()
		{
			Dispose( false );
		}

		/*--------------------------------------------------------------
		 * Protected.
		 */

		protected void OnItemAdded( ITestItem item )
		{
			if ( this.ItemAdded != null )
			{
				this.ItemAdded( this, new TestItemEventArgs( item ) );
			}
		}

		protected void OnItemRemoved( ITestItem item )
		{
			if ( this.ItemRemoved != null )
			{
				this.ItemRemoved( this, new TestItemEventArgs( item ) );
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

		public bool IsDisposed
		{
			get { return this.disposed; }
		}

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

		public ITestItemCollection Parent
		{
			get
			{
				return this.parent;
			}
		}

		/*--------------------------------------------------------------
		 * ITestItemCollection.
		 */

		public event EventHandler< TestItemEventArgs > ItemAdded;
		public event EventHandler< TestItemEventArgs > ItemRemoved;

		//public void CreateAction(
		//    ICompositeAction actionToComposeWith,
		//    SchedulingOptions schedulingOptions,
		//    CompositionOptions compositionOptions
		//    )
		//{
		//    lock ( this.listLock )
		//    {
		//        foreach ( ITestItem item in this.list )
		//        {
		//            item.CreateAction(
		//                actionToComposeWith,
		//                schedulingOptions,
		//                compositionOptions );
		//        }
		//    }
		//}

		public IAction CreateAction(
			IHost hostToRunOn,
			SchedulingOptions schedOpts,
			ThreadingOptions threadingOptions
			)
		{
			throw new NotImplementedException();
		}

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

		public uint ItemCountRecursive
		{
			get
			{
				lock ( this.listLock )
				{
					uint count = 0;
					foreach ( ITestItem item in this.list )
					{
						ITestItemCollection subCont = item
							as ITestItemCollection;

						if ( subCont != null )
						{
							count += subCont.ItemCountRecursive;
						}
						else
						{
							count++;
						}
					}

					return count;
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

		public void Clear()
		{
			lock ( this.listLock )
			{
				foreach ( ITestItem item in this.list )
				{
					OnItemRemoved( item );
					item.Dispose();
				}

				this.list.Clear();
			}
		}

		protected virtual void Dispose( bool disposing )
		{
			lock ( this.listLock )
			{
				foreach ( ITestItem item in this.list )
				{
					item.Dispose();
				}
			}

			if ( ! this.disposed )
			{
				if ( Disposed != null )
				{
					Disposed( this, EventArgs.Empty );
				}

				this.disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

	}
}
