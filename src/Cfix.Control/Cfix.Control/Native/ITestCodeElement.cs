using System;
using Cfixctl;

namespace Cfix.Control.Native
{
	public enum ApiType
	{
		Unknown = CfixTestApiType.CfixTestApiUnknown,
		CfixBase = CfixTestApiType.CfixTestApiTypeBase,
		CfixCc = CfixTestApiType.CfixTestApiTypeCc,
		WinUnitFixture = CfixTestApiType.CfixTestApiTypeWinUnitFixture,
		WinUnitStandalone = CfixTestApiType.CfixTestApiTypeWinUnitStandalone,
	}

	public interface ITestCodeElement
	{
		ApiType ApiType { get; }
		string CodeElementName { get; }
	}
}
