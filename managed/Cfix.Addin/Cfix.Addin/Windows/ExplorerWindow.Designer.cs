namespace Cfix.Addin.Windows
{
	partial class ExplorerWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ExplorerWindow ) );
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.selectModeButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.selectDirModeButton = new System.Windows.Forms.ToolStripMenuItem();
			this.selectSlnModeButton = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshButton = new System.Windows.Forms.ToolStripButton();
			this.separator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
			this.startDebuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startWithoutDebuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.abortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.terminateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.explorer = new Cfix.Control.Ui.Explorer.TestExplorer();
			this.throbberPic = new System.Windows.Forms.PictureBox();
			this.statusText = new System.Windows.Forms.TextBox();
			this.toolbar.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.throbberPic ) ).BeginInit();
			this.SuspendLayout();
			// 
			// toolbar
			// 
			this.toolbar.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.selectModeButton,
            this.refreshButton,
            this.separator1,
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton1} );
			resources.ApplyResources( this.toolbar, "toolbar" );
			this.toolbar.Name = "toolbar";
			// 
			// selectModeButton
			// 
			this.selectModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.selectModeButton.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.selectDirModeButton,
            this.selectSlnModeButton} );
			resources.ApplyResources( this.selectModeButton, "selectModeButton" );
			this.selectModeButton.Name = "selectModeButton";
			// 
			// selectDirModeButton
			// 
			resources.ApplyResources( this.selectDirModeButton, "selectDirModeButton" );
			this.selectDirModeButton.Name = "selectDirModeButton";
			this.selectDirModeButton.Click += new System.EventHandler( this.selectDirModeButton_Click );
			// 
			// selectSlnModeButton
			// 
			resources.ApplyResources( this.selectSlnModeButton, "selectSlnModeButton" );
			this.selectSlnModeButton.Image = global::Cfix.Addin.Icons.Solution;
			this.selectSlnModeButton.Name = "selectSlnModeButton";
			this.selectSlnModeButton.Click += new System.EventHandler( this.selectSlnModeButton_Click );
			// 
			// refreshButton
			// 
			this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources( this.refreshButton, "refreshButton" );
			this.refreshButton.Image = global::Cfix.Addin.Icons.Refresh;
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Click += new System.EventHandler( this.refreshButton_Click );
			// 
			// separator1
			// 
			this.separator1.Name = "separator1";
			resources.ApplyResources( this.separator1, "separator1" );
			// 
			// toolStripDropDownButton2
			// 
			this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripDropDownButton2.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.startDebuggingToolStripMenuItem,
            this.startWithoutDebuggingToolStripMenuItem} );
			resources.ApplyResources( this.toolStripDropDownButton2, "toolStripDropDownButton2" );
			this.toolStripDropDownButton2.Image = global::Cfix.Addin.Icons.Start;
			this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
			// 
			// startDebuggingToolStripMenuItem
			// 
			this.startDebuggingToolStripMenuItem.Image = global::Cfix.Addin.Icons.Start;
			resources.ApplyResources( this.startDebuggingToolStripMenuItem, "startDebuggingToolStripMenuItem" );
			this.startDebuggingToolStripMenuItem.Name = "startDebuggingToolStripMenuItem";
			// 
			// startWithoutDebuggingToolStripMenuItem
			// 
			this.startWithoutDebuggingToolStripMenuItem.Image = global::Cfix.Addin.Icons.Ffwd;
			resources.ApplyResources( this.startWithoutDebuggingToolStripMenuItem, "startWithoutDebuggingToolStripMenuItem" );
			this.startWithoutDebuggingToolStripMenuItem.Name = "startWithoutDebuggingToolStripMenuItem";
			// 
			// toolStripDropDownButton1
			// 
			this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripDropDownButton1.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.abortToolStripMenuItem,
            this.terminateToolStripMenuItem} );
			resources.ApplyResources( this.toolStripDropDownButton1, "toolStripDropDownButton1" );
			this.toolStripDropDownButton1.Image = global::Cfix.Addin.Icons.StopHS;
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			// 
			// abortToolStripMenuItem
			// 
			this.abortToolStripMenuItem.Image = global::Cfix.Addin.Icons.StopHS;
			resources.ApplyResources( this.abortToolStripMenuItem, "abortToolStripMenuItem" );
			this.abortToolStripMenuItem.Name = "abortToolStripMenuItem";
			// 
			// terminateToolStripMenuItem
			// 
			this.terminateToolStripMenuItem.Image = global::Cfix.Addin.Icons.Delete;
			resources.ApplyResources( this.terminateToolStripMenuItem, "terminateToolStripMenuItem" );
			this.terminateToolStripMenuItem.Name = "terminateToolStripMenuItem";
			// 
			// explorer
			// 
			resources.ApplyResources( this.explorer, "explorer" );
			this.explorer.Name = "explorer";
			// 
			// throbberPic
			// 
			resources.ApplyResources( this.throbberPic, "throbberPic" );
			this.throbberPic.Image = global::Cfix.Addin.Icons.Throb;
			this.throbberPic.InitialImage = null;
			this.throbberPic.Name = "throbberPic";
			this.throbberPic.TabStop = false;
			// 
			// statusText
			// 
			resources.ApplyResources( this.statusText, "statusText" );
			this.statusText.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.statusText.HideSelection = false;
			this.statusText.Name = "statusText";
			this.statusText.ReadOnly = true;
			this.statusText.ShortcutsEnabled = false;
			// 
			// ExplorerWindow
			// 
			resources.ApplyResources( this, "$this" );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.statusText );
			this.Controls.Add( this.throbberPic );
			this.Controls.Add( this.explorer );
			this.Controls.Add( this.toolbar );
			this.Name = "ExplorerWindow";
			this.toolbar.ResumeLayout( false );
			this.toolbar.PerformLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.throbberPic ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolbar;
		private Cfix.Control.Ui.Explorer.TestExplorer explorer;
		private System.Windows.Forms.ToolStripButton refreshButton;
		private System.Windows.Forms.ToolStripDropDownButton selectModeButton;
		private System.Windows.Forms.ToolStripMenuItem selectDirModeButton;
		private System.Windows.Forms.ToolStripMenuItem selectSlnModeButton;
		private System.Windows.Forms.ToolStripSeparator separator1;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
		private System.Windows.Forms.ToolStripMenuItem abortToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem terminateToolStripMenuItem;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
		private System.Windows.Forms.ToolStripMenuItem startDebuggingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startWithoutDebuggingToolStripMenuItem;
		private System.Windows.Forms.PictureBox throbberPic;
		private System.Windows.Forms.TextBox statusText;
	}
}
