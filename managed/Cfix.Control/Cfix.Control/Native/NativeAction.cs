using System;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class NativeAction : IComponentAction
	{
		private TestItem item;
		private ICfixAction action;
		private ICfixHost host;

		public NativeAction( TestItem item, ICfixAction action )
		{
			this.item = item;
			this.action = action;
		}

		/*----------------------------------------------------------------------
		 * IComponentAction.
		 */ 

		public ITestItem TestItem
		{
			get { return this.item; }
		}

		public uint TestCaseCount
		{
			get { return this.action.GetTestCaseCount(); }
		}

		public uint HostProcessId
		{
			get { return this.host.GetHostProcessId(); }
		}

		public Architecture Architecture
		{
			get { return this.item.Module.Architecture; }
		}

		public void Run( ICfixEventSink sink )
		{
			try
			{
				this.action.Run( sink );
			}
			catch ( COMException x )
			{
				throw this.item.Module.Target.WrapException( x );
			}
		}

		public void Stop()
		{
			try
			{
				this.action.Stop();
			}
			catch ( COMException x )
			{
				throw this.item.Module.Target.WrapException( x );
			}
		}

		public void TerminateHost()
		{
			try
			{
				this.host.Terminate();
				this.host = null;
				this.action = null;
			}
			catch ( COMException x )
			{
				throw this.item.Module.Target.WrapException( x );
			}
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( this.action != null )
			{
				this.item.Module.Target.ReleaseObject( this.action );
				this.action = null;
			}

			if ( this.host != null )
			{
				this.item.Module.Target.ReleaseObject( this.host );
				this.host = null;
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}
	}
}
