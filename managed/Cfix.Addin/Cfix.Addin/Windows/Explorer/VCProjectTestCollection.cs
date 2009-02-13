using System;
using EnvDTE;
using EnvDTE80;
using Cfix.Control;

namespace Cfix.Addin.Windows.Explorer
{
	internal class VCProjectTestCollection : GenericTestItemCollection
	{
		private readonly string uniqueName;

		public VCProjectTestCollection(
			Project project
			)
			: base( null, project.Name )
		{
			this.uniqueName = project.UniqueName;
		}

		public string UniqueName
		{
			get { return uniqueName; }
		} 

	}
}
