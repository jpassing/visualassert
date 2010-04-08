using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using Cfix.Control;
using System.IO;
using System.Globalization;

namespace Cfix.Addin.IntelParallelStudio
{
	/*++
	 * 
	 * Format:
	 *	Element 'sev'
	 *		0	Information
	 *		1	?
	 *		2	Warning
	 *		3	Warning
	 *		4	Error
	 * 
	 * Element 'dec_func: decorated function name.
	 * Element 'func: Undecorated function name.
	 * 
	--*/
	public class InspectorResultFile
	{
		public delegate bool FilterDelegate( InspectorResult result );

		private readonly ICollection< InspectorResult > results =
			new List< InspectorResult >();

		public ICollection<InspectorResult> Results
		{
			get { return this.results; }
		}

		public InspectorResultFile Filter( FilterDelegate filter )
		{
			InspectorResultFile file = new InspectorResultFile();

			foreach ( InspectorResult result in this.results )
			{
				if ( filter( result ) )
				{
					file.results.Add( result );
				}
			}

			return file;
		}

		/*----------------------------------------------------------------------
		 * Parsing logic.
		 */

		private static uint ParseOptionalUInt32( string s )
		{
			if ( String.IsNullOrEmpty( s ) )
			{
				return 0;
			}
			else
			{
				return UInt32.Parse( s );
			}
		}

		private static uint ParseOptionalUInt32Hex( string s )
		{
			if ( String.IsNullOrEmpty( s ) )
			{
				return 0;
			}
			else
			{
				if ( s.StartsWith( "0x" ) )
				{
					s = s.Substring( 2 );
				}

				return UInt32.Parse( s, NumberStyles.HexNumber );
			}
		}

		private static IStackTraceFrame ParseLoc( XmlReader reader )
		{
			string module = null;
			string function = null;
			string sourceFile = null;
			uint sourceLine = 0;
			uint rva = 0;

			StringBuilder buffer = new StringBuilder();
			bool exit = false;
			while ( !exit && reader.Read() )
			{
				switch ( reader.NodeType )
				{
					case XmlNodeType.EndElement:
						switch ( reader.Name )
						{
							case "mod":
								module = buffer.ToString();
								break;

							case "func":
								function = buffer.ToString();
								break;

							case "file":
								sourceFile = buffer.ToString();
								break;

							case "line":
								sourceLine = ParseOptionalUInt32( 
									buffer.ToString().Trim() );
								break;

							case "rva":
								rva = ParseOptionalUInt32Hex(
									buffer.ToString().Trim() );
								break;

							case "loc":
								exit = true;
								break;
						}

						buffer = new StringBuilder();
						break;

					case XmlNodeType.Text:
						buffer.Append( reader.Value );
						break;

					default:
						//
						// Ignore.
						//
						break;
				}
			}

			if ( String.IsNullOrEmpty( module ) &&
				String.IsNullOrEmpty( function ) )
			{
				return null;
			}
			else
			{
				//
				// Strip module path.
				//
				module = Path.GetFileName( module );

				return new StackTraceFrame(
					module,
					function ?? String.Format( "0x{0:x}", rva ),
					0,	// Not included in data.
					sourceFile,
					sourceLine );
			}
		}

