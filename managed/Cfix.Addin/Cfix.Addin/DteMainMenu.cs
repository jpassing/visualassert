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

		private int GetToolsMenuIndex()
		{
			try
			{
				string resourceName = String.Concat( 
					new CultureInfo( this.connect.DTE.LocaleID ).TwoLetterISOLanguageName, "Tools" );

				ResourceManager resourceManager = new ResourceManager(
					"Cfix.Addin.CommandBar", Assembly.GetExecutingAssembly() );
				String name = resourceManager.GetString( resourceName );

				CommandBar menuBarCommandBar = 
					( ( CommandBars ) this.connect.DTE.CommandBars )[ "MenuBar" ];
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
				CommandBar menuBarCommandBar = 
					( ( CommandBars ) this.connect.DTE.CommandBars )[ "MenuBar" ];
				int menuCount = menuBarCommandBar.Controls.Count;
				return menuCount;
			}
		}

		/*----------------------------------------------------------------------
		 * Protected.
		 */

		protected override CommandBarControl CreateControl()
		{
			CommandBars cmdBars = ( CommandBars ) this.connect.DTE.CommandBars;

			CommandBars dteCommandBars = ( CommandBars ) this.connect.DTE.CommandBars;
			CommandBar dteMainMenuBar = dteCommandBars[ "MenuBar" ];

			this.popup = ( CommandBarPopup ) dteMainMenuBar.Controls.Add(
				MsoControlType.msoControlPopup,
				Type.Missing,
				Type.Missing,
				CalculateMenuIndex(),
				true );
			this.popup.CommandBar.Name = this.name;
			return this.popup;
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public DteMainMenu( 
			DteConnect connect, 
			String name, 
			String caption ) : base( connect, name, caption )
		{
		}

		public override void Add( DteCommand item )
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
