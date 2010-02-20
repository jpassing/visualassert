using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	public class HostEnvironment
	{
		private readonly object envLock = new object();
		private readonly IDictionary<string, string > env
			= new Dictionary<string, string>();

		private string currentDirectory;

		public string CurrentDirectory
		{
			get { return this.currentDirectory; }
			set { this.currentDirectory = value; }
		}

		public void MergeEnvironmentVariables( System.Collections.IDictionary vars )
		{
			foreach ( System.Collections.DictionaryEntry pair in vars )
			{
				//
				// N.B. Values may be composite, ignore.
				//
				Add( ( string ) pair.Key, ( string ) pair.Value );
			}
		}

		public HostEnvironment Merge( HostEnvironment other )
		{
			HostEnvironment merged = new HostEnvironment();
			foreach ( KeyValuePair< string, string > p in this.env )
			{
				merged.Add( p.Key, p.Value );
			}

			foreach ( KeyValuePair<string, string> p in other.env )
			{
				merged.Add( p.Key, p.Value );
			}

			if ( this.currentDirectory != null && 
				 other.currentDirectory != null &&
				 this.currentDirectory != other.currentDirectory )
			{
				throw new ArgumentException(
					"Conflicting current directory settings" );
			}
			else if ( this.currentDirectory != null )
			{
				merged.currentDirectory = this.currentDirectory;
			}
			else if ( other.currentDirectory != null )
			{
				merged.currentDirectory = other.currentDirectory;
			}

			return merged;
		}

		public void AddSearchPath( string module )
		{
			AddSearchPath( module, false );
		}

		public void AddSearchPath( string module, bool prioritize )
		{
			Add( "PATH", module, prioritize );
		}

		public void Add( string name, string value )
		{
			Add( name, value, false );
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower" )]
		public void Add( string name, string value, bool prioritize )
		{
			//
			// To avoid casing-conflicts, force everything to lcase.
			//
			name = name.ToLower();

			lock ( this.envLock )
			{
				string existing;
				if ( this.env.TryGetValue( name, out existing ) )
				{
					if ( prioritize )
					{
						//
						// Put new value in front.
						//
						value = String.Format( "{0};{1}", value, existing );
					}
					else
					{
						value = String.Format( "{0};{1}", existing, value );
					}
				}

				this.env[ name ] = value;
			}
		}

		public String NativeFormat
		{
			get
			{
				if ( this.env.Count == 0 )
				{
					return null;
				}
				else
				{
					StringBuilder buf = new StringBuilder();
					lock ( this.envLock )
					{
						foreach ( KeyValuePair<string, string> pair in this.env )
						{
							buf.Append( pair.Key );
							buf.Append( '=' );
							buf.Append( pair.Value );
							buf.Append( '\n' );
						}
					}
					return buf.ToString();
				}
			}
		}
	}
}
