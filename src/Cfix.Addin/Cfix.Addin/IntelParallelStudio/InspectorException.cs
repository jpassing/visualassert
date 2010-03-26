using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Cfix.Addin.IntelParallelStudio
{
	[Serializable]
	public class InspectorException : Exception
	{
		public InspectorException()
		{ }

		protected InspectorException( SerializationInfo info, StreamingContext ctx )
			: base( info, ctx )
		{ }

		public InspectorException( String msg )
			: base( msg )
		{ }

		public InspectorException( String msg, Exception inner )
			: base( msg, inner )
		{ }
	}
}
