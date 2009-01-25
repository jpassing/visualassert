using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	public class TestModuleCollection : GenericTestItemCollection
	{
		private const uint CFIXCTL_SEARCH_FLAG_RECURSIVE = 1;
		
		private readonly DirectoryInfo dirInfo;

		public interface ISearchListener
		{
			void InvalidModule(
				String path,
				String reason
				);
		}

		/*--------------------------------------------------------------
		 * Loading.
		 */

		private class Loader : ICfixSearchModulesCallback
		{
			private readonly ISearchListener listener;
			private readonly bool ignoreDuplicates;
			private readonly MultiTarget target;
			private readonly Stack<TestModuleCollection> collectionStack =
				new Stack<TestModuleCollection>();

			private bool firstCallback = true;

			public Loader( 
				MultiTarget target,
				TestModuleCollection current,
				ISearchListener listener,
				bool ignoreDuplicates
				)
			{
				this.target = target;
				this.listener = listener;
				this.ignoreDuplicates = ignoreDuplicates;

				this.collectionStack.Push( current );
			}

			public void EnterDirectory( 
				String path 
				)
			{
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
						new DirectoryInfo( path ),
						this.target );

					Debug.Print( "Enter: " + path );

					this.collectionStack.Push( nested );
				}
			}

			public void FoundModule(
				String path, 
				CfixTestModuleType type,
				CfixTestModuleArch nativeArch
				)
			{
				firstCallback = false;

				Debug.Assert( new FileInfo( path ).Directory.Name.Equals(
						this.collectionStack.Peek().dirInfo.Name ) );

				Architecture arch = ( Architecture ) nativeArch;
				if ( this.target.IsArchitectureSupported( arch ) )
				{
					try
					{
						this.collectionStack.Peek().Add(
							TestModule.LoadModule(
								target.GetTarget( arch ),
								path,
								this.ignoreDuplicates ) );
					}
					catch ( Exception x )
					{
						this.listener.InvalidModule( path, x.Message );
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

		private void Populate(
			String filter,
			Target searchTarget,
			MultiTarget runTargets,
			bool userOnly,
			bool ignoreDuplicates,
			ISearchListener listener
			)
		{
			ICfixHost host = null;
			try
			{
				host = searchTarget.CreateHost();

				host.SearchModules(
					this.dirInfo.FullName + "\\" + filter,
					CFIXCTL_SEARCH_FLAG_RECURSIVE,
					userOnly
						? ( uint ) CfixTestModuleType.CfixTestModuleTypeUser
						: UInt32.MaxValue,
					runTargets.GetArchitectureMask(),
					new Loader( runTargets, this, listener, ignoreDuplicates ) );
			}
			catch ( COMException x )
			{
				throw searchTarget.WrapException( x );
			}
			finally
			{
				if ( host != null )
				{
					searchTarget.ReleaseObject( host );
				}
			}
		}

		/*--------------------------------------------------------------
		 * Ctor.
		 */

		private TestModuleCollection( 
			DirectoryInfo dir, 
			MultiTarget target )
			: base( dir.Name )
		{
			this.dirInfo = dir;
		}

		/*--------------------------------------------------------------
		 * Overrides.
		 */

		public override void Refresh()
		{
			lock ( this.listLock )
			{
				Clear();
			}
		}
		
		/*--------------------------------------------------------------
		 * Statics.
		 */

		public static TestModuleCollection Search(
			DirectoryInfo dir,
			String filter, 
			Target searchTarget,
			MultiTarget runTargets,
			bool userOnly,
			bool ignoreDuplicates,
			ISearchListener listener
			)
		{
			TestModuleCollection result = new TestModuleCollection(
					dir,
					runTargets );
			result.Populate(
				filter,
				searchTarget,
				runTargets,
				userOnly,
				ignoreDuplicates,
				listener );

			return result;
		}

	}
}

