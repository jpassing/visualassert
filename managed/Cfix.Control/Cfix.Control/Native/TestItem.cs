using System;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	public class TestItem : ITestItem
	{
		private readonly TestItemCollection parent;
		private readonly uint ordinal;
		private readonly String name;

		private bool disposed;

		public event EventHandler Disposed;

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

		internal virtual ICfixTestItem GetNativeItem( IHost host )
		{
			ICfixTestContainer parentContainer =
				( ICfixTestContainer ) parent.GetNativeItem( host );

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
			catch ( COMException x )
			{
				throw this.Module.Target.WrapException( x );
			}
			finally
			{
				this.Module.Target.ReleaseObject( parentContainer );
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

		~TestItem()
		{
			Dispose( false );
		}


		protected virtual void Dispose( bool disposing )
		{
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
				return this.ordinal;
			}
		}

		public virtual ITestItemCollection Parent
		{
			get
			{
				return this.parent;
			}
		}

		public IAction CreateAction(
			IHost host,
			SchedulingOptions schedOptions,
			ThreadingOptions threadingOptions
			)
		{
			return new NativeAction(
				host,
				this,
				schedOptions,
				threadingOptions );
		}
	}

}
