/*----------------------------------------------------------------------
 * Purpose:
 *		Command control handling.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Cfix.Addin.Dte
{
	internal abstract class DteCommandBarControl
	{
		protected readonly DteConnect connect;

		protected readonly CommandBarControl control = null;

		protected ICollection<DteCommand> commands = new LinkedList<DteCommand>();

		/*----------------------------------------------------------------------
		 * Abstract.
		 */


		/*----------------------------------------------------------------------
		 * Public.
		 */

		public DteCommandBarControl( 
			DteConnect connect, 
			CommandBarControl control )
		{
			this.connect = connect;
			this.control = control;
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
				try
				{
					this.control.Delete( true );
				}
				catch ( Exception )
				{
					//
					// VS Bug!? See 
					// http://www.mztools.com/articles/2006/MZ2006010.aspx.
					//
				}
			}
		}

	}
}
