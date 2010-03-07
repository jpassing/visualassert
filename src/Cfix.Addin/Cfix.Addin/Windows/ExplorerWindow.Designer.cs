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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ExplorerWindow ) );
			Cfix.Control.Ui.Explorer.NodeFactory nodeFactory1 = new Cfix.Control.Ui.Explorer.NodeFactory();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.debugButton = new System.Windows.Forms.ToolStripButton();
			this.runButton = new System.Windows.Forms.ToolStripButton();
			this.separator2 = new System.Windows.Forms.ToolStripSeparator();
			this.refreshButton = new System.Windows.Forms.ToolStripButton();
			this.abortRefreshButton = new System.Windows.Forms.ToolStripButton();
			this.separator3 = new System.Windows.Forms.ToolStripSeparator();
			this.optionsButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.shortCircuitFixtureOnFailureMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.shortCircuitRunOnFailureMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.captureStackTracesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshAfterBuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.breakOnFailedAssertionsWhenDebuggingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.breakOnUnhandledExceptionsWhenDebuggingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.separator4 = new System.Windows.Forms.ToolStripSeparator();
			this.selectModeButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.selectDirModeButton = new System.Windows.Forms.ToolStripMenuItem();
			this.selectSlnModeButton = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.lameButton = new System.Windows.Forms.ToolStripButton();
			this.ctxMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.ctxMenuDebugButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRunButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRunInConsole = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuDebugWithWindbg = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRunInInspector = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRunInInspectorCheckDeadlocks = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRunInInspectorCheckDeadlocksOrRaces = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRunInInspectorLocateDeadlocks = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuRunInInspectorComplete = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.ctxMenuRefreshButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuViewCodeButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.ctxMenuAddFixtureButton = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxMenuSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.ctxMenuViewProperties = new System.Windows.Forms.ToolStripMenuItem();
			this.explorer = new Cfix.Control.Ui.Explorer.TestExplorer();
			this.infoLabel = new Cfix.Addin.Windows.TransparentLabel();
			this.infoBar = new Cfix.Addin.Windows.PlainProgressBar();
			this.infoIcon = new System.Windows.Forms.PictureBox();
			this.toolbar.SuspendLayout();
			this.ctxMenu.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize ) ( this.infoIcon ) ).BeginInit();
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
			// optionsButton
			// 
			this.optionsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.optionsButton.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.shortCircuitFixtureOnFailureMenuItem,
            this.shortCircuitRunOnFailureMenuItem,
            this.toolStripSeparator2,
            this.captureStackTracesMenuItem,
            this.refreshAfterBuildToolStripMenuItem,
            this.toolStripMenuItem1,
            this.breakOnFailedAssertionsWhenDebuggingMenuItem,
            this.breakOnUnhandledExceptionsWhenDebuggingMenuItem} );
			this.optionsButton.Image = global::Cfix.Addin.Icons.Options;
			resources.ApplyResources( this.optionsButton, "optionsButton" );
			this.optionsButton.Name = "optionsButton";
			// 
			// shortCircuitFixtureOnFailureMenuItem
			// 
			this.shortCircuitFixtureOnFailureMenuItem.CheckOnClick = true;
			this.shortCircuitFixtureOnFailureMenuItem.Name = "shortCircuitFixtureOnFailureMenuItem";
			resources.ApplyResources( this.shortCircuitFixtureOnFailureMenuItem, "shortCircuitFixtureOnFailureMenuItem" );
			this.shortCircuitFixtureOnFailureMenuItem.Click += new System.EventHandler( this.shurtcutFixtureOnFailureButton_Click );
			// 
			// shortCircuitRunOnFailureMenuItem
			// 
			this.shortCircuitRunOnFailureMenuItem.CheckOnClick = true;
			this.shortCircuitRunOnFailureMenuItem.Name = "shortCircuitRunOnFailureMenuItem";
			resources.ApplyResources( this.shortCircuitRunOnFailureMenuItem, "shortCircuitRunOnFailureMenuItem" );
			this.shortCircuitRunOnFailureMenuItem.Click += new System.EventHandler( this.shurtcutRunOnFailureButton_Click );
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources( this.toolStripSeparator2, "toolStripSeparator2" );
			// 
			// captureStackTracesMenuItem
			// 
			this.captureStackTracesMenuItem.CheckOnClick = true;
			this.captureStackTracesMenuItem.Image = global::Cfix.Addin.Icons.CaptureStackTraces;
			resources.ApplyResources( this.captureStackTracesMenuItem, "captureStackTracesMenuItem" );
			this.captureStackTracesMenuItem.Name = "captureStackTracesMenuItem";
			this.captureStackTracesMenuItem.Click += new System.EventHandler( this.captureStackTracesMenuItem_Click );
			// 
			// refreshAfterBuildToolStripMenuItem
			// 
			this.refreshAfterBuildToolStripMenuItem.CheckOnClick = true;
			this.refreshAfterBuildToolStripMenuItem.Image = global::Cfix.Addin.Icons.RefreshOnBuild;
			resources.ApplyResources( this.refreshAfterBuildToolStripMenuItem, "refreshAfterBuildToolStripMenuItem" );
			this.refreshAfterBuildToolStripMenuItem.Name = "refreshAfterBuildToolStripMenuItem";
			this.refreshAfterBuildToolStripMenuItem.Click += new System.EventHandler( this.refreshAfterBuildToolStripMenuItem_Click );
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			resources.ApplyResources( this.toolStripMenuItem1, "toolStripMenuItem1" );
			// 
			// breakOnFailedAssertionsWhenDebuggingMenuItem
			// 
			this.breakOnFailedAssertionsWhenDebuggingMenuItem.CheckOnClick = true;
			this.breakOnFailedAssertionsWhenDebuggingMenuItem.Name = "breakOnFailedAssertionsWhenDebuggingMenuItem";
			resources.ApplyResources( this.breakOnFailedAssertionsWhenDebuggingMenuItem, "breakOnFailedAssertionsWhenDebuggingMenuItem" );
			this.breakOnFailedAssertionsWhenDebuggingMenuItem.Click += new System.EventHandler( this.breakOnFailedAssertionsWhenDebuggingMenuItem_Click );
			// 
			// breakOnUnhandledExceptionsWhenDebuggingMenuItem
			// 
			this.breakOnUnhandledExceptionsWhenDebuggingMenuItem.CheckOnClick = true;
			this.breakOnUnhandledExceptionsWhenDebuggingMenuItem.Name = "breakOnUnhandledExceptionsWhenDebuggingMenuItem";
			resources.ApplyResources( this.breakOnUnhandledExceptionsWhenDebuggingMenuItem, "breakOnUnhandledExceptionsWhenDebuggingMenuItem" );
			this.breakOnUnhandledExceptionsWhenDebuggingMenuItem.Click += new System.EventHandler( this.breakOnUnhandledExceptionsWhenDebuggingMenuItem_Click );
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
			// ctxMenu
			// 
			this.ctxMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.ctxMenuDebugButton,
            this.ctxMenuRunButton,
            this.ctxMenuRunInConsole,
            this.ctxMenuDebugWithWindbg,
            this.ctxMenuRunInInspector,
            this.ctxMenuSeparator4,
            this.ctxMenuRefreshButton,
            this.ctxMenuViewCodeButton,
            this.ctxMenuSeparator1,
            this.ctxMenuAddFixtureButton,
            this.ctxMenuSeparator2,
            this.ctxMenuViewProperties} );
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
			// ctxMenuRunInConsole
			// 
			this.ctxMenuRunInConsole.Image = global::Cfix.Addin.Icons.RunOnConsole;
			resources.ApplyResources( this.ctxMenuRunInConsole, "ctxMenuRunInConsole" );
			this.ctxMenuRunInConsole.Name = "ctxMenuRunInConsole";
			this.ctxMenuRunInConsole.Click += new System.EventHandler( this.ctxMenuRunOnConsole_Click );
			// 
			// ctxMenuDebugWithWindbg
			// 
			this.ctxMenuDebugWithWindbg.Image = global::Cfix.Addin.Icons.DebugWithWinDbg;
			resources.ApplyResources( this.ctxMenuDebugWithWindbg, "ctxMenuDebugWithWindbg" );
			this.ctxMenuDebugWithWindbg.Name = "ctxMenuDebugWithWindbg";
			this.ctxMenuDebugWithWindbg.Click += new System.EventHandler( this.ctxMenuDebugWithWindbg_Click );
			// 
			// ctxMenuRunInInspector
			// 
			this.ctxMenuRunInInspector.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.ctxMenuRunInInspectorCheckDeadlocks,
            this.ctxMenuRunInInspectorCheckDeadlocksOrRaces,
            this.ctxMenuRunInInspectorLocateDeadlocks,
            this.ctxMenuRunInInspectorComplete} );
			this.ctxMenuRunInInspector.Image = global::Cfix.Addin.Icons.IntelInspector;
			this.ctxMenuRunInInspector.Name = "ctxMenuRunInInspector";
			resources.ApplyResources( this.ctxMenuRunInInspector, "ctxMenuRunInInspector" );
			// 
			// ctxMenuRunInInspectorCheckDeadlocks
			// 
			this.ctxMenuRunInInspectorCheckDeadlocks.Image = global::Cfix.Addin.Icons.RunInInspector;
			this.ctxMenuRunInInspectorCheckDeadlocks.Name = "ctxMenuRunInInspectorCheckDeadlocks";
			resources.ApplyResources( this.ctxMenuRunInInspectorCheckDeadlocks, "ctxMenuRunInInspectorCheckDeadlocks" );
			this.ctxMenuRunInInspectorCheckDeadlocks.Click += new System.EventHandler( this.ctxMenuRunInInspectorCheckDeadlocks_Click );
			// 
			// ctxMenuRunInInspectorCheckDeadlocksOrRaces
			// 
			this.ctxMenuRunInInspectorCheckDeadlocksOrRaces.Image = global::Cfix.Addin.Icons.RunInInspector;
			this.ctxMenuRunInInspectorCheckDeadlocksOrRaces.Name = "ctxMenuRunInInspectorCheckDeadlocksOrRaces";
			resources.ApplyResources( this.ctxMenuRunInInspectorCheckDeadlocksOrRaces, "ctxMenuRunInInspectorCheckDeadlocksOrRaces" );
			this.ctxMenuRunInInspectorCheckDeadlocksOrRaces.Click += new System.EventHandler( this.ctxMenuRunInInspectorCheckDeadlocksOrRaces_Click );
			// 
			// ctxMenuRunInInspectorLocateDeadlocks
			// 
			this.ctxMenuRunInInspectorLocateDeadlocks.Image = global::Cfix.Addin.Icons.RunInInspector;
			this.ctxMenuRunInInspectorLocateDeadlocks.Name = "ctxMenuRunInInspectorLocateDeadlocks";
			resources.ApplyResources( this.ctxMenuRunInInspectorLocateDeadlocks, "ctxMenuRunInInspectorLocateDeadlocks" );
			this.ctxMenuRunInInspectorLocateDeadlocks.Click += new System.EventHandler( this.ctxMenuRunInInspectorLocateDeadlocks_Click );
			// 
			// ctxMenuRunInInspectorComplete
			// 
			this.ctxMenuRunInInspectorComplete.Image = global::Cfix.Addin.Icons.RunInInspector;
			this.ctxMenuRunInInspectorComplete.Name = "ctxMenuRunInInspectorComplete";
			resources.ApplyResources( this.ctxMenuRunInInspectorComplete, "ctxMenuRunInInspectorComplete" );
			this.ctxMenuRunInInspectorComplete.Click += new System.EventHandler( this.ctxMenuRunInInspectorComplete_Click );
			// 
			// ctxMenuSeparator4
			// 
			this.ctxMenuSeparator4.Name = "ctxMenuSeparator4";
			resources.ApplyResources( this.ctxMenuSeparator4, "ctxMenuSeparator4" );
			// 
			// ctxMenuRefreshButton
			// 
			this.ctxMenuRefreshButton.Image = global::Cfix.Addin.Icons.Refresh;
			resources.ApplyResources( this.ctxMenuRefreshButton, "ctxMenuRefreshButton" );
			this.ctxMenuRefreshButton.Name = "ctxMenuRefreshButton";
			// 
			// ctxMenuViewCodeButton
			// 
			this.ctxMenuViewCodeButton.Image = global::Cfix.Addin.Icons.Code;
			resources.ApplyResources( this.ctxMenuViewCodeButton, "ctxMenuViewCodeButton" );
			this.ctxMenuViewCodeButton.Name = "ctxMenuViewCodeButton";
			this.ctxMenuViewCodeButton.Click += new System.EventHandler( this.ctxMenuViewCodeButton_Click );
			// 
			// ctxMenuSeparator1
			// 
			this.ctxMenuSeparator1.Name = "ctxMenuSeparator1";
			resources.ApplyResources( this.ctxMenuSeparator1, "ctxMenuSeparator1" );
			// 
			// ctxMenuAddFixtureButton
			// 
			this.ctxMenuAddFixtureButton.Image = global::Cfix.Addin.Icons.Fixture;
			resources.ApplyResources( this.ctxMenuAddFixtureButton, "ctxMenuAddFixtureButton" );
			this.ctxMenuAddFixtureButton.Name = "ctxMenuAddFixtureButton";
			this.ctxMenuAddFixtureButton.Click += new System.EventHandler( this.ctxMenuAddFixtureButton_Click );
			// 
			// ctxMenuSeparator2
			// 
			this.ctxMenuSeparator2.Name = "ctxMenuSeparator2";
			resources.ApplyResources( this.ctxMenuSeparator2, "ctxMenuSeparator2" );
			// 
			// ctxMenuViewProperties
			// 
			this.ctxMenuViewProperties.Image = global::Cfix.Addin.Icons.Properties;
			resources.ApplyResources( this.ctxMenuViewProperties, "ctxMenuViewProperties" );
			this.ctxMenuViewProperties.Name = "ctxMenuViewProperties";
			this.ctxMenuViewProperties.Click += new System.EventHandler( this.ctxMenuViewProperties_Click );
			// 
			// explorer
			// 
			resources.ApplyResources( this.explorer, "explorer" );
			this.explorer.Name = "explorer";
			this.explorer.NodeContextMenu = null;
			this.explorer.NodeFactory = nodeFactory1;
			// 
			// infoLabel
			// 
			resources.ApplyResources( this.infoLabel, "infoLabel" );
			this.infoLabel.Name = "infoLabel";
			this.infoLabel.TabStop = false;
			// 
			// infoBar
			// 
			resources.ApplyResources( this.infoBar, "infoBar" );
			this.infoBar.BackColor = System.Drawing.Color.LightYellow;
			this.infoBar.Name = "infoBar";
			this.infoBar.ProgressBarColor = System.Drawing.Color.Blue;
			this.infoBar.Value = 0;
			// 
			// infoIcon
			// 
			this.infoIcon.BackColor = System.Drawing.Color.LightYellow;
			resources.ApplyResources( this.infoIcon, "infoIcon" );
			this.infoIcon.InitialImage = null;
			this.infoIcon.Name = "infoIcon";
			this.infoIcon.TabStop = false;
			// 
			// ExplorerWindow
			// 
			resources.ApplyResources( this, "$this" );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.infoIcon );
			this.Controls.Add( this.infoLabel );
			this.Controls.Add( this.infoBar );
			this.Controls.Add( this.explorer );
			this.Controls.Add( this.toolbar );
			this.Name = "ExplorerWindow";
			this.toolbar.ResumeLayout( false );
			this.toolbar.PerformLayout();
			this.ctxMenu.ResumeLayout( false );
			( ( System.ComponentModel.ISupportInitialize ) ( this.infoIcon ) ).EndInit();
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
		private System.Windows.Forms.ToolStripButton abortRefreshButton;
		private System.Windows.Forms.ContextMenuStrip ctxMenu;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuDebugButton;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRunButton;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRefreshButton;
		private System.Windows.Forms.ToolStripButton debugButton;
		private System.Windows.Forms.ToolStripButton runButton;
		private System.Windows.Forms.ToolStripSeparator separator2;
		private System.Windows.Forms.ToolStripDropDownButton optionsButton;
		private System.Windows.Forms.ToolStripMenuItem shortCircuitFixtureOnFailureMenuItem;
		private System.Windows.Forms.ToolStripMenuItem shortCircuitRunOnFailureMenuItem;
		private System.Windows.Forms.ToolStripSeparator separator3;
		private System.Windows.Forms.ToolStripSeparator separator4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton lameButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem captureStackTracesMenuItem;
		private System.Windows.Forms.ToolStripMenuItem refreshAfterBuildToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator ctxMenuSeparator1;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuAddFixtureButton;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuViewCodeButton;
		private System.Windows.Forms.ToolStripSeparator ctxMenuSeparator2;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuViewProperties;
		private TransparentLabel infoLabel;
		private PlainProgressBar infoBar;
		private System.Windows.Forms.PictureBox infoIcon;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRunInConsole;
		private System.Windows.Forms.ToolStripSeparator ctxMenuSeparator4;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuDebugWithWindbg;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem breakOnFailedAssertionsWhenDebuggingMenuItem;
		private System.Windows.Forms.ToolStripMenuItem breakOnUnhandledExceptionsWhenDebuggingMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRunInInspector;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRunInInspectorCheckDeadlocks;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRunInInspectorCheckDeadlocksOrRaces;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRunInInspectorLocateDeadlocks;
		private System.Windows.Forms.ToolStripMenuItem ctxMenuRunInInspectorComplete;
	}
}
