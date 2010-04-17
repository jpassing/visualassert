namespace Cfix.Addin.Windows
{
	partial class OptionsPageGeneral
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
			this.generalGroupBox = new System.Windows.Forms.GroupBox();
			this.autoRegisterVcDirs = new System.Windows.Forms.CheckBox();
			this.autoAdjustCwd = new System.Windows.Forms.CheckBox();
			this.eventDllGroupBox = new System.Windows.Forms.GroupBox();
			this.archComboBox = new System.Windows.Forms.ComboBox();
			this.optsLabel = new System.Windows.Forms.Label();
			this.archLabel = new System.Windows.Forms.Label();
			this.dllLabel = new System.Windows.Forms.Label();
			this.optionsTextBox = new System.Windows.Forms.TextBox();
			this.dllTextBox = new System.Windows.Forms.TextBox();
			this.browseDllButton = new System.Windows.Forms.Button();
			this.restartNotice = new System.Windows.Forms.Label();
			this.generalGroupBox.SuspendLayout();
			this.eventDllGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// generalGroupBox
			// 
			this.generalGroupBox.Controls.Add( this.autoRegisterVcDirs );
			this.generalGroupBox.Controls.Add( this.autoAdjustCwd );
			this.generalGroupBox.Location = new System.Drawing.Point( 3, 3 );
			this.generalGroupBox.Name = "generalGroupBox";
			this.generalGroupBox.Size = new System.Drawing.Size( 384, 87 );
			this.generalGroupBox.TabIndex = 3;
			this.generalGroupBox.TabStop = false;
			this.generalGroupBox.Text = "General";
			// 
			// autoRegisterVcDirs
			// 
			this.autoRegisterVcDirs.AutoSize = true;
			this.autoRegisterVcDirs.Location = new System.Drawing.Point( 8, 51 );
			this.autoRegisterVcDirs.Name = "autoRegisterVcDirs";
			this.autoRegisterVcDirs.Size = new System.Drawing.Size( 255, 17 );
			this.autoRegisterVcDirs.TabIndex = 1;
			this.autoRegisterVcDirs.Text = "Automatically register VC++ directories on startup";
			this.autoRegisterVcDirs.UseVisualStyleBackColor = true;
			// 
			// autoAdjustCwd
			// 
			this.autoAdjustCwd.AutoSize = true;
			this.autoAdjustCwd.Location = new System.Drawing.Point( 9, 28 );
			this.autoAdjustCwd.Name = "autoAdjustCwd";
			this.autoAdjustCwd.Size = new System.Drawing.Size( 302, 17 );
			this.autoAdjustCwd.TabIndex = 0;
			this.autoAdjustCwd.Text = "Automatically adjust current directory to module base folder";
			this.autoAdjustCwd.UseVisualStyleBackColor = true;
			// 
			// eventDllGroupBox
			// 
			this.eventDllGroupBox.Controls.Add( this.archComboBox );
			this.eventDllGroupBox.Controls.Add( this.optsLabel );
			this.eventDllGroupBox.Controls.Add( this.archLabel );
			this.eventDllGroupBox.Controls.Add( this.dllLabel );
			this.eventDllGroupBox.Controls.Add( this.optionsTextBox );
			this.eventDllGroupBox.Controls.Add( this.dllTextBox );
			this.eventDllGroupBox.Controls.Add( this.browseDllButton );
			this.eventDllGroupBox.Location = new System.Drawing.Point( 3, 96 );
			this.eventDllGroupBox.Name = "eventDllGroupBox";
			this.eventDllGroupBox.Size = new System.Drawing.Size( 384, 153 );
			this.eventDllGroupBox.TabIndex = 4;
			this.eventDllGroupBox.TabStop = false;
			this.eventDllGroupBox.Text = "Event DLL";
			// 
			// archComboBox
			// 
			this.archComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.archComboBox.FormattingEnabled = true;
			this.archComboBox.Location = new System.Drawing.Point( 9, 41 );
			this.archComboBox.Name = "archComboBox";
			this.archComboBox.Size = new System.Drawing.Size( 121, 21 );
			this.archComboBox.TabIndex = 2;
			this.archComboBox.SelectedIndexChanged += new System.EventHandler( this.archComboBox_SelectedIndexChanged );
			// 
			// optsLabel
			// 
			this.optsLabel.AutoSize = true;
			this.optsLabel.Location = new System.Drawing.Point( 5, 104 );
			this.optsLabel.Name = "optsLabel";
			this.optsLabel.Size = new System.Drawing.Size( 115, 13 );
			this.optsLabel.TabIndex = 7;
			this.optsLabel.Text = "Options to pass to DLL";
			// 
			// archLabel
			// 
			this.archLabel.AutoSize = true;
			this.archLabel.Location = new System.Drawing.Point( 6, 25 );
			this.archLabel.Name = "archLabel";
			this.archLabel.Size = new System.Drawing.Size( 132, 13 );
			this.archLabel.TabIndex = 7;
			this.archLabel.Text = "Platform/CPU Architecture";
			// 
			// dllLabel
			// 
			this.dllLabel.AutoSize = true;
			this.dllLabel.Location = new System.Drawing.Point( 5, 65 );
			this.dllLabel.Name = "dllLabel";
			this.dllLabel.Size = new System.Drawing.Size( 58, 13 );
			this.dllLabel.TabIndex = 7;
			this.dllLabel.Text = "Event DLL";
			// 
			// optionsTextBox
			// 
			this.optionsTextBox.Location = new System.Drawing.Point( 8, 120 );
			this.optionsTextBox.Name = "optionsTextBox";
			this.optionsTextBox.Size = new System.Drawing.Size( 327, 20 );
			this.optionsTextBox.TabIndex = 4;
			this.optionsTextBox.TextChanged += new System.EventHandler( this.optionsTextBox_TextChanged );
			// 
			// dllTextBox
			// 
			this.dllTextBox.Location = new System.Drawing.Point( 9, 81 );
			this.dllTextBox.Name = "dllTextBox";
			this.dllTextBox.Size = new System.Drawing.Size( 327, 20 );
			this.dllTextBox.TabIndex = 3;
			this.dllTextBox.TextChanged += new System.EventHandler( this.dllTextBox_TextChanged );
			// 
			// browseDllButton
			// 
			this.browseDllButton.Location = new System.Drawing.Point( 342, 81 );
			this.browseDllButton.Name = "browseDllButton";
			this.browseDllButton.Size = new System.Drawing.Size( 31, 20 );
			this.browseDllButton.TabIndex = 5;
			this.browseDllButton.Text = "...";
			this.browseDllButton.UseVisualStyleBackColor = true;
			this.browseDllButton.Click += new System.EventHandler( this.browseDllButton_Click );
			// 
			// restartNotice
			// 
			this.restartNotice.AutoSize = true;
			this.restartNotice.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ( ( byte ) ( 0 ) ) );
			this.restartNotice.Location = new System.Drawing.Point( 9, 262 );
			this.restartNotice.Name = "restartNotice";
			this.restartNotice.Size = new System.Drawing.Size( 273, 13 );
			this.restartNotice.TabIndex = 4;
			this.restartNotice.Text = "Please restart the IDE to have your changes take effect.";
			// 
			// OptionsPageGeneral
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.eventDllGroupBox );
			this.Controls.Add( this.generalGroupBox );
			this.Controls.Add( this.restartNotice );
			this.Name = "OptionsPageGeneral";
			this.Size = new System.Drawing.Size( 400, 310 );
			this.generalGroupBox.ResumeLayout( false );
			this.generalGroupBox.PerformLayout();
			this.eventDllGroupBox.ResumeLayout( false );
			this.eventDllGroupBox.PerformLayout();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox generalGroupBox;
		private System.Windows.Forms.CheckBox autoRegisterVcDirs;
		private System.Windows.Forms.CheckBox autoAdjustCwd;
		private System.Windows.Forms.GroupBox eventDllGroupBox;
		private System.Windows.Forms.TextBox dllTextBox;
		private System.Windows.Forms.Button browseDllButton;
		private System.Windows.Forms.Label restartNotice;
		private System.Windows.Forms.TextBox optionsTextBox;
		private System.Windows.Forms.Label optsLabel;
		private System.Windows.Forms.Label dllLabel;
		private System.Windows.Forms.ComboBox archComboBox;
		private System.Windows.Forms.Label archLabel;
	}
}
