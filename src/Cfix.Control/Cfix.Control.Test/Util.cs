using System;
using Cfix.Control;

namespace Cfix.Control.Test
{
	static class Util
	{
		public delegate void VisitHandler( IResultItem item );

		public static void Traverse(
			IResultItemCollection coll,
			VisitHandler handler
			)
		{
			handler( coll );

			foreach ( IResultItem item in coll )
			{
				IResultItemCollection subColl = item as IResultItemCollection;
				if ( subColl != null )
				{
					Traverse( subColl, handler );
				}
				else
				{
					handler( item );
				}
			}
		}

		public static uint TrialLicenseCookie
		{
			get
			{
				return ( uint ) DateTime.Now.Subtract( DateTime.FromFileTime( 0 ) ).Days;
			}
		}
	}
}
