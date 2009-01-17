using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Cfix.Addin
{
	public delegate void QueryStatusDelegate( 
		vsCommandStatusTextWanted neededText, 
		ref vsCommandStatus status, 
		ref object commandText );

	public delegate void ExecDelegate( 
		vsCommandExecOption executeOption, 
		ref object varIn, 
		ref object varOut, 
		ref bool handled );

	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		private const String CommandPrefix = "Cfix.Addin.Connect.";

		private DTE2 dte;
		private AddIn addin;

		private MainMenu mainMenu;

		private IDictionary< String, CommandRegistration > commandRegistrations =
			new Dictionary< String, CommandRegistration >();

		internal AddIn Addin
		{
			get
			{
				return addin;
			}
		}

		/*----------------------------------------------------------------------
		 * Command registrations.
		 */

		private class CommandRegistration
		{
			public readonly QueryStatusDelegate QueryStatus;
			public readonly ExecDelegate Exec;

			public CommandRegistration(
				QueryStatusDelegate queryStatus,
				ExecDelegate exec
				)
			{
				this.QueryStatus = queryStatus;
				this.Exec = exec;
			}
		}

		internal void RegisterCommand(
			String commandName,
			QueryStatusDelegate queryStatus,
			ExecDelegate execDelegate
			)
		{
			Debug.Print( "RegisterCommand: " + commandName );
			this.commandRegistrations.Add(
				commandName,
				new CommandRegistration( queryStatus, execDelegate ) );
		}

		internal void UnregisterCommand(
			String commandName
			)
		{
			this.commandRegistrations.Remove( commandName );
		}

		/*----------------------------------------------------------------------
		 * AddIn methods.
		 */

		public void OnConnection( 
			object applicationObject, 
			ext_ConnectMode connectMode,
			object addinObject, 
			ref Array custom 
			)
		{
			dte = ( DTE2 ) applicationObject;
			addin = ( AddIn ) addinObject;

			switch ( connectMode )
			{
				case ext_ConnectMode.ext_cm_Startup: 
					//
					// VS will call OnStartupComplete.
					//
					break;

				case ext_ConnectMode.ext_cm_AfterStartup:
					OnStartupComplete();
					break;

				default:
					break;
			}
		}

		public void OnDisconnection( ext_DisconnectMode disconnectMode, ref Array custom )
		{
			if ( this.mainMenu != null )
			{
				this.mainMenu.Delete();
			}
		}

		public void OnAddInsUpdate( ref Array custom )
		{
		}

		public void OnStartupComplete( ref Array custom )
		{
			OnStartupComplete();
		}

		private void OnStartupComplete()
		{
			try
			{
				this.mainMenu = new MainMenu( 
					this.dte, 
					this,
					"MainMenu",
					"cfix" );
				this.mainMenu.Show();
			}
			catch ( Exception x )
			{
				MessageBox.Show( x.Message );
			}
		}

		public void OnBeginShutdown( ref Array custom )
		{
		}

		public void QueryStatus( 
			string commandName, 
			vsCommandStatusTextWanted neededText, 
			ref vsCommandStatus status, 
			ref object commandText )
		{
			Debug.Assert( commandName.StartsWith( CommandPrefix ) );
			Debug.Print( "QueryStatus: " + commandName.Substring( CommandPrefix.Length ) );

			CommandRegistration reg;
			if ( this.commandRegistrations.TryGetValue(
				commandName.Substring( CommandPrefix.Length ), out reg ) )
			{
				reg.QueryStatus( neededText, ref status, ref commandText );
			}
		}

		public void Exec( 
			string commandName, 
			vsCommandExecOption executeOption, 
			ref object varIn, 
			ref object varOut, 
			ref bool handled )
		{
			Debug.Assert( commandName.StartsWith( CommandPrefix ) );
			Debug.Print( "Exec: " + commandName.Substring( CommandPrefix.Length ) );

			handled = false;

			CommandRegistration reg;
			if ( this.commandRegistrations.TryGetValue(
				commandName.Substring( CommandPrefix.Length ), out reg ) )
			{
				reg.Exec( executeOption, ref varIn, ref varOut, ref handled );
			}
			
			//if ( executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault )
			//{
			//    if ( commandName == "Cfix.Addin.Connect.Cfix.Addin" )
			//    {
			//        handled = true;
			//        return;
			//    }
			//}
		}
	}
}