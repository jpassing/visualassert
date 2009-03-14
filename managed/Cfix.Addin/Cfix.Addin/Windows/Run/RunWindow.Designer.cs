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
			this.results = new Cfix.Control.Ui.Result.ResultExplorer();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// toolbar
			// 
			this.toolbar.Location = new System.Drawing.Point( 0, 0 );
			this.toolbar.Name = "toolbar";
			this.toolbar.Size = new System.Drawing.Size( 558, 25 );
			this.toolbar.TabIndex = 0;
			this.toolbar.Text = "toolStrip1";
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
			this.progressBar.Location = new System.Drawing.Point( 3, 48 );
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size( 552, 10 );
			this.progressBar.TabIndex = 2;
			this.progressBar.Value = 20;
			// 
			// RunWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.progressBar );
			this.Controls.Add( this.results );
			this.Controls.Add( this.toolbar );
			this.Name = "RunWindow";
			this.Size = new System.Drawing.Size( 558, 268 );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolbar;
		private Cfix.Control.Ui.Result.ResultExplorer results;
		private System.Windows.Forms.ProgressBar progressBar;
	}
}
