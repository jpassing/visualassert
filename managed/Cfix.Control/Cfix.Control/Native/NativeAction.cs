using System;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class NativeAction : IAction
	{
		private TestItem item;
		private ICfixAction action;

		public NativeAction( TestItem item, ICfixAction action )
		{
			this.item = item;
			this.action = action;
		}

		public ITestItem TestItem
		{
			get
			{
				return this.item;
			}
		}

		public uint TestCaseCount
		{
			get
			{
				return this.action.GetTestCaseCount();
			}
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

		protected virtual void Dispose( bool disposing )
		{
			if ( this.action != null )
			{
				this.item.Module.Target.ReleaseObject( this.action );
				this.action = null;
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}
	}
}
