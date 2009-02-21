using System;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	public class TestItem : ITestItem, IComponentActionSource
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

		private NativeAction CreateNativeAction( SchedulingOptions schedOptions )
		{
			NativeConnection connection = this.NativeConnection;
			ICfixTestItem ctlItem = connection.Item;
			try
			{
				uint nativeFlags = 0;
				if ( ( schedOptions & SchedulingOptions.ComNeutralThreading ) != 0 )
				{
					nativeFlags |= NativeAction.CFIXCTL_ACTION_COM_NEUTRAL;
				}

				return new NativeAction(
					this,
					connection.Host,
					ctlItem.CreateExecutionAction( ( uint ) schedOptions, 0 ),
					nativeFlags );
			}
			catch ( COMException x )
			{
				throw this.Module.Target.WrapException( x );
			}
			finally
			{
				this.Module.Target.ReleaseObject( ctlItem );
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

		internal virtual NativeConnection NativeConnection
		{
			get
			{
				NativeConnection parentConnection =
					parent.NativeConnection;

				ICfixTestContainer parentContainer =
					( ICfixTestContainer ) parentConnection.Item;

				try
				{
					if ( ordinal >= parentContainer.GetItemCount() )
					{
						//
						// Module must have changed in the meantime.
						//
						throw new TestItemDisappearedException();
					}

					return new NativeConnection(
						parentConnection.Host,
						parentContainer.GetItem( this.ordinal ) );
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

		public IComponentAction CreateAction( SchedulingOptions schedulingOptions )
		{
			return CreateNativeAction( schedulingOptions );
		}

		public virtual void CreateAction( 
			ICompositeAction actionToComposeWith,
			SchedulingOptions schedulingOptions,
			CompositionOptions compositionOptions
			)
		{
			actionToComposeWith.Add( CreateNativeAction( schedulingOptions ) );
		}

		protected virtual void Dispose( bool disposing )
		{
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

	internal class NativeConnection
	{
		public readonly ICfixHost Host;
		public readonly ICfixTestItem Item;

		public NativeConnection(
			ICfixHost host,
			ICfixTestItem item
			)
		{
			this.Host = host;
			this.Item = item;
		}
	}

}