		public static InspectorResultFile Parse( string path )
		{
			try
			{
				InspectorResultFile resultFile = new InspectorResultFile();

				using ( Stream stream = new FileStream( 
					path, 
					FileMode.Open, 
					FileAccess.Read, 
					FileShare.ReadWrite ) )
				using ( XmlReader reader = new XmlTextReader( stream ) )
				{
					StringBuilder buffer = new StringBuilder();

					InspectorResult.ResultSeverity severity =
						InspectorResult.ResultSeverity.Information;
					string description = null;
					uint type = 0;
					uint threadId = 0;
					List< IStackTraceFrame> stackFrames = 
						new List< IStackTraceFrame>();

					//
					// There may be multiple thread element, we only
					// parse the first.
					//
					int threadCount = 0;

					while ( reader.Read() )
					{
						switch ( reader.NodeType )
						{
							case XmlNodeType.Element:
								switch ( reader.Name )
								{
									case "loc":
										if ( threadCount == 0 )
										{
											IStackTraceFrame frame =
												ParseLoc( reader );
											if ( frame != null )
											{
												stackFrames.Add( frame );
											}
										}
										break;

									case "thread":
										if ( threadCount > 0 && stackFrames.Count == 0 )
										{
											//
											// There already was a thread,
											// but it has no stack trace -- try
											// this one.
											//
											threadCount = 0;
											threadId = 0;
										}
										break;

									default:
										break;
								}
								break;

							case XmlNodeType.EndElement:
								switch ( reader.Name )
								{
									case "desc":
										description = buffer.ToString();
										break;

									case "type":
										type = ParseOptionalUInt32( 
											buffer.ToString() );
										break;

									case "sev":
										uint sev = ParseOptionalUInt32( 
											buffer.ToString() );
										if ( sev <= 1 )
										{
											severity = InspectorResult.ResultSeverity.Information;
										}
										else if ( sev <= 3 )
										{
											severity = InspectorResult.ResultSeverity.Warning;
										}
										else
										{
											severity = InspectorResult.ResultSeverity.Error;
										}

										break;

									case "threadid":
										if ( threadCount == 0 )
										{
											threadId = UInt32.Parse( buffer.ToString() );
										}
										break;

									case "message":
										IStackTrace stackTrace = null;
										if ( stackFrames.Count > 0 )
										{
											stackTrace = new StackTrace( stackFrames );
										}

										if ( description != null )
										{
											//
											// Resolve description.
											//
											description = InspectorResult.GetDescriptionFromCode( description );
										}

										resultFile.Results.Add(
											new InspectorResult(
												type,
												severity,
												description,
												threadId,
												stackTrace ) );

										//
										// Reset state.
										//
										severity = InspectorResult.ResultSeverity.Information;
										description = null;
										type = 0;
										threadId = 0;
										stackFrames = new List<IStackTraceFrame>();
										threadCount = 0;

										break;

									case "thread":
										threadCount++;
										break;

									default:
										break;
								}


								buffer = new StringBuilder();
								break;

							case XmlNodeType.Text:
								buffer.Append( reader.Value );
								break;

							default:
								//
								// Ignore.
								//
								break;
						}
					}

					return resultFile;
				}
			}
			catch ( XmlException x )
			{
				Logger.LogError( "Inspector", x );
				throw new InspectorException( 
					"Parsing result file failed: " + x.Message, x );
			}
			catch ( IOException x )
			{
				Logger.LogError( "Inspector", x );
				throw new InspectorException(
					"Reading result file failed: " + x.Message, x );
			}
		}

		public static InspectorResultFile Load( ResultLocation loc )
		{
			//
			// N.B. The ResultLocation identifies the directory Inspector
			// has saved all its results in. We have to find the appropriate
			// .pdr file and load its contents.
			//

			string dataDir = Path.Combine( loc.ResultDirectory, "data.0" );

			string[] pdrFiles = Directory.GetFiles( dataDir, "*.pdr" );

			Debug.Assert( pdrFiles.Length >= 1 );

			//
			// N.B. Stupid .Net also returns the .pdr_bak file.
			//
			string pdrFile = null;
			foreach ( string candidate in pdrFiles )
			{
				if ( candidate.EndsWith( "_bak" ) )
				{
					continue;
				}
				else
				{
					pdrFile = candidate;
					break;
				}
			}

			if ( pdrFile == null )
			{
				throw new InspectorException( "No PDR file found" );
			}

			return Parse( Path.Combine( dataDir, pdrFile ) );
		}

		/*----------------------------------------------------------------------
		 * Internal classes.
		 */

		private class StackTrace : IStackTrace
		{
			private readonly ICollection<IStackTraceFrame> frames;

			public StackTrace( ICollection<IStackTraceFrame> frames )
			{
				this.frames = frames;
			}

			public uint FrameCount
			{
				get { return ( uint ) this.frames.Count; }
			}

			public IEnumerator<IStackTraceFrame> GetEnumerator()
			{
				return this.frames.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.frames.GetEnumerator();
			}

			public override string ToString()
			{
				StringBuilder buf = new StringBuilder();
				foreach ( IStackTraceFrame frame in this.frames )
				{
					buf.Append( frame.ToString() );
					buf.Append( "\r\n" );
				}

				return buf.ToString();
			}
		}
}
}
