using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Cfix.Control.Ui.Explorer;

namespace QuickTest
{
	public partial class ExplorerForm : Form
	{
		public TestExplorer Explorer
		{
			get
			{
				return this.explorer;
			}
		}

		public ExplorerForm()
		{
			InitializeComponent();

			this.explorer.AfterSelected += delegate(
				Object sender,
				TestExplorer.ExplorerNodeEventArgs args
				)
			{
				this.currentNodeLabel.Text = args.Item.Name;
			};
		}

		private void refresh_Click( object sender, EventArgs e )
		{
			Explorer.RefreshSession( true );
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if ( this.progressLabel.Text.Length > 40 )
			{
				this.progressLabel.Text = "";
			}
			else
			{
				this.progressLabel.Text += ".";
			}
		}

		private void abortBtn_Click( object sender, EventArgs e )
		{
			this.explorer.AbortRefreshSession();
		}



	}
}