namespace Cfix.Addin.Windows.Explorer
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ExplorerWindow ) );
			Cfix.Control.Ui.Explorer.NodeFactory nodeFactory2 = new Cfix.Control.Ui.Explorer.NodeFactory();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.debugButton = new System.Windows.Forms.ToolStripButton();
			this.runButton = new System.Windows.Forms.ToolStripButton();
			this.separator2 = new System.Windows.Forms.ToolStripSeparator();
			this.refreshButton = new System.Windows.Forms.ToolStripButton();
			this.abortRefreshButton = new System.Windows.Forms.ToolStripButton();
			this.separator3 = new System.Windows.Forms.ToolStripSeparator();
			this.autoRefreshButton = new System.Windows.Forms.ToolStripButton();
			this.optionsButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.shortCircuitFixtureOnFailureButton = new System.Windows.Forms.ToolStripMenuItem();
			this.shortCircuitRunOnFailureButton = new System.Windows.Forms.ToolStripMenuItem();
			this.separator4 = new System.Windows.Forms.ToolStripSeparator();
			this.selectModeButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.selectDirModeButton = new System.Windows.Forms.ToolStripMenuItem();
			this.selectSlnModeButton = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.lameButton = new System.Windows.Forms.ToolStripButton();
			this.statusText = new System.Windows.Forms.TextBox();
			this.ctxMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.ctxMenuDebugButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRunButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRefreshButton = new System.Windows.Forms.ToolStripMenuItem();
			this.explorer = new Cfix.Control.Ui.Explorer.TestExplorer();
			this.throbberPic = new System.Windows.Forms.PictureBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.captureStackTracesButton = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbar.SuspendLayout();
			this.ctxMenu.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.throbberPic ) ).BeginInit();
			this.SuspendLayout();
			// 
			// toolbar
			// 
			this.toolbar.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.debugButton,
            this.runButton,
            this.separator2,
            this.refreshButton,
            this.abortRefreshButton,
            this.separator3,
            this.autoRefreshButton,
            this.optionsButton,
            this.separator4,
            this.selectModeButton,
            this.toolStripSeparator1,
            this.lameButton} );
			resources.ApplyResources( this.toolbar, "toolbar" );
			this.toolbar.Name = "toolbar";
			// 
			// debugButton
			// 
			this.debugButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources( this.debugButton, "debugButton" );
			this.debugButton.Image = global::Cfix.Addin.Icons.Start2;
			this.debugButton.Name = "debugButton";
			this.debugButton.Click += new System.EventHandler( this.debugButton_Click );
			// 
			// runButton
			// 
			this.runButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources( this.runButton, "runButton" );
			this.runButton.Image = global::Cfix.Addin.Icons.Ffwd;
			this.runButton.Name = "runButton";
			this.runButton.Click += new System.EventHandler( this.runButton_Click );
			// 
			// separator2
			// 
			this.separator2.Name = "separator2";
			resources.ApplyResources( this.separator2, "separator2" );
			// 
			// refreshButton
			// 
			this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources( this.refreshButton, "refreshButton" );
			this.refreshButton.Image = global::Cfix.Addin.Icons.Refresh;
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Click += new System.EventHandler( this.refreshButton_Click );
			// 
			// abortRefreshButton
			// 
			this.abortRefreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources( this.abortRefreshButton, "abortRefreshButton" );
			this.abortRefreshButton.Image = global::Cfix.Addin.Icons.AbortRefresh;
			this.abortRefreshButton.Name = "abortRefreshButton";
			this.abortRefreshButton.Click += new System.EventHandler( this.abortRefreshButton_Click );
			// 
			// separator3
			// 
			this.separator3.Name = "separator3";
			resources.ApplyResources( this.separator3, "separator3" );
			// 
			// autoRefreshButton
			// 
			this.autoRefreshButton.CheckOnClick = true;
			this.autoRefreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources( this.autoRefreshButton, "autoRefreshButton" );
			this.autoRefreshButton.Image = global::Cfix.Addin.Icons.RefreshOnBuild;
			this.autoRefreshButton.Name = "autoRefreshButton";
			this.autoRefreshButton.Click += new System.EventHandler( this.autoRefreshButton_Click );
			// 
			// optionsButton
			// 
			this.optionsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.optionsButton.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.shortCircuitFixtureOnFailureButton,
            this.shortCircuitRunOnFailureButton,
            this.toolStripSeparator2,
            this.captureStackTracesButton} );
			resources.ApplyResources( this.optionsButton, "optionsButton" );
			this.optionsButton.Name = "optionsButton";
			// 
			// shortCircuitFixtureOnFailureButton
			// 
			this.shortCircuitFixtureOnFailureButton.CheckOnClick = true;
			this.shortCircuitFixtureOnFailureButton.Name = "shortCircuitFixtureOnFailureButton";
			resources.ApplyResources( this.shortCircuitFixtureOnFailureButton, "shortCircuitFixtureOnFailureButton" );
			this.shortCircuitFixtureOnFailureButton.Click += new System.EventHandler( this.shurtcutFixtureOnFailureButton_Click );
			// 
			// shortCircuitRunOnFailureButton
			// 
			this.shortCircuitRunOnFailureButton.CheckOnClick = true;
			this.shortCircuitRunOnFailureButton.Name = "shortCircuitRunOnFailureButton";
			resources.ApplyResources( this.shortCircuitRunOnFailureButton, "shortCircuitRunOnFailureButton" );
			this.shortCircuitRunOnFailureButton.Click += new System.EventHandler( this.shurtcutRunOnFailureButton_Click );
			// 
			// separator4
			// 
			this.separator4.Name = "separator4";
			resources.ApplyResources( this.separator4, "separator4" );
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
			this.selectSlnModeButton.Image = global::Cfix.Addin.Icons.Solution;
			resources.ApplyResources( this.selectSlnModeButton, "selectSlnModeButton" );
			this.selectSlnModeButton.Name = "selectSlnModeButton";
			this.selectSlnModeButton.Click += new System.EventHandler( this.selectSlnModeButton_Click );
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources( this.toolStripSeparator1, "toolStripSeparator1" );
			// 
			// lameButton
			// 
			this.lameButton.Image = global::Cfix.Addin.Icons.Feedback;
			resources.ApplyResources( this.lameButton, "lameButton" );
			this.lameButton.Name = "lameButton";
			this.lameButton.Click += new System.EventHandler( this.lameButton_Click );
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
			// ctxMenu
			// 
			this.ctxMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.ctxMenuDebugButton,
            this.ctxMenuRunButton,
            this.ctxMenuRefreshButton} );
			this.ctxMenu.Name = "ctxMenu";
			resources.ApplyResources( this.ctxMenu, "ctxMenu" );
			// 
			// ctxMenuDebugButton
			// 
			this.ctxMenuDebugButton.Image = global::Cfix.Addin.Icons.Start;
			resources.ApplyResources( this.ctxMenuDebugButton, "ctxMenuDebugButton" );
			this.ctxMenuDebugButton.Name = "ctxMenuDebugButton";
			this.ctxMenuDebugButton.Click += new System.EventHandler( this.ctxMenuDebugButton_Click );
			// 
			// ctxMenuRunButton
			// 
			this.ctxMenuRunButton.Image = global::Cfix.Addin.Icons.Ffwd;
			resources.ApplyResources( this.ctxMenuRunButton, "ctxMenuRunButton" );
			this.ctxMenuRunButton.Name = "ctxMenuRunButton";
			this.ctxMenuRunButton.Click += new System.EventHandler( this.ctxMenuRunButton_Click );
			// 
			// ctxMenuRefreshButton
			// 
			this.ctxMenuRefreshButton.Image = global::Cfix.Addin.Icons.Refresh;
			resources.ApplyResources( this.ctxMenuRefreshButton, "ctxMenuRefreshButton" );
			this.ctxMenuRefreshButton.Name = "ctxMenuRefreshButton";
			// 
			// explorer
			// 
			resources.ApplyResources( this.explorer, "explorer" );
			this.explorer.Name = "explorer";
			this.explorer.NodeContextMenu = null;
			this.explorer.NodeFactory = nodeFactory2;
			// 
			// throbberPic
			// 
			resources.ApplyResources( this.throbberPic, "throbberPic" );
			this.throbberPic.Image = global::Cfix.Addin.Icons.Throb;
			this.throbberPic.InitialImage = null;
			this.throbberPic.Name = "throbberPic";
			this.throbberPic.TabStop = false;
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources( this.toolStripSeparator2, "toolStripSeparator2" );
			// 
			// captureStackTracesButton
			// 
			resources.ApplyResources( this.captureStackTracesButton, "captureStackTracesButton" );
			this.captureStackTracesButton.Name = "captureStackTracesButton";
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
			this.ctxMenu.ResumeLayout( false );
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
		private System.Windows.Forms.PictureBox throbberPic;
		private System.Windows.Forms.TextBox statusText;
		private System.Windows.Forms.ToolStripButton abortRefreshButton;
		private System.Windows.Forms.ToolStripButton autoRefreshButton;
		private System.Windows.Forms.ContextMenuStrip ctxMenu;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuDebugButton;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRunButton;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRefreshButton;
		private System.Windows.Forms.ToolStripButton debugButton;
		private System.Windows.Forms.ToolStripButton runButton;
		private System.Windows.Forms.ToolStripSeparator separator2;
		private System.Windows.Forms.ToolStripDropDownButton optionsButton;
		private System.Windows.Forms.ToolStripMenuItem shortCircuitFixtureOnFailureButton;
		private System.Windows.Forms.ToolStripMenuItem shortCircuitRunOnFailureButton;
		private System.Windows.Forms.ToolStripSeparator separator3;
		private System.Windows.Forms.ToolStripSeparator separator4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton lameButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem captureStackTracesButton;
	}
}
