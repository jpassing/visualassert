using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Cfixctl;

namespace Cfix.Control
{
	public class TestItemContainer : TestItem, ITestItemContainer
	{
		private ITestItem[] subItems;

		/*--------------------------------------------------------------
		 * Publics.
		 */

		internal TestItemContainer(
			TestItemContainer parent,
			uint ordinal,
			ICfixTestItem item
			)
			: base( parent, ordinal, item )
		{
		}

		internal void Update( ICfixTestContainer container )
		{
			this.subItems = new ITestItem[ container.GetItemCount() ];

			for ( uint i = 0; i < subItems.Length; i++ )
			{
				ICfixTestItem newItem = container.GetItem( i );

				try
				{
					this.subItems[ i ] = TestItem.Wrap(
						this,
						i,
						newItem );

					TestItemContainer subContainer =
						this.subItems[ i ] as TestItemContainer;
					if ( subContainer != null )
					{
						subContainer.Update( ( ICfixTestContainer ) newItem );
					}
				}
				finally
				{
					Marshal.ReleaseComObject( newItem );
				}
			}
		}

		/*--------------------------------------------------------------
		 * ITestItemContainer.
		 */

		public ITestItem GetItem( uint ordinal )
		{
			return this.subItems[ ordinal ];
		}

		public uint ItemCount
		{
			get
			{
				return ( uint ) this.subItems.Length;
			}
		}
	}
}
