using System;
using System.Collections.Generic;
using System.Text;
using Cfixctl;

namespace Cfix.Control.Native
{
	/*++
	 * Class Description:
	 *		Represents an EXE test module.
	 * 
	 *		Threadsafe.
	 --*/
	public class HostTestModule : TestModule
	{
		internal HostTestModule(
			ITestItemCollection parentCollection,
			String path,
			Agent target,
			ICfixTestModule ctlModule,
			bool ignoreDuplicates
			)
			: base( parentCollection, path, target, ctlModule, ignoreDuplicates )
		{
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		internal override ICfixTestItem GetNativeItem( IHost host )
		{
			//
			// Load the host module itself.
			//
			return ( ( Host ) host ).GetNativeItem().LoadModule( null );
		}
	}
}
