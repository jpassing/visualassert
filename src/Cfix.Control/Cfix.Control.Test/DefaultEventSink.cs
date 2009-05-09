using System;
using Cfixctl;

namespace Cfix.Control.Test
{
	internal class DefaultEventSink : IActionEvents
	{
		public uint Notifications;
		public uint HostSpawns;

		public IDispositionPolicy DispositionPolicy 
		{
			get
			{
				return new StandardDispositionPolicy(
					 Disposition.Break, Disposition.Break );
			}
		}

		public void OnNotification( IResultItem item, int hr )
		{
			Notifications++;
		}

		public void OnHostSpawned( uint processId )
		{
			HostSpawns++;
		}

		public void OnLog( IResultItem item, String message )
		{
		}

		public void OnThreadStarted( IResultItem item, uint threadId )
		{
		}

		public void OnThreadFinished( IResultItem item, uint threadId )
		{
		}

		public void OnStatusChanged( IResultItem item )
		{
		}

		public void OnFailureOccured( IResultItem item )
		{
		}
	}
}
