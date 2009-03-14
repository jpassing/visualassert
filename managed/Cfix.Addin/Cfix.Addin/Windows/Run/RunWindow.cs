using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Cfix.Control;

namespace Cfix.Addin.Windows.Run
{
	public partial class RunWindow : UserControl
	{
		public static readonly Guid Guid = 
			new Guid( "80c3fde2-9f60-4709-b1d2-28561e3cff9f" );

		private readonly object runLock = new object();
		
		//
		// Current run, guarded by runLock.
		//
		private IRun run;

		/*----------------------------------------------------------------------
		 * Private - Run events.
		 * 
		 * N.B. Execute on non-GUI thread.
		 */

		private void run_StatusChanged( object sender, EventArgs e )
		{
		}

		private void run_Log( object sender, LogEventArgs e )
		{
		}

		private void run_Finished( object sender, FinishedEventArgs e )
		{
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */
		
		public RunWindow()
		{
			InitializeComponent();
		}

		public void Initialize( Workspace ws )
		{
		}

		public IRun Run
		{
			get { return this.run; }
			set
			{
				lock ( this.runLock )
				{
					if ( this.run != null )
					{
						this.run.Finished -= new EventHandler<FinishedEventArgs>( run_Finished );
						this.run.Log -= new EventHandler<LogEventArgs>( run_Log );
						this.run.StatusChanged -= new EventHandler( run_StatusChanged );

						this.run.Dispose();
					}

					this.run = value;
					this.results.Run = value;

					this.run.Finished += new EventHandler<FinishedEventArgs>( run_Finished );
					this.run.Log += new EventHandler<LogEventArgs>( run_Log );
					this.run.StatusChanged += new EventHandler( run_StatusChanged );
				}
			}
		}

		public void StartRun()
		{
			lock ( this.runLock )
			{
				Debug.Assert( this.run != null );
				this.run.Start();
			}
		}
	}
}
