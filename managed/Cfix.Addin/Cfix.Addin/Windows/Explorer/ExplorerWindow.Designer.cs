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
			Cfix.Control.Ui.Explorer.NodeFactory nodeFactory1 = new Cfix.Control.Ui.Explorer.NodeFactory();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.selectModeButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.selectDirModeButton = new System.Windows.Forms.ToolStripMenuItem();
			this.selectSlnModeButton = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshButton = new System.Windows.Forms.ToolStripButton();
			this.abortRefreshButton = new System.Windows.Forms.ToolStripButton();
			this.autoRefreshButton = new System.Windows.Forms.ToolStripButton();
			this.separator1 = new System.Windows.Forms.ToolStripSeparator();
			this.debugButton = new System.Windows.Forms.ToolStripButton();
			this.runButton = new System.Windows.Forms.ToolStripButton();
			this.explorer = new Cfix.Control.Ui.Explorer.TestExplorer();
			this.statusText = new System.Windows.Forms.TextBox();
			this.throbberPic = new System.Windows.Forms.PictureBox();
			this.ctxMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.ctxMenuDebugButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRunButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRefreshButton = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbar.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.throbberPic ) ).BeginInit();
			this.ctxMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolbar
			// 
			this.toolbar.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.selectModeButton,
            this.refreshButton,
            this.abortRefreshButton,
            this.autoRefreshButton,
            this.separator1,
            this.debugButton,
            this.runButton} );
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
			this.selectSlnModeButton.Image = global::Cfix.Addin.Icons.Solution;
			resources.ApplyResources( this.selectSlnModeButton, "selectSlnModeButton" );
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
			// abortRefreshButton
			// 
			this.abortRefreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources( this.abortRefreshButton, "abortRefreshButton" );
			this.abortRefreshButton.Image = global::Cfix.Addin.Icons.AbortRefresh;
			this.abortRefreshButton.Name = "abortRefreshButton";
			this.abortRefreshButton.Click += new System.EventHandler( this.abortRefreshButton_Click );
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
			// separator1
			// 
			this.separator1.Name = "separator1";
			resources.ApplyResources( this.separator1, "separator1" );
			// 
			// debugButton
			// 
			this.debugButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.debugButton.Image = global::Cfix.Addin.Icons.Start;
			resources.ApplyResources( this.debugButton, "debugButton" );
			this.debugButton.Name = "debugButton";
			this.debugButton.Click += new System.EventHandler( this.debugButton_Click );
			// 
			// runButton
			// 
			this.runButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.runButton.Image = global::Cfix.Addin.Icons.Ffwd;
			resources.ApplyResources( this.runButton, "runButton" );
			this.runButton.Name = "runButton";
			this.runButton.Click += new System.EventHandler( this.runButton_Click );
			// 
			// explorer
			// 
			resources.ApplyResources( this.explorer, "explorer" );
			this.explorer.Name = "explorer";
			this.explorer.NodeContextMenu = null;
			this.explorer.NodeFactory = nodeFactory1;
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
			// throbberPic
			// 
			resources.ApplyResources( this.throbberPic, "throbberPic" );
			this.throbberPic.Image = global::Cfix.Addin.Icons.Throb;
			this.throbberPic.InitialImage = null;
			this.throbberPic.Name = "throbberPic";
			this.throbberPic.TabStop = false;
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
			this.ctxMenu.ResumeLayout( false );
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
	}
}
