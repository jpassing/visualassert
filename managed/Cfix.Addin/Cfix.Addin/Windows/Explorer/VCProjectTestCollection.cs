using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;
using Cfix.Control.Native;
using Microsoft.VisualStudio.VCProjectEngine;

namespace Cfix.Addin.Windows.Explorer
{
	internal class VCProjectTestCollection 
		: GenericTestItemCollection, IBuildableTestItem
	{
		private readonly Configuration config;
		private readonly string uniqueName;
		private readonly Solution2 solution;
		private readonly Project project;
		private readonly AgentSet agentSet;

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

				Debug.Assert( this.agentSet.IsArchitectureSupported( arch ) );

				this.currentPath = vcConfig.PrimaryOutput;
				Debug.Print( this.currentPath );

				if ( File.Exists( this.currentPath ) &&
					 this.config.IsSupportedTestModulePath( this.currentPath ) )
				{
					ITestItem module;
					try
					{
						using ( IHost host = this.agentSet.GetAgent( arch ).CreateHost() )
						{
							module = host.LoadModule(
								this,
								this.currentPath,
								false );
						}
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
			Solution2 solution,
			Project project,
			AgentSet agents,
			Configuration config
			)
			: base( null, project.Name )
		{
			this.solution = solution;
			this.uniqueName = project.UniqueName;
			this.project = project;
			this.agentSet = agents;
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
			VCConfiguration vcConfig;
			try
			{
				vcConfig = CurrentConfiguration;
			}
			catch ( COMException )
			{
				//
				// This may occur when VC is currently being shut down.
				// ABort operation.
				//
				return;
			}

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
				
				//
				// N.B. Full reload required to support 
				// Invalid <-> Valid transitions.
				//
				Clear();
				LoadPrimaryOutputModule( vcConfig ); 
				
				//base.Refresh();
			}
		}

		/*----------------------------------------------------------------------
		 * IBuildableTestItem.
		 */

		public bool Build()
		{
			SolutionBuild build = this.solution.SolutionBuild;
			build.BuildProject(
				CurrentConfiguration.ConfigurationName,
				this.uniqueName, 
				true );

			if ( build.BuildState != vsBuildState.vsBuildStateDone )
			{
				Debug.Fail( "Unexpected build state" );
				return false;
			}

			return build.LastBuildInfo == 0;
		}
	}
}
