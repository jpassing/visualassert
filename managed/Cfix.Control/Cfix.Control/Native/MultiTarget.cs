using System;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control.Native
{
	public class MultiTarget : IDisposable
	{
		private Agent[] targets =
			new Agent[ ( int ) Architecture.Max + 1 ];

		~MultiTarget()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			foreach ( Agent tgt in this.targets )
			{
				if ( tgt != null )
				{
					tgt.Dispose();
				}
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		public void AddArchitecture( Agent target )
		{
			if ( target == null )
			{
				throw new ArgumentException( "null", "target" );
			}

			Architecture arch = target.Architecture;
			Debug.Assert( this.targets[ ( int ) arch ] == null );
			this.targets[ ( int ) arch ] = target;
		}

		public Agent GetTarget( Architecture arch )
		{
			Agent value = this.targets[ ( int ) arch ];
			if ( value != null )
			{
				return value;
			}
			else
			{
				throw new UnsupportedArchitectureException();
			}
		}

		public bool IsArchitectureSupported( Architecture arch )
		{
			return this.targets[ ( int ) arch ] != null;
		}

		internal Architecture GetArchitectures()
		{
			Architecture arch = 0;
			for ( int i = 0; i < this.targets.Length; i++ )
			{
				if ( this.targets[ i ] != null )
				{
					arch |= this.targets[ i ].Architecture;
				}
			}

			return arch;
		}

	}
}
