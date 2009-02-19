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
		private MultiTarget target;

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

		private Architecture GetArchitecture( VCConfiguration config )
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

		private void LoadPrimaryOutputModule()
		{
			//
			// (Re-) obtain path to primary output as it may have
			// changed.
			//
			VCConfiguration vcConfig = CurrentConfiguration;
			Architecture arch = GetArchitecture( vcConfig );

			Debug.Assert( this.target.IsArchitectureSupported( arch ) );

			string modulePath = vcConfig.PrimaryOutput;
			Debug.Print( modulePath );

			if ( File.Exists( modulePath ) &&
				 this.config.IsSupportedTestModulePath( modulePath ) )
			{
				ITestItem module;
				try
				{
					module = TestModule.LoadModule(
						this.target.GetTarget( arch ),
						modulePath,
						true );
				}
				catch ( Exception x )
				{
					module = new InvalidModule(
						this,
						new FileInfo( modulePath ).Name,
						x );
				}

				Add( module );
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
			LoadPrimaryOutputModule();
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
			Clear();
			LoadPrimaryOutputModule();
		}
	}
}
