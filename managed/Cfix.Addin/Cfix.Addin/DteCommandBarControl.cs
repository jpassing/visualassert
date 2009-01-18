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
	internal abstract class DteCommandBarControl
	{
		protected readonly String name;
		protected readonly String caption;

		protected readonly DteConnect connect;

		protected CommandBarControl control = null;

		protected ICollection<DteCommand> commands = new LinkedList<DteCommand>();

		/*----------------------------------------------------------------------
		 * Abstract.
		 */

		protected abstract CommandBarControl CreateControl();
		public abstract void Add( DteCommand item );


		/*----------------------------------------------------------------------
		 * Public.
		 */

		public DteCommandBarControl( 
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

		public virtual void Delete()
		{
			if ( this.control != null )
			{
				this.control.Delete( true );
			}

			foreach( DteCommand item in this.commands )
			{
				item.Delete();
			}
		}

	}
}
