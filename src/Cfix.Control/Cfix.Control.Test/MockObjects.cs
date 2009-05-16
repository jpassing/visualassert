using System;
using System.Collections.Generic;
using Cfix.Control.Native;
using Cfixctl;

namespace Cfix.Control.Test
{
	internal class MockItem : ICfixTestItem
	{
		private String name;

		public MockItem( String name )
		{
			this.name = name;
		}

		public ICfixAction CreateExecutionAction( uint SchedulingFlags, uint Reserved )
		{
			throw new NotImplementedException();
		}

		public string GetName()
		{
			return this.name;
		}
	}

	internal class MockContainer : MockItem, ICfixTestContainer
	{
		public IList<ICfixTestItem> Children = new List<ICfixTestItem>();

		public MockContainer( String name )
			: base( name )
		{
		}

		public ICfixTestItem GetItem( uint Ordinal )
		{
			return Children[ ( int ) Ordinal ];
		}

		public uint GetItemCount()
		{
			return ( uint ) Children.Count;
		}

		public void EnumItems( uint Flags, out IEnumUnknown Enum )
		{
			throw new NotImplementedException();
		}
	}

	internal class MockModule : MockContainer, ICfixTestModule
	{
		private String path;

		public MockModule( String path )
			: base( "mock.dll" )
		{
			this.path = path;
		}

		public string GetPath()
		{
			return this.path;
		}

		public void GetType( out CfixTestModuleType Type, out CfixTestModuleArch Arch )
		{
			Type = CfixTestModuleType.CfixTestModuleTypeUser;
			Arch = CfixTestModuleArch.CfixTestModuleArchI386;
		}
	}

	internal class MockHost : ICfixHost
	{
		private MockModule module;

		public MockHost( MockModule module )
		{
			this.module = module;
		}

		public void GetArchitecture( out CfixTestModuleArch Arch )
		{
			Arch = CfixTestModuleArch.CfixTestModuleArchI386;
		}

		public ICfixTestModule LoadModule( string Path )
		{
			return this.module;
		}

		public uint GetHostProcessId()
		{
			return 0;
		}

		public void Terminate()
		{
		}

		public void SearchModules( 
			string PathFilter, 
			uint flags, 
			uint Type, 
			uint Architecture, 
			ICfixSearchModulesCallback Callback )
		{
		}
	}

	internal class MockAgent : ICfixAgent, ICfixMessageResolver
	{
		private MockModule module;

		public MockAgent( MockModule module )
		{
			this.module = module;
		}

		public ICfixHost CreateHost( 
			CfixTestModuleArch Arch, 
			uint Clsctx, 
			uint Flags, 
			uint Timeout, 
			string environment,
			string CurrentDirectory )
		{
			return new MockHost( this.module );
		}

		public string GetHostPath( CfixTestModuleArch Arch )
		{
			throw new NotImplementedException();
		}

		public void RegisterHost( uint Cookie, ICfixHost Host )
		{
			throw new NotImplementedException();
		}

		public ICfixHost WaitForHostConnection( uint Cookie, uint Timeout )
		{
			throw new NotImplementedException();
		}

		public ICfixMessageResolver CreateMessageResolver()
		{
			return this;
		}

		public string ResolveMessage( uint MessageId, uint Reserved )
		{
			return "mock";
		}

		public void SetTrialLicenseCookie( uint Cookie )
		{ }
	}

	internal class MockTarget : Agent
	{
		public MockTarget( MockModule module )
			: base( 
				new MockAgent( module ), 
				CfixTestModuleArch.CfixTestModuleArchI386, 
				true,
				HostCreationOptions.None )
		{
		}

		public override void ReleaseObject( Object obj )
		{
		}
	}
}
