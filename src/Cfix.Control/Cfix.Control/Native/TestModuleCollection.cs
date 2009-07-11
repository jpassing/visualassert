using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Cfix.Control.Diag;
using Cfixctl;

namespace Cfix.Control.Native
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly" )]
	public class TestModuleCollection : 
		GenericTestItemCollection, IAbortableTestItemCollection
	{
		private const uint CFIXCTL_SEARCH_FLAG_RECURSIVE = 1;

		private class LoadAbortException : COMException
		{
		}

		private readonly DirectoryInfo dirInfo;
		private readonly String filter;
		private readonly IAgent searchTarget;
		private readonly AgentSet runTargets;
		private readonly bool userOnly;
		private readonly bool ignoreDuplicates;

		private readonly Object loadLock = new Object();

		//
		// Currently running load (if any). Guarded by loadLock.
		//
		private Loader currentLoader;
		
		//
		// Flag indicating whether the current load has been aborted.
		// May be set from a different thread.
		//
		private volatile bool currentLoadAborted;

		/*--------------------------------------------------------------
		 * Loading.
		 */

		private class Loader : ICfixSearchModulesCallback
		{
			private readonly TestModuleCollection collection;
			private readonly Stack<TestModuleCollection> collectionStack =
				new Stack<TestModuleCollection>();

			//
			// Flag indicating whether the current load shall be aborted.
			// May be set from a different thread.
			//
			private volatile bool abort;
			private bool firstCallback = true;

			public Loader(
				TestModuleCollection collection
				)
			{
				this.collection = collection;

				this.collectionStack.Push( collection );
			}

			public void Abort()
			{
				this.abort = true;
			}

			public void EnterDirectory( 
				String path 
				)
			{
				if ( this.abort )
				{
					throw new LoadAbortException();
				}

				if ( firstCallback )
				{
					firstCallback = false;

					//
					// Ignore the first directory callback as it is 
					// redundant for our usage scenario.
					//
				}
				else
				{
					Debug.Assert( new DirectoryInfo( path ).Parent.Name.Equals(
						this.collectionStack.Peek().dirInfo.Name ) );

					TestModuleCollection nested = new TestModuleCollection(
						this.collectionStack.Peek(),
						new DirectoryInfo( path ),
						this.collection.filter,
						this.collection.searchTarget,
						this.collection.runTargets,
						this.collection.userOnly,
						this.collection.ignoreDuplicates );

					Debug.Print( "Enter: " + path );

					this.collectionStack.Push( nested );
				}
			}

			[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes" )]
			public void FoundModule(
				String path, 
				CfixTestModuleType type,
				CfixTestModuleArch nativeArch
				)
			{
				firstCallback = false;

				Debug.Assert( new FileInfo( path ).Directory.Name.Equals(
						this.collectionStack.Peek().dirInfo.Name ) );

				//
				// N.B. cfixctl has already checked that this module is indeed
				// a test module, so no additional checking is required.
				//

				Architecture arch = ( Architecture ) nativeArch;
				if ( this.collection.runTargets.IsArchitectureSupported( arch ) )
				{
					//
					// N.B. This module may be importing symbols from a DLL
					// that resides in the same directory. 
					//
					// Augment search path.
					//
					IAgent agent = this.collection.runTargets.GetAgent( arch );
					FileInfo pathInfo = new FileInfo( path );
					
					HostEnvironment env = new HostEnvironment();
					env.AddSearchPath( pathInfo.Directory.FullName );

					try
					{
						ITestItemCollection module = agent.LoadModule(
							env,
							this.collectionStack.Peek(),
							path,
							this.collection.ignoreDuplicates );

						this.collectionStack.Peek().Add( module );
					}
					catch ( Exception x )
					{
						this.collectionStack.Peek().Add(
							new InvalidModule(
								this.collectionStack.Peek(),
								new DirectoryInfo( path ).Name,
								x ) );
					}
				}
			}

			public void LeaveDirectory(
				String path
				)
			{
				Debug.Assert( this.collectionStack.Count != 0 );
				Debug.Assert( this.collectionStack.Peek().dirInfo.Name.Equals(
					new DirectoryInfo( path ).Name ) );

				if ( this.collectionStack.Count == 1 )
				{
					//
					// Root - ignore in the same manner it has been
					// ignored in EnterDirectory.
					//
					Debug.Print( "Leave -- Nop" );
				}
				else
				{
					TestModuleCollection loaded = this.collectionStack.Pop();

					if ( loaded.ItemCount > 0 )
					{
						Debug.Print( "Leave -- Added: " + path );
						this.collectionStack.Peek().Add( loaded );
					}
					else
					{
						Debug.Print( "Leave -- Ignored: " + path );
						loaded.Dispose();
					}
				}
			}
		}

		private void Populate()
		{
			lock ( this.loadLock )
			{
				Debug.Assert( this.currentLoader == null );

				this.currentLoadAborted = false;
				this.currentLoader = new Loader( this );

				//
				// Search, but do not load modules. For this 
				// operation, the default environment suffices.
				//
				using ( IHost host = searchTarget.CreateHost() )
				{
					uint types;
					if ( userOnly )
					{
						types = 
							( ( uint ) CfixTestModuleType.CfixTestModuleTypeUser ) |
							( ( uint ) CfixTestModuleType.CfixTestModuleTypeUserEmbedded );
					}
					else
					{
						types = UInt32.MaxValue;
					}

					try
					{
						( ( Host ) host ).GetNativeItem().SearchModules(
							this.dirInfo.FullName + "\\" + this.filter,
							CFIXCTL_SEARCH_FLAG_RECURSIVE,
							types,
							( uint ) runTargets.GetArchitectures(),
							this.currentLoader );
					}
					catch ( COMException x )
					{
						if ( this.currentLoadAborted )
						{
							//
							// Load has been aborted - not a true error.
							// Note that thanks to marvelous Interop, we cannot rely 
							// on the exception's HRESULT.
							//
						}
						else
						{
							Logger.LogError( "ModuleCollection", "Failed to populate", x );
							throw ( ( Agent ) searchTarget ).WrapException( x );
						}
					}
					finally
					{
						this.currentLoadAborted = false;
						this.currentLoader = null;
					}
				}
			}
		}

		/*--------------------------------------------------------------
		 * Ctor.
		 */

		private TestModuleCollection( 
			ITestItemCollection parent,
			DirectoryInfo dir, 
			String filter,
			IAgent searchTarget,
			AgentSet runTargets,
			bool userOnly,
			bool ignoreDuplicates
			)
			: base( parent, dir.Name )
		{
			this.dirInfo = dir;
			this.filter = filter;
			this.searchTarget = searchTarget;
			this.runTargets = runTargets;
			this.userOnly = userOnly;
			this.ignoreDuplicates = ignoreDuplicates;
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		public override void Refresh()
		{
			//
			// N.B. Population of list is already guarded by 
			// this.listlock and this.loadLock. 
			//
			Clear();
			Populate();
		}

		/*--------------------------------------------------------------
		 * IAbortableTestItemCollection.
		 */

		public void AbortRefresh()
		{
			Loader cur = this.currentLoader;
			if ( cur != null )
			{
				this.currentLoadAborted = true;
				cur.Abort();
			}
		}
		
		/*--------------------------------------------------------------
		 * Statics.
		 */

		internal static TestModuleCollection Search(
			DirectoryInfo dir,
			String filter, 
			IAgent searchTarget,
			AgentSet runTargets,
			bool userOnly,
			bool ignoreDuplicates
			)
		{
			TestModuleCollection result = new TestModuleCollection(
				null,
				dir,
				filter,
				searchTarget,
				runTargets,
				userOnly,
				ignoreDuplicates );

			return result;
		}

	}
}

