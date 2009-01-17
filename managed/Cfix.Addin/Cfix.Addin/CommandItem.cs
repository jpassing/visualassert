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
	internal class CommandItem
	{
		private readonly String name;
		private readonly String caption;

		private readonly DTE2 dte;
		private readonly Connect connect;

		private Command command;

		private void QueryStatus(
			vsCommandStatusTextWanted neededText,
			ref vsCommandStatus status,
			ref object commandText )
		{
			status = vsCommandStatus.vsCommandStatusSupported |
					 vsCommandStatus.vsCommandStatusEnabled;
		}

		private void Exec(
			vsCommandExecOption executeOption,
			ref object varIn,
			ref object varOut,
			ref bool handled )
		{
		}

		/*----------------------------------------------------------------------
		 * Publics.
		 */

		public CommandItem( DTE2 dte, Connect connect, String name, String caption )
		{
			this.dte = dte;
			this.connect = connect;
			this.name = name;
			this.caption = caption;

			Object[] nullArr = null;
			this.command = this.dte.Commands.AddNamedCommand(
				this.connect.Addin,
				this.name,
				this.caption,
				"",
				true,
				59,
				ref nullArr,
				( int ) ( vsCommandStatus.vsCommandStatusSupported |
					vsCommandStatus.vsCommandStatusEnabled ) );

			connect.RegisterCommand( name, QueryStatus, Exec );
		}

		public void AddTo( MainMenu menu )
		{
			this.command.AddControl( menu.CommandBar, 1 );
		}

		public void Delete()
		{
			this.command.Delete();
		}
	}
}
