using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Cfix.Addin
{
	internal class DteCommandBarButton : DteCommandBarControl
	{
		private DteCommandBar bar;

		protected override CommandBarControl CreateControl()
		{
			DteCommand cmd = new DteCommand(
				this.connect, name, caption );
			
			CommandBarButton button = this.bar.Add( cmd );

			this.commands.Add( cmd );
			button.Visible = true;
			return button;
		}

		public override void Add( DteCommand item )
		{
			throw new NotImplementedException();
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public DteCommandBarButton(
			DteConnect connect, 
			DteCommandBar bar,
			String name,
			String caption )
			: base( connect, name, caption )
		{
			this.bar = bar;
		}
	}
}
