using System;
using System.IO;
using System.Reflection;

namespace Cfix.Addin
{
	internal class Configuration
	{
		private String iconDirectory;

		private Configuration()
		{
		}

		public static Configuration Load()
		{
			return new Configuration();
		}

		internal String IconsDirectory
		{
			get
			{
				if ( this.iconDirectory == null )
				{
					this.iconDirectory = Path.Combine(
						new FileInfo(
							Assembly.GetExecutingAssembly().Location ).Directory.FullName,
							"Icons" );
				}

				return this.iconDirectory;
			}
		}
	}
}
