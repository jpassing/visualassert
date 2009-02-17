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

		private void Load()
		{
			//
			// (Re-) obtain path to primary output as it may have
			// changed.
			//
			VCConfiguration vcConfig = CurrentConfiguration;

			string modulePath = vcConfig.PrimaryOutput;
			Debug.Print( modulePath );

			if ( File.Exists( modulePath ) )
			{
				Add( TestModule.LoadModule(
					this.target.GetTarget( GetArchitecture( vcConfig ) ),
					modulePath,
					true ) );
			}
		}

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public VCProjectTestCollection(
			Project project,
			MultiTarget target
			)
			: base( null, project.Name )
		{
			this.uniqueName = project.UniqueName;
			this.project = project;
			this.target = target;

			Load();
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
			Load();
		}
	}
}
