using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Cfix.Control;

namespace Cfix.Addin
{
	public class Configuration : IDisposable
	{
		private readonly String[] supportedExtensions = new String[] { ".DLL", ".SYS" };

		private const String BaseKeyPath = "Software\\cfix\\addin\\1.0";
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
		 * Settings (constants).
		 */

		public String[] SupportedExtensions
		{
			get { return this.supportedExtensions; }
		}
		
		public bool IsSupportedTestModuleExtension( string extension )
		{
			foreach ( String supported in this.SupportedExtensions )
			{
				Debug.Assert( supported.ToUpper() == supported );
				if ( supported == extension.ToUpper() )
				{
					return true;
				}
			}

			return false;
		}

		public bool IsSupportedTestModulePath( string path )
		{
			return IsSupportedTestModuleExtension(
				new FileInfo( path ).Extension );
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

		public bool UseComNeutralThread
		{
			get
			{
				return ( ( int ) this.key.GetValue( "UseComNeutralThread", 0 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"UseComNeutralThread",
					value ? 1 : 0,
					RegistryValueKind.DWord );
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
				return ( ( int ) this.key.GetValue( "ExplorerWindowVisible", 1 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"ExplorerWindowVisible",
					value ? 1 : 0,
					RegistryValueKind.DWord );
			}
		}

		public bool RunWindowVisible
		{
			get
			{
				return ( ( int ) this.key.GetValue( "RunWindowVisible", 1 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"RunWindowVisible",
					value ? 1 : 0,
					RegistryValueKind.DWord );
			}
		}

		public Disposition DefaultFailedAssertionDisposition
		{
			get
			{
				Disposition val = ( Disposition ) this.key.GetValue( 
					"DefaultFailedAssertionDisposition", Disposition.Break );
				if ( Enum.IsDefined( typeof( Disposition ), val ) )
				{
					return val;
				}
				else
				{
					return Disposition.Break;
				}
			}
			set
			{
				this.key.SetValue(
					"DefaultFailedAssertionDisposition",
					value,
					RegistryValueKind.DWord );
			}
		}

		public Disposition DefaultUnhandledExceptionDisposition
		{
			get
			{
				Disposition val = ( Disposition ) this.key.GetValue( 
					"DefaultUnhandledExceptionDisposition", Disposition.Continue );
				if ( Enum.IsDefined( typeof( Disposition ), val ) )
				{
					return val;
				}
				else
				{
					return Disposition.Continue;
				}
			}
			set
			{
				this.key.SetValue(
					"DefaultUnhandledExceptionDisposition",
					value,
					RegistryValueKind.DWord );
			}
		}

		public SchedulingOptions SchedulingOptions
		{
			get
			{
				SchedulingOptions val = ( SchedulingOptions ) this.key.GetValue(
					"SchedulingOptions", SchedulingOptions.None );
				if ( Enum.IsDefined( typeof( SchedulingOptions ), val ) )
				{
					return val;
				}
				else
				{
					return SchedulingOptions.None;
				}
			}
			set
			{
				this.key.SetValue(
					"SchedulingOptions",
					value,
					RegistryValueKind.DWord );
			}
		}

		public ThreadingOptions ThreadingOptions
		{
			get
			{
				ThreadingOptions val = ( ThreadingOptions ) this.key.GetValue(
					"ThreadingOptions", ThreadingOptions.ComNeutralThreading );
				if ( Enum.IsDefined( typeof( ThreadingOptions ), val ) )
				{
					return val;
				}
				else
				{
					return ThreadingOptions.ComNeutralThreading;
				}
			}
			set
			{
				this.key.SetValue(
					"ThreadingOptions",
					value,
					RegistryValueKind.DWord );
			}
		}
	}
}
