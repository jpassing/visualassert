using System;
using System.Diagnostics;
using System.IO;

namespace Cfix.Addin
{
	internal class Logger
	{
		private readonly TraceListener listener;
		private readonly TraceEventCache eventCache = new TraceEventCache();
		private readonly object logLock = new object();

		public Logger( string file )
		{
			new FileInfo( file ).Directory.Create();
			FileStream fs = new FileStream( 
				file, 
				FileMode.Append, 
				FileAccess.Write, 
				FileShare.ReadWrite );
			this.listener = new TextWriterTraceListener( fs );
			this.listener.Write(
				string.Format( "Trace started: {0}\r\n", DateTime.Now ) );
			this.listener.Flush();
		}

		public void Close()
		{
			this.listener.Close();
		}

		public void LogInfo( string source, string format, params object[] args )
		{
			lock ( this.logLock )
			{
				this.listener.TraceEvent(
					this.eventCache,
					String.Format( "[{0}] {1}", Process.GetCurrentProcess().Id, source ),
					TraceEventType.Information,
					0,
					format,
					args );
				this.listener.Flush();
			}
		}

		public void LogWarning( string source, string format, params object[] args )
		{
			lock ( this.logLock )
			{
				this.listener.TraceEvent(
					this.eventCache,
					String.Format( "[{0}] {1}", Process.GetCurrentProcess().Id, source ),
					TraceEventType.Warning,
					0,
					format,
					args );
				this.listener.Flush();
			}
		}

		public void LogError( string source, string format, params object[] args )
		{
			lock ( this.logLock )
			{
				this.listener.TraceEvent(
					this.eventCache,
					String.Format( "[{0}] {1}", Process.GetCurrentProcess().Id, source ),
					TraceEventType.Error,
					0,
					format,
					args );
				this.listener.Flush();
			}
		}
	}
}
