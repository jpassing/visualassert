using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Cfix.Addin.Windows
{
	/*++
		Progress bar that is independent of visual style, i.e.
		looks the same on Win2000, XP/Luna and Vista/Aero.
	--*/
	public partial class PlainProgressBar : UserControl
	{
		private int min = 0;	
		private int max = 100;	
		private int val = 0;	
		private Color BarColor = Color.Blue;

		public PlainProgressBar()
		{
			InitializeComponent();
		}

		protected override void OnResize( EventArgs e )
		{
			this.Invalidate();
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			Graphics g = e.Graphics;
			SolidBrush brush = new SolidBrush( BarColor );
			float percent = ( float ) ( val - min ) / ( float ) ( max - min );
			Rectangle rect = this.ClientRectangle;

			// Calculate area for drawing the progress.
			rect.Width = ( int ) ( ( float ) rect.Width * percent );

			// Draw the progress meter.
			g.FillRectangle( brush, rect );

			// Draw a three-dimensional border around the control.
			Draw3DBorder( g );

			// Clean up.
			brush.Dispose();
			g.Dispose();
		}

		public int Value
		{
			get
			{
				return val;
			}

			set
			{
				int oldValue = val;

				// Make sure that the value does not stray outside the valid range.
				if ( value < min )
				{
					val = min;
				}
				else if ( value > max )
				{
					val = max;
				}
				else
				{
					val = value;
				}

				// Invalidate only the changed area.
				float percent;

				Rectangle newValueRect = this.ClientRectangle;
				Rectangle oldValueRect = this.ClientRectangle;

				// Use a new value to calculate the rectangle for progress.
				percent = ( float ) ( val - min ) / ( float ) ( max - min );
				newValueRect.Width = ( int ) ( ( float ) newValueRect.Width * percent );

				// Use an old value to calculate the rectangle for progress.
				percent = ( float ) ( oldValue - min ) / ( float ) ( max - min );
				oldValueRect.Width = ( int ) ( ( float ) oldValueRect.Width * percent );

				Rectangle updateRect = new Rectangle();

				// Find only the part of the screen that must be updated.
				if ( newValueRect.Width > oldValueRect.Width )
				{
					updateRect.X = oldValueRect.Size.Width;
					updateRect.Width = newValueRect.Width - oldValueRect.Width;
				}
				else
				{
					updateRect.X = newValueRect.Size.Width;
					updateRect.Width = oldValueRect.Width - newValueRect.Width;
				}

				updateRect.Height = this.Height;

				// Invalidate the intersection region only.
				this.Invalidate( updateRect );
			}
		}

		public Color ProgressBarColor
		{
			get
			{
				return BarColor;
			}

			set
			{
				BarColor = value;

				// Invalidate the control to get a repaint.
				this.Invalidate();
			}
		}

		private void Draw3DBorder( Graphics g )
		{
			int PenWidth = ( int ) Pens.White.Width;

			g.DrawLine( Pens.DarkGray,
				new Point( this.ClientRectangle.Left, this.ClientRectangle.Top ),
				new Point( this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Top ) );
			g.DrawLine( Pens.DarkGray,
				new Point( this.ClientRectangle.Left, this.ClientRectangle.Top ),
				new Point( this.ClientRectangle.Left, this.ClientRectangle.Height - PenWidth ) );
			g.DrawLine( Pens.White,
				new Point( this.ClientRectangle.Left, this.ClientRectangle.Height - PenWidth ),
				new Point( this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Height - PenWidth ) );
			g.DrawLine( Pens.White,
				new Point( this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Top ),
				new Point( this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Height - PenWidth ) );
		} 

	}
}
