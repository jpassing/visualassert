using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.Ui.Result;
using EnvDTE80;

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

		private IResultNode contextMenuReferenceItem;

		private DTE2 dte;
		private Workspace workspace;

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
							Strings.ProgressInfo,
							completed,
							total,
							this.run.TaskCount,
							this.run.Status );
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

				if ( this.run.Status == TaskStatus.Suceeded )
				{
					this.progressLabel.Text = this.run.RootResult.Status.ToString();
				}
				else
				{
					this.progressLabel.Text = this.run.Status.ToString();
				}

				this.progressBar.Invalidate();
				this.progressLabel.Invalidate();
			} );
		}

		private void run_Started( object sender, EventArgs e )
		{
			this.BeginInvoke( ( VoidDelegate ) delegate
			{
				this.progressLabel.Text =
					String.Format(
						Strings.ProgressInfo,
						0,
						this.run.ItemCount,
						this.run.TaskCount,
						this.run.Status );
				this.progressBar.Invalidate(); 
				this.progressLabel.Invalidate();
			} );
		}

		/*----------------------------------------------------------------------
		 * Private - Events.
		 */

		private void stopButton_Click( object sender, EventArgs e )
		{
			try
			{
				lock ( this.runLock )
				{
					this.aborted = true;
					this.run.Stop();
				}
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
			}
		}

		private void terminateButton_Click( object sender, EventArgs e )
		{
			try
			{
				lock ( this.runLock )
				{
					this.aborted = true;
					this.run.Terminate();
				}
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
			}
		}

		/*----------------------------------------------------------------------
		 * Context Menu.
		 */

		private void results_ContextMenuRequested(
			object sender, 
			ContextMenuEventArgs e
			)
		{
			try
			{
				IResultNode item = ( IResultNode ) sender;
				ResultItemNode resultItem = item as ResultItemNode;
				if ( resultItem != null )
				{
					this.resultCtxMenu.Show( this.results, e.Location );

					//
					// Remember node to associate menu item clicks with
					// this node.
					//
					this.contextMenuReferenceItem = item;
				}
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
			}
		}

		/*----------------------------------------------------------------------
		 * Run/Debug/GotoSource.
		 */

		private void ctxMenuDebugButton_Click( object sender, EventArgs e )
		{
			ResultItemNode resultItem = this.contextMenuReferenceItem as ResultItemNode;
			if ( resultItem != null )
			{
				CommonUiOperations.RunItem(
					this.workspace,
					resultItem.ResultItem.Item,
					true );
			}
		}

		private void ctxMenuRunButton_Click( object sender, EventArgs e )
		{
			ResultItemNode resultItem = this.contextMenuReferenceItem as ResultItemNode;
			if ( resultItem != null )
			{
				CommonUiOperations.RunItem(
					this.workspace,
					resultItem.ResultItem.Item,
					false );
			}
		}

		private void results_TreeKeyDown( object sender, KeyEventArgs e )
		{
			if ( e.KeyCode == Keys.Enter )
			{
				if ( e.Control )
				{
					ResultItemNode resultNode = sender as ResultItemNode;
					if ( resultNode != null )
					{
						CommonUiOperations.RunItem(
							this.workspace,
							resultNode.ResultItem.Item,
							! e.Shift );
					}

					e.Handled = true;
				}
				else
				{
					FailureNode failNode = sender as FailureNode;
					if ( failNode != null )
					{
						if ( failNode.File != null )
						{
							CommonUiOperations.GoToSource(
								this.dte,
								failNode.File,
								failNode.Line );
						}
					}
				}
			}
		}

		private void results_TreeDoubleClick( object sender, MouseEventArgs e )
		{
			FailureNode failNode = sender as FailureNode;
			if ( failNode != null )
			{
				if ( failNode.File != null )
				{
					CommonUiOperations.GoToSource(
						this.dte,
						failNode.File,
						failNode.Line );
				}
			}
		}
		
		/*----------------------------------------------------------------------
		 * Public.
		 */
		
		public RunWindow()
		{
			InitializeComponent();

			this.results.ContextMenuRequested += new EventHandler<Cfix.Control.Ui.Result.ContextMenuEventArgs>(results_ContextMenuRequested);
			this.results.TreeKeyDown += new KeyEventHandler( results_TreeKeyDown );
			this.results.TreeDoubleClick += new MouseEventHandler( results_TreeDoubleClick );
		}

		public void Initialize(
			Workspace ws,
			DTE2 dte )
		{
			this.workspace = ws;
			this.dte = dte;
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
