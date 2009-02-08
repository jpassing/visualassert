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
			this.separator1 = new System.Windows.Forms.ToolStripSeparator();
			this.explorer = new Cfix.Control.Ui.Explorer.TestExplorer();
			this.selectModeButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.directoryExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.currentSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
			this.startDebuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startWithoutDebuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.abortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.terminateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbar.SuspendLayout();
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
			// separator1
			// 
			this.separator1.Name = "separator1";
			resources.ApplyResources( this.separator1, "separator1" );
			// 
			// explorer
			// 
			resources.ApplyResources( this.explorer, "explorer" );
			this.explorer.Name = "explorer";
			// 
			// selectModeButton
			// 
			this.selectModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.selectModeButton.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.directoryExplorerToolStripMenuItem,
            this.currentSolutionToolStripMenuItem} );
			resources.ApplyResources( this.selectModeButton, "selectModeButton" );
			this.selectModeButton.Name = "selectModeButton";
			// 
			// directoryExplorerToolStripMenuItem
			// 
			resources.ApplyResources( this.directoryExplorerToolStripMenuItem, "directoryExplorerToolStripMenuItem" );
			this.directoryExplorerToolStripMenuItem.Name = "directoryExplorerToolStripMenuItem";
			// 
			// currentSolutionToolStripMenuItem
			// 
			this.currentSolutionToolStripMenuItem.Image = global::Cfix.Addin.Icons.Solution;
			resources.ApplyResources( this.currentSolutionToolStripMenuItem, "currentSolutionToolStripMenuItem" );
			this.currentSolutionToolStripMenuItem.Name = "currentSolutionToolStripMenuItem";
			// 
			// refreshButton
			// 
			this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.refreshButton.Image = global::Cfix.Addin.Icons.Refresh;
			resources.ApplyResources( this.refreshButton, "refreshButton" );
			this.refreshButton.Name = "refreshButton";
			// 
			// toolStripDropDownButton2
			// 
			this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripDropDownButton2.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.startDebuggingToolStripMenuItem,
            this.startWithoutDebuggingToolStripMenuItem} );
			this.toolStripDropDownButton2.Image = global::Cfix.Addin.Icons.Start;
			resources.ApplyResources( this.toolStripDropDownButton2, "toolStripDropDownButton2" );
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
			this.toolStripDropDownButton1.Image = global::Cfix.Addin.Icons.StopHS;
			resources.ApplyResources( this.toolStripDropDownButton1, "toolStripDropDownButton1" );
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
			// ExplorerWindow
			// 
			resources.ApplyResources( this, "$this" );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.explorer );
			this.Controls.Add( this.toolbar );
			this.Name = "ExplorerWindow";
			this.toolbar.ResumeLayout( false );
			this.toolbar.PerformLayout();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolbar;
		private Cfix.Control.Ui.Explorer.TestExplorer explorer;
		private System.Windows.Forms.ToolStripButton refreshButton;
		private System.Windows.Forms.ToolStripDropDownButton selectModeButton;
		private System.Windows.Forms.ToolStripMenuItem directoryExplorerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem currentSolutionToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator separator1;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
		private System.Windows.Forms.ToolStripMenuItem abortToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem terminateToolStripMenuItem;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
		private System.Windows.Forms.ToolStripMenuItem startDebuggingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startWithoutDebuggingToolStripMenuItem;
	}
}
