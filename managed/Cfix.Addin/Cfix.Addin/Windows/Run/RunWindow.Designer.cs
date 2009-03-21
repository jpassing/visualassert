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
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.stopButton = new System.Windows.Forms.ToolStripButton();
			this.terminateButton = new System.Windows.Forms.ToolStripButton();
			this.results = new Cfix.Control.Ui.Result.ResultExplorer();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.progressLabel = new Cfix.Addin.Windows.Run.TransparentLabel();
			this.toolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolbar
			// 
			this.toolbar.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.stopButton,
            this.terminateButton} );
			this.toolbar.Location = new System.Drawing.Point( 0, 0 );
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size( 558, 25 );
			this.toolbar.TabIndex = 0;
			this.toolbar.Text = "toolStrip1";
			// 
			// stopButton
			// 
			this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.stopButton.Enabled = false;
			this.stopButton.Image = global::Cfix.Addin.Icons.StopHS;
			this.stopButton.ImageTransparentColor = System.Drawing.Color.Black;
			this.stopButton.Name = "stopButton";
			this.stopButton.Size = new System.Drawing.Size( 23, 22 );
			this.stopButton.Text = "Stop testrun";
			this.stopButton.Click += new System.EventHandler( this.stopButton_Click );
			// 
			// terminateButton
			// 
			this.terminateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.terminateButton.Enabled = false;
			this.terminateButton.Image = global::Cfix.Addin.Icons.Delete;
			this.terminateButton.ImageTransparentColor = System.Drawing.Color.Black;
			this.terminateButton.Name = "terminateButton";
			this.terminateButton.Size = new System.Drawing.Size( 23, 22 );
			this.terminateButton.Text = "Terminate testrun (kills processes)";
			this.terminateButton.Click += new System.EventHandler( this.terminateButton_Click );
			// 
			// results
			// 
			this.results.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.results.Location = new System.Drawing.Point( 3, 64 );
			this.results.Name = "results";
			this.results.Run = null;
			this.results.Size = new System.Drawing.Size( 555, 201 );
			this.results.TabIndex = 1;
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
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolbar;
		private Cfix.Control.Ui.Result.ResultExplorer results;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.ToolStripButton stopButton;
		private System.Windows.Forms.ToolStripButton terminateButton;
		private TransparentLabel progressLabel;
	}
}
