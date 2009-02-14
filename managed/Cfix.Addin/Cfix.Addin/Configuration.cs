using System;
using Microsoft.Win32;

namespace Cfix.Addin
{
	public class Configuration : IDisposable
	{
		private const String BaseKeyPath = "Software\\cfix\\cfixplus\\1.0";
		private readonly RegistryKey key;

		private Configuration(
			RegistryKey key 
			)
		{
			this.key = key;
		}

		~Configuration()
		{
			Dispose( false );
		}

		protected void Dispose( bool disposing )
		{
			this.key.Close();
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		public static Configuration Load()
		{
			return new Configuration(
				Registry.CurrentUser.CreateSubKey( BaseKeyPath )
				);
		}

		/*----------------------------------------------------------------------
		 * Settings.
		 */

		public void Reset()
		{
			foreach ( String valueName in this.key.GetValueNames() )
			{
				key.DeleteValue( valueName );
			}
		}

		public bool KernelModeFeaturesEnabled
		{
			get
			{
				return ( ( int ) this.key.GetValue( "KernelMode", 0 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"KernelMode",
					value ? 1 : 0,
					RegistryValueKind.DWord );
			}
		}

		public bool AutoRefreshAfterBuild
		{
			get
			{
				return ( ( int ) this.key.GetValue( "AutoRefreshAfterBuild", 0 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"AutoRefreshAfterBuild",
					value ? 1 : 0,
					RegistryValueKind.DWord );
			}
		}

		public bool ExplorerWindowVisible
		{
			get
			{
				return ( ( int ) this.key.GetValue( "ExplorerWindowVisible", 0 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"ExplorerWindowVisible",
					value ? 1 : 0,
					RegistryValueKind.DWord );
			}
		}
	}
}
