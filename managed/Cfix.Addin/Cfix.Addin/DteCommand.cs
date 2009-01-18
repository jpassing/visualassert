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
	internal class DteCommand
	{
		public static readonly String CommandPrefix = typeof( DteConnect ).FullName + ".";
		
		private readonly String name;
		private readonly String caption;

		private readonly DteConnect connect;

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

		private static void FindAndDeleteCommand(
			DteConnect connect, 
			String name
			)
		{
			name = CommandPrefix + name;
			foreach ( Command cmd in connect.DTE.Commands )
			{
				if ( cmd.Name == name )
				{
					cmd.Delete();
					return;
				}
			}
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

		public DteCommand( DteConnect connect, String name, String caption )
		{
			this.connect = connect;
			this.name = name;
			this.caption = caption;

			try
			{
				this.command = CreateCommand( connect, name, caption );
			}
			catch ( ArgumentException )
			{
				//
				// Already exists - delete and try again.
				//
				FindAndDeleteCommand( connect, name );
				this.command = CreateCommand( connect, name, caption );
			}

			connect.RegisterCommand( name, QueryStatus, Exec );
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

		public void AddTo( DteCommandBarControl bar )
		{
			bar.Add( this );
		}
	}
}
