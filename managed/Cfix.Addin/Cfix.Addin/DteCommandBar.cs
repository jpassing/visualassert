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
		protected readonly DteConnect connect;
		private CommandBar control = null;

		private ICollection<DteCommandBarControl> controls = new LinkedList<DteCommandBarControl>();

		/*----------------------------------------------------------------------
		 * Protected.
		 */

		protected virtual CommandBar CreateControl()
		{
			CommandBars cmdBars = ( CommandBars ) this.connect.DTE.CommandBars;

			return ( CommandBar ) cmdBars.Add(
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
			String name )
		{
			this.connect = connect;
			this.name = name;
		}

		public void Load()
		{
			this.control = CreateControl();
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
				this.control.Delete();
			}

			foreach ( DteCommandBarControl ctl in this.controls )
			{
				ctl.Delete();
			}
		}

		public CommandBar CommandBar
		{
			get { return this.control; }
		}

		public CommandBarButton Add( DteCommand cmd )
		{
			// TODO: position.
			return ( CommandBarButton ) cmd.Command.AddControl( this.control, 1 );
		}

		public DteCommandBarControl AddButton( String name, String caption )
		{
			DteCommandBarButton button = new DteCommandBarButton(
				this.connect, this, name, caption );
			button.Load();
			this.controls.Add( button );

			return button;
		}
		
		//public DteCommandBarButton AddButton(
		//    String name,
		//    String caption
		//    )
		//{
		//    DteCommand cmd = new DteCommand(
		//        this.connect, name, caption );

		//    // TODO: position.
		//    CommandBarButton button = ( CommandBarButton )
		//        cmd.Command.AddControl( this.control, 1 );

		//    return new DteCommandBarButton( button );
		//}
	}
}
