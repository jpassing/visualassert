using System;
using System.Diagnostics;

namespace Cfix.Control
{
	public class GenericSession : ISession
	{
		private readonly Object testsLock = new Object();
		private ITestItemCollection tests;

		public event EventHandler BeforeSetTests;
		public event EventHandler AfterSetTests;

		public GenericSession()
		{
		}

		~GenericSession()
		{
			Dispose( false );
		}

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

					if ( value != null )
					{
						if ( this.BeforeSetTests != null )
						{
							this.BeforeSetTests( this, EventArgs.Empty );
						}

						this.tests = value;

						if ( this.AfterSetTests != null )
						{
							this.AfterSetTests( this, EventArgs.Empty );
						}
					}
				}
			}
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
	}
}
