using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.VCProjectEngine;
using EnvDTE;
using Cfix.Control;
using EnvDTE80;
using System.Reflection;
using System.Diagnostics;

namespace Cfix.Addin.Test
{
	internal class ICProjectTestCollection : VCProjectTestCollection
	{
		public ICProjectTestCollection(
			ITestItemCollection parent,
			Solution2 solution,
			Project project,
			AgentSet agents,
			Configuration config
			)
			: base(
				parent,
				solution,
				project,
				agents,
				config )
		{
			Debug.Assert( project.Kind == ProjectKinds.IcProject );
		}
	}
}
