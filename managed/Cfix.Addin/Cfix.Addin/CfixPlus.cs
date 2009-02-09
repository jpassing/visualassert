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
		private DteCommandBar toolbar;

		private DteCommand explorerCommand;
		private DteCommand resultsCommand;

		private Configuration config;
		private Workspace workspace;
		private ToolWindows toolWindows;

		/*----------------------------------------------------------------------
		 * Internal.
		 */

		internal Workspace Workspace
		{
			get { return workspace; }
		}

		internal static void HandleError( Exception x )
		{
			MessageBox.Show( x.Message + "\n\n" + x.StackTrace );
		}

		/*----------------------------------------------------------------------
		 * DteConnect overrides.
		 */

		public override String CommandPrefix
		{
			get { return CommandPrefixConstant; }
		}

		protected override void Setup()
		{
			try
			{
				this.config = Configuration.Load();
				this.workspace = new Workspace();
				this.toolWindows = new ToolWindows( this );

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
					this.toolWindows.Explorer.Activate();
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
				this.toolWindows.RestoreWindowState();
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
				if ( this.mainMenu != null )
				{
					this.mainMenu.Delete();
				}

				if ( this.toolbar != null )
				{
					this.toolbar.Delete();
				}

				if ( this.explorerCommand != null )
				{
					this.explorerCommand.Delete();
				}

				if ( this.resultsCommand != null )
				{
					this.resultsCommand.Delete();
				}

				if ( this.toolWindows != null )
				{
					this.toolWindows.SaveWindowState();
					this.toolWindows.CloseAll();
				}
			}
			catch ( Exception x )
			{
				HandleError( x );
			}
		}

		
	}
}