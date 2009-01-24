using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
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

	public class DteConnect : IDTExtensibility2, IDTCommandTarget
	{
		private DTE2 dte;
		private AddIn addin;

		private DteMainMenu mainMenu;
		private DteCommandBar toolbar;

		private IDictionary< String, CommandRegistration > commandRegistrations =
			new Dictionary< String, CommandRegistration >();

		internal AddIn Addin
		{
			get { return addin; }
		}

		internal DTE2 DTE
		{
			get { return this.dte; }
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
			try
			{
				if ( this.mainMenu != null )
				{
					this.mainMenu.Delete();
				}

				if ( this.toolbar != null )
				{
					this.toolbar.Delete();
				}
			}
			catch ( Exception x )
			{
				MessageBox.Show( x.Message + "\n\n" + x.StackTrace );
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
				this.mainMenu = DteMainMenu.Create( 
					this,
					"MainMenu",
					"cfix" );
				this.mainMenu.Add( new DteCommand(
					this,
					"Sample1",
					"Sample #1" ) );
				this.mainMenu.Add( new DteCommand(
					this,
					"Sample2",
					"Sample #2" ) );
				this.mainMenu.Visible = true;

				this.toolbar = DteCommandBar.Create(
					this,
					"SampleToolbar" );

				DteCommand cmd = new DteCommand(
					this,
					"Button1",
					"Button #1" );
				cmd.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					DteToolWindow wnd = DteToolWindow.Create( 
						this, "sample", 
						Guid.NewGuid(),
						typeof( SampleControl ) );

					wnd.Visible = true;
					
				};

				this.toolbar.AddButton( cmd );
				this.toolbar.AddButton( new DteCommand(
					this,
					"Button2", 
					"Button #2" ) );
				this.toolbar.Visible = true;
			}
			catch ( Exception x )
			{
				MessageBox.Show( x.Message + "\n\n" + x.StackTrace );
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
			Debug.Assert( commandName.StartsWith( DteCommand.CommandPrefix ) );
			Debug.Print( "QueryStatus: " + commandName.Substring( DteCommand.CommandPrefix.Length ) );

			CommandRegistration reg;
			if ( this.commandRegistrations.TryGetValue(
				commandName.Substring( DteCommand.CommandPrefix.Length ), out reg ) )
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
			Debug.Assert( commandName.StartsWith( DteCommand.CommandPrefix ) );
			Debug.Print( "Exec: " + commandName.Substring( DteCommand.CommandPrefix.Length ) );

			handled = false;

			CommandRegistration reg;
			if ( this.commandRegistrations.TryGetValue(
				commandName.Substring( DteCommand.CommandPrefix.Length ), out reg ) )
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