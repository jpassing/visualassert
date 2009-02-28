using System;
using System.IO;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	public class Host : IHost
	{
		private readonly Agent agent;
		private readonly ICfixHost host;

		internal Host(
			Agent agent,
			ICfixHost host
			)
		{
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
			bool userOnly,
			bool ignoreDuplicates
			)
		{
			return TestModuleCollection.Search(
				dir,
				filter,
				this.agent,
				runAgents,
				userOnly,
				ignoreDuplicates );
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

