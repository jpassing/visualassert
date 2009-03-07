using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Cfix.Control.Native
{
	/*++
	 * Class Description:
	 *		Represents a module that, for whichever reason, is invalid
	 *		and cannot bed represented as a TestModule.
	 * 
	 *		Threadsafe.
	 --*/
	public sealed class InvalidModule : ITestItem
	{
		private readonly ITestItemCollection parent;
		private readonly String name;
		private readonly Exception invalidityCause;

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
			Debug.Assert( name != null );
			Debug.Assert( invalidityCause != null );

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
			IActionEvents events,
			IResultItem result
			)
		{
			//
			// Nothing to add.
			//
		}
	}
}
