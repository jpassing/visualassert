/*----------------------------------------------------------------------
 * Purpose:
 *		Main addin class.
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
using Cfix.Addin.Dte;
using Cfix.Addin.Windows.Explorer;

namespace Cfix.Addin
{
	public class CfixPlus : DteConnect
	{
		private static readonly String CommandPrefixConstant = typeof( CfixPlus ).FullName + ".";

		private Workspace workspace;

		/*----------------------------------------------------------------------
		 * Internal.
		 */

		internal Workspace Workspace
		{
			get { return this.workspace; }
		}

		internal static void HandleError( Exception x )
		{
			MessageBox.Show( x.Message );
		}

		internal static void ShowInfo( String msg )
		{
			MessageBox.Show( 
				msg, 
				Strings.MsgBoxCaption, 
				MessageBoxButtons.OK, 
				MessageBoxIcon.Information );
		}

		internal static bool ShowQuestion( String msg )
		{
			return MessageBox.Show(
				msg,
				Strings.MsgBoxCaption,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question ) == DialogResult.Yes;
		}

		/*----------------------------------------------------------------------
		 * DteConnect overrides.
		 */

		public override String CommandPrefix
		{
			get { return CommandPrefixConstant; }
		}

		protected override void SetupOnce()
		{
		}

		protected override void Setup()
		{
			try
			{
				this.workspace = new Workspace( this );

				bool explorerCommandCreated;
				DteCommand explorerCommand = new DteCommand(
					this,
					"Explorer",
					Strings.ExplorerCmdCaption,
					"Global::Ctrl+1,Ctrl+e",
					false,
					out explorerCommandCreated );

				explorerCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					this.workspace.ToolWindows.Explorer.Activate();
				} ;

				bool resultsCommandCreated;
				DteCommand resultsCommand = new DteCommand(
					this,
					"Results",
					Strings.RunCmdCaption,
					"Global::Ctrl+1,Ctrl+r",
					false,
					out resultsCommandCreated );
				
				resultsCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					this.workspace.ToolWindows.Run.Activate();
				};

				bool redebugRunCommandCreated;
				DteCommand redebugRunCommand = new DteCommand(
					this,
					"ReDebugRun",
					Strings.ReDebugRunCmdCaption,
					"Global::Ctrl+1,Ctrl+d",
					false,
					out redebugRunCommandCreated );

				redebugRunCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					Cfix.Addin.Windows.CommonUiOperations.RunItem(
						this.workspace, null, true );
				};

				redebugRunCommand.QueryStatus += delegate(
					vsCommandStatusTextWanted neededText,
					ref vsCommandStatus status,
					ref object commandText )
				{
					status = vsCommandStatus.vsCommandStatusSupported |
						( this.workspace.RerunLastItemPossible 
							? vsCommandStatus.vsCommandStatusEnabled
							: 0 );
				};

				bool restartRunCommandCreated;
				DteCommand restartRunCommand = new DteCommand(
					this,
					"ReStartRun",
					Strings.ReStartRunCmdCaption,
					"Global::Ctrl+1,Ctrl+s",
					false,
					out restartRunCommandCreated );

				restartRunCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					Cfix.Addin.Windows.CommonUiOperations.RunItem(
						this.workspace, null, false );
				};

				restartRunCommand.QueryStatus += delegate(
					vsCommandStatusTextWanted neededText,
					ref vsCommandStatus status,
					ref object commandText )
				{
					status = vsCommandStatus.vsCommandStatusSupported |
						( this.workspace.RerunLastItemPossible
							? vsCommandStatus.vsCommandStatusEnabled
							: 0 );
				};

				bool aboutCommandCreated;
				DteCommand aboutCommand = new DteCommand(
					this,
					"About",
					Strings.AboutCmdCaption,
					null,
					false,
					out aboutCommandCreated );

				aboutCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					new Cfix.Addin.Windows.About.AboutWindow( this.workspace ).ShowDialog();
				};

				bool enterLicenseCommandCreated;
				DteCommand enterLicenseCommand = new DteCommand(
					this,
					"EnterLicense",
					Strings.EnterLicenseCmdCaption,
					null,
					false,
					out enterLicenseCommandCreated );

				enterLicenseCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					this.workspace.ToolWindows.LaunchLicenseAdmin( "license" );
				};

				enterLicenseCommand.QueryStatus += delegate(
					vsCommandStatusTextWanted neededText,
					ref vsCommandStatus status,
					ref object commandText )
				{
					try
					{
						LicenseInfo licInfo = this.workspace.QueryLicenseInfo();
						if ( !licInfo.IsTrial )
						{
							//
							// Licensed - do not show.
							//
							status = vsCommandStatus.vsCommandStatusInvisible;
							return;
						}
					}
					catch
					{ }

					status = vsCommandStatus.vsCommandStatusSupported |
						 vsCommandStatus.vsCommandStatusEnabled;
				};

				//
				// Setup main menu entries.
				//
				DteMainMenu mainMenu = DteMainMenu.GetToolsMenu( this );
				if ( explorerCommandCreated )
				{
					mainMenu.Add(
						explorerCommand,
						1,
						Icons.Explorer,
						Icons.ExplorerMask,
						true );
				}

				if ( resultsCommandCreated )
				{
					mainMenu.Add(
						resultsCommand,
						2,
						Icons.Results,
						Icons.ResultsMask,
						false );
				}

				if ( redebugRunCommandCreated )
				{
					mainMenu.Add(
						redebugRunCommand,
						3,
						Icons.Start,
						Icons.StartMask,
						false );
				}

				if ( restartRunCommandCreated )
				{
					mainMenu.Add(
						restartRunCommand,
						4,
						Icons.Ffwd,
						Icons.FfwdMask,
						false );
				}

				DteMainMenu helpMenu = DteMainMenu.GetHelpMenu( this );
				if ( enterLicenseCommandCreated )
				{
					helpMenu.Add(
						enterLicenseCommand,
						helpMenu.LastOrdinal,
						Icons.cfix,
						Icons.cfix,
						true );
				}

				if ( aboutCommandCreated )
				{
					helpMenu.Add(
						aboutCommand,
						helpMenu.LastOrdinal,
						Icons.cfix,
						Icons.cfix,
						false );
				}

				//
				// Setup toolbar.
				//
				//this.toolbar = DteCommandBar.Create(
				//    this,
				//    "cfix" );

				//this.toolbar.AddButton(
				//    this.explorerCommand,
				//    1,
				//    Icons.Explorer,
				//    Icons.ExplorerMask,
				//    MsoButtonStyle.msoButtonIcon );
				//this.toolbar.AddButton(
				//    this.resultsCommand,
				//    2,
				//    Icons.Results,
				//    Icons.ResultsMask,
				//    MsoButtonStyle.msoButtonIcon );
				//
				//this.toolbar.Visible = true;

				//
				// Restore tool windows.
				//
				this.workspace.ToolWindows.RestoreWindowState();
			}
			catch ( Exception x )
			{
				HandleError( x );
			}
		}

		protected override void Teardown()
		{
			try
			{
				//
				// N.B. Commands/CommandBars are permanent, so they
				// may not be deleted.
				//

				if ( this.workspace != null )
				{
					this.workspace.Dispose();
				}
			}
			catch ( Exception x )
			{
				HandleError( x );
			}
		}
	}
}