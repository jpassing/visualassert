using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	/*++
	 * Class Description:
	 *		Wrapper for ICfixAction. Additionally, this object handles
	 *		provides a ICfixProcessEventSink to handle action/module 
	 *		associations.
	 * 
	 *		Threadsafe.
	 --*/
	internal class NativeAction : IAction
	{
		public const uint CFIXCTL_ACTION_COM_NEUTRAL = 1;

		public const uint CFIX_FIXTURE_EXECUTION_SHORTCUT_FIXTURE_ON_FAILURE	= 1;
		public const uint CFIX_FIXTURE_EXECUTION_SHORTCUT_RUN_ON_SETUP_FAILURE	= 2;
		public const uint CFIX_FIXTURE_EXECUTION_ESCALATE_FIXTURE_FAILUES = 4;
		public const uint CFIX_FIXTURE_EXECUTION_SHORTCUT_RUN_ON_FAILURE = 7;

		private readonly TestItem item;
		private readonly SchedulingOptions schedOptions;
		private readonly ThreadingOptions threadingOptions;
		private readonly IResultItem result;
		private readonly IActionEvents events;

		private readonly object runLock = new object();
		
		private volatile ICfixAction action;


		private class Sink : ICfixProcessEventSink, ICfixEventSink
		{
			private readonly Agent agent;
			private readonly IResultItem result;
			private readonly IActionEvents events;

			public Sink( 
				Agent agent,
				IResultItem result, 
				IActionEvents events )
			{
				this.agent = agent;
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

			ICfixTestÌtemContainerEventSink ICfixProcessEventSink.GetTestItemContainerEventSink(
				ICfixTestModule module,
				uint fixtureOrdinal
				)
			{
				try
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
				finally
				{
					//
					// N.B. This is required in order to have the refcount
					// after return equal the refcount before having entered.
					//
					this.agent.ReleaseObject( module );
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
			TestItem item,
			IActionEvents events,
			IResultItem result,
			SchedulingOptions schedOptions,
			ThreadingOptions threadingOptions
			)
		{
			Debug.Assert( item != null );
			Debug.Assert( events != null );

			this.item = item;
			this.events = events;
			this.schedOptions = schedOptions;
			this.threadingOptions = threadingOptions;

			this.result = result;
		}

		~NativeAction()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( this.action != null )
			{
				this.item.Module.Agent.ReleaseObject( this.action );
				this.action = null;
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		private ICfixAction CreateNativeAction( IHost host )
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
				throw this.item.Module.Agent.WrapException( x );
			}
			finally
			{
				if ( ctlItem != null )
				{
					this.item.Module.Agent.ReleaseObject( ctlItem );
				}
			}
		}

		/*----------------------------------------------------------------------
		 * IAction.
		 */

		public ITestItem Item
		{
			get { return this.item; }
		}

		public IResultItem Result
		{
			get { return this.result; }
		}
		
		public ITestItem TestItem
		{
			get { return this.item; }
		}

		public Architecture Architecture
		{
			get { return this.item.Module.Architecture; }
		}

		public void Run( IHost host )
		{
			try
			{
				//
				// Avoid concurrent run requests.
				//
				lock ( runLock )
				{
					if ( this.action != null )
					{
						throw new InvalidOperationException(
							"Already started" );
					}

					this.action = CreateNativeAction( host );
					this.action.Run(
						new Sink(
							this.item.Module.Agent,
							this.result,
							this.events ),
						( uint ) this.threadingOptions );
				}
			}
			finally
			{
				if ( this.action != null )
				{
					this.item.Module.Agent.ReleaseObject( this.action );
					this.action = null;
				}
			}
		}

		public void Stop()
		{
			try
			{
				ICfixAction current = this.action;
				if ( current != null )
				{
					current.Stop();
				}
			}
			catch ( COMException x )
			{
				throw this.item.Module.Agent.WrapException( x );
			}
		}
	}
}
