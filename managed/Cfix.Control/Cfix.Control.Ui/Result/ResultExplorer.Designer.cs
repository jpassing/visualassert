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
			this.tree = new Aga.Controls.Tree.TreeViewAdv();
			this.colName = new Aga.Controls.Tree.TreeColumn();
			this.colLocation = new Aga.Controls.Tree.TreeColumn();
			this.colStatus = new Aga.Controls.Tree.TreeColumn();
			this.colFailures = new Aga.Controls.Tree.TreeColumn();
			this.SuspendLayout();
			// 
			// tree
			// 
			this.tree.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.tree.BackColor = System.Drawing.SystemColors.Window;
			this.tree.Columns.Add( this.colName );
			this.tree.Columns.Add( this.colLocation );
			this.tree.Columns.Add( this.colStatus );
			this.tree.Columns.Add( this.colFailures );
			this.tree.DefaultToolTipProvider = null;
			this.tree.DragDropMarkColor = System.Drawing.Color.Black;
			this.tree.FullRowSelect = true;
			this.tree.GridLineStyle = ( ( Aga.Controls.Tree.GridLineStyle ) ( ( Aga.Controls.Tree.GridLineStyle.Horizontal | Aga.Controls.Tree.GridLineStyle.Vertical ) ) );
			this.tree.LineColor = System.Drawing.SystemColors.ControlDark;
			this.tree.LoadOnDemand = true;
			this.tree.Location = new System.Drawing.Point( 3, 34 );
			this.tree.Model = null;
			this.tree.Name = "tree";
			this.tree.SelectedNode = null;
			this.tree.Size = new System.Drawing.Size( 482, 277 );
			this.tree.TabIndex = 0;
			this.tree.UseColumns = true;
			// 
			// colName
			// 
			this.colName.Header = "Name";
			this.colName.SortOrder = System.Windows.Forms.SortOrder.None;
			this.colName.TooltipText = null;
			this.colName.Width = 300;
			// 
			// colLocation
			// 
			this.colLocation.Header = "Location";
			this.colLocation.SortOrder = System.Windows.Forms.SortOrder.None;
			this.colLocation.TooltipText = null;
			this.colLocation.Width = 300;
			// 
			// colStatus
			// 
			this.colStatus.Header = "Status";
			this.colStatus.SortOrder = System.Windows.Forms.SortOrder.None;
			this.colStatus.TooltipText = null;
			this.colStatus.Width = 80;
			// 
			// colFailures
			// 
			this.colFailures.Header = "Failures";
			this.colFailures.SortOrder = System.Windows.Forms.SortOrder.None;
			this.colFailures.TooltipText = null;
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
		private Aga.Controls.Tree.TreeColumn colFailures;
	}
}
