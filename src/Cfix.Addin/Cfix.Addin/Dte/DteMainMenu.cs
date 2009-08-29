/*----------------------------------------------------------------------
 * Purpose:
 *		Menu handling.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Globalization;
using System.Drawing;
using System.Reflection;
using System.Resources;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Cfix.Addin.Dte
{
	internal class DteMainMenu : DteCommandBarControl
	{
		private CommandBarPopup popup;

		private static string GetMenuName( 
			DteConnect connect, 
			string genericName 
			)
		{
			string resourceName = String.Concat( 
				new CultureInfo( connect.DTE.LocaleID ).TwoLetterISOLanguageName,
				genericName );

			ResourceManager resourceManager = new ResourceManager(
				"Cfix.Addin.VSStrings", Assembly.GetExecutingAssembly() );
			return resourceManager.GetString( resourceName );
		}

		private static int GetMenuIndex( DteConnect connect, string genericName )
		{
			try
			{
				CommandBar menuBarCommandBar = 
					( ( CommandBars ) connect.DTE.CommandBars )[ "MenuBar" ];
				return menuBarCommandBar.Controls[ GetMenuName( connect, genericName ) ].Index + 1;
			}
			catch
			{
				return -1;
			}
		}

		private static int CalculateMenuIndex( DteConnect connect  )
		{
			//
			// Use Tools menu as starting point.
			//
			int toolsIndex = GetMenuIndex( connect, "Tools" );
			if ( toolsIndex > 0 )
			{
				return toolsIndex;
			}
			else
			{
				CommandBar menuBarCommandBar = 
					( ( CommandBars ) connect.DTE.CommandBars )[ "MenuBar" ];
				int menuCount = menuBarCommandBar.Controls.Count;
				return menuCount;
			}
		}

		private static CommandBar FindCommandBar( 
			DteConnect connect,
			Guid guidCmdGroup, 
			uint menuID )
		{
			//
			// Touch MenuBar to make sure it is loaded. Otherwise, we may
			// get E_FAILs.
			//
			CommandBar menuBarCommandBar =
				( ( CommandBars ) connect.DTE.CommandBars )[ "MenuBar" ];

			Guid guidSvc = typeof( IVsProfferCommands ).GUID;
			Object objService;

			IOleServiceProvider sp = ( IOleServiceProvider ) connect.DTE;
			sp.QueryService( ref guidSvc, ref guidSvc, out objService );
			IVsProfferCommands vsProfferCmds = ( IVsProfferCommands ) objService;

			return ( CommandBar ) vsProfferCmds.FindCommandBar(
				IntPtr.Zero,
				ref guidCmdGroup,
				menuID ) as CommandBar;
		}

		private DteMainMenu(
			DteConnect connect,
			CommandBarPopup popup )
			: base( connect, popup )
		{
			this.popup = popup;
		}

		/*----------------------------------------------------------------------
		 * Static.
		 */

		public static DteMainMenu Create( DteConnect connect, String name, String caption )
		{
			CommandBars cmdBars = ( CommandBars ) connect.DTE.CommandBars;

			CommandBars dteCommandBars = ( CommandBars ) connect.DTE.CommandBars;
			CommandBar dteMainMenuBar = dteCommandBars[ "MenuBar" ];

			//
			// If already installed, delete them.
			//
			try
			{
				cmdBars[ name ].Delete();
				dteMainMenuBar.Controls[ caption ].Delete( false );
			}
			catch ( Exception )
			{ }

			CommandBarPopup popup = ( CommandBarPopup ) dteMainMenuBar.Controls.Add(
				MsoControlType.msoControlPopup,
				Type.Missing,
				Type.Missing,
				CalculateMenuIndex( connect ),
				true );
			popup.CommandBar.Name = name;
			popup.Caption = caption;
			return new DteMainMenu( connect, popup );
		}

		public static DteMainMenu CreatePermanent( DteConnect connect, String name, String caption )
		{
			CommandBars cmdBars = ( CommandBars ) connect.DTE.CommandBars;

			CommandBars dteCommandBars = ( CommandBars ) connect.DTE.CommandBars;
			CommandBar dteMainMenuBar = dteCommandBars[ "MenuBar" ];

			//
			// If already installed, delete them.
			//
			try
			{
				cmdBars[ name ].Delete();
				dteMainMenuBar.Controls[ caption ].Delete( false );
			}
			catch ( Exception )
			{ }
			
			CommandBar menu = ( CommandBar ) connect.DTE.Commands.AddCommandBar(
				name,
				vsCommandBarType.vsCommandBarTypeMenu,
				dteMainMenuBar,
				CalculateMenuIndex( connect ) );
			CommandBarPopup popup = ( CommandBarPopup ) menu.Parent;
			popup.Caption = caption;
			return new DteMainMenu( connect, popup );
		}

		public static DteMainMenu GetToolsMenu( DteConnect connect )
		{
			CommandBar bar = FindCommandBar(
				connect,
				new Guid( "D309F791-903F-11D0-9EFC-00A0C911004F" ),
				133 );
			CommandBarPopup popup = ( CommandBarPopup ) bar.Parent;
			return new DteMainMenu( connect, popup );
		}

		public static DteMainMenu GetHelpMenu( DteConnect connect )
		{
			CommandBar bar = FindCommandBar(
				connect,
				new Guid( "D309F791-903F-11D0-9EFC-00A0C911004F" ),
				136 );
			CommandBarPopup popup = ( CommandBarPopup ) bar.Parent;
			return new DteMainMenu( connect, popup );
		}

		public static DteCommandBar GetProjectContextMenu( DteConnect connect )
		{
			CommandBar commandBar = FindCommandBar(
				connect,
				new Guid( "D309F791-903F-11D0-9EFC-00A0C911004F" ),
				1026 );
			return new DteCommandBar( connect, commandBar );
		}

		public static DteCommandBar GetSolutionContextMenu( DteConnect connect )
		{
			CommandBar commandBar = FindCommandBar(
				connect,
				new Guid( "D309F791-903F-11D0-9EFC-00A0C911004F" ),
				1043 );
			return new DteCommandBar( connect, commandBar );
		}

		public static DteCommandBar GetAddItemMenu( DteConnect connect )
		{
			//
			// This FindControl call, although seemingly pointless, seems
			// to have the side effect that the menu is loaded, which in 
			// turn is a prerequisite for the subsequent FindCommandBar
			// call to work.
			//
			GetProjectContextMenu( connect ).CommandBar.FindControl(
				null,
				850,
				null,
				false,
				true );
			CommandBar commandBar = FindCommandBar(
				connect,
				new Guid( "D309F791-903F-11D0-9EFC-00A0C911004F" ),
				850 );
			return new DteCommandBar( connect, commandBar );
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public void Add( 
			DteCommand item, 
			int ordinal, 
			Image icon, 
			Image maskIcon,
			bool beginGroup
			)
		{
			CommandBarButton buttonCtl = ( CommandBarButton ) 
				item.Command.AddControl(
					this.popup.CommandBar, 
					ordinal );

			buttonCtl.BeginGroup = beginGroup;

			//
			// N.B. See KB555417 for details on icon handling.
			//
			buttonCtl.Picture = ( stdole.StdPicture ) IconUtil.GetIPictureDispFromImage( icon );
			buttonCtl.Mask = ( stdole.StdPicture ) IconUtil.GetIPictureDispFromImage( maskIcon );
			this.commands.Add( item );
		}

		public int LastOrdinal
		{
			get
			{
				return this.popup.Controls.Count;
			}
		}

		public override void Delete()
		{
			base.Delete();
			this.popup = null;
		}
	}
}
