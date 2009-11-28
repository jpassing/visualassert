namespace Cfix.Addin.Windows
{
	partial class OptionsPageWindbg
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
			this.winDbgGroupBox = new System.Windows.Forms.GroupBox();
			this.windbgDesc = new System.Windows.Forms.Label();
			this.windbg64Browse = new System.Windows.Forms.Button();
			this.windbg64Label = new System.Windows.Forms.Label();
			this.windbg32Browse = new System.Windows.Forms.Button();
			this.windbg64Path = new System.Windows.Forms.TextBox();
			this.windbg32Label = new System.Windows.Forms.Label();
			this.windbg32Path = new System.Windows.Forms.TextBox();
			this.windbgAdditionalOpts = new System.Windows.Forms.TextBox();
			this.windbgAdditionalOptsLabel = new System.Windows.Forms.Label();
			this.winDbgGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// winDbgGroupBox
			// 
			this.winDbgGroupBox.Controls.Add( this.windbgDesc );
			this.winDbgGroupBox.Controls.Add( this.windbg64Browse );
			this.winDbgGroupBox.Controls.Add( this.windbgAdditionalOptsLabel );
			this.winDbgGroupBox.Controls.Add( this.windbg64Label );
			this.winDbgGroupBox.Controls.Add( this.windbg32Browse );
			this.winDbgGroupBox.Controls.Add( this.windbgAdditionalOpts );
			this.winDbgGroupBox.Controls.Add( this.windbg64Path );
			this.winDbgGroupBox.Controls.Add( this.windbg32Label );
			this.winDbgGroupBox.Controls.Add( this.windbg32Path );
			this.winDbgGroupBox.Location = new System.Drawing.Point( 3, 3 );
			this.winDbgGroupBox.Name = "winDbgGroupBox";
			this.winDbgGroupBox.Size = new System.Drawing.Size( 385, 193 );
			this.winDbgGroupBox.TabIndex = 0;
			this.winDbgGroupBox.TabStop = false;
			this.winDbgGroupBox.Text = "WinDBG Integration";
			// 
			// windbgDesc
			// 
			this.windbgDesc.AutoSize = true;
			this.windbgDesc.Location = new System.Drawing.Point( 9, 20 );
			this.windbgDesc.Name = "windbgDesc";
			this.windbgDesc.Size = new System.Drawing.Size( 359, 26 );
			this.windbgDesc.TabIndex = 3;
			this.windbgDesc.Text = "Install locations of your WinDBG installations, if available. WinDBG is used \r\nfo" +
				"r the \'Debug Using Console Runner and WinDBG\' feature.";
			// 
			// windbg64Browse
			// 
			this.windbg64Browse.Location = new System.Drawing.Point( 281, 88 );
			this.windbg64Browse.Name = "windbg64Browse";
			this.windbg64Browse.Size = new System.Drawing.Size( 94, 23 );
			this.windbg64Browse.TabIndex = 3;
			this.windbg64Browse.Text = "Browse...";
			this.windbg64Browse.UseVisualStyleBackColor = true;
			this.windbg64Browse.Click += new System.EventHandler( this.windbg64Browse_Click );
			// 
			// windbg64Label
			// 
			this.windbg64Label.AutoSize = true;
			this.windbg64Label.Location = new System.Drawing.Point( 15, 91 );
			this.windbg64Label.Name = "windbg64Label";
			this.windbg64Label.Size = new System.Drawing.Size( 33, 13 );
			this.windbg64Label.TabIndex = 1;
			this.windbg64Label.Text = "64 bit";
			// 
			// windbg32Browse
			// 
			this.windbg32Browse.Location = new System.Drawing.Point( 281, 59 );
			this.windbg32Browse.Name = "windbg32Browse";
			this.windbg32Browse.Size = new System.Drawing.Size( 94, 23 );
			this.windbg32Browse.TabIndex = 1;
			this.windbg32Browse.Text = "Browse...";
			this.windbg32Browse.UseVisualStyleBackColor = true;
			this.windbg32Browse.Click += new System.EventHandler( this.windbg32Browse_Click );
			// 
			// windbg64Path
			// 
			this.windbg64Path.Location = new System.Drawing.Point( 54, 88 );
			this.windbg64Path.Name = "windbg64Path";
			this.windbg64Path.Size = new System.Drawing.Size( 221, 20 );
			this.windbg64Path.TabIndex = 2;
			// 
			// windbg32Label
			// 
			this.windbg32Label.AutoSize = true;
			this.windbg32Label.Location = new System.Drawing.Point( 15, 62 );
			this.windbg32Label.Name = "windbg32Label";
			this.windbg32Label.Size = new System.Drawing.Size( 33, 13 );
			this.windbg32Label.TabIndex = 1;
			this.windbg32Label.Text = "32 bit";
			// 
			// windbg32Path
			// 
			this.windbg32Path.Location = new System.Drawing.Point( 54, 59 );
			this.windbg32Path.Name = "windbg32Path";
			this.windbg32Path.Size = new System.Drawing.Size( 221, 20 );
			this.windbg32Path.TabIndex = 0;
			// 
			// windbgAdditionalOpts
			// 
			this.windbgAdditionalOpts.Location = new System.Drawing.Point( 18, 157 );
			this.windbgAdditionalOpts.Name = "windbgAdditionalOpts";
			this.windbgAdditionalOpts.Size = new System.Drawing.Size( 357, 20 );
			this.windbgAdditionalOpts.TabIndex = 4;
			// 
			// windbgAdditionalOptsLabel
			// 
			this.windbgAdditionalOptsLabel.AutoSize = true;
			this.windbgAdditionalOptsLabel.Location = new System.Drawing.Point( 15, 136 );
			this.windbgAdditionalOptsLabel.Name = "windbgAdditionalOptsLabel";
			this.windbgAdditionalOptsLabel.Size = new System.Drawing.Size( 252, 13 );
			this.windbgAdditionalOptsLabel.TabIndex = 1;
			this.windbgAdditionalOptsLabel.Text = "Additional command line options to pass to WinDBG";
			// 
			// OptionsPageWindbg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.winDbgGroupBox );
			this.Name = "OptionsPageWindbg";
			this.Size = new System.Drawing.Size( 400, 280 );
			this.winDbgGroupBox.ResumeLayout( false );
			this.winDbgGroupBox.PerformLayout();
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.GroupBox winDbgGroupBox;
		private System.Windows.Forms.Button windbg32Browse;
		private System.Windows.Forms.Label windbg32Label;
		private System.Windows.Forms.TextBox windbg32Path;
		private System.Windows.Forms.Button windbg64Browse;
		private System.Windows.Forms.Label windbg64Label;
		private System.Windows.Forms.TextBox windbg64Path;
		private System.Windows.Forms.Label windbgDesc;
		private System.Windows.Forms.Label windbgAdditionalOptsLabel;
		private System.Windows.Forms.TextBox windbgAdditionalOpts;

	}
}
