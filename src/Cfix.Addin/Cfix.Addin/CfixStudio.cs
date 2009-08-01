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
using Cfix.Addin;
using Cfix.Addin.Dte;
using Cfix.Addin.Windows;
using Cfix.Addin.Windows.Explorer;
using Cfix.Control;

//
// N.B. Outside namespace s.t. command names are more pretty.
//
public class CfixStudio : DteConnect
{
	private static readonly String CommandPrefixConstant = typeof( CfixStudio ).FullName + ".";

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
		Logger.LogWarning( "Popup", "{0}", x.Message );
		MessageBox.Show( x.Message );
	}

	internal static void ShowInfo( String msg )
	{
		Logger.LogInfo( "Popup", msg );
		MessageBox.Show( 
			msg, 
			Strings.MsgBoxCaption, 
			MessageBoxButtons.OK, 
			MessageBoxIcon.Information );
	}

	internal static bool ShowQuestion( String msg )
	{
		Logger.LogInfo( "Popup", msg );
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
			Logger.SetOutput( Directories.LogDirectory + "\\addin.log" );
			Logger.LogInfo( "Addin", "Version: {0}", Version );
		}
		catch ( Exception x )
		{
			MessageBox.Show( "Failed to open log file: " + x.Message );
		}

		try
		{
			this.workspace = new Workspace( this );

			//
			// Create commands.
			//
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
				CommonUiOperations.RunItem(
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
				CommonUiOperations.RunItem(
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

			bool docCommandCreated;
			DteCommand docCommand = new DteCommand(
				this,
				"Doc",
				Strings.DocCmdCaption,
				null,
				false,
				out docCommandCreated );

			docCommand.Execute += delegate(
				vsCommandExecOption executeOption,
				ref object varIn,
				ref object varOut,
				ref bool handled )
			{
				CommonUiOperations.OpenDocumentation();
			};

			bool quickStartCommandCreated;
			DteCommand quickStartCommand = new DteCommand(
				this,
				"QuickStart",
				Strings.QuickStartCmdCaption,
				null,
				false,
				out quickStartCommandCreated );

			quickStartCommand.Execute += delegate(
				vsCommandExecOption executeOption,
				ref object varIn,
				ref object varOut,
				ref bool handled )
			{
				CommonUiOperations.OpenQuickStartPage( this.DTE );
			};

			bool startCurrentHierItemCommandCreated;
			DteCommand startCurrentHierItemCommand = new DteCommand(
				this,
				"StartCurrentHierarchyItem",
				Strings.StartCurrentHierItemCmdCaption,
				null,
				false,
				out startCurrentHierItemCommandCreated );

			startCurrentHierItemCommand.Execute += delegate(
				vsCommandExecOption executeOption,
				ref object varIn,
				ref object varOut,
				ref bool handled )
			{
				CommonUiOperations.RunSelectedUiHierarchyItem(
					this.DTE,
					this.workspace,
					false );
			};

			bool debugCurrentHierItemCommandCreated;
			DteCommand debugCurrentHierItemCommand = new DteCommand(
				this,
				"DebugCurrentHierarchyItem",
				Strings.DebugCurrentHierItemCaption,
				null,
				false,
				out debugCurrentHierItemCommandCreated );

			debugCurrentHierItemCommand.Execute += delegate(
				vsCommandExecOption executeOption,
				ref object varIn,
				ref object varOut,
				ref bool handled )
			{
				CommonUiOperations.RunSelectedUiHierarchyItem(
					this.DTE,
					this.workspace,
					true );
			};

			bool addUnitTestItemCommandCreated;
			DteCommand addUnitTestItemCommand = new DteCommand(
				this,
				"AddUnitTest",
				Strings.AddUnitTestCmdCaption,
				null,
				false,
				out addUnitTestItemCommandCreated );

			addUnitTestItemCommand.Execute += delegate(
				vsCommandExecOption executeOption,
				ref object varIn,
				ref object varOut,
				ref bool handled )
			{
				CommonUiOperations.LaunchAddFixtureWizardForCurrentHierarchyItem( this.DTE );
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
#if !BETA
			if ( enterLicenseCommandCreated )
			{
				helpMenu.Add(
					enterLicenseCommand,
					helpMenu.LastOrdinal,
					Icons.cfix,
					Icons.cfix,
					true );
			}
#endif

			if ( aboutCommandCreated )
			{
				helpMenu.Add(
					aboutCommand,
					helpMenu.LastOrdinal,
					Icons.cfix,
					Icons.cfix,
					false );
			}

			if ( quickStartCommandCreated )
			{
				helpMenu.Add(
					quickStartCommand,
					1,	// Top
					Icons.Doc,
					Icons.DocMask,
					false );
			}

			if ( docCommandCreated )
			{
				helpMenu.Add(
					docCommand,
					1,	// Top
					Icons.Doc,
					Icons.DocMask,
					false );
			}

			//
			// Setup context menu entries.
			//

			DteCommandBar projectCtxMenu = DteMainMenu.GetProjectContextMenu( this );
			DteCommandBar solutionCtxMenu = DteMainMenu.GetSolutionContextMenu( this );
			
			if ( startCurrentHierItemCommandCreated )
			{
				projectCtxMenu.AddButton(
					startCurrentHierItemCommand,
					3,
					Icons.cfix,
					Icons.cfix,
					MsoButtonStyle.msoButtonIconAndCaption );
				solutionCtxMenu.AddButton(
					startCurrentHierItemCommand,
					3,
					Icons.cfix,
					Icons.cfix,
					MsoButtonStyle.msoButtonIconAndCaption );
			}

			if ( debugCurrentHierItemCommandCreated )
			{
				projectCtxMenu.AddButton(
					debugCurrentHierItemCommand,
					3,
					Icons.cfix,
					Icons.cfix,
					MsoButtonStyle.msoButtonIconAndCaption );
				solutionCtxMenu.AddButton(
					debugCurrentHierItemCommand,
					3,
					Icons.cfix,
					Icons.cfix,
					MsoButtonStyle.msoButtonIconAndCaption );
			}

			DteCommandBar addItemCtxMenu = DteMainMenu.GetAddItemMenu( this );

			if ( addUnitTestItemCommandCreated )
			{
				addItemCtxMenu.AddButton(
					addUnitTestItemCommand,
					addItemCtxMenu.CommandBar.Controls.Count,	// End.
					Icons.cfix,
					Icons.cfix,
					MsoButtonStyle.msoButtonIconAndCaption );
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

			if ( this.workspace.Configuration.ShowQuickStartPage )
			{
				CommonUiOperations.OpenQuickStartPage( this.DTE );

				//
				// Never show again.
				//
				this.workspace.Configuration.ShowQuickStartPage = false;
			}

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

		try
		{
			Logger.CloseOutput();
		}
		catch
		{ }
	}

	internal static string Version
	{
		get
		{
			Version version = typeof( CfixStudio ).Assembly.GetName().Version;
			return String.Format(
				"{0}.{1}.{2} Build {3}",
				version.Major,
				version.Minor,
				version.MajorRevision,
				version.MinorRevision );
		}
	}
}
