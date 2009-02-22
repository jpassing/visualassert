using System;
using System.Collections.Generic;
using Cfix.Control;
using Cfixctl;

namespace Cfix.Control.Native
{
	public class Run : AbstractRun, ICfixProcessEventSink, ICfixEventSink
	{
		private readonly IResultItemCollection rootResult;

		//
		// Map between module paths and module result objects.
		//
		private readonly IDictionary<String, TestItemCollectionResult> modules =
			new Dictionary<String, TestItemCollectionResult>();

		public Run( 
			IDispositionPolicy policy,
			SchedulingOptions schedulingOptions,
			CompositionOptions compositionOptions,
			ITestItemCollection rootItem 
			)
			: base( 
				policy,
				schedulingOptions,
				compositionOptions,
				rootItem )
		{
			this.rootResult = TestItemCollectionResult.CreateResult(
					this, rootItem, ExecutionStatus.Pending );
		}

		/*--------------------------------------------------------------
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
			TestItemCollectionResult moduleResult;
			this.modules.TryGetValue( module.GetPath(), out moduleResult );

			if ( moduleResult == null )
			{
				if ( this.RootItem is TestItemCollection &&
					 !( this.RootItem is TestModule ) &&
					 this.RootItem.Ordinal == fixtureOrdinal )
				{
					//
					// The run comprises a fixture (without its enclosing
					// module) only.
					//
					return ( ICfixTestÌtemContainerEventSink ) this.rootResult;
				}
				else
				{
					throw new CfixException( "Unknown fixture requested" );
				}
			}
			else
			{
				return ( ICfixTestÌtemContainerEventSink )
					moduleResult.GetItem( fixtureOrdinal );
			}
		}

		void ICfixProcessEventSink.Notification( int hr )
		{
			OnNotification( this.RootResult, hr );
		}

		/*--------------------------------------------------------------
		 * ICfixEventSink.
		 */

		ICfixProcessEventSink ICfixEventSink.GetProcessEventSink( uint processId )
		{
			OnHostSpawned( processId );

			return this;
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		protected override void RunAction( IAction action )
		{
			action.Run( this );
		}

		public override IResultItemCollection RootResult
		{
			get { return this.rootResult; }
		}

		/*--------------------------------------------------------------
		 * Internal.
		 */

		internal void OnItemAdded( IResultItem item )
		{
			//
			// Remember the item's module for use as event sink.
			//
			TestModule module = item.Item as TestModule;
			if ( module != null )
			{
				this.modules[ module.Path ] = ( TestItemCollectionResult ) item;
				return;
			}
		}

	}
}
