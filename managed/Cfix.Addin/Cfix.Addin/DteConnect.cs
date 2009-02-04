using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
		private Events events;

		private DteMainMenu mainMenu;
		private DteCommandBar toolbar;

		private DteCommand explorerCommand;
		private DteCommand resultsCommand;
		
		private IDictionary<String, CommandRegistration> commandRegistrations =
			new Dictionary< String, CommandRegistration >();

		private Configuration config;

		internal AddIn Addin
		{
			get { return this.addin; }
		}

		internal Events Events
		{
			get { return this.events; }
		}

		internal DTE2 DTE
		{
			get { return this.dte; }
		}

		internal static void HandleError( Exception x )
		{
			MessageBox.Show( x.Message + "\n\n" + x.StackTrace );
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
			this.dte = ( DTE2 ) applicationObject;
			this.addin = ( AddIn ) addinObject;

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

				this.explorerCommand.Delete();
				this.resultsCommand.Delete();
			}
			catch ( Exception x )
			{
				HandleError( x );
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
				this.config = Configuration.Load();

				this.events = this.dte.Events;

				this.explorerCommand = new DteCommand(
					this,
					"Explorer",
					"Test Explorer" );
				this.explorerCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					DteToolWindow wnd = DteToolWindow.Create( 
						this, "sample", 
						new Guid( "e89c09c9-4e89-4ae2-b328-79dcbdfd852c" ),
						typeof( SampleControl ),
						Icons.Explorer );

					wnd.Visible = true;
				} ;

				this.resultsCommand = new DteCommand(
					this,
					"Results",
					"Test Results" );

				//
				// Setup main menu entry.
				//
				this.mainMenu = DteMainMenu.Create( 
					this,
					"MainMenu",
					"cfi&x" );
				this.mainMenu.Add(
					this.explorerCommand,
					1,
					Icons.Explorer,
					Icons.ExplorerMask );
				this.mainMenu.Add(
					this.resultsCommand,
					2,
					Icons.Results,
					Icons.ResultsMask );

				//
				// Setup toolbar.
				//
				this.toolbar = DteCommandBar.Create(
					this,
					"cfix" );

				this.toolbar.AddButton(
					this.explorerCommand,
					1,
					Icons.Explorer,
					Icons.ExplorerMask,
					MsoButtonStyle.msoButtonIcon );
				this.toolbar.AddButton(
					this.resultsCommand,
					2,
					Icons.Results,
					Icons.ResultsMask,
					MsoButtonStyle.msoButtonIcon );

				this.mainMenu.Visible = true;
				this.toolbar.Visible = true;

				//
				// Restore tool windows.
				//
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