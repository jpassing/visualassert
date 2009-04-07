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

		private static readonly Color FailedColor = Color.Tomato;
		private static readonly Color SuccessColor = Color.PaleGreen;
		private static readonly Color DefaultColor = SystemColors.Control;
		private static readonly Color InitialColor = Color.LightYellow;

		private readonly object runLock = new object();
		
		//
		// Current run, guarded by runLock.
		//
		private IRun run;
		private bool aborted;

		private IResultNode contextMenuReferenceItem;

		private DTE2 dte;
		private Workspace workspace;

		private static string GetStatusText( ExecutionStatus status )
		{
			string text = Strings.ResourceManager.GetString( status.ToString() );
			if ( text == null )
			{
				text = status.ToString();
			}

			return text;
		}

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

						this.progressBar.BackColor =
							completed > 0 ? DefaultColor : InitialColor;
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
							this.run.Status.ToString(),
							this.run.ItemsFailed,
							this.run.ItemsInconclusive );
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

				if ( this.run.Status == TaskStatus.Suceeded )
				{
					//
					// Run succeeded, but may have produced failures.
					//
					this.progressLabel.Text =
						String.Format(
							Strings.ProgressInfoFinish,
							GetStatusText( this.run.RootResult.Status ),
							this.run.ItemsFailed,
							this.run.ItemsInconclusive );
				}
				else
				{
					//
					// Run failed.
					//
					this.progressLabel.Text = this.run.Status.ToString();
					this.progressBar.ForeColor = FailedColor;
				}

				this.progressBar.Value = 100;
				this.progressBar.Invalidate();
				this.progressLabel.Invalidate();

				Exception excp = e.Exception;
				if ( excp != null )
				{
					CfixPlus.HandleError( excp );
				}
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
						this.run.Status.ToString(),
						0,
						0 );
				this.progressBar.Invalidate(); 
				this.progressLabel.Invalidate();
			} );
		}

		/*----------------------------------------------------------------------
		 * Private - Events.
		 */

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

				//
				// Remember node to associate menu item clicks with
				// this node.
				//
				this.contextMenuReferenceItem = item;

				ResultItemNode resultItem = item as ResultItemNode;
				if ( resultItem != null )
				{
					this.resultCtxMenu.Show( this.results, e.Location );
					return;
				}

				FailureNode failNode = item as FailureNode;
				if ( failNode != null && failNode.StackTrace != null )
				{
					this.stackTraceCtxMenu.Show( this.results, e.Location );
					return;
				}
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
			}
		}

		private void ctxMenuCopyTraceButton_Click( object sender, EventArgs e )
		{
			FailureNode failNode = this.contextMenuReferenceItem as FailureNode;
			if ( failNode != null && failNode.StackTrace != null )
			{
				Clipboard.SetText( failNode.StackTrace.ToString() );
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
				}
				else
				{
					ISourceReference src = sender as ISourceReference;
					if ( src != null )
					{
						if ( src.File != null )
						{
							CommonUiOperations.GoToSource(
								this.dte,
								src.File,
								src.Line );
						}
					}
				}

				e.Handled = true;
			}
		}

		private void results_TreeDoubleClick( object sender, MouseEventArgs e )
		{
			ISourceReference src = sender as ISourceReference;
			if ( src != null )
			{
				if ( src.File != null )
				{
					CommonUiOperations.GoToSource(
						this.dte,
						src.File,
						src.Line );
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
					this.progressBar.BackColor = InitialColor;

					this.terminateButton.Enabled = true;

					this.run = value;
					this.aborted = false;
					this.results.Run = value;

					if ( this.run != null )
					{
						this.run.Started += new EventHandler( run_Started );
						this.run.Finished += new EventHandler<FinishedEventArgs>( run_Finished );
						this.run.Log += new EventHandler<LogEventArgs>( run_Log );
						this.run.StatusChanged += new EventHandler( run_StatusChanged );
					}
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
