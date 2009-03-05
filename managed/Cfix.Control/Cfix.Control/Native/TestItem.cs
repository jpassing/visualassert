using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	/*++
	 * Class Description:
	 *		Wrapper for a ICfixTestItem and base class for other
	 *		test item wrapper classes.
	 * 
	 *		Threadsafe.
	 --*/
	public class TestItem : ITestItem, IResultItemFactory
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
				throw this.Module.Agent.WrapException( x );
			}
			finally
			{
				this.Module.Agent.ReleaseObject( parentContainer );
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
			Debug.Assert( item != null );

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
			get { return this.name; }
		}

		public uint Ordinal
		{
			get { return this.ordinal; }
		}

		public virtual ITestItemCollection Parent
		{
			get { return this.parent; }
		}

		public void Add( 
			IRunCompiler compiler,
			IResultItemCollection parentResult,
			IActionEvents events
			)
		{
			compiler.Add(
				new NativeAction(
					this,
					parentResult,
					events,
					compiler.SchedulingOptions,
					compiler.ThreadingOptions ) );
		}

		/*--------------------------------------------------------------
		 * IResultItemFactory.
		 */

		public virtual IResultItem CreateResultItem(
			IResultItemCollection parentResult ,
			IActionEvents events,
			ExecutionStatus interimStatus
			)
		{
			return new TestItemResult(
				events,
				parentResult,
				this,
				interimStatus );
		}
	}
}
