using System;
using System.Collections.Generic;
using System.Text;
using Cfix.Control.Native;
using Cfix.Addin.IntelParallelStudio;
using NUnit.Framework;

namespace Cfix.Addin.Test
{
	[TestFixture]
	public class TestResultFilter
	{
		[Test]
		public void TestFilter()
		{
			InspectorResult one = new InspectorResult(
				1,
				InspectorResult.ResultSeverity.Error,
				"foo",
				123,
				null );
			InspectorResult two = new InspectorResult(
				1,
				InspectorResult.ResultSeverity.Error,
				"foo",
				123,
				null );
			InspectorResult three = new InspectorResult(
				1,
				InspectorResult.ResultSeverity.Error,
				"foo",
				124,
				null );

			Assert.AreEqual( one, two );
			Assert.AreNotEqual( one, three );

			InspectorResultFilter filt = new InspectorResultFilter();
			Assert.IsFalse( filt.EqualsLastResult( one ) );
			Assert.IsTrue( filt.EqualsLastResult( one ) );
			Assert.IsTrue( filt.EqualsLastResult( two ) );
			Assert.IsFalse( filt.EqualsLastResult( three ) );
		}

	}
}
