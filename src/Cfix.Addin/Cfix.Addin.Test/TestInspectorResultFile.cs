using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Cfix.Addin.IntelParallelStudio;
using System.IO;
using System.Reflection;
using Cfix.Control;

namespace Cfix.Addin.Test
{
	[TestFixture]
	public class TestInspectorResultFile
	{
		private String testdataDir;
		
		[SetUp]
		public void Setup()
		{
			string binDir = new FileInfo(
				Assembly.GetExecutingAssembly().FullName ).Directory.FullName;
			this.testdataDir =
				binDir +
				@"\..\..\..\src\Cfix.Addin\Cfix.Addin.Test\test\IntelParallelStudio\InspectorResults";
		}

		[Test]
		public void FileNotFoundRaisesInspectorException()
		{
			try
			{
				InspectorResultFile.Parse( "idonotexist.pdr" );
				Assert.Fail();
			}
			catch ( InspectorException )
			{
			}
		}

		[Test]
		public void ReadCorrectNumberOfResults()
		{
			InspectorResultFile file = 
				InspectorResultFile.Parse( this.testdataDir + @"\01.pdr" );

			Assert.AreEqual( 17, file.Results.Count );
		}

		[Test]
		public void ReadCorrectValues01()
		{
			InspectorResultFile file =
				InspectorResultFile.Parse( this.testdataDir + @"\01.pdr" );

			IEnumerator<InspectorResult> en = file.Results.GetEnumerator();
			en.MoveNext();
			InspectorResult result = en.Current;

			Assert.AreEqual( 201, result.Type );
			Assert.AreEqual( "Thread information", result.Description );
			Assert.AreEqual( InspectorResult.ResultSeverity.Information, result.Severity );
			Assert.AreEqual( 2428, result.ThreadId );
			Assert.IsNotNull( result.StackTrace );

			Assert.AreEqual( 2, result.StackTrace.FrameCount );

			IEnumerator<IStackTraceFrame> frames = result.StackTrace.GetEnumerator();
			frames.MoveNext();

			IStackTraceFrame frame = frames.Current;
			Assert.AreEqual( "cfixhs32.exe", frame.Module );
			Assert.AreEqual( "_tmainCRTStartup", frame.Function );
			Assert.AreEqual( frame.Function, result.Function );
			Assert.AreEqual( 0, frame.Dispacement );
			Assert.AreEqual( @"startup\crt0.c", frame.SourceFile );
			Assert.AreEqual( frame.SourceFile, result.SourceFile );
			Assert.AreEqual( 475, frame.SourceLine );
			Assert.AreEqual( frame.SourceLine, result.SourceLine );

			en.MoveNext();
			result = en.Current;

			Assert.AreEqual( "foo", result.Description );
			Assert.AreEqual( InspectorResult.ResultSeverity.Warning, result.Severity );
		}

		[Test]
		public void ReadCorrectValues02()
		{
			InspectorResultFile file =
				InspectorResultFile.Parse( this.testdataDir + @"\02.pdr" );

			IEnumerator<InspectorResult> en = file.Results.GetEnumerator();
			en.MoveNext();
			InspectorResult result = en.Current;

			Assert.AreEqual( 201, result.Type );
			Assert.AreEqual( InspectorResult.ResultSeverity.Information, result.Severity );
			Assert.AreEqual( 6616, result.ThreadId );
			Assert.IsNull( result.StackTrace );

			en.MoveNext();
			result = en.Current;

			Assert.AreEqual( 202, result.Type );
			Assert.AreEqual( InspectorResult.ResultSeverity.Information, result.Severity );
			Assert.AreEqual( 3152, result.ThreadId );
			Assert.IsNotNull( result.StackTrace );
			Assert.AreEqual( 1, result.StackTrace.FrameCount );

			IEnumerator<IStackTraceFrame> frames = result.StackTrace.GetEnumerator();
			frames.MoveNext();

			Assert.AreEqual( "cfixctl.dll", frames.Current.Module );
			Assert.AreEqual( "0xce2e", frames.Current.Function );
		}

		[Test]
		public void IgnoreSecondThread()
		{
			InspectorResultFile file =
				InspectorResultFile.Parse( this.testdataDir + @"\ignoresecondthread.pdr" );

			IEnumerator<InspectorResult> en = file.Results.GetEnumerator();
			en.MoveNext();
			InspectorResult result = en.Current;

			Assert.AreEqual( "Thread information", result.Description );
			Assert.AreEqual( InspectorResult.ResultSeverity.Information, result.Severity );
			Assert.AreEqual( 2428, result.ThreadId );
			Assert.IsNotNull( result.StackTrace );

			Assert.AreEqual( 2, result.StackTrace.FrameCount );
		}

