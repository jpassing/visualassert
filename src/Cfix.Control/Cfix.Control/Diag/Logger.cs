using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Cfix.Control.Diag
{
	public sealed class Logger
	{
		private static TraceListener listener;
		private static readonly TraceEventCache eventCache = new TraceEventCache();
		private static readonly object logLock = new object();

		private Logger()
		{ }

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope" )]
		public static void SetOutput( string file )
		{
			new FileInfo( file ).Directory.Create();
			FileStream fs = new FileStream(
				file,
				FileMode.Append,
				FileAccess.Write,
				FileShare.ReadWrite );
			SetOutput( new TextWriterTraceListener( fs ) );
		}

		public static void SetOutput( TraceListener lst )
		{
			lst.Write(
				string.Format( "Trace started: {0}\r\n", DateTime.Now ) );
			lst.Flush();
			listener = lst;
		}

		public static void CloseOutput()
		{
			TraceListener lst = listener;
			listener = null;
			lst.Close();
		}

		public static void LogInfo( string source, string format, params object[] args )
		{
			if ( listener != null )
			{
				lock ( logLock )
				{
					listener.TraceEvent(
						eventCache,
						String.Format( "[{0}] {1}", Process.GetCurrentProcess().Id, source ),
						TraceEventType.Information,
						0,
						format,
						args );
					listener.Flush();
				}
			}
		}

		public static void LogWarning( string source, string format, params object[] args )
		{
			if ( listener != null )
			{
				lock ( logLock )
				{
					listener.TraceEvent(
						eventCache,
						String.Format( "[{0}] {1}", Process.GetCurrentProcess().Id, source ),
						TraceEventType.Warning,
						0,
						format,
						args );
					listener.Flush();
				}
			}
		}

		public static void LogError( string source, string format, params object[] args )
		{
			if ( listener != null )
			{
				lock ( logLock )
				{
					listener.TraceEvent(
						eventCache,
						String.Format( "[{0}] {1}", Process.GetCurrentProcess().Id, source ),
						TraceEventType.Error,
						0,
						format,
						args );
					listener.Flush();
				}
			}
		}

		public static void LogError( string source, Exception x )
		{
			if ( listener != null )
			{
				lock ( logLock )
				{
					listener.TraceEvent(
						eventCache,
						String.Format( "[{0}] {1}", Process.GetCurrentProcess().Id, source ),
						TraceEventType.Error,
						0,
						"[Exception Message {1}] {2}",
						x.Message, 
						x.StackTrace );
					listener.Flush();
				}
			}
		}

		public static void LogError( string source, string message, Exception x )
		{
			if ( listener != null )
			{
				lock ( logLock )
				{
					listener.TraceEvent(
						eventCache,
						String.Format( "[{0}] {1}", Process.GetCurrentProcess().Id, source ),
						TraceEventType.Error,
						0,
						"{0} [Exception Message {1}] {2}",
						message,
						x.Message,
						x.StackTrace );
					listener.Flush();
				}
			}
		}

		public static void LogError( string source, string message, ExternalException x )
		{
			if ( listener != null )
			{
				lock ( logLock )
				{
					listener.TraceEvent(
						eventCache,
						String.Format( "[{0}] {1}", Process.GetCurrentProcess().Id, source ),
						TraceEventType.Error,
						0,
						"{0} [Exception Message {1}, HRESULT 0x{3:X}] {2}",
						message,
						x.Message,
						x.StackTrace,
						x.ErrorCode );
					listener.Flush();
				}
			}
		}
	}
}
