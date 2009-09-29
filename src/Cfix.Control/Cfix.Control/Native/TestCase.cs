using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cfixctl;


namespace Cfix.Control.Native
{
	public class TestCase : NativeTestItem, ITestCodeElement
	{
		internal TestCase(
			NativeTestItemCollection parent,
			uint ordinal,
			ICfixTestItem item )
			: base( parent, ordinal, item )
		{
		}

		/*----------------------------------------------------------------------
		 * ITestCodeElement.
		 */

		public ApiType ApiType 
		{ 
			get 
			{ 
				//
				// The fixture determines the API type.
				//
				return ( ( ITestCodeElement ) this.Parent ).ApiType;
			}
		}

		public string CodeElementName 
		{
			get 
			{
				switch ( ApiType )
				{
					case ApiType.CfixBase:
						return this.Name;

					case ApiType.CfixCc:
						return String.Format( "{0}::{1}", Parent.Name, this.Name );

					case ApiType.WinUnitFixture:
						return String.Format( "{0}_{1}", Parent.Name, this.Name );

					case ApiType.WinUnitStandalone:
						return this.Name;

					default:
						Debug.Fail( "Unrecognized ApiType" );
						return null;
				}
			}
		}
	}
}
