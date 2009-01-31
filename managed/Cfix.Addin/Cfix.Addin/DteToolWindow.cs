using System;
using EnvDTE;
using EnvDTE80;

namespace Cfix.Addin
{
	internal class DteToolWindow
	{
		private Window window;
		private Object userControl;

		private DteToolWindow(
			Window window,
			Object userControl
			)
		{
			this.window = window;
			this.userControl = userControl;
		}

		public static DteToolWindow Create(
			DteConnect connect,
			String caption,
			Guid positionGuid,
			Type userControlType
			)
		{
			Windows2 win = ( Windows2 ) connect.DTE.Windows;
			object userControl = null;

			Window toolWin = win.CreateToolWindow2(
				connect.Addin,
				userControlType.Assembly.Location,
				userControlType.FullName,
				caption,
				"{" + positionGuid.ToString() + "}",
				ref userControl );

			return new DteToolWindow( toolWin, userControl );
		}

		public Window Window
		{
			get { return window; }
		}

		public Object UserControl
		{
			get { return userControl; }
		}

		public bool Visible
		{
			get { return this.window.Visible; }
			set { this.window.Visible = value; }
		}
	}
}
