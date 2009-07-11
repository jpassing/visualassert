using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Cfix.Control.Diag;
using Cfixctl;

namespace Cfix.Control.Native
{
	/*++
	 * Class Description:
	 *		Wrapper for a ICfixTestItem and base class for other
	 *		test item wrapper classes.
	 * 
	 *		Threadsafe.
	 --*/
	public class TestItem : ITestItem, IRunnableTestItem
	{
		private readonly TestItemCollection parent;
		private readonly uint ordinal;
		private readonly String name;

		private bool disposed;

		public event EventHandler Disposed;

		internal static ITestItem Wrap(
			TestItemCollection parent,
			uint ordinal,
			ICfixTestItem ctlItem
			)
		{
			if ( ctlItem is ICfixTestContainer )
			{
				return new TestItemCollection(
					parent,
					ordinal,
					ctlItem );
			}
			else
			{
				return new TestItem(
					parent,
					ordinal,
					ctlItem );
			}
		}

		internal virtual ICfixTestItem GetNativeItem( IHost host )
		{
			ICfixTestContainer parentContainer =
				( ICfixTestContainer ) parent.GetNativeItem( host );

			try
			{
				if ( ordinal >= parentContainer.GetItemCount() )
				{
					//
					// Module must have changed in the meantime.
					//
					throw new TestItemDisappearedException();
				}

				return parentContainer.GetItem( this.ordinal );
			}
			catch ( COMException x )
			{
				Logger.LogError( "Agent", "Failed to obtain native item", x );
				throw this.Module.Agent.WrapException( x );
			}
			finally
			{
				this.Module.Agent.ReleaseObject( parentContainer );
			}
		}

		internal virtual IHost CreateHost(
			IAgent agent,
			HostEnvironment env )
		{
			TestItemCollection parentColl = this.Parent as TestItemCollection;
			if ( parentColl != null )
			{
				return parentColl.CreateHost( agent, env );
			}
			else
			{
				return agent.CreateHost( env );
			}
		}

		/*--------------------------------------------------------------
		 * Publics.
		 */

		internal TestItem(
			TestItemCollection parent,
			uint ordinal,
			ICfixTestItem item )
		{
			Debug.Assert( item != null );

			this.parent = parent;
			this.ordinal = ordinal;
			this.name = item.GetName();
		}

		~TestItem()
		{
			Dispose( false );
		}


		protected virtual void Dispose( bool disposing )
		{
			if ( !this.disposed )
			{
				if ( Disposed != null )
				{
					Disposed( this, EventArgs.Empty );
				}

				this.disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}
		
		internal virtual TestModule Module
		{
			get
			{
				return this.parent.Module;
			}
		}

		/*--------------------------------------------------------------
		 * ITestItem.
		 */

		public bool IsDisposed
		{
			get { return this.disposed; }
		}

		public String Name
		{
			get { return this.name; }
		}

		public virtual String FullName
		{
			get 
			{ 
				//
				// Depending on the type of API used, this may or
				// may not be correct.
				//
				return this.name; 
			}
		}

		public uint Ordinal
		{
			get { return this.ordinal; }
		}

		public virtual ITestItemCollection Parent
		{
			get { return this.parent; }
		}

		public void Add(
			IRunCompiler compiler,
			IActionEvents events,
			IResultItem result
			)
		{
			Debug.Assert( compiler != null );
			Debug.Assert( events != null );
			Debug.Assert( result != null );

			compiler.Add(
				new NativeAction(
					this,
					events,
					result,
					compiler.ExecutionOptions,
					compiler.ThreadingOptions,
					compiler.EnvironmentOptions ) );

			FileInfo modInfo = new FileInfo( this.Module.Path );
			compiler.Environment.AddSearchPath( modInfo.Directory.FullName );
		}

		/*--------------------------------------------------------------
		 * IResultItemFactory.
		 */

		public virtual IResultItem CreateResultItem(
			IResultItemCollection parentResult,
			IActionEvents events,
			ExecutionStatus interimStatus
			)
		{
			if ( parentResult == null )
			{
				//
				// This is a single-testcase run. Single test cases
				// execute within a fixture -- therefore, in order to provide
				// a place for the fixture setup/teardown failures and status,
				// we have to inject a parent result item.
				//
				Debug.Assert( this.parent != null );
				return this.parent.CreateResultItemForSingleTestCaseRun(
					events,
					this,
					interimStatus );
			}
			else
			{
				return new TestItemResult(
					events,
					parentResult,
					this,
					interimStatus );
			}
		}
	}
}
