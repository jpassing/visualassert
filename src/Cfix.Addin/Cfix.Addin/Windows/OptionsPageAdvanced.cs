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
		}
	}
}
