using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Reflection;

namespace Cfix.Addin
{
	public struct VersionInfo
	{
		public Version Version;
		public string DownloadUrl;

		public bool IsNewer
		{
			get
			{
				Version curVersion = Assembly.GetExecutingAssembly().GetName().Version;
				return curVersion.CompareTo( this.Version ) < 0;
			}
		}
	}
		
	public static class UpdateCheck
	{
		private const string CurrentVersionInfoUrl = 
			@"http://www.visualassert.com/download/versioninfo.xml";

		public static VersionInfo ReadCurrentVersionInfo()
		{
			return ReadCurrentVersionInfo( CurrentVersionInfoUrl );
		}

		public static VersionInfo ReadCurrentVersionInfo( string url )
		{
			XmlTextReader reader = null;
			
			try
			{
				reader = new XmlTextReader( url );

				reader.MoveToContent();

				VersionInfo versionInfo = new VersionInfo();

				if ( reader.NodeType == XmlNodeType.Element &&
					  reader.Name == "versioninfo" )
				{
					string currentElementName = null;

					while ( reader.Read() )
					{
						if ( reader.NodeType == XmlNodeType.Element )
						{
							currentElementName = reader.Name;
						}
						else if ( reader.NodeType == XmlNodeType.Text )
						{
							switch ( currentElementName )
							{
								case "version":
									versionInfo.Version = new Version( reader.Value );
									break;

								case "url":
									versionInfo.DownloadUrl = reader.Value;
									break;
							}
						}
					}
				}
				else
				{
					throw new CfixAddinException(
						"Malformed version information" );
				}

				if ( versionInfo.DownloadUrl != null && versionInfo.Version != null )
				{
					return versionInfo;
				}
				else
				{
					throw new CfixAddinException( "Incomplete version information" );
				}
			}
			finally
			{
				if ( reader != null )
				{
					reader.Close();
				}
			}
		}
	}
}
