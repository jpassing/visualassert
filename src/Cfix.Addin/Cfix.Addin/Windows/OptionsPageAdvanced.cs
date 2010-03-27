using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using System.Diagnostics;
using Cfix.Control;

namespace Cfix.Addin.Windows
{
	public partial class OptionsPageAdvanced : UserControl, IDTToolsOptionsPage
	{
		private DTE2 dte;
		private Configuration configuration;

		public OptionsPageAdvanced()
		{
			InitializeComponent();
		}

		/*----------------------------------------------------------------------
		 * IDTToolsOptionsPage Members.
		 */

		public void GetProperties( ref object propertiesObject )
		{
			Debug.Assert( this.configuration != null );

			propertiesObject = this.configuration;
		}

		public void OnAfterCreated( DTE dte )
		{
			this.dte = ( DTE2 ) dte;
			this.configuration = Configuration.Load( this.dte );

			this.hostRegTimeout.Value = 
				this.configuration.HostRegistrationTimeout / 1000;
			this.instrumentedHostRegTimeout.Value =
				this.configuration.InstrumentedHostRegistrationTimeout / 1000;

			EnvironmentOptions envOpts = this.configuration.EnvironmentOptions;

			this.autoAdjustCwd.Checked =
				( ( envOpts & EnvironmentOptions.AutoAdjustCurrentDirectory ) != 0 );
			this.autoRegisterVcDirs.Checked =
				this.configuration.AutoRegisterVcDirectories;

			if ( ( envOpts & EnvironmentOptions.HugeStack ) != 0 )
			{
				this.stackSizeHuge.Checked = true;
			}
			else if ( ( envOpts & EnvironmentOptions.LargeStack ) != 0 )
			{
				this.stackSizeLarge.Checked = true;
			}
			else
			{
				this.stackSizeStd.Checked = true;
			}
		}

		public void OnCancel()
		{
			Debug.Assert( this.configuration != null );
		}

		public void OnHelp()
		{
			Debug.Assert( this.configuration != null );
		}

		public void OnOK()
		{
			Debug.Assert( this.configuration != null );

			this.configuration.HostRegistrationTimeout = 
				( uint ) this.hostRegTimeout.Value * 1000;
			this.configuration.InstrumentedHostRegistrationTimeout = 
				( uint ) this.instrumentedHostRegTimeout.Value * 1000;

			EnvironmentOptions envOpts = EnvironmentOptions.ComNeutralThreading;
			if ( this.autoAdjustCwd.Checked )
			{
				envOpts |= EnvironmentOptions.AutoAdjustCurrentDirectory;
			}

			if ( this.stackSizeHuge.Checked )
			{
				envOpts |= EnvironmentOptions.HugeStack;
			}
			else if ( this.stackSizeLarge.Checked )
			{
				envOpts |= EnvironmentOptions.LargeStack;
			}

			this.configuration.EnvironmentOptions = envOpts;

			this.configuration.AutoRegisterVcDirectories = 
				this.autoRegisterVcDirs.Checked;
		}
	}
}
