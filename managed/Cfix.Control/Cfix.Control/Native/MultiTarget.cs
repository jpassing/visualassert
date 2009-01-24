using System;
using System.Diagnostics;
using Cfixctl;

namespace Cfix.Control.Native
{
	public class MultiTarget
	{
		private Target[] targets =
			new Target[ ( int ) Architecture.Max ];

		public void AddArchitecture( Target target )
		{
			Architecture arch = target.Architecture;
			Debug.Assert( this.targets[ ( int ) arch ] == null );
			this.targets[ ( int ) arch ] = target;
		}

		public Target GetTarget( Architecture arch )
		{
			Target value = this.targets[ ( int ) arch ];
			if ( value != null )
			{
				return value;
			}
			else
			{
				throw new UnsupportedArchitectureException();
			}
		}

		public Target GetAnyTarget()
		{
			for ( int i = 0; i < this.targets.Length; i++ )
			{
				if ( this.targets[ i ] != null )
				{
					return this.targets[ i ];
				}
			}

			throw new UnsupportedArchitectureException();
		}

		public bool IsArchitectureSupported( Architecture arch )
		{
			return this.targets[ ( int ) arch ] != null;
		}

		internal uint GetArchitectureMask()
		{
			uint mask = 0;
			for ( int i = 0; i < this.targets.Length; i++ )
			{
				if ( this.targets[ i ] != null )
				{
					if ( mask > 0 )
					{
						return UInt32.MaxValue;
					}
					else
					{
						mask = ( uint ) this.targets[ i ].Architecture;
					}
				}
			}

			return mask;
		}

	}
}
