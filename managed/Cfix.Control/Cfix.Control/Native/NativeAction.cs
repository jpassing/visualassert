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
		private readonly IResultItem result;
		private readonly IActionEvents events;
		private ICfixAction action;

		public event EventHandler<NotificationEventArgs> Notification;
		public event EventHandler<HostEventArgs> HostSpawned;

		private class Sink : ICfixProcessEventSink, ICfixEventSink
		{
			private readonly IResultItem result;
			private readonly IActionEvents events;

			public Sink( IResultItem result, IActionEvents events )
			{
				this.result = result;
				this.events = events;
			}

			/*------------------------------------------------------------------
				 * ICfixProcessEventSink.
				 */

			void ICfixProcessEventSink.AfterRunFinish()
			{
				//
				// Not used.
				//
			}

			void ICfixProcessEventSink.BeforeRunStart()
			{
				//
				// Not used.
				//
			}

			ICfixTestÌtemContainerEventSink ICfixProcessEventSink.GetTestÌtemContainerEventSink(
				ICfixTestModule module,
				uint fixtureOrdinal
				)
			{
				if ( this.result.Item is TestModule )
				{
					//
					// This is a entire-module run, just request fixture.
					//

					Debug.Assert( module.GetPath() ==
						( ( TestModule ) this.result.Item ).Path );

					TestItemCollectionResult coll = ( TestItemCollectionResult ) this.result;
					return ( ICfixTestÌtemContainerEventSink )
						coll.GetItem( fixtureOrdinal );
				}
				else if ( this.result is IResultItemCollection )
				{
					//
					// The run comprises a fixture (without its enclosing
					// module) only.
					//

					if ( this.result.Item.Ordinal == fixtureOrdinal )
					{
						return ( ICfixTestÌtemContainerEventSink ) this.result;
					}
					else
					{
						throw new CfixException( "Unknown fixture requested" );
					}
				}
				else
				{
					throw new CfixException( "Unexpected sink request" );
				}
			}

			void ICfixProcessEventSink.Notification( int hr )
			{
				this.events.OnNotification( this.result, hr );
			}

			/*------------------------------------------------------------------
			 * ICfixEventSink.
			 */

			ICfixProcessEventSink ICfixEventSink.GetProcessEventSink( 
				uint processId 
				)
			{
				this.events.OnHostSpawned( processId );

				return this;
			}
		}

		public NativeAction( 
			IHost host,
			TestItem item,
			IActionEvents events,
			SchedulingOptions schedOptions,
			ThreadingOptions threadingOptions
			)
		{
			this.item = item;
			this.host = host;
			this.events = events;
			this.schedOptions = schedOptions;
			this.threadingOptions = threadingOptions;

			this.result = item.CreateResultItem(
				null,
				events,
				ExecutionStatus.Pending );
		}

		~NativeAction()
		{
			Dispose( false );
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
		 * IAction.
		 */ 

		public ITestItem TestItem
		{
			get { return this.item; }
		}

		public Architecture Architecture
		{
			get { return this.item.Module.Architecture; }
		}

		public void Run()
		{
			try
			{
				this.action = CreateNativeAction();
				this.action.Run( 
					new Sink( this.result, this.events ), 
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

		

	}

}
