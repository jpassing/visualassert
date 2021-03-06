/*----------------------------------------------------------------------
 * Purpose:
 *		Run window.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.Native;
using Cfix.Control.Ui.Result;
using Cfix.Addin.Windows;
using EnvDTE;
using EnvDTE80;
using Cfix.Addin.IntelParallelStudio;

namespace Cfix.Addin.Windows
{
	public partial class RunWindow : UserControl
	{
		public static readonly Guid Guid = 
			new Guid( "1e1dc7d9-818b-422c-8d16-92c4655c071c" );

		private static readonly Color FailedColor = Color.Tomato;
		private static readonly Color SuccessColor = Color.PaleGreen;
		private static readonly Color DefaultColor = SystemColors.Control;
		private static readonly Color InitialColor = Color.LightYellow;
		private static readonly Color PostprocessingFailedColor = Color.DarkOrange;
		private static readonly Color InitializingColor = Color.Gray;

		private readonly object runLock = new object();
		
		//
		// Current run, guarded by runLock.
		//
		private IRun run;
		private bool aborted;

		private IResultNode contextMenuReferenceItem;

		private DTE2 dte;
		private Window window;
		private Workspace workspace;

		private SolutionEvents solutionEvents;
		
		private static string GetStatusText( ExecutionStatus status )
		{
			string text = Strings.ResourceManager.GetString( status.ToString() );
			if ( text == null )
			{
				text = status.ToString();
			}

			return text;
		}

		private uint anticipatedInitializationTimeTicks;
		private uint elapsedInitializationTimeTicks;

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
					}
					else
					{
						this.progressBar.Value = 
							( int ) ( completed * 100 / total );
					}

					if ( this.progressBar.ProgressBarColor == InitializingColor )
					{
						this.progressBar.ProgressBarColor = SuccessColor;
					}

					if ( this.progressBar.ProgressBarColor == SuccessColor )
					{
						if ( item.Status == ExecutionStatus.Failed )
						{
							this.progressBar.ProgressBarColor = FailedColor;
						}
						else if ( item.Status == ExecutionStatus.PostprocessingFailed )
						{
							this.progressBar.ProgressBarColor = PostprocessingFailedColor;
						}
					}
					
					this.progressLabel.Text =
						String.Format(
							Strings.ProgressInfo,
							completed,
							total,
							this.run.TaskCount,
							this.run.Status.ToString(),
							this.run.ItemsFailed,
							this.run.ItemsInconclusive,
							this.run.ItemsSucceeded );
					this.progressBar.Invalidate();
					this.progressLabel.Invalidate();
				} );
		}

		private void run_Log( object sender, LogEventArgs e )
		{
			IResultItem item = ( IResultItem ) sender;
			this.workspace.ToolWindows.LogWindow.OutputString(
				String.Format( "[{0}] {1}\n",
					item.Name,
					e.Message ) );
		}

		private void run_Finished( object sender, FinishedEventArgs e )
		{
			this.BeginInvoke( ( VoidDelegate ) delegate
			{
				this.terminateButton.Enabled = false;
				this.redebugButton.Enabled = true;
				this.restartButton.Enabled = true;

				this.initTimer.Stop();

				if ( this.run.Status == TaskStatus.Suceeded )
				{
					//
					// Run succeeded, but may have produced failures.
					//
					Debug.Assert(
						this.run.ItemsCompleted >=
						this.run.ItemsSucceeded +
						this.run.ItemsFailed +
						this.run.ItemsInconclusive );

					this.progressLabel.Text =
						String.Format(
							Strings.ProgressInfoFinish,
							GetStatusText( this.run.RootResult.Status ),
							this.run.ItemsFailed,
							this.run.ItemsInconclusive,
							this.run.ItemsSucceeded );
				}
				else
				{
					//
					// Run failed.
					//
					this.progressLabel.Text = this.run.Status.ToString();
					this.progressBar.ProgressBarColor = FailedColor;
				}

				this.progressBar.Value = 100;
				this.progressBar.Invalidate();
				this.progressLabel.Invalidate();

				this.showFailuresOnlyButton.Enabled = true;

				Exception excp = e.Exception;
				if ( excp != null )
				{
					VisualAssert.HandleError( excp );
				}
			} );
		}

		private void run_Started( object sender, EventArgs e )
		{
			this.BeginInvoke( ( VoidDelegate ) delegate
			{
				this.showFailuresOnlyButton.Checked = false;
				this.showFailuresOnlyButton.Enabled = false;

				if ( this.run.InvolvesPostprocessing )
				{
					this.progressBar.ProgressBarColor = InitializingColor;
					this.progressLabel.Text = Strings.InitializingRun;
				}
				else
				{
					this.progressLabel.Text =
						String.Format(
							Strings.ProgressInfo,
							0,
							this.run.ItemCount,
							this.run.TaskCount,
							this.run.Status.ToString(),
							0,
							0,
							0 );

					this.progressBar.ProgressBarColor = SuccessColor;
				}

				this.progressBar.Value = 0;
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
					if ( this.run != null )
					{
						this.run.Terminate();
					}
				}
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void showLogButton_Click( object sender, EventArgs e )
		{
			try
			{
				this.dte.ToolWindows.OutputWindow.Parent.Activate();
				this.workspace.ToolWindows.LogWindow.Activate();
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
			}
		}

		private void results_SelectionChanged( object sender, EventArgs e )
		{
			CommonUiOperations.SetActiveSelectionItem( 
                this.window, 
                this.results.SelectedItem );
		}

		/*----------------------------------------------------------------------
		 * Context Menu.
		 */

		private void results_ContextMenuRequested(
			object sender, 
			ContextMenuEventArgs e
			)
		{
			bool running = ( this.run.Status == TaskStatus.Running );

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
					this.ctxMenuDebugButton.Enabled = !running;
					this.ctxMenuRunButton.Enabled = !running;

					this.ctxMenuViewCodeButton.Visible = resultItem.ResultItem.Item is ITestCodeElement;

#if INTELINSPECTOR
					object associatedObj = resultItem.ResultItem.Object;
					this.ctxMenuViewInspectorResult.Visible =
						associatedObj != null && associatedObj is ResultLocation;
#endif

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
				VisualAssert.HandleError( x );
			}
		}

		private void ctxMenuCopyTraceButton_Click( object sender, EventArgs e )
		{
			try
			{
				FailureNode failNode = this.contextMenuReferenceItem as FailureNode;
				if ( failNode != null && 
					 failNode.StackTrace != null &&
					 failNode.StackTrace.FrameCount > 0 )
				{
					Clipboard.SetText( failNode.StackTrace.ToString() );
				}
			}
			catch ( Exception x )
			{
				VisualAssert.HandleError( x );
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
					RunMode.Debug );
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
					RunMode.Normal );
			}
		}

		private void ctxMenuViewCodeButton_Click( object sender, EventArgs e )
		{
			GoTo( this.contextMenuReferenceItem );
		}
		
		private void ctxMenuViewInspectorResult_Click( object sender, EventArgs e )
		{
			ResultItemNode resultItemNode =
				this.contextMenuReferenceItem as ResultItemNode;
			if ( resultItemNode == null )
			{
				return;
			}

#if INTELINSPECTOR
			ResultLocation resultLocation = 
				resultItemNode.ResultItem.Object as ResultLocation;
			if ( resultLocation != null )
			{
				this.dte.ItemOperations.OpenFile(
					resultLocation.ResultFile,
					Constants.vsViewKindAny );
			}
#endif
		}

		private void GoTo( object node )
		{
			ISourceReference src = node as ISourceReference;
			if ( src != null )
			{
				if ( src.File != null )
				{
					CommonUiOperations.GoToSource(
						this.dte,
						src.File,
						src.Line );
					return;
				}
			}

			ResultItemNode resNode = node as ResultItemNode;
			if ( resNode != null )
			{
				CommonUiOperations.GoToTestItem( this.dte, resNode.ResultItem.Item );
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
							e.Shift ? RunMode.Normal : RunMode.Debug );
					}
				}
				else
				{
					GoTo( sender );
				}

				e.Handled = true;
			}
		}

		private void results_TreeDoubleClick( object sender, MouseEventArgs e )
		{
			GoTo( sender );
		}

		/*----------------------------------------------------------------------
		 * Misc events.
		 */

		private void RunWindow_Disposed( object sender, EventArgs e )
		{
			this.solutionEvents.BeforeClosing -= new _dispSolutionEvents_BeforeClosingEventHandler( solutionEvents_BeforeClosing );
		}

		private void solutionEvents_BeforeClosing()
		{
			//
			// Clear results.
			//
			this.results.Run = null;
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */
		
		public RunWindow()
		{
			InitializeComponent();

#if VS100
            this.BackColor = Chrome.WindowBackColor;
            this.toolbar.BackColor = Chrome.WindowBackColor;
#endif

			this.results.ContextMenuRequested += new EventHandler<Cfix.Control.Ui.Result.ContextMenuEventArgs>(results_ContextMenuRequested);
			this.results.TreeKeyDown += new KeyEventHandler( results_TreeKeyDown );
			this.results.TreeDoubleClick += new MouseEventHandler( results_TreeDoubleClick );

			this.Disposed += new EventHandler( RunWindow_Disposed );


#if !BETA
			this.lameButton.Visible = false;
#endif
		}

		public void Initialize(
			Workspace ws,
			DTE2 dte,
			Window window )
		{
			this.workspace = ws;
			this.dte = dte;
			this.window = window;

			this.solutionEvents = dte.Events.SolutionEvents;

			this.solutionEvents.BeforeClosing += new _dispSolutionEvents_BeforeClosingEventHandler( solutionEvents_BeforeClosing );

			this.scrollLockButton.Checked = ws.Configuration.ResultsScrollLock;

			this.results.AutoScrollToActiveNode = ! ws.Configuration.ResultsScrollLock;
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
							// vs. UI updates being delivered via Invoke to
							// this (UI) thread.
							//
							// Ask whether to terminate.
							//
							if ( VisualAssert.ShowQuestion( Strings.TerminateActiveRun ) )
							{
								this.aborted = true;
								this.run.Terminate();

								try
								{
									this.dte.Debugger.Stop( false );
								}
								catch
								{ }
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

					this.progressBar.ProgressBarColor = SuccessColor;

					this.terminateButton.Enabled = true;
					this.restartButton.Enabled = false;
					this.redebugButton.Enabled = false;

					this.elapsedInitializationTimeTicks = 0;
					this.anticipatedInitializationTimeTicks = 40;

					this.run = value;
					this.aborted = false;
					this.results.Run = value;

					if ( this.run != null )
					{
						this.run.Started += new EventHandler( run_Started );
						this.run.Finished += new EventHandler<FinishedEventArgs>( run_Finished );
						this.run.Log += new EventHandler<LogEventArgs>( run_Log );
						this.run.StatusChanged += new EventHandler( run_StatusChanged );

						if ( this.run.InvolvesPostprocessing )
						{
							this.initTimer.Start();
						}
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

		private void redebugButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.RunItem( this.workspace, null, RunMode.Debug );
		}

		private void restartButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.RunItem( this.workspace, null, RunMode.Normal );
		}

		private void autoScrollButton_Click( object sender, EventArgs e )
		{
			this.results.AutoScrollToActiveNode = !this.scrollLockButton.Checked;
			this.workspace.Configuration.ResultsScrollLock = this.scrollLockButton.Checked;
		}

		private void lameButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.OpenLameWebpage( this.dte, "Run" );
		}

		private void docButton_Click( object sender, EventArgs e )
		{
			CommonUiOperations.OpenDocumentation();
		}

		private void ctxMenuViewProperties_Click( object sender, EventArgs e )
		{
			CommonUiOperations.ActivatePropertyWindow( this.dte );
		}

		private void showFailuresOnlyButton_Click( object sender, EventArgs e )
		{
			if ( this.showFailuresOnlyButton.Checked )
			{
				this.results.Filter = ResultNodeFilter.FailureNodes;
				this.results.ExpandAll();
			}
			else
			{
				this.results.Filter = ResultNodeFilter.All;
			}
		}

		private void selectNextFailureButton_Click( object sender, EventArgs e )
		{
			this.results.HighlightNextFailure( false );
		}

		private void selectPrevFailureButton_Click( object sender, EventArgs e )
		{
			this.results.HighlightNextFailure( true );
		}

		private void initTimer_Tick( object sender, EventArgs e )
		{
			if ( this.elapsedInitializationTimeTicks < 
				this.anticipatedInitializationTimeTicks )
			{
				this.elapsedInitializationTimeTicks++;

				uint fraction = this.elapsedInitializationTimeTicks * 100 /
					this.anticipatedInitializationTimeTicks;
				Debug.Assert( fraction <= 100 );
				Debug.Assert( fraction >= 0 );

				//
				// Use the 90% trick.
				//
				fraction = Math.Min( fraction, 90 );

				this.progressBar.Value = ( int ) fraction;
				this.progressBar.Invalidate();
				this.progressLabel.Invalidate();
			}
		}

	}
}
