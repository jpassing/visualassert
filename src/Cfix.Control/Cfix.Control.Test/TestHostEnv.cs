using System;
using NUnit.Framework;
using Cfix.Control;

namespace Cfix.Control.Test
{
	[TestFixture]
	public class TestHostEnv
	{
		[Test]
		public void Basics()
		{
			HostEnvironment env = new HostEnvironment();
			env.Add( "FOO", "1" );
			env.Add( "FOO", "2" );
			env.Add( "BAR", "3" );

			Assert.AreEqual( "FOO=1;2\0BAR=3\0\0", env.NativeFormat );

			Assert.IsNull( new HostEnvironment().NativeFormat );
		}

		[Test]
		public void Merge()
		{
			Environment.SetEnvironmentVariable( "__TEST", "1" );
			HostEnvironment env = new HostEnvironment();
			env.Merge( EnvironmentVariableTarget.Process );
			env.Add( "FOO", "2" );

			Assert.IsTrue( env.NativeFormat.Contains( "__TEST=1\0" ) );
			Assert.IsTrue( env.NativeFormat.Contains( "FOO=2\0" ) );
		}
	}
}
