using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree.NodeControls;

namespace Cfix.Control.Ui.Result
{
	public partial class ResultExplorer : UserControl
	{
		private readonly ResultModel model = new ResultModel();
		private readonly NodeTextBox nameBinding = new NodeTextBox();
		private readonly NodeTextBox locationBinding = new NodeTextBox();
		private readonly NodeTextBox failuresBinding = new NodeTextBox();
		private readonly NodeTextBox statusBinding = new NodeTextBox();

		public ResultExplorer()
		{
			InitializeComponent();

			this.tree.Model = model;

			//
			// Bind node fields to columns.
			//
			this.nameBinding.DataPropertyName = "Name";
			this.nameBinding.IncrementalSearchEnabled = true;
			this.nameBinding.ParentColumn = this.colName;
			this.nameBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.nameBinding.UseCompatibleTextRendering = true;
			this.tree.NodeControls.Add( this.nameBinding );

			this.locationBinding.DataPropertyName = "Location";
			this.locationBinding.ParentColumn = this.colLocation;
			this.locationBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.locationBinding.UseCompatibleTextRendering = true;
			this.tree.NodeControls.Add( this.locationBinding );

			this.failuresBinding.DataPropertyName = "Failures";
			this.failuresBinding.ParentColumn = this.colFailures;
			this.tree.NodeControls.Add( this.failuresBinding );

			this.statusBinding.DataPropertyName = "Status";
			this.statusBinding.ParentColumn = this.colLocation;
			this.statusBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.statusBinding.UseCompatibleTextRendering = true;
			this.tree.NodeControls.Add( this.statusBinding );
		}

		public IRun Run
		{
			get { return this.model.Run; }
			set { this.model.Run = value; }
		}
	}
}
