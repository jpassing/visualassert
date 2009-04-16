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

		private DteMainMenu mainMenu;
		//private DteCommandBar toolbar;

		private DteCommand explorerCommand;
		private DteCommand resultsCommand;

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

				this.explorerCommand = new DteCommand(
					this,
					"Explorer",
					Strings.ExplorerCmdCaption );
				this.explorerCommand.SetShortcut( "Global::Ctrl+Alt+K,e" );

				this.explorerCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					this.workspace.ToolWindows.Explorer.Activate();
				} ;

				this.resultsCommand = new DteCommand(
					this,
					"Results",
					Strings.RunCmdCaption );
				this.resultsCommand.SetShortcut( "Global::Ctrl+Alt+K,r" );
				
				this.resultsCommand.Execute += delegate(
					vsCommandExecOption executeOption,
					ref object varIn,
					ref object varOut,
					ref bool handled )
				{
					this.workspace.ToolWindows.Run.Activate();
				};

				//
				// Setup main menu entry.
				//
				//this.mainMenu = DteMainMenu.CreatePermanent( 
				//    this,
				//    "MainMenu",
				//    "cfi&x" );
				this.mainMenu = DteMainMenu.GetToolsMenu( this );
				this.mainMenu.Add(
					this.explorerCommand,
					1,
					Icons.Explorer,
					Icons.ExplorerMask,
					true );
				this.mainMenu.Add(
					this.resultsCommand,
					2,
					Icons.Results,
					Icons.ResultsMask,
					false );

				this.mainMenu.Visible = true;
				
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