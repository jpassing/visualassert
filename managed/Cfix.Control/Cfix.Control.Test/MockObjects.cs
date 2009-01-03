using System;
using System.Collections.Generic;
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

		public FixtureExecutionAction CreateExecutionAction( uint SchedulingFlags, uint Reserved )
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
	}

	internal class MockAgent : ICfixAgent
	{
		private MockModule module;

		public MockAgent( MockModule module )
		{
			this.module = module;
		}

		public ICfixHost CreateHost( CfixTestModuleArch Arch, uint Clsctx )
		{
			return new MockHost( this.module );
		}
	}

	internal class MockTarget : Target
	{
		public MockTarget( MockModule module )
			: base( new MockAgent( module ), CfixTestModuleArch.CfixTestModuleArchI386, true )
		{
		}

		public override void ReleaseObject( Object obj )
		{
		}
	}
}
