namespace Cfix.Addin.Windows
{
	partial class OptionsPageAdvanced
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
			this.advGroupBox = new System.Windows.Forms.GroupBox();
			this.restartNotice = new System.Windows.Forms.Label();
			this.sec2 = new System.Windows.Forms.Label();
			this.sec1 = new System.Windows.Forms.Label();
			this.instrHostRegTimeoutLabel = new System.Windows.Forms.Label();
			this.hostRegTimeoutLabel = new System.Windows.Forms.Label();
			this.instrumentedHostRegTimeout = new System.Windows.Forms.NumericUpDown();
			this.hostRegTimeout = new System.Windows.Forms.NumericUpDown();
			this.advGroupBox.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.instrumentedHostRegTimeout ) ).BeginInit();
			( ( System.ComponentModel.ISupportInitialize ) ( this.hostRegTimeout ) ).BeginInit();
			this.SuspendLayout();
			// 
			// advGroupBox
			// 
			this.advGroupBox.Controls.Add( this.restartNotice );
			this.advGroupBox.Controls.Add( this.sec2 );
			this.advGroupBox.Controls.Add( this.sec1 );
			this.advGroupBox.Controls.Add( this.instrHostRegTimeoutLabel );
			this.advGroupBox.Controls.Add( this.hostRegTimeoutLabel );
			this.advGroupBox.Controls.Add( this.instrumentedHostRegTimeout );
			this.advGroupBox.Controls.Add( this.hostRegTimeout );
			this.advGroupBox.Location = new System.Drawing.Point( 3, 3 );
			this.advGroupBox.Name = "advGroupBox";
			this.advGroupBox.Size = new System.Drawing.Size( 385, 117 );
			this.advGroupBox.TabIndex = 0;
			this.advGroupBox.TabStop = false;
			this.advGroupBox.Text = "Advanced Settings";
			// 
			// restartNotice
			// 
			this.restartNotice.AutoSize = true;
			this.restartNotice.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ( ( byte ) ( 0 ) ) );
			this.restartNotice.Location = new System.Drawing.Point( 6, 91 );
			this.restartNotice.Name = "restartNotice";
			this.restartNotice.Size = new System.Drawing.Size( 273, 13 );
			this.restartNotice.TabIndex = 3;
			this.restartNotice.Text = "Please restart the IDE to have your changes take effect.";
			// 
			// sec2
			// 
			this.sec2.AutoSize = true;
			this.sec2.Location = new System.Drawing.Point( 317, 52 );
			this.sec2.Name = "sec2";
			this.sec2.Size = new System.Drawing.Size( 47, 13 );
			this.sec2.TabIndex = 2;
			this.sec2.Text = "seconds";
			// 
			// sec1
			// 
			this.sec1.AutoSize = true;
			this.sec1.Location = new System.Drawing.Point( 317, 25 );
			this.sec1.Name = "sec1";
			this.sec1.Size = new System.Drawing.Size( 47, 13 );
			this.sec1.TabIndex = 2;
			this.sec1.Text = "seconds";
			// 
			// instrHostRegTimeoutLabel
			// 
			this.instrHostRegTimeoutLabel.AutoSize = true;
			this.instrHostRegTimeoutLabel.Location = new System.Drawing.Point( 6, 52 );
			this.instrHostRegTimeoutLabel.Name = "instrHostRegTimeoutLabel";
			this.instrHostRegTimeoutLabel.Size = new System.Drawing.Size( 229, 13 );
			this.instrHostRegTimeoutLabel.TabIndex = 1;
			this.instrHostRegTimeoutLabel.Text = "Spawn timeout for instrumented host processes";
			// 
			// hostRegTimeoutLabel
			// 
			this.hostRegTimeoutLabel.AutoSize = true;
			this.hostRegTimeoutLabel.Location = new System.Drawing.Point( 6, 26 );
			this.hostRegTimeoutLabel.Name = "hostRegTimeoutLabel";
			this.hostRegTimeoutLabel.Size = new System.Drawing.Size( 169, 13 );
			this.hostRegTimeoutLabel.TabIndex = 1;
			this.hostRegTimeoutLabel.Text = "Spawn timeout for host processes:";
			// 
			// instrumentedHostRegTimeout
			// 
			this.instrumentedHostRegTimeout.Location = new System.Drawing.Point( 245, 45 );
			this.instrumentedHostRegTimeout.Maximum = new decimal( new int[] {
            300,
            0,
            0,
            0} );
			this.instrumentedHostRegTimeout.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			this.instrumentedHostRegTimeout.Name = "instrumentedHostRegTimeout";
			this.instrumentedHostRegTimeout.Size = new System.Drawing.Size( 65, 20 );
			this.instrumentedHostRegTimeout.TabIndex = 0;
			this.instrumentedHostRegTimeout.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			// 
			// hostRegTimeout
			// 
			this.hostRegTimeout.Location = new System.Drawing.Point( 245, 19 );
			this.hostRegTimeout.Maximum = new decimal( new int[] {
            300,
            0,
            0,
            0} );
			this.hostRegTimeout.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			this.hostRegTimeout.Name = "hostRegTimeout";
			this.hostRegTimeout.Size = new System.Drawing.Size( 65, 20 );
			this.hostRegTimeout.TabIndex = 0;
			this.hostRegTimeout.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			// 
			// OptionsPageAdvanced
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.advGroupBox );
			this.Name = "OptionsPageAdvanced";
			this.Size = new System.Drawing.Size( 400, 280 );
			this.advGroupBox.ResumeLayout( false );
			this.advGroupBox.PerformLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.instrumentedHostRegTimeout ) ).EndInit();
			( ( System.ComponentModel.ISupportInitialize ) ( this.hostRegTimeout ) ).EndInit();
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.GroupBox advGroupBox;
		private System.Windows.Forms.NumericUpDown hostRegTimeout;
		private System.Windows.Forms.Label sec2;
		private System.Windows.Forms.Label sec1;
		private System.Windows.Forms.Label instrHostRegTimeoutLabel;
		private System.Windows.Forms.Label hostRegTimeoutLabel;
		private System.Windows.Forms.NumericUpDown instrumentedHostRegTimeout;
		private System.Windows.Forms.Label restartNotice;
	}
}
