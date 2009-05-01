namespace Cfix.LicAdmin
{
	partial class LicenseDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( LicenseDialog ) );
			this.headerGrpBox = new System.Windows.Forms.GroupBox();
			this.headerLabel = new Cfix.LicAdmin.TransparentLabel();
			this.headerPicture = new System.Windows.Forms.PictureBox();
			this.licenseKeyTextBox = new System.Windows.Forms.TextBox();
			this.licenseKeyLabel = new System.Windows.Forms.Label();
			this.footerGrpBox = new System.Windows.Forms.GroupBox();
			this.okButton = new Cfix.LicAdmin.ElevatedButton();
			this.purchaseButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.infoIcon = new Cfix.LicAdmin.TransparentControl();
			this.infoLabel = new Cfix.LicAdmin.TransparentLabel();
			this.elevationInfoLabel = new System.Windows.Forms.Label();
			this.headerGrpBox.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.headerPicture ) ).BeginInit();
			this.footerGrpBox.SuspendLayout();
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
			// licenseKeyTextBox
			// 
			resources.ApplyResources( this.licenseKeyTextBox, "licenseKeyTextBox" );
			this.licenseKeyTextBox.Name = "licenseKeyTextBox";
			this.licenseKeyTextBox.TextChanged += new System.EventHandler( this.licenseKeyTextBox_TextChanged );
			// 
			// licenseKeyLabel
			// 
			resources.ApplyResources( this.licenseKeyLabel, "licenseKeyLabel" );
			this.licenseKeyLabel.Name = "licenseKeyLabel";
			// 
			// footerGrpBox
			// 
			this.footerGrpBox.Controls.Add( this.okButton );
			this.footerGrpBox.Controls.Add( this.purchaseButton );
			this.footerGrpBox.Controls.Add( this.cancelButton );
			resources.ApplyResources( this.footerGrpBox, "footerGrpBox" );
			this.footerGrpBox.Name = "footerGrpBox";
			this.footerGrpBox.TabStop = false;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources( this.okButton, "okButton" );
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler( this.okButton_Click );
			// 
			// purchaseButton
			// 
			resources.ApplyResources( this.purchaseButton, "purchaseButton" );
			this.purchaseButton.Name = "purchaseButton";
			this.purchaseButton.UseVisualStyleBackColor = true;
			this.purchaseButton.Click += new System.EventHandler( this.purchaseButton_Click );
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources( this.cancelButton, "cancelButton" );
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler( this.cancelButton_Click );
			// 
			// infoIcon
			// 
			this.infoIcon.BackColor = System.Drawing.Color.Transparent;
			this.infoIcon.Image = ( ( System.Drawing.Image ) ( resources.GetObject( "infoIcon.Image" ) ) );
			resources.ApplyResources( this.infoIcon, "infoIcon" );
			this.infoIcon.Name = "infoIcon";
			// 
			// infoLabel
			// 
			resources.ApplyResources( this.infoLabel, "infoLabel" );
			this.infoLabel.Name = "infoLabel";
			this.infoLabel.TabStop = false;
			// 
			// elevationInfoLabel
			// 
			resources.ApplyResources( this.elevationInfoLabel, "elevationInfoLabel" );
			this.elevationInfoLabel.Name = "elevationInfoLabel";
			// 
			// LicenseDialog
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources( this, "$this" );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.Controls.Add( this.infoIcon );
			this.Controls.Add( this.infoLabel );
			this.Controls.Add( this.footerGrpBox );
			this.Controls.Add( this.elevationInfoLabel );
			this.Controls.Add( this.licenseKeyLabel );
			this.Controls.Add( this.licenseKeyTextBox );
			this.Controls.Add( this.headerGrpBox );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LicenseDialog";
			this.headerGrpBox.ResumeLayout( false );
			( ( System.ComponentModel.ISupportInitialize ) ( this.headerPicture ) ).EndInit();
			this.footerGrpBox.ResumeLayout( false );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox headerGrpBox;
		private System.Windows.Forms.PictureBox headerPicture;
		private Cfix.LicAdmin.TransparentLabel headerLabel;
		private System.Windows.Forms.TextBox licenseKeyTextBox;
		private System.Windows.Forms.Label licenseKeyLabel;
		private System.Windows.Forms.GroupBox footerGrpBox;
		private Cfix.LicAdmin.ElevatedButton okButton;
		private System.Windows.Forms.Button purchaseButton;
		private System.Windows.Forms.Button cancelButton;
		private Cfix.LicAdmin.TransparentLabel infoLabel;
		private TransparentControl infoIcon;
		private System.Windows.Forms.Label elevationInfoLabel;
	}
}