		[Test]
		public void UseSecondThreadIfFirstHasNoStackTrace()
		{
			InspectorResultFile file =
				InspectorResultFile.Parse( this.testdataDir + @"\multiplethreads.pdr" );

			IEnumerator<InspectorResult> en = file.Results.GetEnumerator();
			en.MoveNext();
			InspectorResult result = en.Current;

			Assert.AreEqual( "Potential privacy infringement", result.Description );
			Assert.AreEqual( InspectorResult.ResultSeverity.Warning, result.Severity );
			Assert.AreEqual( 4580, result.ThreadId );
			Assert.IsNotNull( result.StackTrace );
			Assert.AreEqual( 1, result.StackTrace.FrameCount );
		}

		[Test]
		public void EmptyLocEntriesAreIgnored()
		{
			InspectorResultFile file =
				InspectorResultFile.Parse( this.testdataDir + @"\emptyloc.pdr" );

			IEnumerator<InspectorResult> en = file.Results.GetEnumerator();
			en.MoveNext();
			InspectorResult result = en.Current;
			Assert.AreEqual( 1, result.StackTrace.FrameCount );

			IEnumerator<IStackTraceFrame> frames = result.StackTrace.GetEnumerator();
			frames.MoveNext();
		}

		[Test]
		public void EmptyStackTracesAreIgnored()
		{
			InspectorResultFile file =
				InspectorResultFile.Parse( this.testdataDir + @"\emptytrace.pdr" );

			IEnumerator<InspectorResult> en = file.Results.GetEnumerator();
			en.MoveNext();
			InspectorResult result = en.Current;
			Assert.IsNull( result.StackTrace );
			Assert.IsNull( result.Function );
			Assert.AreEqual( 0, result.SourceLine );
		}

		[Test]
		public void MissingStackTracesAreIgnored()
		{
			InspectorResultFile file =
				InspectorResultFile.Parse( this.testdataDir + @"\emptytrace2.pdr" );

			IEnumerator<InspectorResult> en = file.Results.GetEnumerator();
			en.MoveNext();
			InspectorResult result = en.Current;
			Assert.IsNull( result.StackTrace );
		}

		[Test]
		public void ModulePathIsStrippedToBaseName()
		{
			InspectorResultFile file =
				InspectorResultFile.Parse( this.testdataDir + @"\modulewithpath.pdr" );

			IEnumerator<InspectorResult> en = file.Results.GetEnumerator();
			en.MoveNext();
			InspectorResult result = en.Current;
			Assert.AreEqual( 1, result.StackTrace.FrameCount );

			IEnumerator<IStackTraceFrame> frames = result.StackTrace.GetEnumerator();
			frames.MoveNext();

			Assert.AreEqual( "cfixhs32.exe", frames.Current.Module );
		}

		[Test]
		public void EmptySourceLineIsTreatedAsZero()
		{
			InspectorResultFile file =
				InspectorResultFile.Parse( this.testdataDir + @"\emptyline.pdr" );

			IEnumerator<InspectorResult> en = file.Results.GetEnumerator();
			en.MoveNext();
			InspectorResult result = en.Current;
			Assert.AreEqual( 1, result.StackTrace.FrameCount );

			IEnumerator<IStackTraceFrame> frames = result.StackTrace.GetEnumerator();
			frames.MoveNext();

			Assert.AreEqual( 0, frames.Current.SourceLine );
		}

		[Test]
		public void NonNumericSourceLineRaisesException()
		{
			try
			{
				InspectorResultFile.Parse( "nonnumline.pdr" );
				Assert.Fail();
			}
			catch ( InspectorException )
			{
			}
		}

		[Test]
		public void TestFilterAll()
		{
			try
			{
				InspectorResultFile	file = InspectorResultFile.Parse( "01.pdr" );
				Assert.IsTrue( file.Results.Count > 0 );

				InspectorResultFile filtered = file.Filter(
					( InspectorResultFile.FilterDelegate ) delegate( InspectorResult result )
					{
						return false;
					} );

				Assert.AreEqual( 0, filtered.Results.Count );
			}
			catch ( InspectorException )
			{
			}
		}

		[Test]
		public void TestFilterOne()
		{
			try
			{
				InspectorResultFile file = InspectorResultFile.Parse( "01.pdr" );
				Assert.IsTrue( file.Results.Count > 0 );

				int count = 0;
				InspectorResultFile filtered = file.Filter(
					( InspectorResultFile.FilterDelegate ) delegate( InspectorResult result )
					{
						return count++ == 0;
					} );

				Assert.AreEqual( file.Results.Count - 1, filtered.Results.Count );
			}
			catch ( InspectorException )
			{
			}
		}

		[Test]
		public void TestGetDescriptionFromCode()
		{
			Assert.AreEqual(
				"Data race",
				InspectorResult.GetDescriptionFromCode( "pdr_race" ) );
			Assert.AreEqual(
				"Data race",
				InspectorResult.GetDescriptionFromCode( "pdr_race_rw" ) );
			Assert.AreEqual(
				"Data race",
				InspectorResult.GetDescriptionFromCode( "pdr_race_xs_ay" ) );
			Assert.AreEqual( 
				"pdr_xs_ay",
				InspectorResult.GetDescriptionFromCode( "pdr_xs_ay" ) );
		}
	}
}
