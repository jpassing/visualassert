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
			this.hostGroupBox = new System.Windows.Forms.GroupBox();
			this.restartNotice = new System.Windows.Forms.Label();
			this.insrSec = new System.Windows.Forms.Label();
			this.sec1 = new System.Windows.Forms.Label();
			this.instrHostRegTimeoutLabel = new System.Windows.Forms.Label();
			this.hostRegTimeoutLabel = new System.Windows.Forms.Label();
			this.instrumentedHostRegTimeout = new System.Windows.Forms.NumericUpDown();
			this.hostRegTimeout = new System.Windows.Forms.NumericUpDown();
			this.threadGroupBox = new System.Windows.Forms.GroupBox();
			this.stackSizeHuge = new System.Windows.Forms.RadioButton();
			this.stackSizeLarge = new System.Windows.Forms.RadioButton();
			this.stackSizeStd = new System.Windows.Forms.RadioButton();
			this.stackSizeLabel = new System.Windows.Forms.Label();
			this.hostGroupBox.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.instrumentedHostRegTimeout ) ).BeginInit();
			( ( System.ComponentModel.ISupportInitialize ) ( this.hostRegTimeout ) ).BeginInit();
			this.threadGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// hostGroupBox
			// 
			this.hostGroupBox.Controls.Add( this.restartNotice );
			this.hostGroupBox.Controls.Add( this.insrSec );
			this.hostGroupBox.Controls.Add( this.sec1 );
			this.hostGroupBox.Controls.Add( this.instrHostRegTimeoutLabel );
			this.hostGroupBox.Controls.Add( this.hostRegTimeoutLabel );
			this.hostGroupBox.Controls.Add( this.instrumentedHostRegTimeout );
			this.hostGroupBox.Controls.Add( this.hostRegTimeout );
			this.hostGroupBox.Location = new System.Drawing.Point( 3, 70 );
			this.hostGroupBox.Name = "hostGroupBox";
			this.hostGroupBox.Size = new System.Drawing.Size( 385, 117 );
			this.hostGroupBox.TabIndex = 0;
			this.hostGroupBox.TabStop = false;
			this.hostGroupBox.Text = "Host Process";
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
			// insrSec
			// 
			this.insrSec.AutoSize = true;
			this.insrSec.Location = new System.Drawing.Point( 317, 52 );
			this.insrSec.Name = "insrSec";
			this.insrSec.Size = new System.Drawing.Size( 47, 13 );
			this.insrSec.TabIndex = 2;
			this.insrSec.Text = "seconds";
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
			this.instrHostRegTimeoutLabel.Size = new System.Drawing.Size( 200, 13 );
			this.instrHostRegTimeoutLabel.TabIndex = 1;
			this.instrHostRegTimeoutLabel.Text = "Spawn timeout (instrumented processes):";
			// 
			// hostRegTimeoutLabel
			// 
			this.hostRegTimeoutLabel.AutoSize = true;
			this.hostRegTimeoutLabel.Location = new System.Drawing.Point( 6, 26 );
			this.hostRegTimeoutLabel.Name = "hostRegTimeoutLabel";
			this.hostRegTimeoutLabel.Size = new System.Drawing.Size( 172, 13 );
			this.hostRegTimeoutLabel.TabIndex = 1;
			this.hostRegTimeoutLabel.Text = "Spawn timeout (regular processes):";
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
			this.instrumentedHostRegTimeout.TabIndex = 5;
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
			this.hostRegTimeout.TabIndex = 4;
			this.hostRegTimeout.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
			// 
			// threadGroupBox
			// 
			this.threadGroupBox.Controls.Add( this.stackSizeHuge );
			this.threadGroupBox.Controls.Add( this.stackSizeLarge );
			this.threadGroupBox.Controls.Add( this.stackSizeStd );
			this.threadGroupBox.Controls.Add( this.stackSizeLabel );
			this.threadGroupBox.Location = new System.Drawing.Point( 3, 3 );
			this.threadGroupBox.Name = "threadGroupBox";
			this.threadGroupBox.Size = new System.Drawing.Size( 385, 61 );
			this.threadGroupBox.TabIndex = 1;
			this.threadGroupBox.TabStop = false;
			this.threadGroupBox.Text = "Threads";
			// 
			// stackSizeHuge
			// 
			this.stackSizeHuge.AutoSize = true;
			this.stackSizeHuge.Location = new System.Drawing.Point( 285, 25 );
			this.stackSizeHuge.Name = "stackSizeHuge";
			this.stackSizeHuge.Size = new System.Drawing.Size( 88, 17 );
			this.stackSizeHuge.TabIndex = 3;
			this.stackSizeHuge.TabStop = true;
			this.stackSizeHuge.Text = "Huge (16MB)";
			this.stackSizeHuge.UseVisualStyleBackColor = true;
			// 
			// stackSizeLarge
			// 
			this.stackSizeLarge.AutoSize = true;
			this.stackSizeLarge.Location = new System.Drawing.Point( 196, 25 );
			this.stackSizeLarge.Name = "stackSizeLarge";
			this.stackSizeLarge.Size = new System.Drawing.Size( 83, 17 );
			this.stackSizeLarge.TabIndex = 2;
			this.stackSizeLarge.TabStop = true;
			this.stackSizeLarge.Text = "Large (4MB)";
			this.stackSizeLarge.UseVisualStyleBackColor = true;
			// 
			// stackSizeStd
			// 
			this.stackSizeStd.AutoSize = true;
			this.stackSizeStd.Location = new System.Drawing.Point( 91, 25 );
			this.stackSizeStd.Name = "stackSizeStd";
			this.stackSizeStd.Size = new System.Drawing.Size( 99, 17 );
			this.stackSizeStd.TabIndex = 1;
			this.stackSizeStd.TabStop = true;
			this.stackSizeStd.Text = "Standard (1MB)";
			this.stackSizeStd.UseVisualStyleBackColor = true;
			// 
			// stackSizeLabel
			// 
			this.stackSizeLabel.AutoSize = true;
			this.stackSizeLabel.Location = new System.Drawing.Point( 6, 25 );
			this.stackSizeLabel.Name = "stackSizeLabel";
			this.stackSizeLabel.Size = new System.Drawing.Size( 59, 13 );
			this.stackSizeLabel.TabIndex = 1;
			this.stackSizeLabel.Text = "Stack size:";
			// 
			// OptionsPageAdvanced
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.threadGroupBox );
			this.Controls.Add( this.hostGroupBox );
			this.Name = "OptionsPageAdvanced";
			this.Size = new System.Drawing.Size( 400, 310 );
			this.hostGroupBox.ResumeLayout( false );
			this.hostGroupBox.PerformLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.instrumentedHostRegTimeout ) ).EndInit();
			( ( System.ComponentModel.ISupportInitialize ) ( this.hostRegTimeout ) ).EndInit();
			this.threadGroupBox.ResumeLayout( false );
			this.threadGroupBox.PerformLayout();
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.GroupBox hostGroupBox;
		private System.Windows.Forms.NumericUpDown hostRegTimeout;
		private System.Windows.Forms.Label insrSec;
		private System.Windows.Forms.Label sec1;
		private System.Windows.Forms.Label instrHostRegTimeoutLabel;
		private System.Windows.Forms.Label hostRegTimeoutLabel;
		private System.Windows.Forms.NumericUpDown instrumentedHostRegTimeout;
		private System.Windows.Forms.Label restartNotice;
		private System.Windows.Forms.GroupBox threadGroupBox;
		private System.Windows.Forms.RadioButton stackSizeStd;
		private System.Windows.Forms.Label stackSizeLabel;
		private System.Windows.Forms.RadioButton stackSizeHuge;
		private System.Windows.Forms.RadioButton stackSizeLarge;
	}
}
