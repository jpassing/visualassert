namespace Cfix.LicAdmin
{
	partial class ThankYouDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ThankYouDialog ) );
			this.headerGrpBox = new System.Windows.Forms.GroupBox();
			this.headerLabel = new Cfix.LicAdmin.TransparentLabel();
			this.headerPicture = new System.Windows.Forms.PictureBox();
			this.thanksLabel = new System.Windows.Forms.Label();
			this.closeButton = new System.Windows.Forms.Button();
			this.headerGrpBox.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.headerPicture ) ).BeginInit();
			this.SuspendLayout();
			// 
			// headerGrpBox
			// 
			this.headerGrpBox.Controls.Add( this.headerLabel );
			this.headerGrpBox.Controls.Add( this.headerPicture );
			resources.ApplyResources( this.headerGrpBox, "headerGrpBox" );
			this.headerGrpBox.Name = "headerGrpBox";
			this.headerGrpBox.TabStop = false;
			// 
			// headerLabel
			// 
			resources.ApplyResources( this.headerLabel, "headerLabel" );
			this.headerLabel.ForeColor = System.Drawing.Color.White;
			this.headerLabel.Name = "headerLabel";
			this.headerLabel.TabStop = false;
			// 
			// headerPicture
			// 
			resources.ApplyResources( this.headerPicture, "headerPicture" );
			this.headerPicture.Name = "headerPicture";
			this.headerPicture.TabStop = false;
			// 
			// thanksLabel
			// 
			resources.ApplyResources( this.thanksLabel, "thanksLabel" );
			this.thanksLabel.Name = "thanksLabel";
			// 
			// closeButton
			// 
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources( this.closeButton, "closeButton" );
			this.closeButton.Name = "closeButton";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler( this.closeButton_Click );
			// 
			// ThankYouDialog
			// 
			this.AcceptButton = this.closeButton;
			resources.ApplyResources( this, "$this" );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.closeButton;
			this.Controls.Add( this.closeButton );
			this.Controls.Add( this.thanksLabel );
			this.Controls.Add( this.headerGrpBox );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ThankYouDialog";
			this.headerGrpBox.ResumeLayout( false );
			( ( System.ComponentModel.ISupportInitialize ) ( this.headerPicture ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox headerGrpBox;
		private System.Windows.Forms.PictureBox headerPicture;
		private System.Windows.Forms.Label thanksLabel;
		private TransparentLabel headerLabel;
		private System.Windows.Forms.Button closeButton;
	}
}

