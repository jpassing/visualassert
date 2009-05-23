using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Cfix.Control.Diag;
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

		internal Host(
			Agent agent,
			ICfixHost host
			)
		{
			Debug.Assert( agent != null );
			Debug.Assert( host != null );

			this.agent = agent;
			this.host = host;
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
			ICfixTestModule ctlModule = null;
			try
			{
				ctlModule = this.host.LoadModule( path );

				TestModule mod = new TestModule(
					parentCollection,
					path,
					this.agent,
					ctlModule,
					ignoreDuplicates );

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
	}
}

