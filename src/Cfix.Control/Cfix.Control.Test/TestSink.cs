using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using NUnit.Framework;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Control.Test
{
	[TestFixture]
	public class TestSink
	{
		private class Sink : 
			Cfixctl.ICfixEventSink,
			Cfixctl.ICfixProcessEventSink,
			Cfixctl.ICfixTestÌtemContainerEventSink,
			Cfixctl.ICfixTestÌtemEventSink
		{
			private readonly int mask;

			public Sink( int mask )
			{
				this.mask = mask;
			}

			public Cfixctl.ICfixProcessEventSink GetProcessEventSink( 
				uint ProcessId )
			{
				if ( ( this.mask & 1 ) != 0 )
				{
					throw new ArgumentException();
				}

				return this;
			}

			public void AfterRunFinish()
			{
				if ( ( this.mask & 2 ) != 0 )
				{
					throw new ArgumentException();
				}
			}

			public void BeforeRunStart()
			{
				if ( ( this.mask & 4 ) != 0 )
				{
					throw new ArgumentException();
				}
			}

			public Cfixctl.ICfixTestÌtemContainerEventSink GetTestItemContainerEventSink( 
				Cfixctl.ICfixTestModule Module, uint FixtureOrdinal )
			{
				if ( ( this.mask & 8 ) != 0 )
				{
					throw new ArgumentException();
				}
				
				return this;
			}

			public void Notification( int Hr )
			{
				if ( ( this.mask & 16 ) != 0 )
				{
					throw new ArgumentException();
				}
			}

			public void AfterChildThreadFinish( uint ThreadId )
			{
				if ( ( this.mask & 32 ) != 0 )
				{
					throw new ArgumentException();
				}
			}

			public void AfterFixtureFinish( int RanToCompletion )
			{
				if ( ( this.mask & 64 ) != 0 )
				{
					throw new ArgumentException();
				}
			}

			public void BeforeChildThreadStart( uint ThreadId )
			{
				if ( ( this.mask & 128 ) != 0 )
				{
					throw new ArgumentException();
				}
			}

			public void BeforeFixtureStart()
			{
				if ( ( this.mask & 256 ) != 0 )
				{
					throw new ArgumentException();
				}
			}

			public Cfixctl.CFIXCTL_REPORT_DISPOSITION FailedAssertion(
				string Expression, string Routine, string File, string Message, 
				uint Line, uint LastError, uint Flags, uint Reserved, Cfixctl.ICfixStackTrace StackTrace )
			{
				if ( ( this.mask & 512 ) != 0 )
				{
					throw new ArgumentException();
				}

				return Cfixctl.CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionContinue;
			}

			public Cfixctl.CFIXCTL_REPORT_DISPOSITION FailedRelateAssertion( 
				Cfixctl.CFIXCTL_RELATE_OPERATOR Operator, object ExpectedValue, 
				object ActualValue, string Routine, string File, string Message, uint Line, uint LastError, 
				uint Flags, uint Reserved, Cfixctl.ICfixStackTrace StackTrace )
			{
				if ( ( this.mask & 1024 ) != 0 )
				{
					throw new ArgumentException();
				}

				return Cfixctl.CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionContinue;
			}

			public Cfixctl.ICfixTestÌtemEventSink GetTestItemEventSink( 
				uint TestCaseOrdinal, uint ThreadId )
			{
				if ( ( this.mask & 2048 ) != 0 )
				{
					throw new ArgumentException();
				}
				
				return this;
			}

			public void Inconclusive( 
				string Message, uint Reserved, Cfixctl.ICfixStackTrace StackTrace )
			{
				if ( ( this.mask & 4096 ) != 0 )
				{
					throw new ArgumentException();
				}
			}

			public void Log( 
				string Message, uint Reserved, Cfixctl.ICfixStackTrace StackTrace )
			{
				if ( ( this.mask & 8192 ) != 0 )
				{
					throw new ArgumentException();
				}
			}

			public Cfixctl.CFIXCTL_REPORT_DISPOSITION QueryDefaultFailedAssertionDisposition()
			{
				if ( ( this.mask & 16384 ) != 0 )
				{
					throw new ArgumentException();
				}

				return Cfixctl.CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionContinue;
			}

			public Cfixctl.CFIXCTL_REPORT_DISPOSITION QueryDefaultUnhandledExceptionDisposition()
			{
				if ( ( this.mask & 32768 ) != 0 )
				{
					throw new ArgumentException();
				}

				return Cfixctl.CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionContinue;
			}

			public Cfixctl.CFIXCTL_REPORT_DISPOSITION UnhandledException( 
				uint ExceptionCode, uint Reserved, Cfixctl.ICfixStackTrace StackTrace )
			{
				if ( ( this.mask & 65536 ) != 0 )
				{
					throw new ArgumentException();
				}

				return Cfixctl.CFIXCTL_REPORT_DISPOSITION.CfixctlDispositionContinue;
			}

			public void AfterTestCaseFinish( int RanToCompletion )
			{
				if ( ( this.mask & 131072 ) != 0 )
				{
					throw new ArgumentException();
				}
			}

			public void BeforeTestCaseStart()
			{
				if ( ( this.mask & 262144 ) != 0 )
				{
					throw new ArgumentException();
				}
			}
		}

		private Agent ooProcTarget;
		private Agent inProcTarget;
		private AgentSet multiTarget;
		private String binDir;

		[SetUp]
		public void Setup()
		{
			this.ooProcTarget = Agent.CreateLocalAgent(
				Architecture.I386,
				false );
			this.ooProcTarget.DefaultEnvironment.MergeEnvironmentVariables(
				Environment.GetEnvironmentVariables() );

			this.inProcTarget = Agent.CreateLocalAgent(
				Architecture.I386,
				true );

			this.multiTarget = new AgentSet();
			this.multiTarget.AddArchitecture( ooProcTarget );

			this.binDir = new FileInfo(
				Assembly.GetExecutingAssembly().FullName ).Directory.FullName;
		}

		[TearDown]
		public void TearDown()
		{
			if ( this.ooProcTarget != null )
			{
				this.ooProcTarget.Dispose();
			}

			if ( this.inProcTarget != null )
			{
				this.inProcTarget.Dispose();
			}
		}

		private ITestItemCollection GetFixture( TestModule mod, string name )
		{
			foreach ( ITestItem item in mod )
			{
				if ( item.Name == name )
				{
					return ( ITestItemCollection ) item;
				}
			}

			Assert.Fail( "Fixture not available" );
			return null;
		}

		[Ignore]
		[Test]
		public void TestHostHandlingOfSinkExceptions()
		{
			using ( IHost host = this.inProcTarget.CreateHost() )
			using ( IHost oohost = this.ooProcTarget.CreateHost() )
			using ( TestModule mod = ( TestModule ) host.LoadModule(
					null,
					this.binDir + "\\testmanaged.dll",
					true ) )
			using ( TestFixture fixture =
				( TestFixture ) GetFixture( mod, "SinkTest" ) )
			{
				Cfixctl.ICfixTestItem ctlItem = fixture.GetNativeItem( oohost );
				try
				{
					Cfixctl.ICfixAction ctlAction = ctlItem.CreateExecutionAction( 0, 0 );
					try
					{
						//
						// Try all combinations and look for host crashes.
						//
						for ( int mask = 0;
							  mask <= 524287;
							  mask++ )
						{
							try
							{
								Debug.Print( "Mask: " + mask );
								Sink sink = new Sink( mask );
								ctlAction.Run(
									sink,
									( uint ) EnvironmentOptions.ComNeutralThreading );
							}
							catch ( ArgumentException )
							{
								//
								// Passed through, ok.
								//
							}
							catch ( COMException x )
							{
								//
								// Cfixctl HRESULTs are ok, too.
								//
								Assert.AreEqual(
									0x8004b,
									( x.ErrorCode >> 12 ) & 0xFFFFF );
							}
						}
					}
					finally
					{
						Marshal.ReleaseComObject( ctlAction );
					}
				}
				finally
				{
					Marshal.ReleaseComObject( ctlItem );
				}
			}
			//524287
		}
	}
}
