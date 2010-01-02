using System;
using NUnit.Framework;
using Cfix.Addin;
using System.IO;
using System.Reflection;

namespace Cfix.Addin.Test
{
	[TestFixture]
	public class TestUpdateCheck
	{
		[TestCase, ExpectedException( typeof( FileNotFoundException ) )]
		public void ReadFileNotFound()
		{
			UpdateCheck.ReadCurrentVersionInfo( "notexistant" );
		}

		[TestCase]
		public void ReadValidOutdatedFile()
		{
			string binPath = new FileInfo( Assembly.GetExecutingAssembly().FullName ).Directory.FullName;
			string testDir = binPath + @"..\..\..\..\src\Cfix.Addin\Cfix.Addin.Test\test\";

			VersionInfo info = UpdateCheck.ReadCurrentVersionInfo( 
				testDir + "valid-versioninfo.xml" );

			Assert.AreEqual( "1.0.3.2654", info.Version.ToString() );
			Assert.AreEqual(
				@"http://www.visualassert.com/download/VisualAssert_1.0.3.3654.msi",
				info.DownloadUrl );
			Assert.IsFalse( info.IsNewer );
		}

		[TestCase]
		public void ReadValidNewerFile()
		{
			string binPath = new FileInfo( Assembly.GetExecutingAssembly().FullName ).Directory.FullName;
			string testDir = binPath + @"..\..\..\..\src\Cfix.Addin\Cfix.Addin.Test\test\";

			VersionInfo info = UpdateCheck.ReadCurrentVersionInfo(
				testDir + "valid-versioninfo2.xml" );

			Assert.AreEqual( "9.0.3.2654", info.Version.ToString() );
			Assert.AreEqual(
				@"http://www.visualassert.com/download/VisualAssert_1.0.3.3654.msi",
				info.DownloadUrl );
			Assert.IsTrue( info.IsNewer );
		}

		[TestCase, ExpectedException( typeof( CfixAddinException ) )]
		public void ReaIncompleteFile()
		{
			string binPath = new FileInfo( Assembly.GetExecutingAssembly().FullName ).Directory.FullName;
			string testDir = binPath + @"..\..\..\..\src\Cfix.Addin\Cfix.Addin.Test\test\";

			VersionInfo info = UpdateCheck.ReadCurrentVersionInfo(
				testDir + "incomplete-versioninfo.xml" );
		}
	}

}
