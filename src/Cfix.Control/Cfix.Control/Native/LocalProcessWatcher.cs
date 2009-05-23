using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cfix.Control.Native
{
	internal class LocalProcessWatcher : IDisposable
	{
		private readonly IList<Process> processList = new List<Process>();
		private readonly object processListLock = new object();

		private void proc_Exited( object sender, EventArgs e )
		{
			//
			// We do not need to watch this one any more.
			//
			lock ( this.processListLock )
			{
				this.processList.Remove( ( Process ) sender );
			}
		}

		public void Watch( int pid )
		{
			if ( pid == 0 )
			{
				//
				// Mock.
				//
				return;
			}

			Watch( Process.GetProcessById( pid ) );
		}

		public void Dispose()
		{
			foreach ( Process proc in this.processList )
			{
				proc.Dispose();
			}

			this.processList.Clear();
		}

		public void Watch( Process proc )
		{
			//
			// Add to watch list until the process exits.
			//
			proc.EnableRaisingEvents = true;
			proc.Exited += new EventHandler( proc_Exited );
			
			lock ( this.processListLock )
			{
				this.processList.Add( proc );
			}
		}

		public uint TerminateAll()
		{
			uint terminated = 0;
			
			lock ( this.processListLock )
			{
				foreach ( Process proc in this.processList )
				{
					try
					{
						proc.Kill();

						terminated++;
					}
					catch
					{ }
				}
			}

			return terminated;
		}

		public uint ProcessCount
		{
			get { return ( uint ) this.processList.Count; }
		}
	}
}