using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Control
{
	/*++
		Container that allows enumeration to follow a customized order.
	
		Threadsafe.
	--*/
	public class SortedTestItemCollection : GenericTestItemCollection
	{
		private readonly IComparer< ITestItem > comparer;

		public class ByNameComparer : IComparer<ITestItem>
		{
			public int Compare( ITestItem x, ITestItem y )
			{
				return x.Name.CompareTo( y.Name );
			}
		}

		public SortedTestItemCollection( 
			ITestItemCollection parent, 
			String name,
			IComparer<ITestItem> comparer
			) : base( parent, name )
		{
			this.comparer = comparer;
		}

		protected override IEnumerator<ITestItem> CreateEnumerator()
		{
			SortedList<ITestItem, object> sortedList =
				new SortedList<ITestItem, object>( this.comparer );

			foreach ( ITestItem item in this.list )
			{
				sortedList.Add( item, null );
			}

			return sortedList.Keys.GetEnumerator();
		}
	}
}
