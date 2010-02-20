/*----------------------------------------------------------------------
 * Purpose:
 *		Project.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;
using Cfix.Control.Native;
using Cfix.Control.Ui;
using Microsoft.VisualStudio.VCProjectEngine;
using System.ComponentModel;

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

		private static readonly object loadedProjectsLock = new object();
		private static readonly IDictionary<string, VCProjectTestCollection>
			loadedProjects = new Dictionary<string, VCProjectTestCollection>();

		public event EventHandler Refreshed;

		//
		// Current path to module -- this changes whenever the active
		// solution configuration changes.
		//
		private string currentPath;

		private void LoadPrimaryOutputModule( VCConfiguration vcConfig )
		{
			lock ( this.loadLock )
			{
				Architecture arch = Workspace.GetArchitecture( vcConfig );

				Debug.Assert( this.agentSet.IsArchitectureSupported( arch ) );

				string modulePath = vcConfig.PrimaryOutput;
				if ( !File.Exists( modulePath ) )
				{
					//
					// Do not save the path.
					//
					this.currentPath = null;
				}
				else 
				{
					ITestItem module;
					if ( this.config.IsSupportedTestModulePath( modulePath ) )
					{
						IAgent agent = this.agentSet.GetAgent( arch );
						try
						{
							//
							// N.B. This module may be importing symbols from a DLL
							// that resides in the same directory. 
							//
							// Augment search path.
							//
							FileInfo pathInfo = new FileInfo( modulePath );
							HostEnvironment env = new HostEnvironment();
							env.AddSearchPath( pathInfo.Directory.FullName );

							//
							// N.B. Never ignore duplicates -- duplicates lead
							// to failures for lookup during test run, so better 
							// fail early.
							//
							module = agent.LoadModule(
								env,
								this,
								modulePath,
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

							this.currentPath = modulePath;
							Debug.Print( "Loaded " + modulePath );
						}
						catch ( Exception x )
						{
							module = new InvalidModule(
								this,
								new FileInfo( modulePath ).Name,
								agent.WrapException( x ) );
							this.currentPath = null;
						}
					}
					else
					{
						module = new InvalidModule(
								this,
								new FileInfo( modulePath ).Name,
								new CfixException( Strings.UnsupportedModule ) );
						this.currentPath = null;
					}

					if ( module != null )
					{
						Add( module );
					}
				}
			}
		}

		private void FullRefresh( VCConfiguration vcConfig )
		{
			Clear();
			LoadPrimaryOutputModule( vcConfig );
		}

		/*----------------------------------------------------------------------
		 * Protected.
		 */

		protected static VCConfiguration GetCurrentConfiguration( Project project )
		{
			VCProject vcProject = project.Object as VCProject;
			Debug.Assert( vcProject != null );

			EnvDTE.Configuration activeConfig = null;
			try
			{
				activeConfig =
					project.ConfigurationManager.ActiveConfiguration;
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

		/*----------------------------------------------------------------------
		 * Public.
		 */

		public VCProjectTestCollection(
			ITestItemCollection parent,
			Solution2 solution,
			Project project,
			AgentSet agents,
			Configuration config
			)
			: base( parent, project.Name )
		{
			//
			// N.B. When this project is part of a solution folder, then
			// parent != solution.
			//

			this.solution = solution;
			this.uniqueName = project.UniqueName;
			this.project = project;
			this.agentSet = agents;
			this.config = config;

			LoadPrimaryOutputModule( GetCurrentConfiguration( project ) );

			//
			// N.B. Do not register in loadedProjects yet as child
			// items have not been loaded yet.
			//
		}

		protected override void Dispose( bool disposing )
		{
			lock ( loadedProjectsLock )
			{
				loadedProjects.Remove( this.Name );
			}

			base.Dispose( disposing );
		}

		[Browsable( false )]
		public string UniqueName
		{
			get { return uniqueName; }
		}

		[Browsable( false )]
		public Project Project
		{
			get { return this.project; }
		}

		public static VCProjectTestCollection TryGetByName( string name )
		{
			lock ( loadedProjectsLock )
			{
				VCProjectTestCollection prj;
				if ( loadedProjects.TryGetValue( name, out prj ) )
				{
					return prj;
				}
				else
				{
					return null;
				}
			}
		}

		public bool PrimaryOutputAvailable
		{
			get { return this.currentPath != null && File.Exists( this.currentPath ); }
		}

		/*----------------------------------------------------------------------
		 * Overrides.
		 */

		public override void Refresh()
		{
			lock ( loadedProjectsLock )
			{
				//
				// Avoid the object from being used while refreshing.
				//
				loadedProjects.Remove( this.Name );
			}

			//
			// (Re-) obtain path to primary output as it may have
			// changed.
			//
			VCConfiguration vcConfig;
			try
			{
				vcConfig = GetCurrentConfiguration( this.project );
			}
			catch ( COMException )
			{
				//
				// This may occur when VC is currently being shut down.
				// ABort operation.
				//
				return;
			}

			if ( this.currentPath == null ||
				 vcConfig.PrimaryOutput != this.currentPath )
			{
				//
				// First load or configuration changed, reload from scratch.
				//
				Debug.Print( "Full reload" );

				FullRefresh( vcConfig );
			}
			else
			{
				//
				// Refresh module.
				//
				// Note that we optimize for not having to refresh
				// the entire tree.
				//
				if ( ItemCount == 0 )
				{
					//
					// Module did not contain any fixtures -- reload to
					// see whether one has been added (#285).
					//
					FullRefresh( vcConfig );
				}
				else if ( ItemCount >= 1 && GetItem( 0 ) is InvalidModule )
				{
					//
					// Current state is invalid. Allow transition to Valid
					// by performing a full refresh.
					//
					FullRefresh( vcConfig );
				}
				else
				{
					try
					{
						base.Refresh();
					}
					catch ( Exception x )
					{
						//
						// Valid -> Invalid.
						//
						Clear();
						Add( new InvalidModule(
							this,
							new FileInfo( this.currentPath ).Name,
							this.agentSet.GetAgent( Architecture.I386 ).WrapException( x ) ) );
					}
				}
			}

			lock ( loadedProjectsLock )
			{
				loadedProjects.Add( this.Name, this );
			}

			if ( this.Refreshed != null )
			{
				this.Refreshed( this, EventArgs.Empty );
			}
		}

		/*----------------------------------------------------------------------
		 * IBuildableTestItem.
		 */

		public bool Build()
		{
			SolutionBuild build = this.solution.SolutionBuild;
			build.BuildProject(
				GetCurrentConfiguration( this.project ).ConfigurationName,
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
