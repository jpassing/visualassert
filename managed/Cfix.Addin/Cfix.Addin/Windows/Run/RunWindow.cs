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

		private static readonly Color FailedColor = Color.DarkOrange;
		private static readonly Color SuccessColor = Color.PaleGreen;
		private static readonly Color DefaultColor = Color.LightYellow;

		private readonly object runLock = new object();
		
		//
		// Current run, guarded by runLock.
		//
		private IRun run;
		private bool aborted;

		/*----------------------------------------------------------------------
		 * Private - Run events.
		 * 
		 * N.B. Execute on non-GUI thread.
		 */

		private delegate void VoidDelegate();

		private void run_StatusChanged( object sender, EventArgs e )
		{
			this.BeginInvoke(
				( VoidDelegate ) delegate
				{
					if ( this.aborted )
					{
						return;
					}

					IResultItem item = ( IResultItem ) sender;

					uint total = this.run.ItemCount;
					uint completed = this.run.ItemsCompleted;

					if ( total == completed )
					{
						this.progressBar.Value = 100;
						this.progressBar.BackColor = this.progressBar.ForeColor;
					}
					else
					{
						this.progressBar.Value = 
							( int ) ( completed * 100 / total );
					}

					if ( item.Status == ExecutionStatus.Failed &&
						this.progressBar.ForeColor == SuccessColor )
					{
						this.progressBar.ForeColor = FailedColor;
					}

					this.progressLabel.Text =
						String.Format(
							"Running, {0}/{1} completed.",
							completed,
							total );
					this.progressBar.Invalidate();
					this.progressLabel.Invalidate();
				} );
		}

		private void run_Log( object sender, LogEventArgs e )
		{
		}

		private void run_Finished( object sender, FinishedEventArgs e )
		{
			this.BeginInvoke( ( VoidDelegate ) delegate
			{
				this.terminateButton.Enabled = false;
				this.stopButton.Enabled = false;

				this.progressLabel.Text = "Finished.";
				this.progressBar.Invalidate();
				this.progressLabel.Invalidate();
			} );
		}

		private void run_Started( object sender, EventArgs e )
		{
			this.BeginInvoke( ( VoidDelegate ) delegate
			{
				this.progressLabel.Text = "Started.";
				this.progressBar.Invalidate(); 
				this.progressLabel.Invalidate();
			} );
		}

		/*----------------------------------------------------------------------
		 * Private - Events.
		 */

		private void stopButton_Click( object sender, EventArgs e )
		{
			lock ( this.runLock )
			{
				this.aborted = true;
				this.run.Stop();
			}
		}

		private void terminateButton_Click( object sender, EventArgs e )
		{
			lock ( this.runLock )
			{
				this.aborted = true;
				this.run.Terminate();
			}
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
						if ( this.run.Status == TaskStatus.Running )
						{
							//
							// Run still active - there is an imminent 
							// danger to deadlock (waiting for finish/disposal)
							// vs. UP updates being delivered via Invoke to
							// this (UI) thread.
							//
							// Ask whether to terminate.
							//
							if ( CfixPlus.ShowQuestion( Strings.TerminateActiveRun ) )
							{
								this.aborted = true;
								this.run.Terminate();
							}

							//
							// Give up and do not even try to wait.
							//
							throw new ConcurrentRunException();
						}


						this.run.Started -= new EventHandler( run_Started );
						this.run.Finished -= new EventHandler<FinishedEventArgs>( run_Finished );
						this.run.Log -= new EventHandler<LogEventArgs>( run_Log );
						this.run.StatusChanged -= new EventHandler( run_StatusChanged );

						this.run.Dispose();
					}

					this.progressBar.Value = 0;
					this.progressBar.ForeColor = SuccessColor;
					this.progressBar.BackColor = DefaultColor;

					this.terminateButton.Enabled = true;
					this.stopButton.Enabled = true;

					this.run = value;
					this.aborted = false;
					this.results.Run = value;

					this.run.Started += new EventHandler( run_Started );
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
