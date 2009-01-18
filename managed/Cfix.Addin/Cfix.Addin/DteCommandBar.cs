using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Cfix.Addin
{
	internal class DteCommandBar
	{
		protected readonly String name;
		protected readonly String caption;

		protected readonly DteConnect connect;

		protected CommandBarControl control = null;

		protected IList<DteCommandItem> items = new List<DteCommandItem>();

		/*----------------------------------------------------------------------
		 * Protected.
		 */

		protected virtual CommandBarControl CreateControl()
		{
			CommandBars cmdBars = ( CommandBars ) this.connect.DTE.CommandBars;

			return ( CommandBarControl ) cmdBars.Add(
				this.name,
				MsoBarPosition.msoBarTop,
				Type.Missing,
				true );
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public DteCommandBar( 
			DteConnect connect, 
			String name,
			String caption )
		{
			this.connect = connect;
			this.name = name;
			this.caption = caption;
		}

		public void Load()
		{
			this.control = CreateControl();
			this.control.Caption = this.caption;
		}

		public bool Visible
		{
			get { return this.control.Visible; }
			set { this.control.Visible = value; }
		}

		public virtual void Add( DteCommandItem item )
		{
			if ( this.control == null )
			{
				throw new InvalidOperationException( "Not loaded" );
			}

			// TODO: ordinal.
			item.Command.AddControl( this.control, 1 );
			this.items.Add( item );
		}

		public virtual void Delete()
		{
			if ( this.control != null )
			{
				this.control.Delete( true );
			}

			foreach( DteCommandItem item in this.items )
			{
				item.Delete();
			}
		}
	}
}
