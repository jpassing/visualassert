using System;
using System.Collections.Generic;
using System.Drawing;
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
		protected readonly DteConnect connect;
		private readonly CommandBar control;

		private ICollection<DteCommandBarControl> controls = new LinkedList<DteCommandBarControl>();

		private DteCommandBar(
			DteConnect connect,
			CommandBar control )
		{
			this.connect = connect;
			this.control = control;
		}

		/*----------------------------------------------------------------------
		 * Protected.
		 */

		public static DteCommandBar Create( 
			DteConnect connect,
			String name 
			)
		{
			CommandBars cmdBars = ( CommandBars ) connect.DTE.CommandBars;

			CommandBar cmdBarCtl = ( CommandBar ) cmdBars.Add(
				name,
				MsoBarPosition.msoBarTop,
				Type.Missing,
				true );

			return new DteCommandBar( connect, cmdBarCtl );
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public bool Visible
		{
			get { return this.control.Visible; }
			set { this.control.Visible = value; }
		}

		public virtual void Delete()
		{
			foreach ( DteCommandBarControl ctl in this.controls )
			{
				ctl.Delete();
			}

			if ( this.control != null )
			{
				this.control.Delete();
			}
		}

		public CommandBar CommandBar
		{
			get { return this.control; }
		}

		public DteCommandBarControl AddButton( 
			DteCommand cmd, 
			int ordinal, 
			Image icon, 
			Image maskIcon,
			MsoButtonStyle style
			)
		{
			CommandBarButton buttonCtl =
				( CommandBarButton ) cmd.Command.AddControl( this.control, ordinal );

			//
			// N.B. See KB555417 for details on icon handling.
			//
			buttonCtl.Picture = ( stdole.StdPicture ) IconUtil.GetIPictureDispFromImage( icon );
			buttonCtl.Mask = ( stdole.StdPicture ) IconUtil.GetIPictureDispFromImage( maskIcon );
			buttonCtl.Style = style;
			DteCommandBarButton button = new DteCommandBarButton(
				this.connect, 
				buttonCtl, 
				cmd );
			this.controls.Add( button );

			return button;
		}
	}
}
