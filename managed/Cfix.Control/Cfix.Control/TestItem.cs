using System;
using Cfixctl;

namespace Cfix.Control
{
	internal class NativeAction : IAction
	{
		private TestItem item;
		private ICfixAction action;

		public NativeAction( TestItem item, ICfixAction action )
		{
			this.item = item;
			this.action = action;
		}

		public ITestItem TestItem 
		{
			get
			{
				return this.item;
			}
		}

		public void Run( ICfixEventSink sink )
		{
			this.action.Run( sink );
		}

		public void Stop()
		{
			this.action.Stop();
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( this.action != null )
			{
				this.item.Module.Target.ReleaseObject( this.action );
				this.action = null;
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}
	}

	public class TestItem : ITestItem
	{
		private readonly TestItemCollection parent;
		private readonly uint ordinal;
		private readonly String name;

		internal static ITestItem Wrap(
			TestItemCollection parent,
			uint ordinal,
			ICfixTestItem ctlItem
			)
		{
			if ( ctlItem is ICfixTestContainer )
			{
				return new TestItemCollection(
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
			TestItemCollection parent,
			uint ordinal,
			ICfixTestItem item )
		{
			this.parent = parent;
			this.ordinal = ordinal;
			this.name = item.GetName();
		}

		internal virtual TestModule Module
		{
			get
			{
				return this.parent.Module;
			}
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

				try
				{
					if ( ordinal >= parentContainer.GetItemCount() )
					{
						//
						// Module must have changed in the meantime.
						//
						throw new TestItemDisappearedException();
					}

					return parentContainer.GetItem( this.ordinal );
				}
				finally
				{
					this.Module.Target.ReleaseObject( parentContainer );
				}
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

		public IAction CreateAction( SchedulingOptions flags )
		{
			ICfixTestItem ctlItem = this.NativeItem;
			try
			{
				return new NativeAction( 
					this, 
					ctlItem.CreateExecutionAction( ( uint ) flags, 0 ) );
			}
			finally
			{
				this.Module.Target.ReleaseObject( ctlItem );
			}
		}
	}
}