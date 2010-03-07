using System;
using System.Collections.Generic;
using System.Text;
using Cfix.Control;
using Microsoft.Win32;
using EnvDTE80;
using System.IO;

namespace Cfix.Addin.IntelParallelStudio
{
	public class Inspector
	{
		private AgentSet runAgents;
		private readonly string installLocation;

		private InspectorLevel currentLevel;
		private ResultLocation currentResultDirectory;

		private Inspector( 
			string installLocation
			)
		{
			this.installLocation = installLocation;
		}

		public static Inspector TryLoadInspector( 
			Configuration config,
			DTE2 dte 
			)
		{
			string subkey = String.Format(
				@"SOFTWARE\Intel\Inspector\VisualStudio\{0}",
				dte.Version );

			RegistryKey key = Registry.LocalMachine.OpenSubKey( subkey );
			if ( key == null )
			{
				//
				// Not found --> not installed.
				//
				return null;
			}

			string installLocation;
			try
			{
				installLocation = ( string ) key.GetValue( "Install Location" );
			}
			finally
			{
				key.Close();
			}

			if ( ! Directory.Exists( installLocation ) )
			{
				return null;
			}

			Inspector inspector = new Inspector( installLocation );

			//
			// Load run agent set.
			//
			inspector.runAgents = new InspectorAgentFactory( inspector ).CreateRunAgent( config );
			
			return inspector;
		}

		/*--------------------------------------------------------------
		 * Public.
		 */

		public InspectorLevel InspectorLevel
		{
			get { return this.currentLevel; }
			set
			{
				this.currentLevel = value;
			}
		}

		public static ResultLocation CreateResultLocation( string name )
		{
			string resultsBaseDir = Path.Combine(
				Path.GetTempPath(),
				"va-insp" );

			if ( !Directory.Exists( resultsBaseDir ) )
			{
				Directory.CreateDirectory( resultsBaseDir );
			}

			string tempDir = Path.Combine( resultsBaseDir, Guid.NewGuid().ToString() );

			if ( !Directory.Exists( tempDir ) )
			{
				Directory.CreateDirectory( tempDir );
			}

			string resultDir = Path.Combine( tempDir, name );

			return new ResultLocation(
				resultDir,
				Path.Combine( resultDir, name + ".insp" ) );
		}

		public ResultLocation ResultLocation
		{
			get { return this.currentResultDirectory; }
			set { this.currentResultDirectory = value; }
		}

		public string ShimPath
		{
			get 
			{ 
				return String.Format( 
					@"{0}\bin32\insp-cl.exe",
					this.installLocation ); 
			}
		}

		public AgentSet RunAgents
		{
			get { return this.runAgents; }
		}
	}
}
