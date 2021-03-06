using System;
using System.Runtime.Serialization;

namespace Cfix.Control
{
    [Serializable]
    public class CfixException : Exception
    {
        public CfixException()
        {}

        protected CfixException( SerializationInfo info, StreamingContext ctx )
            : base( info, ctx )
        {}

        public CfixException( String msg ) 
            : base( msg )
        {}

        public CfixException( String msg, Exception inner )
            : base( msg, inner )
        {}
    }

	[Serializable]
	public class UnsupportedArchitectureException : CfixException
	{
		public UnsupportedArchitectureException()
		{ }

		protected UnsupportedArchitectureException( SerializationInfo info, StreamingContext ctx )
			: base( info, ctx )
		{ }

		public UnsupportedArchitectureException( String msg )
			: base( msg )
		{ }

		public UnsupportedArchitectureException( String msg, Exception inner )
			: base( msg, inner )
		{ }
	}

	[Serializable]
	public class IncompatibleModulesException : CfixException
	{
		public IncompatibleModulesException()
		{ }

		protected IncompatibleModulesException( SerializationInfo info, StreamingContext ctx )
			: base( info, ctx )
		{ }

		public IncompatibleModulesException( String msg )
			: base( msg )
		{ }

		public IncompatibleModulesException( String msg, Exception inner )
			: base( msg, inner )
		{ }
	}

	[Serializable]
    public class TestItemDisappearedException : CfixException
    {
        public TestItemDisappearedException()
			: this( Strings.TestItemDisappeared )
        {}

        protected TestItemDisappearedException( SerializationInfo info, StreamingContext ctx )
            : base( info, ctx )
        {}

        public TestItemDisappearedException( String msg ) 
            : base( msg )
        {}

        public TestItemDisappearedException( String msg, Exception inner )
            : base( msg, inner )
        {}
    }

	[Serializable]
	public class EmptyRunException : CfixException
	{
		public EmptyRunException()
		{ }

		protected EmptyRunException( SerializationInfo info, StreamingContext ctx )
			: base( info, ctx )
		{ }

		public EmptyRunException( String msg )
			: base( msg )
		{ }

		public EmptyRunException( String msg, Exception inner )
			: base( msg, inner )
		{ }
	}
}
