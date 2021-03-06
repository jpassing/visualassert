/*----------------------------------------------------------------------
 * Purpose:
 *		Tool Window handling.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;

namespace Cfix.Addin.Dte
{
	public interface IDteToolWindowControl
	{
		void OnActivate();
	}

	internal class DteToolWindow< ControlT > : IDisposable where ControlT : UserControl
	{
		private Window window;
		private ControlT userControl;
		private WindowEvents events;

		private bool sizeInitialized;
		private int defaultHeight;
		private int defaultWidth;

		private int activationCount;

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

		private void InitializeSizeLazily()
		{
			//
			// N.B. Setting the size is not allowed until the
			// window has been made visible.
			//
			if ( ! this.sizeInitialized )
			{
				try
				{
					this.Height = this.defaultHeight;
					this.Width = this.defaultWidth;

					this.sizeInitialized = true;
				}
				catch ( Exception )
				{ }
			}
		}

		private DteToolWindow(
			DTE2 dte,
			Window window,
			ControlT userControl
			)
		{
			this.window = window;
			this.userControl = userControl;

			this.events = dte.Events.get_WindowEvents( null );

			this.events.WindowClosing += WindowClosingEvent;
			this.events.WindowActivated += WindowActivateEvent;
			this.events.WindowCreated += WindowCreatedEvent;
			this.events.WindowMoved += WindowMovedEvent;
		}

		~DteToolWindow()
		{
			Dispose( false );
		}

		public void Dispose()
		{
			GC.SuppressFinalize( this );
			Dispose( true );
		}

		public void Dispose( bool disposing )
		{
			this.events.WindowClosing -= WindowClosingEvent;
			this.events.WindowActivated -= WindowActivateEvent;
			this.events.WindowCreated -= WindowCreatedEvent;
			this.events.WindowMoved -= WindowMovedEvent;

			this.userControl.Dispose();
		}

		/*----------------------------------------------------------------------
		 * Publics.
		 */

		public static DteToolWindow<ControlT> Create(
			DTE2 dte,
			AddIn addin,
			String caption,
			Guid positionGuid,
			Bitmap tabIcon
			)
		{
			Windows2 win = ( Windows2 ) dte.Windows;
			object userControl = null;

			Type userControlType = typeof( ControlT );
			
			Window wnd = win.CreateToolWindow2(
				addin,
				userControlType.Assembly.Location,
				userControlType.FullName,
				caption,
				"{" + positionGuid.ToString() + "}",
				ref userControl );

			wnd.SetTabPicture(
				IconUtil.GetIPictureDispFromImage( tabIcon ) );

			return new DteToolWindow<ControlT>( 
				dte, 
				wnd, 
				( ControlT ) userControl ); ;
		}

		public Window Window
		{
			get { return this.window; }
		}

		public ControlT UserControl
		{
			get { return this.userControl; }
		}

		public bool Visible
		{
			get { return this.window.Visible; }
			set 
			{
				this.window.Visible = value; 

				//
				// N.B. Setting the size is not allowed until the
				// window has been made visible.
				//
				if ( value  )
				{
					InitializeSizeLazily();
				}
			}
		}

		public bool ToggleVisibility()
		{
			bool newState = !this.window.Visible;
			this.window.Visible = newState;
			return newState;
		}

		public int DefaultHeight
		{
			get { return this.defaultHeight; }
			set { this.defaultHeight = value; }
		}

		public int DefaultWidth
		{
			get { return this.defaultWidth; }
			set { this.defaultWidth = value; }
		}

		public int Height
		{
			get { return this.window.Height; }
			set { this.window.Height = value; }
		}

		public int Width
		{
			get { return this.window.Width; }
			set { this.window.Width = value; }
		}


		public bool IsFloating
		{
			get { return this.window.IsFloating; }
		}

		public void Activate()
		{
			bool wasVisible = this.window.Visible;
			this.window.Activate();

			if ( this.activationCount == 0 || ( !wasVisible && this.window.Visible ) )
			{
				this.activationCount++;
				//
				// A true activation.
				//
				InitializeSizeLazily();

				IDteToolWindowControl ctl = this.userControl as IDteToolWindowControl;
				if ( ctl != null )
				{
					ctl.OnActivate();
				}
			}
			else
			{
				//
				// Re-activate. Do not notify.
				//
			}
		}

		public void Close()
		{
			this.window.Close( vsSaveChanges.vsSaveChangesYes );
		}
	}
}
