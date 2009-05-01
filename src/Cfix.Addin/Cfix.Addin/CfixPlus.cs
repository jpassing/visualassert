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
	public delegate void QueryStatusDelegate( 
		vsCommandStatusTextWanted neededText, 
		ref vsCommandStatus status, 
		ref object commandText );

	public delegate void ExecDelegate( 
		vsCommandExecOption executeOption, 
		ref object varIn, 
		ref object varOut, 
		ref bool handled );

	public class CfixPlus : DteConnect
	{
		private static readonly String CommandPrefixConstant = typeof( CfixPlus ).FullName + ".";

		private DteCommand explorerCommand;
		private DteCommand resultsCommand;
		private DteCommand aboutCommand;

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
				this.explorerCommand = new DteCommand(
					this,
					"Explorer",
					Strings.ExplorerCmdCaption,
					"Global::Ctrl+Alt+K,e",
					false,
					out explorerCommandCreated );

				this.explorerCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					this.workspace.ToolWindows.Explorer.Activate();
				} ;

				bool resultsCommandCreated;
				this.resultsCommand = new DteCommand(
					this,
					"Results",
					Strings.RunCmdCaption,
					"Global::Ctrl+Alt+K,r",
					false,
					out resultsCommandCreated );
				
				this.resultsCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					this.workspace.ToolWindows.Run.Activate();
				};

				bool aboutCommandCreated;
				this.aboutCommand = new DteCommand(
					this,
					"About",
					Strings.AboutCmdCaption,
					null,
					false,
					out aboutCommandCreated );

				this.aboutCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					new Cfix.Addin.Windows.About.AboutWindow( this.workspace ).ShowDialog();
				};

				//
				// Setup main menu entry.
				//
				//this.mainMenu = DteMainMenu.CreatePermanent( 
				//    this,
				//    "MainMenu",
				//    "cfi&x" );
				DteMainMenu mainMenu = DteMainMenu.GetToolsMenu( this );
				if ( explorerCommandCreated )
				{
					mainMenu.Add(
						this.explorerCommand,
						1,
						Icons.Explorer,
						Icons.ExplorerMask,
						true );
				}

				if ( resultsCommandCreated )
				{
					mainMenu.Add(
						this.resultsCommand,
						2,
						Icons.Results,
						Icons.ResultsMask,
						false );
				}

				DteMainMenu helpMenu = DteMainMenu.GetHelpMenu( this );
				if ( aboutCommandCreated )
				{
					helpMenu.Add(
						this.aboutCommand,
						helpMenu.LastOrdinal,
						Icons.cfix,
						Icons.cfix,
						true );
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