namespace Cfix.Control.Ui.Explorer
{
	partial class Explorer
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( Explorer ) );
			this.treeView = new System.Windows.Forms.TreeView();
			this.imageList = new System.Windows.Forms.ImageList( this.components );
			this.SuspendLayout();
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.HotTracking = true;
			this.treeView.ImageIndex = 0;
			this.treeView.ImageList = this.imageList;
			this.treeView.Location = new System.Drawing.Point( 0, 0 );
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = 0;
			this.treeView.ShowNodeToolTips = true;
			this.treeView.Size = new System.Drawing.Size( 265, 303 );
			this.treeView.StateImageList = this.imageList;
			this.treeView.TabIndex = 0;
			// 
			// imageList
			// 
			this.imageList.ImageStream = ( ( System.Windows.Forms.ImageListStreamer ) ( resources.GetObject( "imageList.ImageStream" ) ) );
			this.imageList.TransparentColor = System.Drawing.Color.Magenta;
			this.imageList.Images.SetKeyName( 0, "VSFolder_closed.bmp" );
			this.imageList.Images.SetKeyName( 1, "VSFolder_open.bmp" );
			this.imageList.Images.SetKeyName( 2, "VSObject_Assembly.bmp" );
			this.imageList.Images.SetKeyName( 3, "VSObject_Class.bmp" );
			this.imageList.Images.SetKeyName( 4, "VSObject_Method.bmp" );
			this.imageList.Images.SetKeyName( 5, "VSObject_InvalidAssembly.bmp" );
			// 
			// Explorer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.treeView );
			this.Name = "Explorer";
			this.Size = new System.Drawing.Size( 265, 303 );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.ImageList imageList;
	}
}
