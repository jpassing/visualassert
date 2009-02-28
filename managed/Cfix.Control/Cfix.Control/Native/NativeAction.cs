using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	internal class NativeAction : IAction
	{
		public const uint CFIXCTL_ACTION_COM_NEUTRAL = 1;

		public const uint CFIX_FIXTURE_EXECUTION_SHORTCUT_FIXTURE_ON_FAILURE	= 1;
		public const uint CFIX_FIXTURE_EXECUTION_SHORTCUT_RUN_ON_SETUP_FAILURE	= 2;
		public const uint CFIX_FIXTURE_EXECUTION_ESCALATE_FIXTURE_FAILUES = 4;
		public const uint CFIX_FIXTURE_EXECUTION_SHORTCUT_RUN_ON_FAILURE = 7;

		private readonly TestItem item;
		private readonly IHost host;
		private readonly SchedulingOptions schedOptions;
		private readonly ThreadingOptions threadingOptions;
		private ICfixAction action;

		public NativeAction( 
			IHost host,
			TestItem item,
			SchedulingOptions schedOptions,
			ThreadingOptions threadingOptions
			)
		{
			this.item = item;
			this.host = host;
			this.schedOptions = schedOptions;
			this.threadingOptions = threadingOptions;
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		private ICfixAction CreateNativeAction()
		{
			ICfixTestItem ctlItem = this.item.GetNativeItem( host );
			try
			{
				return ctlItem.CreateExecutionAction(
					( uint ) this.schedOptions, 
					0 );
			}
			catch ( COMException x )
			{
				throw this.item.Module.Target.WrapException( x );
			}
			finally
			{
				if ( ctlItem != null )
				{
					this.item.Module.Target.ReleaseObject( ctlItem );
				}
			}
		}

		/*----------------------------------------------------------------------
		 * IComponentAction.
		 */ 

		public ITestItem TestItem
		{
			get { return this.item; }
		}

		public Architecture Architecture
		{
			get { return this.item.Module.Architecture; }
		}

		public void Run( ICfixEventSink sink )
		{
			try
			{
				this.action = CreateNativeAction();
				this.action.Run( 
					sink, 
					( uint ) this.threadingOptions );
			}
			finally
			{
				if ( this.action != null )
				{
					this.item.Module.Target.ReleaseObject( this.action );
					this.action = null;
				}
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

		protected virtual void Dispose( bool disposing )
		{
			if ( this.action != null )
			{
				this.item.Module.Target.ReleaseObject( this.action );
				this.action = null;
			}
		}

	}
}
