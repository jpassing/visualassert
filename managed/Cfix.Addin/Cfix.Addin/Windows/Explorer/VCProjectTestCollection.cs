using System;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;

namespace Cfix.Addin.Windows.Explorer
{
	internal class VCProjectTestCollection : GenericTestItemCollection
	{
		public VCProjectTestCollection(
			Project project
			)
			: base( null, project.FileName )
		{
		}

	}
}
