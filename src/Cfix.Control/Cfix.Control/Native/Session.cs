using System;
using System.Diagnostics;

namespace Cfix.Control.Native
{
	/*++
	 * Class Description:
	 *		Utility class.
	 * 
	 *		Threadsafe.
	 --*/
	public class Session : ISession
	{
		private readonly Object testsLock = new Object();
		private ITestItemCollection tests;

		public event EventHandler BeforeSetTests;
		public event EventHandler AfterSetTests;

		public Session()
		{
		}

		~Session()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( this.tests != null )
			{
				this.tests.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/*----------------------------------------------------------------------
		 * ISession.
		 */

		public ITestItemCollection Tests
		{
			get 
			{
				lock ( this.testsLock )
				{
					return this.tests;
				}
			}
			set
			{
				lock ( this.testsLock )
				{
					if ( this.tests != null )
					{
						this.tests.Dispose();
					}

					if ( this.BeforeSetTests != null )
					{
						this.BeforeSetTests( this, EventArgs.Empty );
					}

					this.tests = value;

					if ( value != null )
					{
						if ( this.AfterSetTests != null )
						{
							this.AfterSetTests( this, EventArgs.Empty );
						}
					}
				}
			}
		}
	}
}
