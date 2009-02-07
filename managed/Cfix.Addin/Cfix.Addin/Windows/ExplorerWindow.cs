using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Cfix.Addin.Windows
{
	public partial class ExplorerWindow : UserControl
	{
		public static readonly Guid Guid = new Guid( "e89c09c9-4e89-4ae2-b328-79dcbdfd852c" );

		public ExplorerWindow()
		{
			InitializeComponent();
		}
	}
}
