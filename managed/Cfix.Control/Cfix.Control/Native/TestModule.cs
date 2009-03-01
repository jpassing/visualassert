using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Cfixctl;

namespace Cfix.Control.Native
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix" )]
	public class TestModule : TestItemCollection
	{
		private readonly String path;
		private readonly Agent agent;

		//
		// N.B. We have to use a separate parent (rather than relying
		// on base.parent) to allow arbitrary objects implementing 
		// ITestItemCollection to serve as parent.
		//
		private readonly ITestItemCollection parentCollection;

		private CfixTestModuleType type = 0;

		internal TestModule(
			ITestItemCollection parentCollection,
			String path,
			Agent target,
			ICfixTestModule ctlModule,
			bool ignoreDuplicates
			)
			: base( null, 0, ctlModule, ignoreDuplicates )
		{
			this.parentCollection = parentCollection;
			this.path = path;
			this.agent = target;
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		internal override ICfixTestItem GetNativeItem( IHost host )
		{
			return ( ( Host ) host ).GetNativeItem().LoadModule( this.path );
		}

		public override ITestItemCollection Parent
		{
			get
			{
				return this.parentCollection;
			}
		}

		/*--------------------------------------------------------------
		 * Publics.
		 */

		internal override TestModule Module
		{
			get
			{
				return this;
			}
		}

		internal Agent Agent
		{
			get
			{
				return this.agent;
			}
		}

		public Architecture Architecture
		{
			get { return this.agent.Architecture; }
		}

		public CfixTestModuleType Type
		{
			get { return this.type; }
		}

		public String Path
		{
			get
			{
				return this.path;
			}
		}
	}
}
