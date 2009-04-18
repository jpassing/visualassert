using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using Cfix.Control;

namespace Cfix.Addin.Windows.About
{
	public partial class AboutWindow : Form
	{
		private void PopulateFileVersionsList( Architecture arch )
		{
			string dir = Directories.GetBinDirectory( arch );
			if ( !Directory.Exists( dir ) )
			{
				return;
			}

			foreach ( string file in Directory.GetFiles( dir ) )
			{
				if ( file.EndsWith( ".sys", StringComparison.OrdinalIgnoreCase ) ||
					 file.EndsWith( ".dll", StringComparison.OrdinalIgnoreCase ) ||
					 file.EndsWith( ".exe", StringComparison.OrdinalIgnoreCase ) )
				{
					Native.CDIAG_MODULE_VERSION version =
						new	Native.CDIAG_MODULE_VERSION();
					string versionString = String.Empty;
					if ( 0 == Native.CdiagGetModuleVersion(
						file,
						ref version ) )
					{
						versionString = version.ToString();
					}

					this.fileVersionsList.Items.Add(
						new ListViewItem(
							new string[] {
								new FileInfo( file ).Name,
								arch.ToString(),
								versionString } ) );
				}
			}
		}

		private void PopulateFileVersionsList()
		{
			PopulateFileVersionsList( Architecture.I386 );
			PopulateFileVersionsList( Architecture.Amd64 );
		}

		public AboutWindow()
		{
			InitializeComponent();

			Version version = GetType().Assembly.GetName().Version;
			this.versionLabel.Text += String.Format(
				"{0}.{1}.{2} Build {3}",
				version.Major,
				version.Minor,
				version.MajorRevision,
				version.MinorRevision );

			try
			{
				PopulateFileVersionsList();
			}
			catch
			{ }
		}

		private void linkLabel_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			try
			{
				Process proc = new Process();
				proc.StartInfo.FileName = "http://www.cfix-studio.com/";
				proc.Start();
			}
			catch ( Exception x )
			{
				CfixPlus.HandleError( x );
			}
		}
	}
}