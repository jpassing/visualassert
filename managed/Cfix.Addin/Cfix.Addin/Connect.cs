using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Windows.Forms;
using System.Reflection;
using System.Globalization;

namespace Cfix.Addin
{
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		private const String TopLevelMenuName = "cfix";

		private DTE2 dte;
		private AddIn addin;
		private ResourceManager resourceManager;
		private CultureInfo cultureInfo;

		private CommandBarPopup topLevelMenu;
		private Command sampleCommand;

		public Connect()
		{
		}

		private int GetToolsMenuIndex()
		{
			try
			{
				string resourceName = String.Concat( cultureInfo.TwoLetterISOLanguageName, "Tools" );
				String name = resourceManager.GetString( resourceName );

				CommandBar menuBarCommandBar = ( ( CommandBars ) dte.CommandBars )[ "MenuBar" ];
				return menuBarCommandBar.Controls[ name ].Index + 1;
			}
			catch
			{
				return -1;
			}
		}

		private int CalculateMenuIndex()
		{
			//
			// Use Tools menu as starting point.
			//
			int toolsIndex = GetToolsMenuIndex();
			if ( toolsIndex > 0 )
			{
				return toolsIndex;
			}
			else
			{
				CommandBar menuBarCommandBar = ( ( CommandBars ) dte.CommandBars )[ "MenuBar" ];
				int menuCount = menuBarCommandBar.Controls.Count;
				return menuCount;
			}
		}
		
		public void OnConnection( 
			object applicationObject, 
			ext_ConnectMode connectMode,
			object addinObject, 
			ref Array custom 
			)
		{
			dte = ( DTE2 ) applicationObject;
			addin = ( AddIn ) addinObject;

			resourceManager = new ResourceManager( "Cfix.Addin.CommandBar", Assembly.GetExecutingAssembly() );
			cultureInfo = new CultureInfo( dte.LocaleID );

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
			if ( this.topLevelMenu != null )
			{
				this.topLevelMenu.Delete( true );
			}

			if ( this.sampleCommand != null )
			{
				this.sampleCommand.Delete();
			}
		}

		public void OnAddInsUpdate( ref Array custom )
		{
		}

		public void OnStartupComplete( ref Array custom )
		{
			OnStartupComplete();
		}

		//private Command FindCommand( Commands collection, String name )
		//{
		//    foreach ( Command cmd in collection )
		//    {
		//        if ( cmd.Name == name )
		//        {
		//            return cmd;
		//        }
		//    }

		//    return null;
		//}

		private void OnStartupComplete()
		{
			try
			{
				//this.sampleCommand = FindCommand( 
				//    this.dte.Commands, 
				//    this.addin.ProgID + "." + TopLevelMenuName );

				Object[] nullArr = null;
				this.sampleCommand = this.dte.Commands.AddNamedCommand(
					this.addin,
					TopLevelMenuName,
					TopLevelMenuName,
					"",
					true,
					59,
					ref nullArr,
					( int ) ( vsCommandStatus.vsCommandStatusSupported |
						vsCommandStatus.vsCommandStatusEnabled ) );

				CommandBars dteCommandBars = ( CommandBars ) dte.CommandBars;
				CommandBar dteMainMenuBar = dteCommandBars[ "MenuBar" ];

				this.topLevelMenu = ( CommandBarPopup ) dteMainMenuBar.Controls.Add(
					MsoControlType.msoControlPopup,
					Type.Missing,
					Type.Missing,
					CalculateMenuIndex(),
					true );
				this.topLevelMenu.CommandBar.Name = TopLevelMenuName;
				this.topLevelMenu.Caption = TopLevelMenuName;

				this.sampleCommand.AddControl( topLevelMenu.CommandBar, 1 );
				this.topLevelMenu.Visible = true;
			}
			catch ( Exception x )
			{
				MessageBox.Show( x.Message );
			}
		}

		public void OnBeginShutdown( ref Array custom )
		{
		}

		public void QueryStatus( string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText )
		{
			if ( neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone )
			{
				if ( commandName == "Cfix.Addin.Connect.Cfix.Addin" )
				{
					status = ( vsCommandStatus ) vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
				else
				{
					status = ( vsCommandStatus ) vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
				}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec( string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled )
		{
			handled = false;
			if ( executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault )
			{
				if ( commandName == "Cfix.Addin.Connect.Cfix.Addin" )
				{
					handled = true;
					return;
				}
			}
		}
	}
}