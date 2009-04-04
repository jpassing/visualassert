using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Cfix.Addin.Dte
{
	internal class DteCommandBarButton : DteCommandBarControl
	{
		/*----------------------------------------------------------------------
		 * Public.
		 */

		public DteCommandBarButton(
			DteConnect connect,
			CommandBarControl control,
			DteCommand cmd )
			: base( connect, control )
		{
			this.commands.Add( cmd );
		}
	}
}
