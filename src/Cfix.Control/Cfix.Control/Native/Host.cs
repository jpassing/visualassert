using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	/*++
	 * Class Description:
	 *		Wrapper for ICfixHost.
	 * 
	 *		Threadsafe.
	 --*/
	public class Host : IHost
	{
		private readonly Agent agent;
		private readonly ICfixHost host;
		private readonly string imagePath;
		private readonly bool usesCustomImage;
		private readonly EventDll eventDll;	// optional.

		internal Host(
			Agent agent,
			ICfixHost host,
			bool usesCustomImage,
			string imagePath,
			EventDll eventDll
			)
		{
			Debug.Assert( agent != null );
			Debug.Assert( host != null );
			Debug.Assert( imagePath != null );

			this.agent = agent;
			this.host = host;
			this.usesCustomImage = usesCustomImage;
			this.imagePath = imagePath;
			this.eventDll = eventDll;
		}

		~Host()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( this.host != null )
			{
				this.agent.ReleaseObject( this.host );
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		internal ICfixHost GetNativeItem()
		{
			return this.host;
		}

		/*----------------------------------------------------------------------
		 * IHost.
		 */

		public uint ProcessId
		{
			get { return this.host.GetHostProcessId(); }
		}

		public Architecture Architecture
		{
			get { return this.agent.Architecture; }
		}

		public ITestItemCollection SearchModules(
			DirectoryInfo dir,
			String filter,
			AgentSet runAgents,
			bool userOnly
			)
		{
			//
			// N.B. Duplicates are always ignored as they lead to
			// problems during execution: If a TC is executed a second
			// time, we cannot provide an appropriate event sink -
			// therefore, disallow duplicates.
			//
			return TestModuleCollection.Search(
				dir,
				filter,
				this.agent,
				runAgents,
				userOnly,
				false );
		}

		public ITestItemCollection LoadModule( 
			ITestItemCollection parentCollection,
			string path,
			bool ignoreDuplicates
			)
		{
			Debug.Assert( ( path == null ) == this.usesCustomImage );

			ICfixTestModule ctlModule = null;
			try
			{
				ctlModule = this.host.LoadModule( path );

				TestModule mod;
				if ( this.usesCustomImage && path == null )
				{
					//
					// This is effectively the host module, so use its
					// path.
					//
					mod = new HostTestModule(
						parentCollection,
						this.imagePath,
						this.agent,
						ctlModule,
						ignoreDuplicates ); 
				}
				else
				{
					mod = new TestModule(
						parentCollection,
						path,
						this.agent,
						ctlModule,
						ignoreDuplicates );
				}

				mod.Update( ctlModule );

				return mod;
			}
			catch ( COMException x )
			{
				Logger.LogError( "Agent", "Failed to load module", x );
				throw this.agent.WrapException( x );
			}
			finally
			{
				if ( ctlModule != null )
				{
					this.agent.ReleaseObject( ctlModule );
				}
			}
		}

		public void Terminate()
		{
			this.host.Terminate();
		}

		public string Path
		{
			get { return this.imagePath; }
		}

		public EventDll EventDll
		{
			get { return this.eventDll; }
		}
	}
}

