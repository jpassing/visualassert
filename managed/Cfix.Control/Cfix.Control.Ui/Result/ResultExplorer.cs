using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace Cfix.Control.Ui.Result
{
	public partial class ResultExplorer : UserControl
	{
		public const int InconclusiveneIconIndex = 9;
		public const int UnhandledExceptionIconIndex = 10;
		public const int FailedAssertionIconIndex = 10;

		public event EventHandler<ContextMenuEventArgs> ContextMenuRequested;

		private readonly ResultModel model;

		private readonly NodeIcon iconBinding = new NodeIcon();
		private readonly TextNodeControl statusBinding;
		private readonly TextNodeControl nameBinding;
		private readonly TextNodeControl expressionBinding;
		private readonly TextNodeControl messageBinding;
		private readonly TextNodeControl locationBinding;
		private readonly TextNodeControl routineBinding;
		private readonly TextNodeControl durationBinding;
		private readonly TextNodeControl lastErrorBinding;

		private ContextMenuStrip resultNodeContextMenu;
		private ContextMenuStrip failureNodeContextMenu;

		public ResultExplorer()
		{
			InitializeComponent();

			this.model = new ResultModel( this.icons, this.tree );
			this.tree.Model = model;

			//
			// Bind node fields to columns.
			//
			this.iconBinding = new NodeIcon();
			this.iconBinding.DataPropertyName = "Icon";
			this.iconBinding.ParentColumn = this.colName;
			this.iconBinding.LeftMargin = 3;
			this.tree.NodeControls.Add( this.iconBinding );

			this.nameBinding = new TextNodeControl( this );
			this.nameBinding.DataPropertyName = "Name";
			this.nameBinding.IncrementalSearchEnabled = true;
			this.nameBinding.ParentColumn = this.colName;
			this.nameBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.nameBinding.UseCompatibleTextRendering = true;
			this.nameBinding.LeftMargin = 3;
			this.nameBinding.EditEnabled = false;
			this.nameBinding.DisplayHiddenContentInToolTip = true;
			this.nameBinding.ToolTipProvider = new ToolTipProvider(
				delegate( TreeNodeAdv node )
				{
					IResultNode resNode = node.Tag as IResultNode;
					return ( resNode != null ) ? resNode.Name : null;
				} );
			this.tree.NodeControls.Add( this.nameBinding );

			this.statusBinding = new TextNodeControl( this );
			this.statusBinding.DataPropertyName = "Status";
			this.statusBinding.ParentColumn = this.colStatus;
			this.statusBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.statusBinding.UseCompatibleTextRendering = true;
			this.statusBinding.EditEnabled = false;
			this.statusBinding.ToolTipProvider = new ToolTipProvider(
				delegate( TreeNodeAdv node )
				{
					IResultNode resNode = node.Tag as IResultNode;
					return ( resNode != null ) ? resNode.Status : null;
				} );
			this.tree.NodeControls.Add( this.statusBinding );

			this.expressionBinding = new TextNodeControl( this );
			this.expressionBinding.DataPropertyName = "Expression";
			this.expressionBinding.IncrementalSearchEnabled = true;
			this.expressionBinding.ParentColumn = this.colExpression;
			this.expressionBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.expressionBinding.UseCompatibleTextRendering = true;
			this.expressionBinding.EditEnabled = false;
			this.expressionBinding.ToolTipProvider = new ToolTipProvider(
				delegate( TreeNodeAdv node )
				{
					IResultNode resNode = node.Tag as IResultNode;
					return ( resNode != null ) ? resNode.Expression : null;
				} );
			this.tree.NodeControls.Add( this.expressionBinding );

			this.messageBinding = new TextNodeControl( this );
			this.messageBinding.DataPropertyName = "Message";
			this.messageBinding.IncrementalSearchEnabled = true;
			this.messageBinding.ParentColumn = this.colMessage;
			this.messageBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.messageBinding.UseCompatibleTextRendering = true;
			this.messageBinding.EditEnabled = false;
			this.messageBinding.ToolTipProvider = new ToolTipProvider(
				delegate( TreeNodeAdv node )
				{
					IResultNode resNode = node.Tag as IResultNode;
					return ( resNode != null ) ? resNode.Message : null;
				} );
			this.tree.NodeControls.Add( this.messageBinding );

			this.locationBinding = new TextNodeControl( this );
			this.locationBinding.DataPropertyName = "Location";
			this.locationBinding.ParentColumn = this.colLocation;
			this.locationBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.locationBinding.UseCompatibleTextRendering = true;
			this.locationBinding.EditEnabled = false;
			this.locationBinding.ToolTipProvider = new ToolTipProvider(
				delegate( TreeNodeAdv node )
				{
					IResultNode resNode = node.Tag as IResultNode;
					return ( resNode != null ) ? resNode.Location : null;
				} );
			this.tree.NodeControls.Add( this.locationBinding );

			this.routineBinding = new TextNodeControl( this );
			this.routineBinding.DataPropertyName = "Routine";
			this.routineBinding.ParentColumn = this.colRoutine;
			this.routineBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.routineBinding.UseCompatibleTextRendering = true;
			this.routineBinding.EditEnabled = false;
			this.routineBinding.ToolTipProvider = new ToolTipProvider(
				delegate( TreeNodeAdv node )
				{
					IResultNode resNode = node.Tag as IResultNode;
					return ( resNode != null ) ? resNode.Routine : null;
				} );
			this.tree.NodeControls.Add( this.routineBinding );

			this.durationBinding = new TextNodeControl( this );
			this.durationBinding.DataPropertyName = "Duration";
			this.durationBinding.ParentColumn = this.colDuration;
			this.durationBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.durationBinding.UseCompatibleTextRendering = true;
			this.durationBinding.EditEnabled = false;
			this.durationBinding.ToolTipProvider = new ToolTipProvider(
				delegate( TreeNodeAdv node )
				{
					IResultNode resNode = node.Tag as IResultNode;
					return ( resNode != null ) ? resNode.Duration : null;
				} );
			this.tree.NodeControls.Add( this.durationBinding );

			this.lastErrorBinding = new TextNodeControl( this );
			this.lastErrorBinding.DataPropertyName = "LastError";
			this.lastErrorBinding.ParentColumn = this.colLastError;
			this.lastErrorBinding.Trimming = StringTrimming.EllipsisCharacter;
			this.lastErrorBinding.UseCompatibleTextRendering = true;
			this.lastErrorBinding.EditEnabled = false;
			this.lastErrorBinding.ToolTipProvider = new ToolTipProvider(
				delegate( TreeNodeAdv node )
				{
					IResultNode resNode = node.Tag as IResultNode;
					return ( resNode != null ) ? resNode.LastError : null;
				} );
			this.tree.NodeControls.Add( this.lastErrorBinding );

		}

		/*----------------------------------------------------------------------
		 * Internal.
		 */

		internal TreeViewAdv Tree
		{
			get { return this.tree; }
		}

		internal void OnContextMenuRequested(
			IResultNode node,
			Point location
			)
		{
			if ( this.ContextMenuRequested != null )
			{
				this.ContextMenuRequested(
					node,
					new ContextMenuEventArgs( location ) );
			}
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */ 

		public IRun Run
		{
			get { return this.model.Run; }
			set { this.model.Run = value; }
		}

		public ContextMenuStrip ResultNodeContextMenu
		{
			get { return this.resultNodeContextMenu; }
			set { this.resultNodeContextMenu = value; }
		}

		public ContextMenuStrip FailureNodeContextMenu
		{
			get { return this.failureNodeContextMenu; }
			set { this.failureNodeContextMenu = value; }
		}
	}

	public class ContextMenuEventArgs : EventArgs
	{
		private readonly Point location;

		public ContextMenuEventArgs(
			Point location
			)
		{
			this.location = location;
		}

		public Point Location
		{
			get { return location; }
		} 
	}
}
