using System;

namespace Cfix.Control.Native
{
	internal class Run : IRun
	{
		public event EventHandler<LogEventArgs> Log;
		public event EventHandler<ThreadEventArgs> ThreadStarted;
		public event EventHandler<ThreadEventArgs> ThreadFinished;
		public event EventHandler StatusChanged;

		private readonly IDispositionPolicy dispositionPolicy;
		private readonly IResultItemCollection rootItem;

		public Run( IDispositionPolicy policy )
		{
			this.dispositionPolicy = policy;
		}


		/*--------------------------------------------------------------
		 * IRun.
		 */

		public IResultItemCollection Root 
		{ 
			get { return this.rootItem; }
		}

		/*--------------------------------------------------------------
		 * Internal.
		 */

		internal void OnLog( IResultItem item, String message )
		{
			if ( Log != null )
			{
				Log( item, new LogEventArgs( message ) );
			}
		}

		internal void OnThreadStarted( IResultItem item, uint threadId )
		{
			if ( ThreadStarted != null )
			{
				ThreadStarted( item, new ThreadEventArgs( threadId ) );
			}
		}

		internal void OnThreadFinished( IResultItem item, uint threadId )
		{
			if ( ThreadFinished != null )
			{
				ThreadFinished( item, new ThreadEventArgs( threadId ) );
			}
		}

		internal void OnStatusChanged( IResultItem item )
		{
			if ( StatusChanged != null )
			{
				StatusChanged( item, EventArgs.Empty );
			}
		}

		internal IDispositionPolicy DispositionPolicy
		{
			get { return this.dispositionPolicy; }
		}
	}
}
