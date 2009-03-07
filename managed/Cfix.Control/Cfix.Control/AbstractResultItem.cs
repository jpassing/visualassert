using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cfix.Control
{
	public abstract class AbstractResultItem : IResultItem
	{
		//
		// TestItem this result corresponds to - may or may not be
		// still available.
		//
		private ITestItem item;
		protected IActionEvents events;
		
		private volatile ExecutionStatus status;

		private readonly String itemName;
		
		protected readonly IResultItemCollection parent;
		
		protected AbstractResultItem(
			IActionEvents events,
			IResultItemCollection parent,
			ITestItem item,
			ExecutionStatus status
			)
		{
			this.events = events;
			this.parent = parent;
			this.status = status;
			this.itemName = item.Name;

			this.item = item;
			this.item.Disposed += new EventHandler( item_Disposed );
		}

		private void item_Disposed( object sender, EventArgs e )
		{
			//
			// Remove own reference.
			//
			this.item = null;
			this.events = null;
		}

		/*----------------------------------------------------------------------
		 * IResultItem.
		 */

		public IResultItemCollection Parent
		{
			get { return this.parent; }
		}

		public ITestItem Item
		{
			get { return this.item; }
		}

		public string Name
		{
			get { return this.itemName; }
		}

		public ExecutionStatus Status
		{
			get { return this.status; }
			set
			{
				if ( value != this.status )
				{
					Debug.Assert( this.events != null );
					Debug.Assert( this.item != null );

					this.status = value;
					this.events.OnStatusChanged( this );
				}
			}
		}

		public abstract ICollection<Failure> Failures { get; }
	}
}
