using System;
using System.Diagnostics;
using System.Threading;

namespace Cfix.Control
{
	/*++
	 * Lock protecting against rundown of a data structure while
	 * it is still in use.
	 --*/
	public class RundownLock
	{
		private readonly Object rundownCondVar = new Object();
		private int referenceCount;

		public void Acquire()
		{
			lock ( this.rundownCondVar )
			{
				this.referenceCount++;
			}
		}

		public void Release()
		{
			lock ( this.rundownCondVar )
			{	
				if ( --this.referenceCount == 0 )
				{
					//
					// Notify rundown thread(s).
					//
					Monitor.PulseAll( this.rundownCondVar );
				}
			}
		}

		/*++
		 * Wait until all rundown protections have been released.
		 --*/
		public void Rundown()
		{
			lock ( this.rundownCondVar )
			{
				if ( this.referenceCount > 0 )
				{
					Monitor.Wait( this.rundownCondVar );
				}

				Debug.Assert( this.referenceCount == 0 );
			}
		}
	}
}
