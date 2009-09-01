using System;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Control.Test
{
	[TestFixture]
	public class TestAgent
	{
		[Test]
		public void TestWatchTermination()
		{
			IAgent agent = Agent.CreateLocalAgent(
				Architecture.I386,
				false );
			agent.SetTrialLicenseCookie( Util.TrialLicenseCookie );
			IHost host = agent.CreateHost();

			Assert.AreEqual( 1, agent.ActiveHostCount );
			host.Terminate();
			Thread.Sleep( 200 );
			Assert.AreEqual( 0, agent.ActiveHostCount );
		}

		[Test]
		public void TestTerminateWatched()
		{
			IAgent agent = Agent.CreateLocalAgent(
				Architecture.I386,
				false );
			agent.SetTrialLicenseCookie( Util.TrialLicenseCookie );
			IHost host = agent.CreateHost();

			Assert.AreEqual( 1, agent.ActiveHostCount );
			agent.TerminateActiveHosts();
			Thread.Sleep( 200 );
			Assert.AreEqual( 0, agent.ActiveHostCount );
		}
	}
}
