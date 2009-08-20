using System;
using Cfixctl;

namespace Cfix.Control.Native
{
	public enum ApiType
	{
		Unknown = CfixTestApiType.CfixTestApiUnknown,
		CfixBase = CfixTestApiType.CfixTestApiTypeBase,
		CfixCc = CfixTestApiType.CfixTestApiTypeCc,
		WinUnit = CfixTestApiType.CfixTestApiTypeWinUnit,
	}

	public interface ITestCodeElement
	{
		ApiType ApiType { get; }
		string CodeElementName { get; }
	}
}
