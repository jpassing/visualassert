/*----------------------------------------------------------------------
 * Purpose:
 *		VS Constants.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Addin
{
	public static class ProjectKinds
	{
		//
		// Regular VC++ project.
		//
		public const string VcProject = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";

		//
		// Intel VC++ project.
		//
		public const string IcProject = "{EAF909A5-FA59-4C3D-9431-0FCC20D5BCF9}";

		public const string SolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

		public static bool IsCppProjectKind( string kind )
		{
			return kind == VcProject || kind == IcProject;
		}

		public static bool IsSolutionFolderKind( string kind )
		{
			return kind == SolutionFolder;
		}
	}
}
