using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public class GenericSession : ISession
	{
		private readonly ITestItemCollection tests;

		public GenericSession( ITestItemCollection tests )
		{
			this.tests = tests;
		}

		~GenericSession()
		{
			Dispose( false );
		}

		public ITestItemCollection Tests
		{
			get { return this.tests; }
		}

		protected virtual void Dispose( bool disposing )
		{
			this.tests.Dispose();
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}
	}
}
