using System;
using System.Collections.Generic;
using Cfixctl;

namespace Cfix.Control
{
	/// <summary>
	/// Allows multiple actions to be run in sequence.
	/// 
	/// Threadsafe.
	/// </summary>
	internal class SequenceAction : IAction
	{
		private readonly IList<IAction> actions;

		private volatile bool started = false;

		//
		// Once stop is true, no further sub-action may be run.
		//
		private volatile bool stopped = false;
		private volatile IAction currentAction = null;

		public SequenceAction( IList<IAction> actions )
		{
			this.actions = actions;
		}

		public void Run( ICfixEventSink sink )
		{
			if ( this.started )
			{
				throw new InvalidOperationException( "Already started" );
			}

			this.started = true;

			foreach ( IAction action in this.actions )
			{
				if ( stopped )
				{
					break;
				}

				//
				// Save reference to current action to allow stopping.
				//
				this.currentAction = action;
				
				action.Run( sink );
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.InvalidOperationException.#ctor(System.String)" )]
		public void Stop()
		{
			if ( ! this.started )
			{
				throw new InvalidOperationException( "Not started" );
			}

			this.stopped = true;

			IAction action = this.currentAction;
			if ( action != null )
			{
				action.Stop();
			}
		}

		public void Dispose()
		{
			foreach ( IAction action in this.actions )
			{
				action.Dispose();
			}
		}
	}
}
