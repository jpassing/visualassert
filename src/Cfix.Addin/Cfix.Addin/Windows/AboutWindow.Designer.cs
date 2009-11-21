namespace Cfix.Addin.Windows
{
	partial class AboutWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( AboutWindow ) );
			this.okButton = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.fileVersionsList = new System.Windows.Forms.ListView();
			this.fileCol = new System.Windows.Forms.ColumnHeader();
			this.archCol = new System.Windows.Forms.ColumnHeader();
			this.versionCol = new System.Windows.Forms.ColumnHeader();
			this.linkLabel = new System.Windows.Forms.LinkLabel();
			this.licenseValueLabel = new Cfix.Addin.Windows.TransparentLabel();
			this.licenseLabel = new Cfix.Addin.Windows.TransparentLabel();
			this.fileVersionsLabel = new Cfix.Addin.Windows.TransparentLabel();
			this.copyrightLabel2 = new Cfix.Addin.Windows.TransparentLabel();
			this.copyrightLabel = new Cfix.Addin.Windows.TransparentLabel();
			this.versionLabel = new Cfix.Addin.Windows.TransparentLabel();
			this.enterLicenseButton = new System.Windows.Forms.Button();
			( ( System.ComponentModel.ISupportInitialize ) ( this.pictureBox1 ) ).BeginInit();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.okButton.Location = new System.Drawing.Point( 387, 312 );
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size( 75, 23 );
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.pictureBox1.Image = ( ( System.Drawing.Image ) ( resources.GetObject( "pictureBox1.Image" ) ) );
			this.pictureBox1.Location = new System.Drawing.Point( 0, 0 );
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size( 493, 348 );
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// fileVersionsList
			// 
			this.fileVersionsList.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.fileCol,
            this.archCol,
            this.versionCol} );
			this.fileVersionsList.FullRowSelect = true;
			this.fileVersionsList.Location = new System.Drawing.Point( 24, 105 );
			this.fileVersionsList.MultiSelect = false;
			this.fileVersionsList.Name = "fileVersionsList";
			this.fileVersionsList.ShowGroups = false;
			this.fileVersionsList.Size = new System.Drawing.Size( 438, 98 );
			this.fileVersionsList.TabIndex = 3;
			this.fileVersionsList.UseCompatibleStateImageBehavior = false;
			this.fileVersionsList.View = System.Windows.Forms.View.Details;
			// 
			// fileCol
			// 
			this.fileCol.Text = "File";
			this.fileCol.Width = 180;
			// 
			// archCol
			// 
			this.archCol.Text = "Architecture";
			this.archCol.Width = 80;
			// 
			// versionCol
			// 
			this.versionCol.Text = "Version";
			this.versionCol.Width = 150;
			// 
			// linkLabel
			// 
			this.linkLabel.ActiveLinkColor = System.Drawing.Color.Green;
			this.linkLabel.AutoSize = true;
			this.linkLabel.BackColor = System.Drawing.Color.FromArgb( ( ( int ) ( ( ( byte ) ( 0 ) ) ) ), ( ( int ) ( ( ( byte ) ( 3 ) ) ) ), ( ( int ) ( ( ( byte ) ( 24 ) ) ) ) );
			this.linkLabel.LinkColor = System.Drawing.Color.Lime;
			this.linkLabel.Location = new System.Drawing.Point( 322, 27 );
			this.linkLabel.Name = "linkLabel";
			this.linkLabel.Size = new System.Drawing.Size( 140, 13 );
			this.linkLabel.TabIndex = 4;
			this.linkLabel.TabStop = true;
			this.linkLabel.Text = "http://www.visualassert.com/";
			this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.linkLabel_LinkClicked );
			// 
			// licenseValueLabel
			// 
			this.licenseValueLabel.ForeColor = System.Drawing.Color.White;
			this.licenseValueLabel.Location = new System.Drawing.Point( 107, 56 );
			this.licenseValueLabel.Name = "licenseValueLabel";
			this.licenseValueLabel.Size = new System.Drawing.Size( 241, 23 );
			this.licenseValueLabel.TabIndex = 2;
			this.licenseValueLabel.TabStop = false;
			this.licenseValueLabel.Text = " ";
			// 
			// licenseLabel
			// 
			this.licenseLabel.ForeColor = System.Drawing.Color.White;
			this.licenseLabel.Location = new System.Drawing.Point( 24, 56 );
			this.licenseLabel.Name = "licenseLabel";
			this.licenseLabel.Size = new System.Drawing.Size( 77, 23 );
			this.licenseLabel.TabIndex = 2;
			this.licenseLabel.TabStop = false;
			this.licenseLabel.Text = "License:";
			// 
			// fileVersionsLabel
			// 
			this.fileVersionsLabel.ForeColor = System.Drawing.Color.White;
			this.fileVersionsLabel.Location = new System.Drawing.Point( 24, 85 );
			this.fileVersionsLabel.Name = "fileVersionsLabel";
			this.fileVersionsLabel.Size = new System.Drawing.Size( 221, 23 );
			this.fileVersionsLabel.TabIndex = 2;
			this.fileVersionsLabel.TabStop = false;
			this.fileVersionsLabel.Text = "File Versions:";
			// 
			// copyrightLabel2
			// 
			this.copyrightLabel2.ForeColor = System.Drawing.Color.White;
			this.copyrightLabel2.Location = new System.Drawing.Point( 24, 247 );
			this.copyrightLabel2.Name = "copyrightLabel2";
			this.copyrightLabel2.Size = new System.Drawing.Size( 426, 59 );
			this.copyrightLabel2.TabIndex = 2;
			this.copyrightLabel2.TabStop = false;
			this.copyrightLabel2.Text = resources.GetString( "copyrightLabel2.Text" );
			// 
			// copyrightLabel
			// 
			this.copyrightLabel.ForeColor = System.Drawing.Color.White;
			this.copyrightLabel.Location = new System.Drawing.Point( 24, 215 );
			this.copyrightLabel.Name = "copyrightLabel";
			this.copyrightLabel.Size = new System.Drawing.Size( 426, 26 );
			this.copyrightLabel.TabIndex = 2;
			this.copyrightLabel.TabStop = false;
			this.copyrightLabel.Text = "(c) 2008-2009 Johannes Passing. All rights reserved.\r\nUnauthorized copying is str" +
				"ictly forbidden.";
			// 
			// versionLabel
			// 
			this.versionLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( ( byte ) ( 0 ) ) );
			this.versionLabel.ForeColor = System.Drawing.Color.White;
			this.versionLabel.Location = new System.Drawing.Point( 24, 27 );
			this.versionLabel.Name = "versionLabel";
			this.versionLabel.Size = new System.Drawing.Size( 221, 23 );
			this.versionLabel.TabIndex = 2;
			this.versionLabel.TabStop = false;
			this.versionLabel.Text = "Visual Assert, Version ";
			// 
			// enterLicenseButton
			// 
			this.enterLicenseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.enterLicenseButton.Location = new System.Drawing.Point( 258, 312 );
			this.enterLicenseButton.Name = "enterLicenseButton";
			this.enterLicenseButton.Size = new System.Drawing.Size( 123, 23 );
			this.enterLicenseButton.TabIndex = 5;
			this.enterLicenseButton.Text = "Enter License Key";
			this.enterLicenseButton.UseVisualStyleBackColor = true;
			this.enterLicenseButton.Click += new System.EventHandler( this.enterLicenseButton_Click );
			// 
			// AboutWindow
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.okButton;
			this.ClientSize = new System.Drawing.Size( 485, 347 );
			this.Controls.Add( this.enterLicenseButton );
			this.Controls.Add( this.linkLabel );
			this.Controls.Add( this.okButton );
			this.Controls.Add( this.fileVersionsList );
			this.Controls.Add( this.licenseValueLabel );
			this.Controls.Add( this.licenseLabel );
			this.Controls.Add( this.fileVersionsLabel );
			this.Controls.Add( this.copyrightLabel2 );
			this.Controls.Add( this.copyrightLabel );
			this.Controls.Add( this.versionLabel );
			this.Controls.Add( this.pictureBox1 );
			this.Icon = ( ( System.Drawing.Icon ) ( resources.GetObject( "$this.Icon" ) ) );
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About Visual Assert";
			( ( System.ComponentModel.ISupportInitialize ) ( this.pictureBox1 ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.PictureBox pictureBox1;
		private Cfix.Addin.Windows.TransparentLabel versionLabel;
		private System.Windows.Forms.ListView fileVersionsList;
		private System.Windows.Forms.ColumnHeader fileCol;
		private System.Windows.Forms.ColumnHeader versionCol;
		private Cfix.Addin.Windows.TransparentLabel fileVersionsLabel;
		private Cfix.Addin.Windows.TransparentLabel copyrightLabel;
		private Cfix.Addin.Windows.TransparentLabel copyrightLabel2;
		private System.Windows.Forms.ColumnHeader archCol;
		private System.Windows.Forms.LinkLabel linkLabel;
		private TransparentLabel licenseLabel;
		private TransparentLabel licenseValueLabel;
		private System.Windows.Forms.Button enterLicenseButton;
	}
}