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
using Cfix.Control;

//
// N.B. Outside namespace s.t. command names are more pretty.
//
public class VisualAssert : DteConnect
{
	private static readonly String CommandPrefixConstant = typeof( VisualAssert ).FullName + ".";

	private Workspace workspace;
	private DTEEvents dteEvents;

	private void CheckUiHierarchyItemPartOfVcProject(
		vsCommandStatusTextWanted neededText,
		ref vsCommandStatus status,
		ref object commandText )
	{
		if ( CommonUiOperations.IsCurrentHierarchyItemPartOfVcProject( this.DTE ) ||
			 CommonUiOperations.IsCurrentHierarchyItemSolution( this.DTE ) )
		{
			status = vsCommandStatus.vsCommandStatusSupported |
				 vsCommandStatus.vsCommandStatusEnabled;
		}
		else
		{
			status = vsCommandStatus.vsCommandStatusInvisible;
		}
	}

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

	internal static void HandleError( string message, Exception x )
	{
		Logger.LogWarning( "Popup", "{0}", x.Message );
		MessageBox.Show( 
			String.Format( "{0}\n\nDetails: {1}", message, x.Message ) );
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

	internal static void ShowError( String msg )
	{
		Logger.LogInfo( "Popup", msg );
		MessageBox.Show(
			msg,
			Strings.MsgBoxCaption,
			MessageBoxButtons.OK,
			MessageBoxIcon.Exclamation );
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
			this.workspace = new Workspace( this.DTE, this.Addin );
			this.dteEvents = this.DTE.Events.DTEEvents;
			this.dteEvents.ModeChanged += new _dispDTEEvents_ModeChangedEventHandler( dteEvents_ModeChanged );
			this.dteEvents.OnBeginShutdown += new _dispDTEEvents_OnBeginShutdownEventHandler( dteEvents_OnBeginShutdown );
			this.dteEvents.OnMacrosRuntimeReset += new _dispDTEEvents_OnMacrosRuntimeResetEventHandler( dteEvents_OnMacrosRuntimeReset );
			this.dteEvents.OnStartupComplete += new _dispDTEEvents_OnStartupCompleteEventHandler( dteEvents_OnStartupComplete );

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
					this.workspace, null, RunMode.Debug );
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
					this.workspace, null, RunMode.Normal );
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
				new Cfix.Addin.Windows.AboutWindow( this.workspace ).ShowDialog();
			};

			bool checkForUpdateCommandCreated;
			DteCommand checkForUpdateCommand = new DteCommand(
				this,
				"CheckForUpdate",
				Strings.CheckForUpdatesCmdCaption,
				null,
				false,
				out checkForUpdateCommandCreated );

			checkForUpdateCommand.Execute += delegate(
				vsCommandExecOption executeOption,
				ref object varIn,
				ref object varOut,
				ref bool handled )
			{
				UpdateCheckWindow.CheckForUpdate();
			};

			bool feedbackCommandCreated;
			DteCommand feedbackCommand = new DteCommand(
				this,
				"Feedback",
				Strings.FeedbackCmdCaption,
				null,
				false,
				out feedbackCommandCreated );

			feedbackCommand.Execute += delegate(
				vsCommandExecOption executeOption,
				ref object varIn,
				ref object varOut,
				ref bool handled )
			{
				CommonUiOperations.OpenLameWebpage( this.DTE, "General" );
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

			startCurrentHierItemCommand.QueryStatus += CheckUiHierarchyItemPartOfVcProject;
			startCurrentHierItemCommand.Execute += delegate(
				vsCommandExecOption executeOption,
				ref object varIn,
				ref object varOut,
				ref bool handled )
			{
				CommonUiOperations.RunSelectedUiHierarchyItem(
					this.DTE,
					this.workspace,
					RunMode.Normal );
			};

			bool debugCurrentHierItemCommandCreated;
			DteCommand debugCurrentHierItemCommand = new DteCommand(
				this,
				"DebugCurrentHierarchyItem",
				Strings.DebugCurrentHierItemCaption,
				null,
				false,
				out debugCurrentHierItemCommandCreated );

			debugCurrentHierItemCommand.QueryStatus += CheckUiHierarchyItemPartOfVcProject;
			debugCurrentHierItemCommand.Execute += delegate(
				vsCommandExecOption executeOption,
				ref object varIn,
				ref object varOut,
				ref bool handled )
			{
				CommonUiOperations.RunSelectedUiHierarchyItem(
					this.DTE,
					this.workspace,
					RunMode.Debug );
			};

			bool addUnitTestItemCommandCreated;
			DteCommand addUnitTestItemCommand = new DteCommand(
				this,
				"AddUnitTest",
				Strings.AddUnitTestCmdCaption,
				null,
				false,
				out addUnitTestItemCommandCreated );

			addUnitTestItemCommand.QueryStatus += CheckUiHierarchyItemPartOfVcProject;
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
			DteMainMenu vaMenu = DteMainMenu.Create(
				this, "VisualAssert", Strings.MainMenuCaption );

			vaMenu.Add(
				explorerCommand,
				1,
				Icons.Explorer,
				Icons.ExplorerMask,
				false );
			
			vaMenu.Add(
				resultsCommand,
				2,
				Icons.Results,
				Icons.ResultsMask,
				false );
		
			vaMenu.Add(
				redebugRunCommand,
				3,
				Icons.Start,
				Icons.StartMask,
				true );
		
			vaMenu.Add(
				restartRunCommand,
				4,
				Icons.Ffwd,
				Icons.FfwdMask,
				false );
		
			vaMenu.Add(
				quickStartCommand,
				5,
				Icons.Doc,
				Icons.DocMask,
				true );
		
			vaMenu.Add(
				docCommand,
				6,
				Icons.Doc,
				Icons.DocMask,
				false );

			vaMenu.Add(
				feedbackCommand,
				7,
				Icons.Feedback,
				Icons.FeedbackMask,
				false );

			vaMenu.Add(
				checkForUpdateCommand,
				8,
				Icons.cfix,
				Icons.cfix,
				false );

			vaMenu.Add(
				aboutCommand,
				9,
				Icons.cfix,
				Icons.cfix,
				false );

#if !BETA && !FREEWARE
			if ( enterLicenseCommandCreated )
			{
				vaMenu.Add(
					19,
					vaMenu.LastOrdinal,
					Icons.cfix,
					Icons.cfix,
					true );
			}
#endif

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

#if ! VS100
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
#endif

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

	protected override void Teardown( bool hostShutdown )
	{
		try
		{
			//
			// N.B. Commands/CommandBars are permanent, so they
			// may not be deleted.
			//

			if ( this.workspace != null )
			{
				if ( ! hostShutdown )
				{
					this.workspace.ToolWindows.CloseAll();
				}

				this.workspace.Dispose();
			}

			if ( this.dteEvents != null )
			{
				this.dteEvents.ModeChanged -= new _dispDTEEvents_ModeChangedEventHandler( dteEvents_ModeChanged );
				this.dteEvents.OnBeginShutdown -= new _dispDTEEvents_OnBeginShutdownEventHandler( dteEvents_OnBeginShutdown );
				this.dteEvents.OnMacrosRuntimeReset -= new _dispDTEEvents_OnMacrosRuntimeResetEventHandler( dteEvents_OnMacrosRuntimeReset );
				this.dteEvents.OnStartupComplete -= new _dispDTEEvents_OnStartupCompleteEventHandler( dteEvents_OnStartupComplete );
			}

			this.dteEvents = null;
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
			Version version = typeof( VisualAssert ).Assembly.GetName().Version;
			return String.Format(
				"{0}.{1}.{2} Build {3}",
				version.Major,
				version.Minor,
				version.Build,
				version.MinorRevision );
		}
	}

	/*--------------------------------------------------------------------------
	 * DTE Events.
	 */

	private void dteEvents_ModeChanged( vsIDEMode LastMode )
	{
	}

	private void dteEvents_OnBeginShutdown()
	{
		//
		// N.B. Call SaveWindowState rather than in Teardown as
		// the tool window visibility is not available any more when
		// Teardown is called during IDE shutdown.
		//
		this.workspace.ToolWindows.SaveWindowState();
	}

	private void dteEvents_OnMacrosRuntimeReset()
	{
	}

	private void dteEvents_OnStartupComplete()
	{
	}
}
