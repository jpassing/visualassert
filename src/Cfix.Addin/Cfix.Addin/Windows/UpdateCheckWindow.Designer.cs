namespace Cfix.Addin.Windows
{
	partial class UpdateCheckWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( UpdateCheckWindow ) );
			this.headerGrpBox = new System.Windows.Forms.GroupBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.animation = new System.Windows.Forms.PictureBox();
			this.infoLabel = new System.Windows.Forms.Label();
			this.transparentLabel1 = new Cfix.Addin.Windows.TransparentLabel();
			this.headerGrpBox.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.pictureBox1 ) ).BeginInit();
			( ( System.ComponentModel.ISupportInitialize ) ( this.animation ) ).BeginInit();
			this.SuspendLayout();
			// 
			// headerGrpBox
			// 
			this.headerGrpBox.Controls.Add( this.transparentLabel1 );
			this.headerGrpBox.Controls.Add( this.pictureBox1 );
			this.headerGrpBox.Location = new System.Drawing.Point( 0, -8 );
			this.headerGrpBox.Name = "headerGrpBox";
			this.headerGrpBox.Size = new System.Drawing.Size( 500, 60 );
			this.headerGrpBox.TabIndex = 3;
			this.headerGrpBox.TabStop = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ( ( System.Drawing.Image ) ( resources.GetObject( "pictureBox1.Image" ) ) );
			this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.pictureBox1.Location = new System.Drawing.Point( 0, 0 );
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size( 495, 58 );
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// animation
			// 
			this.animation.Image = global::Cfix.Addin.Icons.update2_00;
			this.animation.Location = new System.Drawing.Point( 21, 58 );
			this.animation.Name = "animation";
			this.animation.Size = new System.Drawing.Size( 220, 45 );
			this.animation.TabIndex = 4;
			this.animation.TabStop = false;
			// 
			// infoLabel
			// 
			this.infoLabel.AutoSize = true;
			this.infoLabel.Location = new System.Drawing.Point( 18, 106 );
			this.infoLabel.Name = "infoLabel";
			this.infoLabel.Size = new System.Drawing.Size( 169, 13 );
			this.infoLabel.TabIndex = 5;
			this.infoLabel.Text = "Downloading version information...";
			// 
			// transparentLabel1
			// 
			this.transparentLabel1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( ( byte ) ( 0 ) ) );
			this.transparentLabel1.ForeColor = System.Drawing.Color.White;
			this.transparentLabel1.Location = new System.Drawing.Point( 21, 26 );
			this.transparentLabel1.Name = "transparentLabel1";
			this.transparentLabel1.Size = new System.Drawing.Size( 297, 23 );
			this.transparentLabel1.TabIndex = 3;
			this.transparentLabel1.TabStop = false;
			this.transparentLabel1.Text = "Checking for updates...";
			// 
			// UpdateCheckWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 271, 135 );
			this.ControlBox = false;
			this.Controls.Add( this.infoLabel );
			this.Controls.Add( this.animation );
			this.Controls.Add( this.headerGrpBox );
			this.Name = "UpdateCheckWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Update";
			this.headerGrpBox.ResumeLayout( false );
			( ( System.ComponentModel.ISupportInitialize ) ( this.pictureBox1 ) ).EndInit();
			( ( System.ComponentModel.ISupportInitialize ) ( this.animation ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox headerGrpBox;
		private TransparentLabel transparentLabel1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox animation;
		private System.Windows.Forms.Label infoLabel;

	}
}