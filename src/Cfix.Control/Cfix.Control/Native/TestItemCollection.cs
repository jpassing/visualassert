using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Cfix.Control.Diag;
using Cfixctl;

namespace Cfix.Control.Native
{
	/*++
	 * Class Description:
	 *		Wrapper for a composite ICfixTestItem (e.g. fixture).
	 * 
	 *		Threadsafe.
	 --*/
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly" )]
	public class TestItemCollection : TestItem, ITestItemCollection
	{
		//
		// Children, indexed by ordinal.
		//
		private ITestItem[] subItems;

		//
		// Required for updating.
		//
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

		protected override void Dispose( bool disposing )
		{
			try
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
			}
			finally
			{
				base.Dispose( disposing );
			}
		}

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
							Module.Agent.ReleaseObject( newItem );
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
					Logger.LogError( "Agent", "Failed to update item", x );
					throw this.Module.Agent.WrapException( x );
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
		 * ITestItem overrides.
		 */

		public override String FullName
		{
			get { return "__CfixFixturePe" + this.Name; }
		}

		/*--------------------------------------------------------------
		 * ITestItemContainer.
		 */

		public event EventHandler<TestItemEventArgs> ItemAdded;
		public event EventHandler<TestItemEventArgs> ItemRemoved;

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

		public uint ItemCountRecursive
		{
			get
			{
				uint count = 0;
				for ( int i = 0; i < this.subItems.Length; i++ )
				{
					if ( this.subItems[ i ] != null )
					{
						ITestItemCollection subCont = this.subItems[ i ]
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
				}

				return count;
			}
		}

		public virtual void Refresh()
		{
			//
			// N.B. This module may be importing symbols from a DLL
			// that resides in the same directory. 
			//
			// Augment search path.
			//
			FileInfo pathInfo = new FileInfo( this.Module.Path );
			HostEnvironment env = new HostEnvironment();
			env.AddSearchPath( pathInfo.Directory.FullName );

			using ( IHost host = this.Module.Agent.CreateHost( env ) )
			{
				ICfixTestItem ctlItem = null;
				try
				{
					ctlItem = GetNativeItem( host );
					Update( ( ICfixTestContainer ) ctlItem );
				}
				catch ( COMException x )
				{
					Logger.LogError( "Agent", "Failed to refresh item", x );
					throw this.Module.Agent.WrapException( x );
				}
				finally
				{
					if ( ctlItem != null )
					{
						this.Module.Agent.ReleaseObject( ctlItem );
					}
				}
			}
		}

		public override IResultItem CreateResultItem(
			IResultItemCollection parentResult,
			IActionEvents events,
			ExecutionStatus interimStatus
			)
		{
			return new TestItemCollectionResult(
				events,
				parentResult,
				this,
				interimStatus );
		}

		internal IResultItem CreateResultItemForSingleTestCaseRun(
			IActionEvents events,
			ITestItem testCase,
			ExecutionStatus interimStatus
			)
		{
			Debug.Assert( !( testCase is ITestItemCollection ) );
			Debug.Assert( ReferenceEquals( testCase.Parent, this ) );

			ITestItem[] children = new ITestItem[] { testCase };
			TestItemCollectionResult result = new TestItemCollectionResult(
				events,
				null,
				this,
				children,
				interimStatus );

			Debug.Assert( result.ItemCount == 1 );
			return result;
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
	}
}
