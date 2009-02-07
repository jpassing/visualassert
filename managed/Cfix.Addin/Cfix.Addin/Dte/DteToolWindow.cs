using System;
using System.Drawing;
using EnvDTE;
using EnvDTE80;

namespace Cfix.Addin.Dte
{
	internal class DteToolWindow
	{
		private Window window;
		private Object userControl;
		private WindowEvents events;

		public delegate void WindowClosingDelegate();
		public event WindowClosingDelegate WindowClosing;

		private void WindowCreatedEvent( Window affectedWnd )
		{ }

		private void WindowActivateEvent( Window gotFocus, Window lostFocus )
		{ }

		private void WindowMovedEvent( Window window, int top, int left, int width, int height )
		{ }
		
		private void WindowClosingEvent( Window affectedWnd )
		{
			if ( ReferenceEquals( this.window, affectedWnd ) )
			{
				if ( WindowClosing != null )
				{
					WindowClosing();
				}
			}
		}

		private DteToolWindow(
			DteConnect connect,
			Window window,
			Object userControl
			)
		{
			this.window = window;
			this.userControl = userControl;

			this.events = connect.Events.get_WindowEvents( null );

			this.events.WindowClosing += WindowClosingEvent;
			this.events.WindowActivated += WindowActivateEvent;
			this.events.WindowCreated += WindowCreatedEvent;
			this.events.WindowMoved += WindowMovedEvent;
		}

		public static DteToolWindow Create(
			DteConnect connect,
			String caption,
			Guid positionGuid,
			Type userControlType,
			Image tabIcon
			)
		{
			Windows2 win = ( Windows2 ) connect.DTE.Windows;
			object userControl = null;

			Window wnd = win.CreateToolWindow2(
				connect.Addin,
				userControlType.Assembly.Location,
				userControlType.FullName,
				caption,
				"{" + positionGuid.ToString() + "}",
				ref userControl );
			wnd.SetTabPicture(
				IconUtil.GetIPictureDispFromImage( tabIcon ) );

			return new DteToolWindow( connect, wnd, userControl ); ;
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
