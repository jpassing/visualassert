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

		public InvalidModule(
			ITestItemCollection parent,
			String name,
			Exception invalidityCause
			)
		{
			this.parent = parent;
			this.name = name;
			this.invalidityCause = invalidityCause;
		}

		public void Dispose()
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

		public void Add(
			IRunCompiler compiler,
			IResultItemCollection parentResult,
			IActionEvents events
			)
		{
			throw new NotImplementedException();
		}

		public IResultItem CreateResultItem(
			IResultItemCollection parentResult,
			IActionEvents events,
			ExecutionStatus interimStatus
			)
		{
			throw new NotImplementedException();
		}		
	}
}
