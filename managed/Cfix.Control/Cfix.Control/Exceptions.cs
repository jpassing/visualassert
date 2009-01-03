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
    public class TestItemDisappearedException : CfixException
    {
        public TestItemDisappearedException()
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
}
