using System;
using System.IO;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;
using Cfix.Control.Native;
using Microsoft.VisualStudio.VCProjectEngine;

namespace Cfix.Addin.Windows.Explorer
{
	internal class VCProjectTestCollection : GenericTestItemCollection
	{
		private readonly Configuration config;
		private readonly string uniqueName;
		private readonly Project project;
		private readonly MultiTarget target;

		private readonly object loadLock = new object();

		//
		// Current path to module -- this changes whenever the active
		// solution configuration changes.
		//
		private string currentPath;

		private VCConfiguration CurrentConfiguration
		{
			get
			{
				VCProject vcProject = this.project.Object as VCProject;
				Debug.Assert( vcProject != null );

				EnvDTE.Configuration activeConfig =
					this.project.ConfigurationManager.ActiveConfiguration;

				IVCCollection vcConfigs = ( IVCCollection ) vcProject.Configurations;
				foreach ( VCConfiguration vcConfig in vcConfigs )
				{
					if ( activeConfig.ConfigurationName == vcConfig.ConfigurationName &&
						 activeConfig.PlatformName == ( ( VCPlatform ) vcConfig.Platform ).Name )
					{
						return vcConfig;
					}
				}

				Debug.Fail( "Missing configuration" );
				throw new CfixAddinException( Strings.FailedToObtainCurrentConfig );
			}
		}

		private static Architecture GetArchitecture( VCConfiguration config )
		{
			string plafName = ( ( VCPlatform ) config.Platform ).Name;
			switch ( plafName )
			{
				case "Win32":
					return Architecture.I386;

				case "x64":
					return Architecture.Amd64;

				default:
					throw new CfixAddinException(
						String.Format( Strings.UnrecognizedPlatform, plafName ) );
			}
		}

		private void LoadPrimaryOutputModule( VCConfiguration vcConfig )
		{
			lock ( this.loadLock )
			{
				Architecture arch = GetArchitecture( vcConfig );

				Debug.Assert( this.target.IsArchitectureSupported( arch ) );

				this.currentPath = vcConfig.PrimaryOutput;
				Debug.Print( this.currentPath );

				if ( File.Exists( this.currentPath ) &&
					 this.config.IsSupportedTestModulePath( this.currentPath ) )
				{
					ITestItem module;
					try
					{
						module = TestModule.LoadModule(
							this.target.GetTarget( arch ),
							this.currentPath,
							true );
					}
					catch ( Exception x )
					{
						module = new InvalidModule(
							this,
							new FileInfo( this.currentPath ).Name,
							x );
					}

					Add( module );
				}
			}
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public VCProjectTestCollection(
			Project project,
			MultiTarget target,
			Configuration config
			)
			: base( null, project.Name )
		{
			this.uniqueName = project.UniqueName;
			this.project = project;
			this.target = target;
			this.config = config;

			LoadPrimaryOutputModule( CurrentConfiguration );
		}

		public string UniqueName
		{
			get { return uniqueName; }
		}

		/*----------------------------------------------------------------------
		 * Overrides.
		 */

		public override void Refresh()
		{
			//
			// (Re-) obtain path to primary output as it may have
			// changed.
			//
			VCConfiguration vcConfig = CurrentConfiguration;

			if ( vcConfig.PrimaryOutput != this.currentPath )
			{
				//
				// Configuration changed, reload from scratch.
				//
				Debug.Print( "Full reload" );

				Clear();
				LoadPrimaryOutputModule( vcConfig );
			}
			else
			{
				//
				// Refresh module.
				//
				base.Refresh();
			}
		}
	}
}
