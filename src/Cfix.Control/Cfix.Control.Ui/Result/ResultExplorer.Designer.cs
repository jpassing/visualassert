namespace Cfix.Control.Ui.Result
{
	partial class ResultExplorer
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ResultExplorer ) );
			this.tree = new Aga.Controls.Tree.TreeViewAdv();
			this.colName = new Aga.Controls.Tree.TreeColumn();
			this.colExpression = new Aga.Controls.Tree.TreeColumn();
			this.colMessage = new Aga.Controls.Tree.TreeColumn();
			this.colLocation = new Aga.Controls.Tree.TreeColumn();
			this.colStatus = new Aga.Controls.Tree.TreeColumn();
			this.colDuration = new Aga.Controls.Tree.TreeColumn();
			this.colLastError = new Aga.Controls.Tree.TreeColumn();
			this.icons = new System.Windows.Forms.ImageList( this.components );
			this.SuspendLayout();
			// 
			// tree
			// 
			this.tree.AllowColumnReorder = true;
			this.tree.BackColor = System.Drawing.SystemColors.Window;
			this.tree.Columns.Add( this.colName );
			this.tree.Columns.Add( this.colExpression );
			this.tree.Columns.Add( this.colMessage );
			this.tree.Columns.Add( this.colLocation );
			this.tree.Columns.Add( this.colStatus );
			this.tree.Columns.Add( this.colDuration );
			this.tree.Columns.Add( this.colLastError );
			this.tree.DefaultToolTipProvider = null;
			this.tree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tree.DragDropMarkColor = System.Drawing.Color.Black;
			this.tree.FullRowSelect = true;
			this.tree.GridLineStyle = ( ( Aga.Controls.Tree.GridLineStyle ) ( ( Aga.Controls.Tree.GridLineStyle.Horizontal | Aga.Controls.Tree.GridLineStyle.Vertical ) ) );
			this.tree.LineColor = System.Drawing.SystemColors.ControlDark;
			this.tree.LoadOnDemand = true;
			this.tree.Location = new System.Drawing.Point( 0, 0 );
			this.tree.Model = null;
			this.tree.Name = "tree";
			this.tree.RowHeight = 18;
			this.tree.SelectedNode = null;
			this.tree.ShowNodeToolTips = true;
			this.tree.Size = new System.Drawing.Size( 488, 314 );
			this.tree.TabIndex = 0;
			this.tree.UseColumns = true;
			this.tree.SelectionChanged += new System.EventHandler( this.tree_SelectionChanged );
			// 
			// colName
			// 
			this.colName.Header = "Name";
			this.colName.SortOrder = System.Windows.Forms.SortOrder.None;
			this.colName.TooltipText = null;
			this.colName.Width = 180;
			// 
			// colExpression
			// 
			this.colExpression.Header = "Expression";
			this.colExpression.SortOrder = System.Windows.Forms.SortOrder.None;
			this.colExpression.TooltipText = null;
			this.colExpression.Width = 200;
			// 
			// colMessage
			// 
			this.colMessage.Header = "Message";
			this.colMessage.SortOrder = System.Windows.Forms.SortOrder.None;
			this.colMessage.TooltipText = null;
			this.colMessage.Width = 180;
			// 
			// colLocation
			// 
			this.colLocation.Header = "Location";
			this.colLocation.SortOrder = System.Windows.Forms.SortOrder.None;
			this.colLocation.TooltipText = null;
			this.colLocation.Width = 180;
			// 
			// colStatus
			// 
			this.colStatus.Header = "Status";
			this.colStatus.SortOrder = System.Windows.Forms.SortOrder.None;
			this.colStatus.TooltipText = null;
			this.colStatus.Width = 80;
			// 
			// colDuration
			// 
			this.colDuration.Header = "Duration";
			this.colDuration.SortOrder = System.Windows.Forms.SortOrder.None;
			this.colDuration.TooltipText = null;
			this.colDuration.Width = 100;
			// 
			// colLastError
			// 
			this.colLastError.Header = "Last Win32 error";
			this.colLastError.SortOrder = System.Windows.Forms.SortOrder.None;
			this.colLastError.TooltipText = null;
			// 
			// icons
			// 
			this.icons.ImageStream = ( ( System.Windows.Forms.ImageListStreamer ) ( resources.GetObject( "icons.ImageStream" ) ) );
			this.icons.TransparentColor = System.Drawing.Color.Magenta;
			this.icons.Images.SetKeyName( 0, "Status_Pending.bmp" );
			this.icons.Images.SetKeyName( 1, "Status_Running.bmp" );
			this.icons.Images.SetKeyName( 2, "Status_Skipped.bmp" );
			this.icons.Images.SetKeyName( 3, "Status_Stopped.bmp" );
			this.icons.Images.SetKeyName( 4, "Status_Succeeded.bmp" );
			this.icons.Images.SetKeyName( 5, "Status_SucceededWithInconclusiveParts.bmp" );
			this.icons.Images.SetKeyName( 6, "Status_SucceededWithSkippedParts.bmp" );
			this.icons.Images.SetKeyName( 7, "Status_Failed.bmp" );
			this.icons.Images.SetKeyName( 8, "Status_Inconclusive.bmp" );
			this.icons.Images.SetKeyName( 9, "Status_PostprocessingFailed.bmp" );
			this.icons.Images.SetKeyName( 10, "Inconclusive.bmp" );
			this.icons.Images.SetKeyName( 11, "Failure.bmp" );
			this.icons.Images.SetKeyName( 12, "StackFrame.bmp" );
			this.icons.Images.SetKeyName( 13, "Information.bmp" );
			this.icons.Images.SetKeyName( 14, "Warning.bmp" );
			this.icons.Images.SetKeyName( 15, "Error.bmp" );
			// 
			// ResultExplorer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.tree );
			this.Name = "ResultExplorer";
			this.Size = new System.Drawing.Size( 488, 314 );
			this.ResumeLayout( false );

		}

		#endregion

		private Aga.Controls.Tree.TreeViewAdv tree;
		private Aga.Controls.Tree.TreeColumn colName;
		private Aga.Controls.Tree.TreeColumn colLocation;
		private Aga.Controls.Tree.TreeColumn colStatus;
		private Aga.Controls.Tree.TreeColumn colMessage;
		private Aga.Controls.Tree.TreeColumn colExpression;
		private Aga.Controls.Tree.TreeColumn colLastError;
		private System.Windows.Forms.ImageList icons;
		private Aga.Controls.Tree.TreeColumn colDuration;
	}
}
