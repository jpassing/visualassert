using System;
using Cfixctl;

namespace Cfix.Control
{
	public class TestItem : ITestItem
	{
		protected readonly TestItemContainer parent;
		protected readonly uint ordinal;
		protected readonly String name;

		internal static ITestItem Wrap(
			TestItemContainer parent,
			uint ordinal,
			ICfixTestItem ctlItem
			)
		{
			if ( ctlItem is ICfixTestContainer )
			{
				return new TestItemContainer(
					parent,
					ordinal,
					ctlItem );
			}
			else
			{
				return new TestItem(
					parent,
					ordinal,
					ctlItem );
			}
		}

		/*--------------------------------------------------------------
		 * Publics.
		 */

		internal TestItem(
			TestItemContainer parent,
			uint ordinal,
			ICfixTestItem item )
		{
			this.parent = parent;
			this.ordinal = ordinal;
			this.name = item.GetName();
		}

		/*--------------------------------------------------------------
		 * ITestItem.
		 */

		public virtual ICfixTestItem NativeItem
		{
			get
			{
				ICfixTestContainer parentContainer =
					( ICfixTestContainer ) parent.NativeItem;

				if ( ordinal >= parentContainer.GetItemCount() )
				{
					//
					// Module must have changed in the meantime.
					//
					throw new TestItemDisappearedException();
				}

				return parentContainer.GetItem( this.ordinal );
			}
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
				return this.ordinal;
			}
		}
	}
}
