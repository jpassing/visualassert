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

		private string currentDirectory = null;

		public string CurrentDirectory
		{
			get { return this.currentDirectory; }
			set { this.currentDirectory = value; }
		}

		public void Merge( EnvironmentVariableTarget target )
		{
			foreach ( System.Collections.DictionaryEntry pair in
				Environment.GetEnvironmentVariables( target ) )
			{
				//
				// N.B. Values may be composite, ignore.
				//
				Add( ( string ) pair.Key, ( string ) pair.Value );
			}
		}

		public void AddSearchPath( string module )
		{
			Add( "PATH", module );
		}

		public void Add( string name, string value )
		{
			lock ( this.envLock )
			{
				string existing;
				if ( this.env.TryGetValue( name, out existing ) )
				{
					value = String.Format( "{0};{1}", existing, value );
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
							buf.Append( '\0' );
						}
					}

					buf.Append( '\0' );
					return buf.ToString();
				}
			}
		}
	}
}
