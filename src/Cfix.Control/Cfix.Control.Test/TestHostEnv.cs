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

			Assert.AreEqual( "foo=1;2\nbar=3\n", env.NativeFormat );

			Assert.IsNull( new HostEnvironment().NativeFormat );
		}

		[Test]
		public void Merge()
		{
			Environment.SetEnvironmentVariable( "__TEST", "1" );
			HostEnvironment env = new HostEnvironment();
			env.MergeEnvironmentVariables( 
				Environment.GetEnvironmentVariables( EnvironmentVariableTarget.Process ) );
			env.Add( "FOO", "2" );

			Assert.IsTrue( env.NativeFormat.Contains( "__test=1\n" ) );
			Assert.IsTrue( env.NativeFormat.Contains( "foo=2\n" ) );
		}
	}
}
