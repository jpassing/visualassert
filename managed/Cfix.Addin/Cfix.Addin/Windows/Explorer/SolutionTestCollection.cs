using System;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;

namespace Cfix.Addin.Windows.Explorer
{
	internal class SolutionTestCollection : GenericTestItemCollection
	{
		public SolutionTestCollection(
			Solution2 solution
			)
			: base( null, solution.FileName )
		{
		}

	}
}
