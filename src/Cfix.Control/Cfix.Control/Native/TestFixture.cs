using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfixctl;


namespace Cfix.Control.Native
{
	public class TestFixture : NativeTestItemCollection, ITestCodeElement
	{
		private ApiType apiType;

		internal TestFixture(
			NativeTestItemCollection parent,
			uint ordinal,
			ICfixTestItem item
			)
			: base( parent, ordinal, item )
		{
			this.apiType = ( ApiType )
				( ( ICfixTestFixture ) item ).GetApiType();
		}

		/*----------------------------------------------------------------------
		 * ITestCodeElement.
		 */

		public ApiType ApiType
		{
			get { return this.apiType; }
		}

		public string CodeElementName
		{
			get { return "__CfixFixturePe" + this.Name; }
		}
	}
}
