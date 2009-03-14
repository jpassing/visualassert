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
		public const int InconclusiveneIconIndex = 8;
		public const int UnhandledExceptionIconIndex = 9;
		public const int FailedAssertionIconIndex = 9;

		private readonly ResultModel model;

		private readonly NodeIcon iconBinding = new NodeIcon();
		private readonly NodeTextBox statusBinding = new NodeTextBox();
		private readonly NodeTextBox nameBinding = new NodeTextBox();
		private readonly NodeTextBox expressionBinding = new NodeTextBox();
		private readonly NodeTextBox messageBinding = new NodeTextBox();
		private readonly NodeTextBox locationBinding = new NodeTextBox();
		private readonly NodeTextBox routineBinding = new NodeTextBox();
		private readonly NodeTextBox lastErrorBinding = new NodeTextBox();

		public ResultExplorer()
		{
			InitializeComponent();

			this.model = new ResultModel( this.icons, this.tree );
			this.tree.Model = model;

			//
			// Bind node fields to columns.
			//
			this.iconBinding.DataPropertyName = "Icon";
			this.iconBinding.ParentColumn = this.colName;
			this.iconBinding.LeftMargin = 3;
			this.tree.NodeControls.Add( this.iconBinding );

			this.nameBinding.DataPropertyName = "Name";
			this.nameBinding.IncrementalSearchEnabled = true;
			this.nameBinding.ParentColumn = this.colName;
			this.nameBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.nameBinding.UseCompatibleTextRendering = true;
			this.nameBinding.LeftMargin = 3;
			this.nameBinding.EditEnabled = false;
			this.tree.NodeControls.Add( this.nameBinding );

			this.statusBinding.DataPropertyName = "Status";
			this.statusBinding.ParentColumn = this.colStatus;
			this.statusBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.statusBinding.UseCompatibleTextRendering = true;
			this.statusBinding.EditEnabled = false; 
			this.tree.NodeControls.Add( this.statusBinding );

			this.expressionBinding.DataPropertyName = "Expression";
			this.expressionBinding.IncrementalSearchEnabled = true;
			this.expressionBinding.ParentColumn = this.colExpression;
			this.expressionBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.expressionBinding.UseCompatibleTextRendering = true;
			this.expressionBinding.EditEnabled = false; 
			this.tree.NodeControls.Add( this.expressionBinding );

			this.messageBinding.DataPropertyName = "Message";
			this.messageBinding.IncrementalSearchEnabled = true;
			this.messageBinding.ParentColumn = this.colMessage;
			this.messageBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.messageBinding.UseCompatibleTextRendering = true;
			this.messageBinding.EditEnabled = false; 
			this.tree.NodeControls.Add( this.messageBinding );

			this.locationBinding.DataPropertyName = "Location";
			this.locationBinding.ParentColumn = this.colLocation;
			this.locationBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.locationBinding.UseCompatibleTextRendering = true;
			this.locationBinding.EditEnabled = false; 
			this.tree.NodeControls.Add( this.locationBinding );

			this.routineBinding.DataPropertyName = "Routine";
			this.routineBinding.ParentColumn = this.colRoutine;
			this.routineBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.routineBinding.UseCompatibleTextRendering = true;
			this.routineBinding.EditEnabled = false; 
			this.tree.NodeControls.Add( this.routineBinding );

			this.lastErrorBinding.DataPropertyName = "LastError";
			this.lastErrorBinding.ParentColumn = this.colLastError;
			this.lastErrorBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.lastErrorBinding.UseCompatibleTextRendering = true;
			this.lastErrorBinding.EditEnabled = false; 
			this.tree.NodeControls.Add( this.lastErrorBinding );

		}

		public IRun Run
		{
			get { return this.model.Run; }
			set { this.model.Run = value; }
		}
	}
}
