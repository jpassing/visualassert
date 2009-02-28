using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control
{
	internal abstract class AbstractResultItem : IResultItem
	{
		//
		// TestItem this result corresponds to - may or may not be
		// still available.
		//
		private ITestItem item;
		private volatile ExecutionStatus status;

		private readonly String itemName;
		
		protected readonly IResultItemCollection parent;
		protected readonly IActionEvents events;

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
					this.status = value;
					this.events.OnStatusChanged( this );
				}
			}
		}

		public abstract ICollection<Failure> Failures { get; }
	}
}
