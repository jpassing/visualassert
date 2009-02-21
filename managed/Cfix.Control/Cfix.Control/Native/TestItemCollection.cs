using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly" )]
	public class TestItemCollection : TestItem, ITestItemCollection
	{
		/// <summary>
		/// Children, indexed by ordinal.
		/// </summary>
		private ITestItem[] subItems;

		/// <summary>
		/// Required for updating.
		/// </summary>
		private IDictionary< String, ITestItem > subItemsDict 
			= new Dictionary< String, ITestItem >();

		private readonly Object updateLock = new Object();
		private readonly bool ignoreDuplicates;
		
		/*--------------------------------------------------------------
		 * Publics.
		 */

		internal TestItemCollection(
			TestItemCollection parent,
			uint ordinal,
			ICfixTestItem item,
			bool ignoreDuplicates
			)
			: base( parent, ordinal, item )
		{
			this.ignoreDuplicates = ignoreDuplicates;
		}

		internal TestItemCollection(
			TestItemCollection parent,
			uint ordinal,
			ICfixTestItem item
			)
			: this( parent, ordinal, item, parent.IgnoreDuplicates )
		{ }

		public bool IgnoreDuplicates
		{
			get { return ignoreDuplicates; }
		}
		
		internal void Update( ICfixTestContainer container )
		{
			lock ( updateLock )
			{
				try
				{
					this.subItems = new ITestItem[ container.GetItemCount() ];

					IDictionary<String, ITestItem> newSubItemsDict
						= new Dictionary<String, ITestItem>();

					for ( uint i = 0; i < subItems.Length; i++ )
					{
						ICfixTestItem newItem = container.GetItem( i );

						try
						{
							bool itemAdded = false;

							if ( newSubItemsDict.ContainsKey( newItem.GetName() ) )
							{
								//
								// Duplicate/ambiguous name.
								//
								if ( this.ignoreDuplicates )
								{
									continue;
								}
								else
								{
									Clear();
									throw new CfixException(
										String.Format( "Ambiguous test case name '{0}'",
										newItem.GetName() ) );
								}
							}

							
							//
							// See if we know this item.
							//
							ITestItem existingItem;
							if ( this.subItemsDict.TryGetValue( newItem.GetName(), out existingItem ) )
							{
								//
								// This item was there before...
								//
								if ( existingItem.Ordinal != i )
								{
									//
									// ...but has changed its position. Re-add.
									//
									OnItemRemoved( existingItem );
									itemAdded = true;
								}
								else
								{
									//
									// ...and remains valid.
									//
								}

								//
								// Remove it from subItemsDict mark it has 
								// having been processed.
								//
								this.subItemsDict.Remove( existingItem.Name );
							}
							else
							{
								//
								// This item is new.
								//
								itemAdded = true;
							}

							if ( itemAdded )
							{
								this.subItems[ i ] = TestItem.Wrap(
									this,
									i,
									newItem );
							}
							else
							{
								this.subItems[ i ] = existingItem;
							}

							Debug.Assert( this.subItems[ i ] != null );

							TestItemCollection subContainer =
								this.subItems[ i ] as TestItemCollection;
							if ( subContainer != null )
							{
								subContainer.Update(
									( ICfixTestContainer ) newItem );
							}

							if ( itemAdded )
							{
								OnItemAdded( this.subItems[ i ] );
							}
						}
						finally
						{
							Module.Target.ReleaseObject( newItem );
						}

						newSubItemsDict.Add( this.subItems[ i ].Name, this.subItems[ i ] );
					}

					//
					// All items left in subItemsDict have been removed.
					//
					foreach ( ITestItem item in this.subItemsDict.Values )
					{
						OnItemRemoved( item );
					}

					this.subItemsDict = newSubItemsDict;
				}
				catch ( COMException x )
				{
					throw this.Module.Target.WrapException( x );
				}
			}
		}

		public void Clear()
		{
			for ( int i = 0; i < this.subItems.Length; i++ )
			{
				if ( this.subItems[ i ] != null )
				{
					OnItemRemoved( this.subItems[ i ] );
				}
			}

			this.subItems = null;
			this.subItemsDict = null;
		}

		/*--------------------------------------------------------------
		 * IEnumerable.
		 */

		public IEnumerator< ITestItem > GetEnumerator()
		{
			return ( ( IList< ITestItem > ) this.subItems ).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.subItems.GetEnumerator();
		}

		/*--------------------------------------------------------------
		 * ITestItemContainer.
		 */

		public event EventHandler<TestItemEventArgs> ItemAdded;
		public event EventHandler<TestItemEventArgs> ItemRemoved;

		public override void CreateAction(
			ICompositeAction actionToComposeWith,
			SchedulingOptions schedulingOptions,
			CompositionOptions compositionOptions
			)
		{
			if ( ( compositionOptions & CompositionOptions.FineGrained ) ==
				 CompositionOptions.FineGrained )
			{
				//
				// Recurse and add items individually - this will result in 
				// multiple small actions rather than few SequenceActions.
				//
				foreach ( ITestItem item in this.subItems )
				{
					item.CreateAction(
						actionToComposeWith,
						schedulingOptions,
						compositionOptions );
				}
			}
			else
			{
				base.CreateAction( 
					actionToComposeWith, 
					schedulingOptions, 
					compositionOptions );
			}
		}

		public ITestItem GetItem( uint ordinal )
		{
			return this.subItems[ ordinal ];
		}

		public uint ItemCount
		{
			get
			{
				if ( this.subItems == null )
				{
					return 0;
				}
				else
				{
					return ( uint ) this.subItems.Length;
				}
			}
		}

		public virtual void Refresh()
		{
			NativeConnection connection = NativeConnection;
			try
			{
				Update( ( ICfixTestContainer ) connection.Item );
			}
			finally
			{
				this.Module.Target.ReleaseObject( connection.Item );
				this.Module.Target.ReleaseObject( connection.Host );
			}
		}

		/*--------------------------------------------------------------
		 * Events.
		 */

		protected virtual void OnItemAdded( ITestItem item )
		{
			if ( ItemAdded != null )
			{
				ItemAdded( this, new TestItemEventArgs( item ) );
			}
		}

		protected virtual void OnItemRemoved( ITestItem item )
		{
			if ( ItemRemoved != null )
			{
				ItemRemoved( this, new TestItemEventArgs( item ) );
			}
		}


		protected override void Dispose( bool disposing )
		{
			if ( this.subItems != null )
			{
				foreach ( ITestItem item in this.subItems )
				{
					if ( item != null )
					{
						item.Dispose();
					}
				}
			}

			base.Dispose( disposing );
		}
	}
}
