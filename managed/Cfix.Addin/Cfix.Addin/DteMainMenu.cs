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
	internal class DteMainMenu : DteCommandBarControl
	{
		private CommandBarPopup popup;

		private static int GetToolsMenuIndex( DteConnect connect  )
		{
			try
			{
				string resourceName = String.Concat( 
					new CultureInfo( connect.DTE.LocaleID ).TwoLetterISOLanguageName, "Tools" );

				ResourceManager resourceManager = new ResourceManager(
					"Cfix.Addin.CommandBar", Assembly.GetExecutingAssembly() );
				String name = resourceManager.GetString( resourceName );

				CommandBar menuBarCommandBar = 
					( ( CommandBars ) connect.DTE.CommandBars )[ "MenuBar" ];
				return menuBarCommandBar.Controls[ name ].Index + 1;
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
			int toolsIndex = GetToolsMenuIndex( connect );
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

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public void Add( DteCommand item )
		{
			// TODO: ordinal.
			item.Command.AddControl( this.popup.CommandBar, 1 );
			this.commands.Add( item );
		}

		public override void Delete()
		{
			base.Delete();
			this.popup = null;
		}
	}
}
