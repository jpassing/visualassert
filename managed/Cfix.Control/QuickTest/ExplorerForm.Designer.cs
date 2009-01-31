namespace QuickTest
{
	partial class ExplorerForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.refresh = new System.Windows.Forms.Button();
			this.progressLabel = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer( this.components );
			this.explorer = new Cfix.Control.Ui.Explorer.Explorer();
			this.currentNodeLabel = new System.Windows.Forms.Label();
			this.abortBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// refresh
			// 
			this.refresh.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.refresh.Location = new System.Drawing.Point( 0, 234 );
			this.refresh.Name = "refresh";
			this.refresh.Size = new System.Drawing.Size( 75, 23 );
			this.refresh.TabIndex = 1;
			this.refresh.Text = "Refresh";
			this.refresh.UseVisualStyleBackColor = true;
			this.refresh.Click += new System.EventHandler( this.refresh_Click );
			// 
			// progressLabel
			// 
			this.progressLabel.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.progressLabel.AutoSize = true;
			this.progressLabel.Location = new System.Drawing.Point( 12, 262 );
			this.progressLabel.Name = "progressLabel";
			this.progressLabel.Size = new System.Drawing.Size( 139, 13 );
			this.progressLabel.TabIndex = 2;
			this.progressLabel.Text = "                                            ";
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler( this.timer1_Tick );
			// 
			// explorer
			// 
			this.explorer.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.explorer.Location = new System.Drawing.Point( 0, 0 );
			this.explorer.Name = "explorer";
			this.explorer.Size = new System.Drawing.Size( 292, 228 );
			this.explorer.TabIndex = 0;
			// 
			// currentNodeLabel
			// 
			this.currentNodeLabel.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.currentNodeLabel.AutoSize = true;
			this.currentNodeLabel.Location = new System.Drawing.Point( 141, 262 );
			this.currentNodeLabel.Name = "currentNodeLabel";
			this.currentNodeLabel.Size = new System.Drawing.Size( 139, 13 );
			this.currentNodeLabel.TabIndex = 2;
			this.currentNodeLabel.Text = "                                            ";
			// 
			// abortBtn
			// 
			this.abortBtn.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.abortBtn.Location = new System.Drawing.Point( 82, 235 );
			this.abortBtn.Name = "abortBtn";
			this.abortBtn.Size = new System.Drawing.Size( 75, 23 );
			this.abortBtn.TabIndex = 3;
			this.abortBtn.Text = "Abort";
			this.abortBtn.UseVisualStyleBackColor = true;
			this.abortBtn.Click += new System.EventHandler( this.abortBtn_Click );
			// 
			// ExplorerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 292, 284 );
			this.Controls.Add( this.abortBtn );
			this.Controls.Add( this.currentNodeLabel );
			this.Controls.Add( this.progressLabel );
			this.Controls.Add( this.refresh );
			this.Controls.Add( this.explorer );
			this.Name = "ExplorerForm";
			this.Text = "ExplorerForm";
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private Cfix.Control.Ui.Explorer.Explorer explorer;
		private System.Windows.Forms.Button refresh;
		private System.Windows.Forms.Label progressLabel;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label currentNodeLabel;
		private System.Windows.Forms.Button abortBtn;
	}
}