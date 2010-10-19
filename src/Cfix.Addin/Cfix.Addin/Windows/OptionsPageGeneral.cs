using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using System.Diagnostics;
using EnvDTE80;
using Cfix.Control;

namespace Cfix.Addin.Windows
{
	public partial class OptionsPageGeneral : UserControl, IDTToolsOptionsPage
	{
		private DTE2 dte;
		private Configuration configuration;

		private class EventDllData
		{
			public readonly Architecture arch;
			public EventDll eventDll;

			public EventDllData( Architecture arch, EventDll eventDll )
			{
				this.arch = arch;
				this.eventDll = eventDll;
			}

			public override string ToString()
			{
				return this.arch.ToString();
			}
		}

		public OptionsPageGeneral()
		{
			InitializeComponent();
		}

		private void LoadEventDllFields()
		{
			EventDllData data = ( EventDllData ) this.archComboBox.SelectedItem;
			if ( data.eventDll == null )
			{
				this.dllTextBox.Text = "";
				this.optionsTextBox.Text = "";
			}
			else
			{
				this.dllTextBox.Text = data.eventDll.Path ?? "";
				this.optionsTextBox.Text = data.eventDll.Options ?? "";
			}
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

			EnvironmentOptions envOpts = this.configuration.EnvironmentOptions;

			this.autoAdjustCwd.Checked =
				( ( envOpts & EnvironmentOptions.AutoAdjustCurrentDirectory ) != 0 );
			this.autoRegisterVcDirs.Checked =
				this.configuration.AutoRegisterVcDirectories;

			this.archComboBox.Items.Clear();
			this.archComboBox.Items.Add( 
				new EventDllData( 
					Architecture.I386,
					this.configuration.GetEventDll( Architecture.I386 ) ) );
			if ( ArchitectureUtil.NativeArchitecture == Architecture.Amd64 )
			{
				this.archComboBox.Items.Add(
					new EventDllData( 
						Architecture.Amd64,
						this.configuration.GetEventDll( Architecture.Amd64 ) ) );
			}

			this.archComboBox.SelectedIndex = 0;
			LoadEventDllFields();
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

			EnvironmentOptions envOpts = this.configuration.EnvironmentOptions;

			envOpts |= EnvironmentOptions.ComNeutralThreading;

			if ( this.autoAdjustCwd.Checked )
			{
				envOpts |= EnvironmentOptions.AutoAdjustCurrentDirectory;
			}

			this.configuration.EnvironmentOptions = envOpts;

			this.configuration.AutoRegisterVcDirectories = 
				this.autoRegisterVcDirs.Checked;

			foreach ( EventDllData data in this.archComboBox.Items )
			{
				this.configuration.SetEventDll(
					data.arch,
					data.eventDll );
			}
		}

		private void archComboBox_SelectedIndexChanged( object sender, EventArgs e )
		{
			LoadEventDllFields();
		}

		private void browseDllButton_Click( object sender, EventArgs e )
		{
			using ( OpenFileDialog dlg = new OpenFileDialog() )
			{
				dlg.Filter =  "Dynamic Link Libraries|*.dll";
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.Multiselect = false;
				dlg.ValidateNames = true;

				dlg.ShowDialog();

				this.dllTextBox.Text = dlg.FileName;
			}
		}

		private void dllTextBox_TextChanged( object sender, EventArgs e )
		{
			EventDllData data = ( EventDllData ) this.archComboBox.SelectedItem;

			data.eventDll = new EventDll(
				this.dllTextBox.Text,
				data.eventDll != null ? data.eventDll.Options : null );

			if ( String.IsNullOrEmpty( data.eventDll.Path ) &&
				 String.IsNullOrEmpty( data.eventDll.Options ) )
			{
				data.eventDll = null;
			}
		}

		private void optionsTextBox_TextChanged( object sender, EventArgs e )
		{
			EventDllData data = ( EventDllData ) this.archComboBox.SelectedItem;

			data.eventDll = new EventDll(
				data.eventDll != null ? data.eventDll.Path : null,
				this.optionsTextBox.Text );

			if ( String.IsNullOrEmpty( data.eventDll.Path ) &&
				 String.IsNullOrEmpty( data.eventDll.Options ) )
			{
				data.eventDll = null;
			}
		}
	}
}
