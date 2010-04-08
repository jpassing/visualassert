using System;
using System.Collections.Generic;
using System.Text;
using Cfix.Control;

#if INTELINSPECTOR
namespace Cfix.Addin.IntelParallelStudio
{
	/*++
		 Host Environment, augmented by a unique name to use for
		 naming inspector results.
	--*/
	public class InspectorHostEnvironment : HostEnvironment
	{
		private readonly InspectorLevel level;
		private readonly Guid guid;

		public InspectorHostEnvironment( 
			InspectorLevel level ,
			Guid guid
			)
			: base()
		{
			this.guid = guid;
			this.level = level;
		}

		public InspectorHostEnvironment(
			InspectorLevel level
			)
			: this( level, Guid.NewGuid() )
		{
		}

		public InspectorHostEnvironment(
			HostEnvironment baseEnv,
			InspectorLevel level
			)
			: base( baseEnv.CurrentDirectory, baseEnv.EnvironmentVariables )
		{
			this.guid = Guid.NewGuid();
			this.level = level;
		}

		protected override HostEnvironment CreateNew( HostEnvironment template )
		{
			InspectorHostEnvironment inspTemplate =
				template as InspectorHostEnvironment;

			if ( inspTemplate != null )
			{
				//
				// Keep guid.
				//
				return new InspectorHostEnvironment( this.level, inspTemplate.Guid );
			}
			else
			{
				return new InspectorHostEnvironment( this.level );
			}
		}

		public InspectorLevel InspectorLevel
		{
			get { return this.level; }
		}

		public Guid Guid
		{
			get { return this.guid; }
		}
	}
}
#endif