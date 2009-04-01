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
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.terminateButton = new System.Windows.Forms.ToolStripButton();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.progressLabel = new Cfix.Addin.Windows.Run.TransparentLabel();
			this.resultCtxMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.ctxMenuDebugButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRunButton = new System.Windows.Forms.ToolStripMenuItem();
			this.results = new Cfix.Control.Ui.Result.ResultExplorer();
			this.toolbar.SuspendLayout();
			this.resultCtxMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolbar
			// 
			this.toolbar.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.terminateButton} );
			this.toolbar.Location = new System.Drawing.Point( 0, 0 );
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size( 558, 25 );
			this.toolbar.TabIndex = 0;
			this.toolbar.Text = "toolStrip1";
			// 
			// terminateButton
			// 
			this.terminateButton.Enabled = false;
			this.terminateButton.Image = global::Cfix.Addin.Icons.StopHS;
			this.terminateButton.ImageTransparentColor = System.Drawing.Color.Black;
			this.terminateButton.Name = "terminateButton";
			this.terminateButton.Size = new System.Drawing.Size( 97, 22 );
			this.terminateButton.Text = "Terminate Run";
			this.terminateButton.Click += new System.EventHandler( this.terminateButton_Click );
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.progressBar.BackColor = System.Drawing.Color.LightYellow;
			this.progressBar.Location = new System.Drawing.Point( 3, 28 );
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size( 552, 20 );
			this.progressBar.TabIndex = 2;
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
			// resultCtxMenu
			// 
			this.resultCtxMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.ctxMenuDebugButton,
            this.ctxMenuRunButton} );
			this.resultCtxMenu.Name = "resultCtxMenu";
			this.resultCtxMenu.Size = new System.Drawing.Size( 243, 70 );
			// 
			// ctxMenuDebugButton
			// 
			this.ctxMenuDebugButton.Image = global::Cfix.Addin.Icons.Start;
			this.ctxMenuDebugButton.ImageTransparentColor = System.Drawing.Color.Black;
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
			// results
			// 
			this.results.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.results.FailureNodeContextMenu = null;
			this.results.Location = new System.Drawing.Point( 3, 64 );
			this.results.Name = "results";
			this.results.ResultNodeContextMenu = null;
			this.results.Run = null;
			this.results.Size = new System.Drawing.Size( 555, 201 );
			this.results.TabIndex = 1;
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
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolbar;
		private Cfix.Control.Ui.Result.ResultExplorer results;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.ToolStripButton terminateButton;
		private TransparentLabel progressLabel;
		private System.Windows.Forms.ContextMenuStrip resultCtxMenu;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuDebugButton;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRunButton;
	}
}
