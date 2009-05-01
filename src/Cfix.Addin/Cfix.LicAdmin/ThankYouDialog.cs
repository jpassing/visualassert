using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Cfix.LicAdmin
{
	public partial class ThankYouDialog : Form
	{
		public ThankYouDialog()
		{
			InitializeComponent();
		}

		private void closeButton_Click( object sender, EventArgs e )
		{
			Close();
		}

	}
}