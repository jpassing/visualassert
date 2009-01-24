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

		public ITestItemCollection Tests
		{
			get { return this.tests; }
		}

		public void Dispose()
		{
			this.tests.Dispose();
		}
	}
}
