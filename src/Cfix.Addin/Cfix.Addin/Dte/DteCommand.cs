/*----------------------------------------------------------------------
 * Purpose:
 *		Command handling.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

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
	internal class DteCommand
	{
		private readonly String name;
		private readonly String caption;

		private readonly DteConnect connect;

		private Command command;

		private bool enabled = true;

		public event ExecDelegate Execute;

		private void QueryStatus(
			vsCommandStatusTextWanted neededText,
			ref vsCommandStatus status,
			ref object commandText )
		{
			if ( enabled )
			{
				status = vsCommandStatus.vsCommandStatusSupported |
						 vsCommandStatus.vsCommandStatusEnabled;
			}
		}

		private void Exec(
			vsCommandExecOption executeOption,
			ref object varIn,
			ref object varOut,
			ref bool handled )
		{
			if ( Execute != null )
			{
				Execute( executeOption, ref varIn, ref varOut, ref handled );
			}
		}

		private static Command FindCommand(
			DteConnect connect, 
			String name
			)
		{
			name = connect.CommandPrefix + name;
			foreach ( Command cmd in connect.DTE.Commands )
			{
				if ( cmd.Name == name )
				{
					return cmd;
				}
			}

			return null;
		}

		private static Command CreateCommand( 
			DteConnect connect, 
			String name, 
			String caption )
		{
			Object[] nullArr = null;
			return connect.DTE.Commands.AddNamedCommand(
				connect.Addin,
				name,
				caption,
				"",
				true,
				59,
				ref nullArr,
				( int ) ( vsCommandStatus.vsCommandStatusSupported |
					vsCommandStatus.vsCommandStatusEnabled ) ); ;
		}

		/*----------------------------------------------------------------------
		 * Publics.
		 */

		public DteCommand( 
			DteConnect connect, 
			String name, 
			String caption,
			String shortcut,
			bool forceRecreation,
			out bool created
			)
		{
			this.connect = connect;
			this.name = name;
			this.caption = caption;

			this.command = FindCommand( connect, name );
			if ( this.command != null && forceRecreation )
			{
				this.command.Delete();
				this.command = null;
			}

			if ( this.command == null )
			{
				this.command = CreateCommand( connect, name, caption );
				created = true;

				if ( shortcut != null )
				{
					SetShortcut( shortcut );
				}
			}
			else
			{
				created = false;
			}

			connect.RegisterCommand( name, QueryStatus, Exec );
		}

		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		public void Delete()
		{
			this.connect.UnregisterCommand( this.name );
			this.command.Delete();
		}

		public Command Command
		{
			get
			{
				return this.command;
			}
		}

		public void SetShortcut( string keys )
		{
			object[] bindings = new object[ 1 ];
			bindings[ 0 ] = keys;

			this.command.Bindings = bindings;
		}
	}
}
