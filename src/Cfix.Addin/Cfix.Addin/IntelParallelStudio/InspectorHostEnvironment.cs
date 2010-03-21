using System;
using System.Collections.Generic;
using System.Text;
using Cfix.Control;

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
			InspectorLevel level 
			)
			: base()
		{
			this.guid = Guid.NewGuid();
			this.level = level;
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

		protected override HostEnvironment CreateNew()
		{
			return new InspectorHostEnvironment( this.level );
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
