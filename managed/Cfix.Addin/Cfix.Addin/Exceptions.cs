using System;
using System.Runtime.Serialization;

namespace Cfix.Addin
{
	[Serializable]
	public class ConcurrentRunException : Exception
	{
		public ConcurrentRunException()
		{ }

		protected ConcurrentRunException( SerializationInfo info, StreamingContext ctx )
			: base( info, ctx )
		{ }

		public ConcurrentRunException( String msg )
			: base( msg )
		{ }

		public ConcurrentRunException( String msg, Exception inner )
			: base( msg, inner )
		{ }
	}
}
