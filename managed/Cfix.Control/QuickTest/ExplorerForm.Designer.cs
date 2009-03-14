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
			Cfix.Control.Ui.Explorer.NodeFactory nodeFactory2 = new Cfix.Control.Ui.Explorer.NodeFactory();
			this.refresh = new System.Windows.Forms.Button();
			this.progressLabel = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer( this.components );
			this.explorer = new Cfix.Control.Ui.Explorer.TestExplorer();
			this.currentNodeLabel = new System.Windows.Forms.Label();
			this.abortBtn = new System.Windows.Forms.Button();
			this.Results = new Cfix.Control.Ui.Result.ResultExplorer();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.statusTxt = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// refresh
			// 
			this.refresh.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.refresh.Location = new System.Drawing.Point( 0, 483 );
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
			this.progressLabel.Location = new System.Drawing.Point( 12, 511 );
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
			this.explorer.NodeContextMenu = null;
			this.explorer.NodeFactory = nodeFactory2;
			this.explorer.Size = new System.Drawing.Size( 808, 221 );
			this.explorer.TabIndex = 0;
			// 
			// currentNodeLabel
			// 
			this.currentNodeLabel.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.currentNodeLabel.AutoSize = true;
			this.currentNodeLabel.Location = new System.Drawing.Point( 141, 511 );
			this.currentNodeLabel.Name = "currentNodeLabel";
			this.currentNodeLabel.Size = new System.Drawing.Size( 139, 13 );
			this.currentNodeLabel.TabIndex = 2;
			this.currentNodeLabel.Text = "                                            ";
			// 
			// abortBtn
			// 
			this.abortBtn.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.abortBtn.Location = new System.Drawing.Point( 82, 484 );
			this.abortBtn.Name = "abortBtn";
			this.abortBtn.Size = new System.Drawing.Size( 75, 23 );
			this.abortBtn.TabIndex = 3;
			this.abortBtn.Text = "Abort";
			this.abortBtn.UseVisualStyleBackColor = true;
			this.abortBtn.Click += new System.EventHandler( this.abortBtn_Click );
			// 
			// Results
			// 
			this.Results.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.Results.Location = new System.Drawing.Point( 0, 227 );
			this.Results.Name = "Results";
			this.Results.Run = null;
			this.Results.Size = new System.Drawing.Size( 808, 251 );
			this.Results.TabIndex = 4;
			// 
			// button1
			// 
			this.button1.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.button1.Location = new System.Drawing.Point( 204, 482 );
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size( 75, 23 );
			this.button1.TabIndex = 5;
			this.button1.Text = "Load 1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler( this.button1_Click );
			// 
			// button2
			// 
			this.button2.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.button2.Location = new System.Drawing.Point( 286, 482 );
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size( 75, 23 );
			this.button2.TabIndex = 6;
			this.button2.Text = "Load 2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler( this.button2_Click );
			// 
			// button3
			// 
			this.button3.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.button3.Location = new System.Drawing.Point( 368, 484 );
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size( 75, 23 );
			this.button3.TabIndex = 7;
			this.button3.Text = "Load ctl";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler( this.button3_Click );
			// 
			// statusTxt
			// 
			this.statusTxt.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.statusTxt.Location = new System.Drawing.Point( 665, 485 );
			this.statusTxt.Name = "statusTxt";
			this.statusTxt.ReadOnly = true;
			this.statusTxt.Size = new System.Drawing.Size( 134, 20 );
			this.statusTxt.TabIndex = 8;
			// 
			// ExplorerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 811, 533 );
			this.Controls.Add( this.statusTxt );
			this.Controls.Add( this.button3 );
			this.Controls.Add( this.button2 );
			this.Controls.Add( this.button1 );
			this.Controls.Add( this.Results );
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

		private Cfix.Control.Ui.Explorer.TestExplorer explorer;
		private System.Windows.Forms.Button refresh;
		private System.Windows.Forms.Label progressLabel;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label currentNodeLabel;
		private System.Windows.Forms.Button abortBtn;
		public Cfix.Control.Ui.Result.ResultExplorer Results;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.TextBox statusTxt;
	}
}