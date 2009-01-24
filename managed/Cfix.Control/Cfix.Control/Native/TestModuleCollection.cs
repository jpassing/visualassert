using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Cfixctl;

namespace Cfix.Control.Native
{
	public class TestModuleCollection : GenericTestItemCollection
	{
		private const uint CFIXCTL_SEARCH_FLAG_RECURSIVE = 1;

		public interface SearchListener
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
			private readonly SearchListener listener;
			private readonly bool ignoreDuplicates;
			private readonly MultiTarget target;
			private readonly Stack<TestModuleCollection> collectionStack =
				new Stack<TestModuleCollection>();

			public Loader( 
				MultiTarget target,
				TestModuleCollection current,
				SearchListener listener,
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
				TestModuleCollection nested = new TestModuleCollection(
					new DirectoryInfo( path ), 
					this.target );

				this.collectionStack.Push( nested );
			}

			public void FoundModule(
				String path, 
				CfixTestModuleType type,
				CfixTestModuleArch nativeArch
				)
			{
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
				TestModuleCollection loaded = this.collectionStack.Pop();
				if ( loaded.ItemCount > 0 )
				{
					this.collectionStack.Pop().Add( loaded );
				}
				else
				{
					loaded.Dispose();
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
		}
		
		/*--------------------------------------------------------------
		 * Statics.
		 */

		public static TestModuleCollection Search(
			DirectoryInfo dir,
			String filter, 
			MultiTarget target,
			bool userOnly,
			bool ignoreDuplicates,
			SearchListener listener
			)
		{
			ICfixHost host = null;
			Target loadTarget = target.GetAnyTarget();
			try
			{
				host = loadTarget.CreateHost();

				TestModuleCollection result = new TestModuleCollection(
					dir,
					target );

				host.SearchModules(
					dir.FullName + "\\" + filter,
					CFIXCTL_SEARCH_FLAG_RECURSIVE,
					userOnly
						? ( uint ) CfixTestModuleType.CfixTestModuleTypeUser
						: UInt32.MaxValue,
					target.GetArchitectureMask(),
					new Loader( target, result, listener, ignoreDuplicates ) );

				return result;
			}
			catch ( COMException x )
			{
				throw loadTarget.WrapException( x );
			}
			finally
			{
				if ( host != null )
				{
					loadTarget.ReleaseObject( host );
				}

				loadTarget.Dispose();
			}
		}

	}
}
