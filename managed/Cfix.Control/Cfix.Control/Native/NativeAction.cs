using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class NativeAction : IComponentAction
	{
		public const uint CFIXCTL_ACTION_COM_NEUTRAL = 1;

		public const uint CFIX_FIXTURE_EXECUTION_SHORTCUT_FIXTURE_ON_FAILURE	= 1;
		public const uint CFIX_FIXTURE_EXECUTION_SHORTCUT_RUN_ON_SETUP_FAILURE	= 2;
		public const uint CFIX_FIXTURE_EXECUTION_ESCALATE_FIXTURE_FAILUES = 4;
		public const uint CFIX_FIXTURE_EXECUTION_SHORTCUT_RUN_ON_FAILURE = 7;

		private TestItem item;
		private ICfixAction action;
		private ICfixHost host;
		private readonly uint flags;

		public NativeAction( 
			TestItem item, 
			ICfixHost host,
			ICfixAction action,
			uint flags )
		{
			this.item = item;
			this.host = host;
			this.action = action;
			this.flags = flags;

			Debug.Assert( host.GetHostProcessId() > 0 );
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
				this.action.Run( sink, this.flags );
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
				if ( this.action != null )
				{
					this.action.Stop();
				}
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
