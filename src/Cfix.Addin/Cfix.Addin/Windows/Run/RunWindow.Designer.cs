namespace Cfix.Addin.Windows.Run
{
	partial class RunWindow
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( RunWindow ) );
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.terminateButton = new System.Windows.Forms.ToolStripButton();
			this.redebugButton = new System.Windows.Forms.ToolStripButton();
			this.restartButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.showLogButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.scrollLockButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.lameButton = new System.Windows.Forms.ToolStripButton();
			this.docButton = new System.Windows.Forms.ToolStripButton();
			this.progressBar = new Cfix.Addin.Windows.PlainProgressBar();
			this.resultCtxMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.ctxMenuDebugButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRunButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuViewCodeButton = new System.Windows.Forms.ToolStripMenuItem();
			this.results = new Cfix.Control.Ui.Result.ResultExplorer();
			this.stackTraceCtxMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.ctxMenuCopyTraceButton = new System.Windows.Forms.ToolStripMenuItem();
			this.progressLabel = new Cfix.Addin.Windows.TransparentLabel();
			this.toolbar.SuspendLayout();
			this.resultCtxMenu.SuspendLayout();
			this.stackTraceCtxMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolbar
			// 
			this.toolbar.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.terminateButton,
            this.redebugButton,
            this.restartButton,
            this.toolStripSeparator2,
            this.showLogButton,
            this.toolStripSeparator1,
            this.scrollLockButton,
            this.toolStripSeparator3,
            this.lameButton,
            this.docButton} );
			this.toolbar.Location = new System.Drawing.Point( 0, 0 );
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size( 558, 25 );
			this.toolbar.TabIndex = 0;
			this.toolbar.Text = "toolStrip1";
			// 
			// terminateButton
			// 
			this.terminateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.terminateButton.Enabled = false;
			this.terminateButton.Image = global::Cfix.Addin.Icons.StopHS;
			this.terminateButton.ImageTransparentColor = System.Drawing.Color.Black;
			this.terminateButton.Name = "terminateButton";
			this.terminateButton.Size = new System.Drawing.Size( 23, 22 );
			this.terminateButton.Text = "Terminate Run";
			this.terminateButton.Click += new System.EventHandler( this.terminateButton_Click );
			// 
			// redebugButton
			// 
			this.redebugButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.redebugButton.Enabled = false;
			this.redebugButton.Image = global::Cfix.Addin.Icons.Start2;
			this.redebugButton.ImageTransparentColor = System.Drawing.Color.Black;
			this.redebugButton.Name = "redebugButton";
			this.redebugButton.Size = new System.Drawing.Size( 23, 22 );
			this.redebugButton.Text = "Restart Last Run";
			this.redebugButton.Click += new System.EventHandler( this.redebugButton_Click );
			// 
			// restartButton
			// 
			this.restartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.restartButton.Enabled = false;
			this.restartButton.Image = global::Cfix.Addin.Icons.Ffwd;
			this.restartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.restartButton.Name = "restartButton";
			this.restartButton.Size = new System.Drawing.Size( 23, 22 );
			this.restartButton.Text = "Restart Last Run Without Debugging";
			this.restartButton.Click += new System.EventHandler( this.restartButton_Click );
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size( 6, 25 );
			// 
			// showLogButton
			// 
			this.showLogButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.showLogButton.Image = global::Cfix.Addin.Icons.Log;
			this.showLogButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.showLogButton.Name = "showLogButton";
			this.showLogButton.Size = new System.Drawing.Size( 23, 22 );
			this.showLogButton.Text = "Show Log";
			this.showLogButton.Click += new System.EventHandler( this.showLogButton_Click );
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size( 6, 25 );
			// 
			// scrollLockButton
			// 
			this.scrollLockButton.CheckOnClick = true;
			this.scrollLockButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.scrollLockButton.Image = ( ( System.Drawing.Image ) ( resources.GetObject( "scrollLockButton.Image" ) ) );
			this.scrollLockButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.scrollLockButton.Name = "scrollLockButton";
			this.scrollLockButton.Size = new System.Drawing.Size( 23, 22 );
			this.scrollLockButton.Text = "Scroll Lock";
			this.scrollLockButton.Click += new System.EventHandler( this.autoScrollButton_Click );
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size( 6, 25 );
			// 
			// lameButton
			// 
			this.lameButton.Image = global::Cfix.Addin.Icons.Feedback;
			this.lameButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.lameButton.Name = "lameButton";
			this.lameButton.Size = new System.Drawing.Size( 82, 22 );
			this.lameButton.Text = "Comments?";
			this.lameButton.ToolTipText = "Have comments about this window or some other feature? Let us now!";
			this.lameButton.Click += new System.EventHandler( this.lameButton_Click );
			// 
			// docButton
			// 
			this.docButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.docButton.Image = global::Cfix.Addin.Icons.Doc;
			this.docButton.ImageTransparentColor = System.Drawing.Color.Black;
			this.docButton.Name = "docButton";
			this.docButton.Size = new System.Drawing.Size( 23, 22 );
			this.docButton.Text = "Open Documentation";
			this.docButton.Click += new System.EventHandler( this.docButton_Click );
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.progressBar.BackColor = System.Drawing.Color.LightYellow;
			this.progressBar.Location = new System.Drawing.Point( 3, 28 );
			this.progressBar.Name = "progressBar";
			this.progressBar.ProgressBarColor = System.Drawing.Color.Blue;
			this.progressBar.Size = new System.Drawing.Size( 552, 20 );
			this.progressBar.TabIndex = 2;
			this.progressBar.Value = 0;
			// 
			// resultCtxMenu
			// 
			this.resultCtxMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.ctxMenuDebugButton,
            this.ctxMenuRunButton,
            this.ctxMenuViewCodeButton} );
			this.resultCtxMenu.Name = "resultCtxMenu";
			this.resultCtxMenu.Size = new System.Drawing.Size( 243, 92 );
			// 
			// ctxMenuDebugButton
			// 
			this.ctxMenuDebugButton.Image = global::Cfix.Addin.Icons.Start;
			this.ctxMenuDebugButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ctxMenuDebugButton.Name = "ctxMenuDebugButton";
			this.ctxMenuDebugButton.Size = new System.Drawing.Size( 242, 22 );
			this.ctxMenuDebugButton.Text = "Debug Test Again";
			this.ctxMenuDebugButton.Click += new System.EventHandler( this.ctxMenuDebugButton_Click );
			// 
			// ctxMenuRunButton
			// 
			this.ctxMenuRunButton.Image = global::Cfix.Addin.Icons.Ffwd;
			this.ctxMenuRunButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ctxMenuRunButton.Name = "ctxMenuRunButton";
			this.ctxMenuRunButton.Size = new System.Drawing.Size( 242, 22 );
			this.ctxMenuRunButton.Text = "Run Test Again Without Debugging";
			this.ctxMenuRunButton.Click += new System.EventHandler( this.ctxMenuRunButton_Click );
			// 
			// ctxMenuViewCodeButton
			// 
			this.ctxMenuViewCodeButton.Image = global::Cfix.Addin.Icons.Code;
			this.ctxMenuViewCodeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ctxMenuViewCodeButton.Name = "ctxMenuViewCodeButton";
			this.ctxMenuViewCodeButton.Size = new System.Drawing.Size( 242, 22 );
			this.ctxMenuViewCodeButton.Text = "View Code";
			this.ctxMenuViewCodeButton.Click += new System.EventHandler( this.ctxMenuViewCodeButton_Click );
			// 
			// results
			// 
			this.results.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.results.AutoScrollToActiveNode = false;
			this.results.FailureNodeContextMenu = null;
			this.results.Location = new System.Drawing.Point( 3, 64 );
			this.results.Name = "results";
			this.results.ResultNodeContextMenu = null;
			this.results.Run = null;
			this.results.Size = new System.Drawing.Size( 555, 201 );
			this.results.TabIndex = 1;
			// 
			// stackTraceCtxMenu
			// 
			this.stackTraceCtxMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.ctxMenuCopyTraceButton} );
			this.stackTraceCtxMenu.Name = "stackTraceCtxMenu";
			this.stackTraceCtxMenu.Size = new System.Drawing.Size( 156, 26 );
			// 
			// ctxMenuCopyTraceButton
			// 
			this.ctxMenuCopyTraceButton.Name = "ctxMenuCopyTraceButton";
			this.ctxMenuCopyTraceButton.Size = new System.Drawing.Size( 155, 22 );
			this.ctxMenuCopyTraceButton.Text = "Copy stack trace";
			this.ctxMenuCopyTraceButton.Click += new System.EventHandler( this.ctxMenuCopyTraceButton_Click );
			// 
			// progressLabel
			// 
			this.progressLabel.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.progressLabel.Location = new System.Drawing.Point( 5, 31 );
			this.progressLabel.Name = "progressLabel";
			this.progressLabel.Size = new System.Drawing.Size( 542, 23 );
			this.progressLabel.TabIndex = 3;
			this.progressLabel.TabStop = false;
			// 
			// RunWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.progressLabel );
			this.Controls.Add( this.progressBar );
			this.Controls.Add( this.results );
			this.Controls.Add( this.toolbar );
			this.Name = "RunWindow";
			this.Size = new System.Drawing.Size( 558, 268 );
			this.toolbar.ResumeLayout( false );
			this.toolbar.PerformLayout();
			this.resultCtxMenu.ResumeLayout( false );
			this.stackTraceCtxMenu.ResumeLayout( false );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolbar;
		private Cfix.Control.Ui.Result.ResultExplorer results;
		private Cfix.Addin.Windows.PlainProgressBar progressBar;
		private System.Windows.Forms.ToolStripButton terminateButton;
		private TransparentLabel progressLabel;
		private System.Windows.Forms.ContextMenuStrip resultCtxMenu;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuDebugButton;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRunButton;
		private System.Windows.Forms.ContextMenuStrip stackTraceCtxMenu;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuCopyTraceButton;
		private System.Windows.Forms.ToolStripButton showLogButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton redebugButton;
		private System.Windows.Forms.ToolStripButton restartButton;
		private System.Windows.Forms.ToolStripButton scrollLockButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripButton lameButton;
		private System.Windows.Forms.ToolStripButton docButton;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuViewCodeButton;
	}
}
