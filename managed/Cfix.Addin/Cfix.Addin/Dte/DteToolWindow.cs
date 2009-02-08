using System;
using System.Drawing;
using EnvDTE;
using EnvDTE80;

namespace Cfix.Addin.Dte
{
	internal class DteToolWindow< ControlT >
	{
		private Window window;
		private ControlT userControl;
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
			ControlT userControl
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

		public static DteToolWindow<ControlT> Create(
			DteConnect connect,
			String caption,
			Guid positionGuid,
			Image tabIcon
			)
		{
			Windows2 win = ( Windows2 ) connect.DTE.Windows;
			object userControl = null;

			Type userControlType = typeof( ControlT );
			
			Window wnd = win.CreateToolWindow2(
				connect.Addin,
				userControlType.Assembly.Location,
				userControlType.FullName,
				caption,
				"{" + positionGuid.ToString() + "}",
				ref userControl );
			wnd.SetTabPicture(
				IconUtil.GetIPictureDispFromImage( tabIcon ) );

			return new DteToolWindow<ControlT>( 
				connect, 
				wnd, 
				( ControlT ) userControl ); ;
		}

		public Window Window
		{
			get { return this.window; }
		}

		public Object UserControl
		{
			get { return this.userControl; }
		}

		public bool Visible
		{
			get { return this.window.Visible; }
			set { this.window.Visible = value; }
		}

		public bool ToggleVisibility()
		{
			bool newState = !this.window.Visible;
			this.window.Visible = newState;
			return newState;
		}

		public void Activate()
		{
			this.window.Activate();
		}

		public void Close()
		{
			this.window.Close( vsSaveChanges.vsSaveChangesYes );
		}
	}
}