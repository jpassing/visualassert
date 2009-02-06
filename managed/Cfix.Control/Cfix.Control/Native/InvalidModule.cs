using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control.Native
{
	public sealed class InvalidModule : ITestItem
	{
		private readonly ITestItemCollection parent;
		private readonly String name;
		private Exception invalidityCause;

		private bool disposed;

		public event EventHandler Disposed;

		/*--------------------------------------------------------------
		 * Publics.
		 */

		internal InvalidModule(
			ITestItemCollection parent,
			String name,
			Exception invalidityCause
			)
		{
			this.parent = parent;
			this.name = name;
			this.invalidityCause = invalidityCause;
		}

		public Exception InvalidityCause
		{
			get { return this.invalidityCause; }
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

		public IAction CreateAction( SchedulingOptions flags )
		{
			throw new CfixException( "Invalid" );
		}

		public void Dispose()
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
	}
}
