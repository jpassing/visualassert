/*----------------------------------------------------------------------
 * Purpose:
 *		User configuration.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Cfix.Control;
using Cfix.Control.Native;

namespace Cfix.Addin
{
	public class Configuration : IDisposable
	{
		private const string CookieName = "CfixCookie";
		private readonly String[] supportedExtensions = new String[] 
			{ ".DLL", ".SYS", ".EXE" };

		private const String BaseKeyPath = "Software\\VisualAssert\\1.0";
		private readonly RegistryKey key;

		//
		// Days elapsed since first use.
		//
		private readonly uint cookie;

		private Configuration(
			RegistryKey key, 
			VisualAssert addin 
			)
		{
			this.key = key;

			if ( addin.DTE.Globals.get_VariableExists( CookieName ) )
			{
				//
				// Try using it.
				//
				try
				{
					this.cookie = uint.Parse( ( string ) addin.DTE.Globals[ CookieName ] );
				}
				catch
				{
				}
			}

			if ( this.cookie == 0 )
			{
				//
				// Calculate and save.
				//
				this.cookie = ( uint ) DateTime.Now.Subtract( DateTime.FromFileTime( 0 ) ).Days;

				addin.DTE.Globals[ CookieName ] = this.cookie.ToString();
				addin.DTE.Globals.set_VariablePersists( CookieName, true );
			}
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

		public static Configuration Load( VisualAssert addin )
		{
			return new Configuration(
				Registry.CurrentUser.CreateSubKey( BaseKeyPath ), 
				addin
				);
		}

		internal uint Cookie
		{
			get { return this.cookie; }
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

		public bool ShowQuickStartPage
		{
			get
			{
				return ( ( int ) this.key.GetValue( "ShowQuickStartPage", 1 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"ShowQuickStartPage",
					value ? 1 : 0,
					RegistryValueKind.DWord );
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
				return ( ( int ) this.key.GetValue( "AutoRefreshAfterBuild", 1 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"AutoRefreshAfterBuild",
					value ? 1 : 0,
					RegistryValueKind.DWord );
			}
		}

		public bool ResultsScrollLock
		{
			get
			{
				return ( ( int ) this.key.GetValue( "ResultsScrollLock", 0 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"ResultsScrollLock",
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
				return ( ( int ) this.key.GetValue( "RunWindowVisible", 0 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"RunWindowVisible",
					value ? 1 : 0,
					RegistryValueKind.DWord );
			}
		}

		public HostCreationOptions HostCreationOptions
		{
			get
			{
				return ( HostCreationOptions ) this.key.GetValue(
					"HostCreationOptions", HostCreationOptions.None );
			}
			set
			{
				this.key.SetValue(
					"HostCreationOptions",
					value,
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

		public Disposition DefaultDebugFailedAssertionDisposition
		{
			get
			{
				Disposition val = ( Disposition ) this.key.GetValue(
					"DefaultDebugFailedAssertionDisposition", Disposition.Break );
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
					"DefaultDebugFailedAssertionDisposition",
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

		public Disposition DefaultDebugUnhandledExceptionDisposition
		{
			get
			{
				Disposition val = ( Disposition ) this.key.GetValue(
					"DefaultDebugUnhandledExceptionDisposition", Disposition.Break );
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
					"DefaultDebugUnhandledExceptionDisposition",
					value,
					RegistryValueKind.DWord );
			}
		}

		public ExecutionOptions ExecutionOptions
		{
			get
			{
				return ( ExecutionOptions ) this.key.GetValue(
					"ExecutionOptions", ExecutionOptions.None );
			}
			set
			{
				this.key.SetValue(
					"ExecutionOptions",
					value,
					RegistryValueKind.DWord );
			}
		}

		public EnvironmentOptions EnvironmentOptions
		{
			get
			{
				return ( EnvironmentOptions ) this.key.GetValue(
					"EnvironmentOptions",
					EnvironmentOptions.ComNeutralThreading | EnvironmentOptions.AutoAdjustCurrentDirectory );
			}
			set
			{
				this.key.SetValue(
					"EnvironmentOptions",
					value,
					RegistryValueKind.DWord );
			}
		}

		public string MostRecentlyUsedDirectory
		{
			get
			{
				return ( string ) this.key.GetValue( "MruDirectory", null );;
			}
			set
			{
				this.key.SetValue( "MruDirectory", value, RegistryValueKind.String );
			}
		}

		/*----------------------------------------------------------------------
		 * Advanced.
		 */

		public bool SearchOutOfProcess
		{
			get
			{
				return ( ( int ) this.key.GetValue( "SearchOutOfProcess", 0 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"SearchOutOfProcess",
					value ? 1 : 0,
					RegistryValueKind.DWord );
			}
		}
	}
}
