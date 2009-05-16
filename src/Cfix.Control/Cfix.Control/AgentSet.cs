using System;
using System.Diagnostics;

namespace Cfix.Control
{
	public class AgentSet : IDisposable
	{
		private IAgent[] agent =
			new IAgent[ ( int ) Architecture.Max + 1 ];

		~AgentSet()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			foreach ( IAgent tgt in this.agent )
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

		public void AddArchitecture( IAgent target )
		{
			if ( target == null )
			{
				throw new ArgumentException( "null", "target" );
			}

			Architecture arch = target.Architecture;
			Debug.Assert( this.agent[ ( int ) arch ] == null );
			this.agent[ ( int ) arch ] = target;
		}

		public IAgent GetAgent( Architecture arch )
		{
			IAgent value = this.agent[ ( int ) arch ];
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
			return this.agent[ ( int ) arch ] != null;
		}

		internal Architecture GetArchitectures()
		{
			Architecture arch = 0;
			for ( int i = 0; i < this.agent.Length; i++ )
			{
				if ( this.agent[ i ] != null )
				{
					arch |= this.agent[ i ].Architecture;
				}
			}

			return arch;
		}

		public uint ActiveHostCount 
		{
			get
			{
				uint count = 0;

				for ( int i = 0; i < this.agent.Length; i++ )
				{
					if ( this.agent[ i ] != null )
					{
						count += this.agent[ i ].ActiveHostCount;
					}
				}

				return count;
			}
		}

		public void TerminateActiveHosts()
		{
			for ( int i = 0; i < this.agent.Length; i++ )
			{
				if ( this.agent[ i ] != null )
				{
					this.agent[ i ].TerminateActiveHosts();
				}
			}
		}
	}
}
