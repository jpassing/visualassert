using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Cfix.Control
{
	/// <summary>
	/// Generic container for test items.
	/// 
	/// Threadsafe.
	/// </summary>
	public class GenericTestItemCollection :
		ITestItemCollection, IRunnableTestItemCollection
	{
		private readonly ITestItemCollection parent;
		private readonly String name;
		protected readonly Object listLock = new Object();
		protected readonly IList<ITestItem> list = new List<ITestItem>();
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

		protected virtual void Dispose( bool disposing )
		{
			lock ( this.listLock )
			{
				foreach ( ITestItem item in this.list )
				{
					if ( item != null )
					{
						item.Dispose();
					}
				}
			}

			if ( !this.disposed )
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

		[Browsable( false )]
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

		[Browsable( false )]
		public String FullName
		{
			get { return null; }
		}

		[Browsable( false )]
		public uint Ordinal
		{
			get
			{
				return 0;
			}
		}

		[Browsable( false )]
		public ITestItemCollection Parent
		{
			get
			{
				return this.parent;
			}
		}

		/*--------------------------------------------------------------
		 * IResultItemFactory.
		 */

		public IResultItem CreateResultItem(
			IResultItemCollection parentResult,
			IActionEvents events,
			ExecutionStatus interimStatus
			)
		{
			return new GenericResultCollection(
				events,
				parentResult,
				this,
				interimStatus );
		}

		/*--------------------------------------------------------------
		 * ITestItemCollection.
		 */

		public event EventHandler< TestItemEventArgs > ItemAdded;
		public event EventHandler< TestItemEventArgs > ItemRemoved;

		public void Add(
			IRunCompiler compiler,
			IActionEvents events,
			IResultItem result
			)
		{
			Debug.Assert( compiler != null );
			Debug.Assert( events != null );
			Debug.Assert( result != null );

			Debug.Assert( result is IResultItemCollection );
			IResultItemCollection resultColl = ( IResultItemCollection ) result;

			//
			// N.B. The item collection may contain invalid modules etc,
			// so the # of results may be smaller than the # of items.
			//
			Debug.Assert( resultColl.ItemCount <= this.ItemCount );

			//
			// Let children add their actions by themselves.
			//
			lock ( this.listLock )
			{
				foreach ( ITestItem child in this.list )
				{
					//
					// Find appropriate result.
					//
					// N.B. For invalid modules etc, childResult will be
					// null.
					//
					IResultItem childResult = resultColl.GetItem( child );
					if ( childResult != null )
					{
						Debug.Assert( ReferenceEquals(
							childResult.Item, child ) );

						child.Add(
							compiler,
							events,
							childResult );
					}
				}
			}
		}

		public ITestItem GetItem( uint ordinal )
		{
			lock ( this.listLock )
			{
				return this.list[ ( int ) ordinal ];
			}
		}

		[DescriptionAttribute( "Number of children" )]
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

		[DescriptionAttribute( "Total number of children" )]
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

		/*--------------------------------------------------------------
		 * IRunnableTestItemCollection.
		 */

		//[Browsable( false )]
		public uint RunnableItemCount
		{
			get
			{
				uint count = 0;
				lock ( this.listLock )
				{
					foreach ( ITestItem item in this.list )
					{
						if ( item is IRunnableTestItem )
						{
							count++;
						}
					}
				}

				return count;
			}
		}

		//[Browsable( false )]
		public uint RunnableItemCountRecursive
		{
			get
			{
				uint count = 0;
				lock ( this.listLock )
				{
					foreach ( ITestItem item in this.list )
					{
						if ( item is IRunnableTestItemCollection )
						{
							count += ( ( IRunnableTestItemCollection ) item ).RunnableItemCountRecursive;
						}
						else if ( item is IRunnableTestItem )
						{
							count++;
						}
					}
				}

				return count;
			}
		}

		//[Browsable( false )]
		public bool IsRunnable
		{
			get
			{
				if ( RunnableItemCountRecursive > 0 )
				{
					return true;
				}
				else
				{
					lock ( this.listLock )
					{
						foreach ( ITestItem item in this.list )
						{
							if ( item is IRunnableTestItem )
							{
								//
								// At least one item is runnable, that
								// is enough.
								//
								return true;
							}
						}

						return false;
					}
				}
			}
		}
	}
}
