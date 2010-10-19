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
using EnvDTE80;
using Cfix.Control.RunControl;
using Cfix.Addin.IntelParallelStudio;

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
			DTE2 dte
			)
		{
			this.key = key;

			if ( dte.Globals.get_VariableExists( CookieName ) )
			{
				//
				// Try using it.
				//
				try
				{
					this.cookie = uint.Parse( ( string ) dte.Globals[ CookieName ] );
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

				dte.Globals[ CookieName ] = this.cookie.ToString();
				dte.Globals.set_VariablePersists( CookieName, true );
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

		public static Configuration Load( DTE2 dte )
		{
			return new Configuration(
				Registry.CurrentUser.CreateSubKey( BaseKeyPath ),
				dte
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

		public RunCompilerType RunCompilerType
		{
			get
			{
				return ( RunCompilerType ) this.key.GetValue(
					"RunCompilerType",
					RunCompilerType.Simple );
			}
			set
			{
				this.key.SetValue(
					"RunCompilerType",
					value,
					RegistryValueKind.DWord );
			}
		}

		public bool AutoRegisterVcDirectories
		{
			get
			{
				return ( ( int ) this.key.GetValue( "AutoRegisterVcDirectories", 1 ) ) > 0;
			}
			set
			{
				this.key.SetValue(
					"AutoRegisterVcDirectories",
					value ? 1 : 0,
					RegistryValueKind.DWord );
			}
		}

#if INTELINSPECTOR
		public bool ShowInspectorCfixResults
		{
			get
			{
				return ( ( int ) this.key.GetValue( "ShowInspectorCfixResults", 0 ) ) == 1;
			}
			set
			{
				this.key.SetValue(
					"ShowInspectorCfixResults",
					value ? 1 : 0,
					RegistryValueKind.DWord );
			}
		}

		public InspectorLevel MostRecentlyUsedInspectorMemoryAnalysisLevel
		{
			get
			{
				return InspectorLevel.FromString(
					( string ) this.key.GetValue( "MruInspectorMemoryAnalysisLevel",
					InspectorLevel.CheckMemoryAccessIssues.ToString() ) ); 
			}
			set
			{
				this.key.SetValue( 
					"MruInspectorMemoryAnalysisLevel", value, RegistryValueKind.String );
			}
		}

		public InspectorLevel MostRecentlyUsedInspectorThreadingAnalysisLevel
		{
			get
			{
				return InspectorLevel.FromString(
					( string ) this.key.GetValue( "MruInspectorThreadingAnalysisLevel",
					InspectorLevel.CheckDeadlocksAndRaces.ToString() ) );
			}
			set
			{
				this.key.SetValue(
					"MruInspectorThreadingAnalysisLevel", value, RegistryValueKind.String );
			}
		}
#endif

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

		public string WinDbgInstallFolder32
		{
			get
			{
				return ( string ) this.key.GetValue( "WinDbgInstallFolder32", null ); ;
			}
			set
			{
				this.key.SetValue( "WinDbgInstallFolder32", value, RegistryValueKind.String );
			}
		}

		public string WinDbgInstallFolder64
		{
			get
			{
				return ( string ) this.key.GetValue( "WinDbgInstallFolder64", null ); ;
			}
			set
			{
				this.key.SetValue( "WinDbgInstallFolder64", value, RegistryValueKind.String );
			}
		}

		public string GetWinDbgInstallFolder( Architecture arch )
		{
			switch ( arch )
			{
				case Architecture.I386:
					return WinDbgInstallFolder32;

				case Architecture.Amd64:
					return WinDbgInstallFolder64;

				default:
					throw new ArgumentException();
			}
		}

		public string WinDbgAdditionalOptions
		{
			get
			{
				return ( string ) this.key.GetValue( "WinDbgAdditionalOptions", null ); ;
			}
			set
			{
				this.key.SetValue( "WinDbgAdditionalOptions", value, RegistryValueKind.String );
			}
		}

		public uint HostRegistrationTimeout
		{
			get
			{
				object value = this.key.GetValue(
					"HostRegistrationTimeout",
					( int ) Agent.DefaultHostRegistrationTimeout );
				return ( uint ) ( int ) value;
			}
			set
			{
				this.key.SetValue(
					"HostRegistrationTimeout",
					value,
					RegistryValueKind.DWord );
			}
		}

		public uint InstrumentedHostRegistrationTimeout
		{
			//
			// N.B. Use longer default timeout to compensate 
			// instrumentation slowdown.
			//

			get
			{
				object value = this.key.GetValue(
					"InstrumentedHostRegistrationTimeout",
					( int ) ( 50 * Agent.DefaultHostRegistrationTimeout ) );
				return ( uint ) ( int ) value;
			}
			set
			{
				this.key.SetValue(
					"InstrumentedHostRegistrationTimeout",
					value,
					RegistryValueKind.DWord );
			}
		}

		public EventDll EventDll32
		{
			get
			{
				string dll = ( string ) this.key.GetValue( "EventDll32", null );
				if ( String.IsNullOrEmpty( dll ) )
				{
					return null;
				}

				return new EventDll(
					dll,
					( string ) this.key.GetValue( "EventDllOptions32", null ) );
			}
			set
			{
				if ( value == null ||  String.IsNullOrEmpty( value.Path ) )
				{
					this.key.DeleteValue( "EventDll32", false );
				}
				else
				{
					this.key.SetValue( "EventDll32", value.Path, RegistryValueKind.String );
				}

				if ( value == null || String.IsNullOrEmpty( value.Options ) )
				{
					this.key.DeleteValue( "EventDllOptions32", false );
				}
				else
				{
					this.key.SetValue( "EventDllOptions32", value.Options, RegistryValueKind.String );
				}
			}
		}

		public EventDll EventDll64
		{
			get
			{
				string dll = ( string ) this.key.GetValue( "EventDll64", null );
				if ( String.IsNullOrEmpty( dll ) )
				{
					return null;
				}

				return new EventDll(
					dll,
					( string ) this.key.GetValue( "EventDllOptions64", null ) );
			}
			set
			{
				if ( value == null || String.IsNullOrEmpty( value.Path ) )
				{
					this.key.DeleteValue( "EventDll64", false );
				}
				else
				{
					this.key.SetValue( "EventDll64", value.Path, RegistryValueKind.String );
				}

				if ( value == null || String.IsNullOrEmpty( value.Options ) )
				{
					this.key.DeleteValue( "EventDllOptions64", false );
				}
				else
				{
					this.key.SetValue( "EventDllOptions64", value.Options, RegistryValueKind.String );
				}
			}
		}

		public EventDll GetEventDll( Architecture arch )
		{
			switch ( arch )
			{
				case Architecture.I386:
					return EventDll32;

				case Architecture.Amd64:
					return EventDll64;

				default:
					throw new ArgumentException();
			}
		}

		public void SetEventDll( Architecture arch, EventDll eventDll )
		{
			switch ( arch )
			{
				case Architecture.I386:
					this.EventDll32 = eventDll;
					break;

				case Architecture.Amd64:
					this.EventDll64 = eventDll;
					break;

				default:
					throw new ArgumentException();
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
