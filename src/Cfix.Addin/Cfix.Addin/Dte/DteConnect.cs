/*----------------------------------------------------------------------
 * Purpose:
 *		Addin base class.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

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
using Cfix.Control;

namespace Cfix.Addin.Dte
{
	public abstract class DteConnect : IDTExtensibility2, IDTCommandTarget
	{
		private DTE2 dte;
		private AddIn addin;

		private IDictionary<String, CommandRegistration> commandRegistrations =
			new Dictionary< String, CommandRegistration >();

		internal AddIn Addin
		{
			get { return this.addin; }
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
		 * Abstract.
		 */

		protected abstract void Setup();
		protected abstract void SetupOnce();
		protected abstract void Teardown( bool hostShutdown );
		public abstract String CommandPrefix { get; }

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
					Logger.LogInfo( "DteConnect", "cm_Startup" );
					break;

				case ext_ConnectMode.ext_cm_UISetup:
					//
					// Called once after installation.
					//
					Logger.LogInfo( "DteConnect", "cm_UISetup" );

					SetupOnce();
					break;

				case ext_ConnectMode.ext_cm_AfterStartup:
					Logger.LogInfo( "DteConnect", "cm_AfterStartup" );

					Setup();
					break;

				default:
					break;
			}
		}

		public void OnDisconnection( ext_DisconnectMode disconnectMode, ref Array custom )
		{
			switch ( disconnectMode )
			{
				case ext_DisconnectMode.ext_dm_HostShutdown:
					Logger.LogInfo( "DteConnect", "dm_HostShutdown" );

					Teardown( true );
					return;

				case ext_DisconnectMode.ext_dm_UserClosed:
					Logger.LogInfo( "DteConnect", "dm_UserClosed" );

					Teardown( false );
					return;

				case ext_DisconnectMode.ext_dm_UISetupComplete:
					Logger.LogInfo( "DteConnect", "dm_UISetupComplete" );
					return;

				case ext_DisconnectMode.ext_dm_SolutionClosed:
					Debug.Fail( "Unexpected disconnect mode" );
					return;
			}
		}

		public void OnAddInsUpdate( ref Array custom )
		{
		}

		public void OnStartupComplete( ref Array custom )
		{
			Setup();
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
			//Debug.Print( "QueryStatus: " + commandName.Substring( CommandPrefix.Length ) );

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
			//Debug.Print( "Exec: " + commandName.Substring( CommandPrefix.Length ) );

			handled = false;

			CommandRegistration reg;
			if ( this.commandRegistrations.TryGetValue(
				commandName.Substring( CommandPrefix.Length ), out reg ) )
			{
				reg.Exec( executeOption, ref varIn, ref varOut, ref handled );
			}
		}
	}
}