using System;
using Cfixctl;

namespace Cfix.Control
{
	public abstract class Failure
	{
		private readonly String message;
		private readonly IStackTrace stackTrace;

		protected Failure(
			String message,
			IStackTrace stackTrace
			) 
		{
			this.message = message;
			this.stackTrace = stackTrace;
		}

		public String Message
		{
			get { return message; }
		}

		public IStackTrace StackTrace
		{
			get { return stackTrace; }  
		} 
	}

	public class Inconclusiveness : Failure
	{
		public Inconclusiveness(
			String reason,
			IStackTrace stackTrace
			)
			: base( reason, stackTrace )
		{
		}
	}

	public class UnhandledExceptionFailure : Failure
	{
		private readonly uint exceptionCode;

		public UnhandledExceptionFailure(
			uint exceptionCode,
			IStackTrace stackTrace
			)
			: base( String.Format( "[0x{0:X}]", exceptionCode ), stackTrace )
		{
			this.exceptionCode = exceptionCode;
		}

		public uint ExceptionCode
		{
			get { return this.exceptionCode; }
		} 

	}


	public abstract class CodeFailure : Failure
	{
		private readonly String file;
		private readonly uint line;
		private readonly String routine;

		protected CodeFailure(
			String message,
			String file,
			uint line,
			String routine,
			IStackTrace stackTrace
			)
			: base( message, stackTrace )
		{
			this.file = file;
			this.line = line;
			this.routine = routine;
		}


		public String File
		{
			get { return file; }
		}

		public uint Line
		{
			get { return line; }
		}

		public String Routine
		{
			get { return routine; }
		}
	}

	public class FailedAssertionFailure : CodeFailure
	{
		private readonly String expression;
		private readonly uint lastError;

		public FailedAssertionFailure(
			String expression,
			String message,
			String file,
			uint line,
			String routine,
			IStackTrace stackTrace,
			uint lastError
			)
			: base( message, file, line, routine, stackTrace )
		{
			this.expression = expression;
			this.lastError = lastError;
		}

		public String Expression
		{
			get { return expression; }
		}

		public uint LastError
		{
			get { return lastError; }
		} 

	}

	public enum RelateOperator
	{
		Equals = CFIXCTL_RELATE_OPERATOR.CfixctlRelateEquals,
		NotEquals = CFIXCTL_RELATE_OPERATOR.CfixctlRelateNotEquals,
		GreaterThan = CFIXCTL_RELATE_OPERATOR.CfixctlRelateGreaterThan,
		GreaterThanEquals = CFIXCTL_RELATE_OPERATOR.CfixctlRelateGreaterThanEquals,
		LessThan = CFIXCTL_RELATE_OPERATOR.CfixctlRelateLessThan,
		LessThanEquals = CFIXCTL_RELATE_OPERATOR.CfixctlRelateLessThanEquals,
	}

	public class FailedRelateExpressionFailure : CodeFailure
	{
		private readonly RelateOperator op;
		private readonly object expectedValue;
		private readonly object actualValue;
		private readonly uint lastError;

		public FailedRelateExpressionFailure(
			RelateOperator op,
			object expectedValue,
			object actualValue,
			String message,
			String file,
			uint line,
			String routine,
			IStackTrace stackTrace,
			uint lastError
			)
			: base( message, file, line, routine, stackTrace )
		{
			this.op = op;
			this.expectedValue = expectedValue;
			this.actualValue = actualValue;
			this.lastError = lastError;
		}

		public RelateOperator RelateOperator
		{
			get { return op; }  
		} 

		public object ExpectedValue
		{
			get { return expectedValue; }  
		} 

		public object ActualValue
		{
			get { return actualValue; }  
		} 

		public uint LastError
		{
			get { return lastError; }
		} 
	}

	public class GenericCodeInformation : CodeFailure
	{
		public GenericCodeInformation(
			String message,
			String file,
			uint line,
			String routine,
			IStackTrace stackTrace
			)
			: base( message, file, line, routine, stackTrace )
		{
		}
	}

	public class GenericCodeWarning : CodeFailure
	{
		public GenericCodeWarning(
			String message,
			String file,
			uint line,
			String routine,
			IStackTrace stackTrace
			)
			: base( message, file, line, routine, stackTrace )
		{
		}
	}

	public class GenericCodeError : CodeFailure
	{
		public GenericCodeError(
			String message,
			String file,
			uint line,
			String routine,
			IStackTrace stackTrace
			)
			: base( message, file, line, routine, stackTrace )
		{
		}
	}

	public class GenericError : Failure
	{
		public GenericError(
			String name,
			IStackTrace stackTrace
			)
			: base(name, stackTrace )
		{
		}
	}
	
}
