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

		
	}
}
