using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Cfix.Addin
{
	internal class MainMenu
	{
		private readonly String name;
		private readonly String caption;

		private readonly DTE2 dte;
		private readonly Connect connect;	

		private readonly ResourceManager resourceManager;
		private readonly CultureInfo cultureInfo;

		private readonly CommandBarPopup topLevelMenu;

		private readonly CommandItem sampleMenuItem;

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

		/*----------------------------------------------------------------------
		 * Publics.
		 */

		public MainMenu( DTE2 dte, Connect connect, String name, String caption )
		{
			this.dte = dte;
			this.connect = connect;
			this.name = name;
			this.caption = caption;

			this.resourceManager = new ResourceManager(
				"Cfix.Addin.CommandBar", Assembly.GetExecutingAssembly() );
			this.cultureInfo = new CultureInfo( dte.LocaleID );
			
			CommandBars dteCommandBars = ( CommandBars ) dte.CommandBars;
			CommandBar dteMainMenuBar = dteCommandBars[ "MenuBar" ];

			this.topLevelMenu = ( CommandBarPopup ) dteMainMenuBar.Controls.Add(
				MsoControlType.msoControlPopup,
				Type.Missing,
				Type.Missing,
				CalculateMenuIndex(),
				true );
			this.topLevelMenu.CommandBar.Name = this.name;
			this.topLevelMenu.Caption = this.caption;

			this.sampleMenuItem = new CommandItem(
				dte, connect, "SampleItem", "Sample" );
			this.sampleMenuItem.AddTo( this );
		}

		public void Show()
		{
			this.topLevelMenu.Visible = true;
		}

		public void Delete()
		{
			this.topLevelMenu.Delete( true );
		}

		public CommandBar CommandBar
		{
			get
			{
				return this.topLevelMenu.CommandBar;
			}
		}
	}
}
