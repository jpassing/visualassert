/*----------------------------------------------------------------------
 * Purpose:
 *		Exceptions.
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

using System;
using System.Runtime.Serialization;

namespace Cfix.Addin
{
	[Serializable]
	public class CfixAddinException : Exception
	{
		public CfixAddinException()
		{ }

		protected CfixAddinException( SerializationInfo info, StreamingContext ctx )
			: base( info, ctx )
		{ }

		public CfixAddinException( String msg )
			: base( msg )
		{ }

		public CfixAddinException( String msg, Exception inner )
			: base( msg, inner )
		{ }
	}
}
