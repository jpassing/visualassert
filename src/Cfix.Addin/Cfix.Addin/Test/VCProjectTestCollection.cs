/*----------------------------------------------------------------------
 * Purpose:
 *		Project.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;
using Cfix.Control.Native;
using Cfix.Control.Ui;
using Microsoft.VisualStudio.VCProjectEngine;

namespace Cfix.Addin.Test
{
	internal class VCProjectTestCollection 
		: GenericTestItemCollection, 
		  IBuildableTestItem,
		  IRelativePathReferenceItem
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

		private static bool vcDirectoriesRegistered;

		/*++
		 * Register cfix directories s.t. the compiler finds them. 
		 --*/
		private static void RegisterVcDirectories( VCProjectEngine engine )
		{
			if ( vcDirectoriesRegistered )
			{
				return;
			}

			IVCCollection platforms = ( IVCCollection ) engine.Platforms;
			foreach ( VCPlatform platform in platforms )
			{
				if ( !IsArchitectureSupported( platform ) )
				{
					continue;
				}

				Architecture arch = GetArchitecture( platform );
				platform.IncludeDirectories +=
					";" + Directories.IncludeDirectory;

				platform.LibraryDirectories +=
					";" + Directories.GetLibDirectory( arch );

				//
				// Write to disk.
				//
				// N.B. VS seems to be smart enough to avoid saving
				// duplicate paths.
				//
				platform.CommitChanges();
			}

			vcDirectoriesRegistered = true;
		}
		
		private VCConfiguration CurrentConfiguration
		{
			get
			{
				VCProject vcProject = this.project.Object as VCProject;
				Debug.Assert( vcProject != null );

				EnvDTE.Configuration activeConfig = null;
				try
				{
					activeConfig =
						this.project.ConfigurationManager.ActiveConfiguration;
				}
				catch ( ArgumentException )
				{
					//
					// Most likely due to a configuration being active that
					// is not fully supported, e.g. amd64 on a i386 system.
					//
				}

				IVCCollection vcConfigs = ( IVCCollection ) vcProject.Configurations;
				foreach ( VCConfiguration vcConfig in vcConfigs )
				{
					if ( activeConfig == null )
					{
						//
						// Just choose the first.
						//
						return vcConfig;
					}
					else if ( activeConfig.ConfigurationName == vcConfig.ConfigurationName &&
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
			return GetArchitecture( ( VCPlatform ) config.Platform );
		}

		private static Architecture GetArchitecture( VCPlatform plaf )
		{
			string plafName = plaf.Name;
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

		private static bool IsArchitectureSupported( VCPlatform plaf )
		{
			string plafName = plaf.Name;
			switch ( plafName )
			{
				case "Win32":
					return true;

				case "x64":
					return true;

				default:
					return false;
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
						//
						// N.B. This module may be importing symbols from a DLL
						// that resides in the same directory. 
						//
						// Augment search path.
						//
						FileInfo pathInfo = new FileInfo( this.currentPath );
						HostEnvironment env = new HostEnvironment();
						env.AddSearchPath( pathInfo.Directory.FullName );

						using ( IHost host = this.agentSet.GetAgent( arch ).CreateHost( env ) )
						{
							module = host.LoadModule(
								this,
								this.currentPath,
								false );

							ITestItemCollection moduleColl =
								module as ITestItemCollection;

							if ( moduleColl != null && moduleColl.ItemCountRecursive == 0 )
							{
								//
								// Technically valid, but treat as invalid.
								//
								module = null;
							}
						}
					}
					catch ( Exception x )
					{
						module = new InvalidModule(
							this,
							new FileInfo( this.currentPath ).Name,
							x );
					}

					if ( module != null )
					{
						Add( module );
					}
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

			VCConfiguration currentConfig = CurrentConfiguration;

			//
			// If not happened yet, register VC directories.
			//
			// N.B. We need a Project to get hold of the VCProjectEngine,
			// thus, this has to occur here.
			//
			VCProjectEngine engine = null;
			VCCLCompilerTool clTool = ( VCCLCompilerTool )
				( ( IVCCollection ) currentConfig.Tools ).Item( "VCCLCompilerTool" );
			if ( clTool != null )
			{
				engine = ( VCProjectEngine ) clTool.VCProjectEngine;
			}
			else
			{
				VCNMakeTool nmakeTool = ( VCNMakeTool )
					( ( IVCCollection ) currentConfig.Tools ).Item( "VCNMakeTool" );
				if ( nmakeTool != null )
				{
					engine = ( VCProjectEngine ) nmakeTool.VCProjectEngine;
				}
			}

			if ( engine != null )
			{
				RegisterVcDirectories( engine );
			}


			LoadPrimaryOutputModule( currentConfig );
		}

		public string UniqueName
		{
			get { return uniqueName; }
		}

		public Project Project
		{
			get { return this.project; }
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

		/*----------------------------------------------------------------------
		 * IRelativePathReferenceItem.
		 */

		public string GetFullPath( string relativePath )
		{
			DirectoryInfo projectDir = 
				new FileInfo( this.project.FullName ).Directory;
			return projectDir.FullName + "\\" + relativePath;
		}
	}
}